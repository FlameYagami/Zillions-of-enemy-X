using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using CardCrawler.Model;
using Common;
using Dialog;
using Dialog.View;
using HtmlAgilityPack;
using MaterialDesignThemes.Wpf;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;

namespace CardCrawler.ViewModel
{
    public class ImagePreviewVm : BaseModel
    {
        private bool _allChecked;
        private IDisposable _dispose;

        private bool _flashChecked;

        private string _webPackValue;

        public ImagePreviewVm()
        {
            CheckModels = new ObservableCollection<CheckModel<ImageModel>>();
            WebPackDic = new ObservableCollection<KeyValuePair<string, string>>();
            CmdImageAnalysis = new DelegateCommand {ExecuteCommand = AnalysisImage_Click};
            CmdPackAsync = new DelegateCommand {ExecuteCommand = PackAsync_Click};
            CmdAllChecked = new DelegateCommand {ExecuteCommand = AllChecked_Click};
            CmdFlashChecked = new DelegateCommand {ExecuteCommand = FlashChecked_Click};
            CmdImageDownload = new DelegateCommand {ExecuteCommand = ImageDownload_Click};
            InitPack();
            PrgModel = new PrgModel();
        }

        public ObservableCollection<CheckModel<ImageModel>> CheckModels { get; set; }
        public DelegateCommand CmdImageAnalysis { get; set; }
        public DelegateCommand CmdImageDownload { get; set; }
        public DelegateCommand CmdPackAsync { get; set; }
        public DelegateCommand CmdAllChecked { get; set; }
        public DelegateCommand CmdFlashChecked { get; set; }

        public ObservableCollection<KeyValuePair<string, string>> WebPackDic { get; set; }

        public PrgModel PrgModel { get; set; }

        public string WebPackValue
        {
            get { return _webPackValue; }
            set
            {
                _webPackValue = value;
                OnPropertyChanged(nameof(WebPackValue));
            }
        }

        public bool AllChecked
        {
            get { return _allChecked; }
            set
            {
                _allChecked = value;
                OnPropertyChanged(nameof(AllChecked));
            }
        }

        public bool FlashChecked
        {
            get { return _flashChecked; }
            set
            {
                _flashChecked = value;
                OnPropertyChanged(nameof(FlashChecked));
            }
        }

        public void ImageDownload_Click(object obj)
        {
            var models = GetImageModels();
            if (0 == models.Count)
            {
                BaseDialogUtils.ShowDialogAuto("请选择要下载的图片");
                return;
            }
            DownloadImages(models);
        }

        private void DownloadImages(List<ImageModel> cardModels)
        {
            var succeed = 0;
            var failed = 0;
            var pathFolder = PathManager.RootPath + "TempPack\\";
            if (!Directory.Exists(pathFolder)) Directory.CreateDirectory(pathFolder);
            PrgModel.Start();
            var taskDic = new Dictionary<int, bool>();
            for (var i = 0; i != cardModels.Count; i++)
            {
                var page = Convert.ToInt32(i);
                taskDic.Add(page, false);
                GetImageDetailUrl(cardModels[page]).Select(pair =>
                {
                    var result = DownloadImage(pair.Key, pair.Value);
                    if (result) succeed++;
                    else failed++;
                    return pair;
                }).ObserveOnDispatcher().Subscribe(urls =>
                {
                    // UI更新状态      
                    taskDic[page] = true;
                    var persent = taskDic.Values.Count(x => x)/float.Parse(cardModels.Count.ToString());
                    PrgModel.Update(Convert.ToInt32(persent*100),
                        $"已完成：{taskDic.Values.Count(x => x)} / {cardModels.Count}");
                });
            }
            // 轮询任务字典，确保整个任务执行完毕，将数据提交至观测者
            _dispose = Observable.Interval(TimeSpan.FromSeconds(1)).ObserveOnDispatcher().Subscribe(time =>
            {
                if (cardModels.Count != taskDic.Values.Count(x => x))
                    return;
                _dispose.Dispose();
                PrgModel.Finish(succeed, failed);
            });
        }

        private IObservable<KeyValuePair<string, string>> GetImageDetailUrl(ImageModel imageModel)
        {
            return Task.Run(() =>
            {
                var imageUrl = GetImageDetailUrl(imageModel.ImageHref);
                var path = PathManager.RootPath + "TempPack\\" + imageModel.Number + ".jpg";
                return new KeyValuePair<string, string>(imageUrl, path);
            }).ToObservable();
        }

        private string GetImageDetailUrl(string hrefUrl)
        {
            var web = new HtmlWeb();
            var frame = web.Load(hrefUrl);
            return frame.DocumentNode
                .SelectNodes(@"//img").First(x => (null != x.Attributes["width"]) && (null != x.Attributes["class"])
                                                  && int.Parse(x.Attributes["width"].Value).Equals(255) &&
                                                  x.Attributes["class"].Value.Equals("main_img")).Attributes["src"]
                .Value.Replace("////", "//");
        }

        public List<ImageModel> GetImageModels()
        {
            return CheckModels.Where(model => model.Checked).Select(model => model.Model).ToList();
        }

        public void FlashChecked_Click(object obj)
        {
            foreach (var checkModel in CheckModels)
                if (checkModel.Model.Number.Contains("H"))
                    checkModel.Checked = FlashChecked;
        }

        public void AllChecked_Click(object obj)
        {
            foreach (var checkModel in CheckModels)
                checkModel.Checked = AllChecked;
        }

        /// <summary>
        ///     卡包初始化
        /// </summary>
        private void InitPack()
        {
            WebPackDic = new ObservableCollection<KeyValuePair<string, string>>();
            var packJsonPath = Environment.CurrentDirectory + "\\ImagePackJson";
            if (!File.Exists(packJsonPath))
                FileUtils.SaveFile(packJsonPath, JsonUtils.Serializer(new List<KeyValuePair<string, string>>()));
            var packJson = FileUtils.GetFileContent(packJsonPath);
            var packList = JsonUtils.Deserialize<List<KeyValuePair<string, string>>>(packJson);
            WebPackDic.Add(new KeyValuePair<string, string>(string.Empty, StringConst.NotApplicable));
            packList.ForEach(WebPackDic.Add);
        }

        /// <summary>
        ///     卡包解析
        /// </summary>
        /// <param name="obj"></param>
        public async void PackAsync_Click(object obj)
        {
            await DialogHost.Show(new DialogProgress("获取信息中"), (object s, DialogOpenedEventArgs e) =>
            {
                Task.Run(() =>
                {
                    const string url = "http://www.orenoturn.com/?mode=cate&csid=0&cbid=2180525";
                    var web = new HtmlWeb {OverrideEncoding = Encoding.GetEncoding("EUC-JP")};
                    var frame = web.Load(url);
                    var nodes =
                        frame.DocumentNode.SelectNodes(@"//div")
                            .First(x => (null != x.Attributes["id"]) && x.Attributes["id"].Value.Equals("main"))
                            .SelectNodes(@"//a")
                            .Where(
                                x =>
                                    (null != x.Attributes["href"]) &&
                                    x.Attributes["href"].Value.Contains("?mode=cate&cbid=2180525") &&
                                    (null != x.ParentNode.Attributes["class"]) &&
                                    x.ParentNode.Attributes["class"].Value.Equals("name"))
                            .ToList();
                    var packList =
                        nodes.Select(x => new KeyValuePair<string, string>(x.Attributes["href"].Value, x.InnerText))
                            .ToList();
                    var packJsonPath = Environment.CurrentDirectory + "\\ImagePackJson";
                    FileUtils.SaveFile(packJsonPath, JsonUtils.Serializer(packList));
                    return packList;
                }).ToObservable().ObserveOnDispatcher().Subscribe(result =>
                {
                    WebPackDic.Clear();
                    result.Insert(0, new KeyValuePair<string, string>(string.Empty, StringConst.NotApplicable));
                    result.ForEach(WebPackDic.Add);
                    e.Session.Close();
                    BaseDialogUtils.ShowDialogAuto(StringConst.UpdateSucceed);
                });
            });
        }

        public async void AnalysisImage_Click(object obj)
        {
            var url = $"http://www.orenoturn.com/{WebPackValue}";
            await DialogHost.Show(new DialogProgress("解析中..."), (object s, DialogOpenedEventArgs e) =>
            {
                GetData(url).ObserveOnDispatcher().Subscribe(result =>
                {
                    CheckModels.Clear();
                    result.ForEach(model => CheckModels.Add(new CheckModel<ImageModel>(model)));
                    e.Session.Close();
                });
            });
        }

        public bool DownloadImage(string url, string path)
        {
            try
            {
                var cln = new WebClient();
                cln.DownloadFile(url, path);
            }
            catch (Exception e)
            {
                LogUtils.Write($"DownloadImage Failed:{e.Message}");
                return false;
            }
            return true;
        }

        private IObservable<List<ImageModel>> GetData(string url)
        {
            return Task.Run(() =>
            {
                var imageModels = new List<ImageModel>();
                var doc = new HtmlWeb {OverrideEncoding = Encoding.GetEncoding("EUC-JP")}.Load(url);
                // 解析卡片
                var nodes = doc.DocumentNode.SelectNodes(@"//div")
                    .Where(
                        x =>
                            (null != x.Attributes["class"]) && x.Attributes["class"].Value.Equals("product_item") &&
                            (null != x.Attributes["align"]) && x.Attributes["align"].Value.Equals("center"))
                    .ToList();
                foreach (var node in nodes)
                {
                    doc = new HtmlDocument();
                    doc.LoadHtml(node.InnerHtml);
                    var imageUrl = "http:" + doc.DocumentNode
                                       .SelectNodes(@"//img")
                                       .First(x => null != x.Attributes["src"])
                                       .Attributes["src"].Value.Trim();
                    var imageHref = "http://www.orenoturn.com/" + doc.DocumentNode
                                        .SelectNodes(@"//a")
                                        .First(x => null != x.Attributes["href"])
                                        .Attributes["href"].Value.Trim();
                    var number = doc.DocumentNode
                        .SelectNodes(@"//font")
                        .First(x => null != x.Attributes["size"])
                        .InnerText.Trim();
                    var name = doc.DocumentNode
                        .SelectNodes(@"//div")
                        .First(x => (null != x.Attributes["align"]) && x.Attributes["align"].Value.Equals("center"))
                        .LastChild.InnerText.Trim().Replace("&nbsp;", "");
                    var model = new ImageModel
                    {
                        ImageUrl = imageUrl,
                        ImageHref = imageHref,
                        Name = name,
                        Number = number
                    };
                    imageModels.Add(model);
                }
                return imageModels.OrderBy(x => x.Number).ToList();
            }).ToObservable();
        }
    }
}