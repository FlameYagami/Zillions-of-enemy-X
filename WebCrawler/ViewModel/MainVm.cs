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
using System.Windows;
using Dialog;
using Dialog.View;
using HtmlAgilityPack;
using MaterialDesignThemes.Wpf;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace WebCrawler.ViewModel
{
    public class MainVm : BaseModel
    {
        private string _packValue;
        public string PackValue { get { return _packValue; } set { _packValue = value;OnPropertyChanged(nameof(PackValue)); } }
        public ObservableCollection<CardModel> CardModels { get; set; }
        public ObservableCollection<string> PackList { get; set; }

        public MainVm()
        {
            CardModels = new ObservableCollection<CardModel>();
            CmdAnalyze = new DelegateCommand {ExecuteCommand = Analyze_Click};
            CmdExit = new DelegateCommand {ExecuteCommand = Exit_Click };
            CmdPackAsync = new DelegateCommand {ExecuteCommand = PackAsync_Click };
            InitPack();
            LogUtils.Show();
        }

        public DelegateCommand CmdPackAsync { get; set; }
        public DelegateCommand CmdExit { get; set; }
        public DelegateCommand CmdAllChecked { get; set; }
        public DelegateCommand CmdAnalyze { get; set; }

        /// <summary>
        /// 同步卡包
        /// </summary>
        /// <param name="obj"></param>
        public async void PackAsync_Click(object obj)
        {
            await DialogHost.Show(new DialogProgress("获取信息中"), (s, e) =>
            {
                Task.Run(() =>
                {
                    const string url = "https://www.zxtcg.com/card/card_search.php";
                    var web = new HtmlWeb();
                    var frame = web.Load(url);
                    var nodes = frame.DocumentNode.SelectNodes(@"//select").First(x => x.Attributes["name"].Value.Equals("pn"));
                    var packList = nodes.InnerText.Trim().Split(new[] {"\r\n"}, StringSplitOptions.None).ToList();
                    packList = packList.Where(str => !string.IsNullOrWhiteSpace(str)).ToList();
                    packList.RemoveAt(0);
                    var packJsonPath = Environment.CurrentDirectory + "\\PackJson";
                    FileUtils.SaveFile(packJsonPath, JsonUtils.Serializer(packList));
                    return packList;
                }).ToObservable().ObserveOnDispatcher().Subscribe(result =>
                {
                    PackList.Clear();
                    result.Insert(0, StringConst.NotApplicable);
                    result.ForEach(PackList.Add);
                    e.Session.Close(false);
                    BaseDialogUtils.ShowDialogAuto(StringConst.UpdateSucceed);
                });
            }, (s, e) => {  });
        }

        private void InitPack()
        {
            PackList = new ObservableCollection<string>();
            var packJsonPath = Environment.CurrentDirectory + "\\PackJson";
            if (!File.Exists(packJsonPath))
                FileUtils.SaveFile(packJsonPath, JsonUtils.Serializer(new List<string>()));
            var packJson = FileUtils.GetFileContent(packJsonPath);
            var packList = JsonUtils.Deserialize<List<string>>(packJson);
            PackList.Add(StringConst.NotApplicable);
            packList.ForEach(PackList.Add);
        }

        public void Exit_Click(object obj)
        {
            Environment.Exit(0);
        }

        public async void Analyze_Click(object obj)
        {
            if (PackValue.Equals(string.Empty))
            {
                BaseDialogUtils.ShowDialogOk("请选择卡包");
                return;
            }

            await DialogHost.Show(new DialogProgress("解析中..."), (s, e) =>
            {
                Task.Run(() =>
                {
                    var searchUrl = "https://www.zxtcg.com/card/?fwcn=1&fwil=1&fwct=1&fwft=1&pn=" + PackValue;
                    var doc = new HtmlWeb().Load(searchUrl);

                    // 总数
//                    var countInnerText =
//                        doc.DocumentNode.SelectNodes(@"//div")
//                            .First(x => x.Attributes["class"].Value.Equals("countRule"))
//                            .InnerText;
                    var pageCount = doc.DocumentNode
                        .SelectNodes(@"//ul")
                        .Where(x => x.Attributes["class"] != null)
                        .First(node => node.Attributes["class"].Value.Equals("pageLinkPC"))
                        .InnerText;
                    Console.WriteLine(pageCount);

//                    var count = countInnerText.Substring(countInnerText.IndexOf("全", StringComparison.Ordinal) + 1,
//                        countInnerText.Length - countInnerText.IndexOf("件", StringComparison.Ordinal));
//
//                    Console.WriteLine(count);



                    //                    var body = doc.DocumentNode.SelectNodes(@"//tbody")[0];
                    //                    var childNodes = body.ChildNodes;
                    //                    var childNodesInnerHtml = (from childNode in childNodes where !string.IsNullOrWhiteSpace(childNode.InnerHtml) select childNode.InnerHtml).ToList();

                    //                    var cardFlag = false;
                    //                    var cardModel = new CardModel();
                    //                    var cardModels = new List<CardModel>();
                    //                    foreach (var childNodeInnerHtml in childNodesInnerHtml)
                    //                    {
                    //                        doc = new HtmlDocument();
                    //                        doc.LoadHtml(childNodeInnerHtml);
                    //                        var nodes = doc.DocumentNode.SelectNodes(@"//td");
                    //                        if (5 == nodes.Count)
                    //                        {
                    //                            cardFlag = true;
                    //                            cardModel = new CardModel();
                    //                            int cost;
                    //                            int power;
                    //                            cardModel.JName = nodes[1].InnerText;
                    //                            cardModel.Cost = int.TryParse(nodes[2].InnerText, out cost) ? cost : -1;
                    //                            cardModel.Power = int.TryParse(nodes[3].InnerText, out power) ? power : -1;
                    //                            continue;
                    //                        }
                    //                        if (!cardFlag || 4 != nodes.Count) continue;
                    //                        cardModel.Number = nodes[0].InnerText;
                    //                        var url = $"https://www.zxtcg.com/card/card_detail.php?n=" + cardModel.Number + "-00";
                    //                        var web = new HtmlWeb();
                    //                        var frame = web.Load(url);
                    //                        var baseInfoNodes = frame.DocumentNode.SelectNodes(@"//table")[0].SelectNodes(@"//thead")[0].SelectNodes(@"//tr");
                    //                        var tbodyInfoNodes = frame.DocumentNode.SelectNodes(@"//table")[0].SelectNodes(@"//tbody")[0];
                    //                        var baList = baseInfoNodes[3].InnerText.Trim().Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
                    //                        var exList = tbodyInfoNodes.InnerText.Trim().Split(new[] { "\r\n" }, StringSplitOptions.None).ToList();
                    //                        baList = baList.Where(str => !string.IsNullOrWhiteSpace(str)).ToList();
                    //                        exList = exList.Where(str => !string.IsNullOrWhiteSpace(str)).ToList();
                    //                        cardModel.Illust = baList[1];
                    //                        cardModel.Lines = exList[1];
                    //                        cardModels.Add(cardModel);
                    //                        cardFlag = false;
                    //                    }
                    //                    return cardModels;
                }).ToObservable().ObserveOnDispatcher().Subscribe(result =>
                {
//                    CardModels.Clear();
//                    result.ForEach(CardModels.Add);
                    e.Session.Close(false);
                    BaseDialogUtils.ShowDialogAuto("解析成功");
                });
            }, (s, e) => { });
        }
    }
}