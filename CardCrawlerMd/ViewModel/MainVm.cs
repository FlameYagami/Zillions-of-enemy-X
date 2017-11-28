using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Common;
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
        private bool _allChecked;
        private string _analyseHint;
        private IDisposable _dispose;
        private string _packValue;
        private int _pageCount = 1;

        public MainVm()
        {
            CheckModels = new ObservableCollection<CheckModel<CardModel>>();
            CmdAnalyze = new DelegateCommand {ExecuteCommand = Analyze_Click};
            CmdExit = new DelegateCommand {ExecuteCommand = Exit_Click};
            CmdPackAsync = new DelegateCommand {ExecuteCommand = PackAsync_Click};
            CmdAllChecked = new DelegateCommand {ExecuteCommand = AllChecked_Click};
            CmdCover = new DelegateCommand {ExecuteCommand = CmdCover_Click};
            InitPack();
//            LogUtils.Show();
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

        public string PackValue
        {
            get { return _packValue; }
            set
            {
                _packValue = value;
                OnPropertyChanged(nameof(PackValue));
            }
        }

        public string AnalyseHint
        {
            get { return _analyseHint; }
            set
            {
                _analyseHint = value;
                OnPropertyChanged(nameof(AnalyseHint));
            }
        }

        public ObservableCollection<CheckModel<CardModel>> CheckModels { get; set; }
        public ObservableCollection<string> PackList { get; set; }

        public DelegateCommand CmdPackAsync { get; set; }
        public DelegateCommand CmdExit { get; set; }
        public DelegateCommand CmdAllChecked { get; set; }
        public DelegateCommand CmdAnalyze { get; set; }
        public DelegateCommand CmdCover { get; set; }

        public async void CmdCover_Click(object obj)
        {
            var cardModels = GetChedkedCardModels();
            if (0 == cardModels.Count)
            {
                BaseDialogUtils.ShowDialogOk("请勾选要覆写的数据");
                return;
            }
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.CoverHint)) return;
            // 以解析卡包前三位文字作为查询条件
            var packLike = PackValue.Substring(0, 3);
            var tempPack = CardUtils.GetPackList().FirstOrDefault(pack => pack.Contains(packLike));
            if (string.IsNullOrWhiteSpace(tempPack))
            {
                BaseDialogUtils.ShowDialogOk("无法查询到对应的卡包");
                return;
            }
            // 为覆写的卡牌赋予卡包属性
            cardModels.ForEach(cardModel => cardModel.Pack = tempPack);
            var allNumberList = CardUtils.GetAllNumberList();
            // 获取添加、更新Sql语句集合
            var sqlList =
                cardModels.Select(
                    checkedModel =>
                        allNumberList.Contains(checkedModel.Number)
                            ? GetUpdateSql(checkedModel)
                            : GetAddSql(checkedModel)).ToList();
            var result = DataManager.Execute(sqlList);
            BaseDialogUtils.ShowDialogAuto(result ? StringConst.CoverSucceed : StringConst.CoverFailed);
        }

        private static string GetAddSql(CardModel card)
        {
            var cost = card.Cost.Equals(-1) ? string.Empty : card.Cost.ToString();
            var power = card.Power.Equals(-1) ? string.Empty : card.Power.ToString();

            var builder = new StringBuilder();
            builder.Append("INSERT INTO " + SqliteConst.TableName);
            builder.Append(" (" + SqliteConst.ColumnMd5 + "," +
                           SqliteConst.ColumnType + "," +
                           SqliteConst.ColumnCamp + "," +
                           SqliteConst.ColumnSign + "," +
                           SqliteConst.ColumnRare + "," +
                           SqliteConst.ColumnJName + "," +
                           SqliteConst.ColumnIllust + "," +
                           SqliteConst.ColumnNumber + "," +
                           SqliteConst.ColumnCost + "," +
                           SqliteConst.ColumnPower + "," +
                           SqliteConst.ColumnPack + "," +
                           SqliteConst.ColumnLines + "," +
                           SqliteConst.ColumnImage + ")");
            builder.Append("VALUES(");
            builder.Append($"'{Md5Utils.GetMd5(card.JName + cost + power)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Type)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Camp)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Sign)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Rare)}',");
            builder.Append($"'{card.JName}',");
            builder.Append($"'{card.Illust}',");
            builder.Append($"'{card.Number}',");
            builder.Append($"'{card.Cost}',");
            builder.Append($"'{card.Power}',");
            builder.Append($"'{card.Pack}',");
            builder.Append($"'{card.Lines}',");
            builder.Append($"'{JsonUtils.Serializer(new List<string> {card.Number})}'");
            // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        private static string GetUpdateSql(CardModel card)
        {
            var cost = card.Cost.Equals(-1) ? string.Empty : card.Cost.ToString();
            var power = card.Power.Equals(-1) ? string.Empty : card.Power.ToString();

            var builder = new StringBuilder();
            builder.Append($"UPDATE {SqliteConst.TableName} SET ");
            builder.Append(
                $"{SqliteConst.ColumnMd5}='{Md5Utils.GetMd5(card.JName + cost + power)}',");
            builder.Append($"{SqliteConst.ColumnType}='{card.Type}',");
            builder.Append($"{SqliteConst.ColumnCamp}= '{card.Camp}',");
            builder.Append($"{SqliteConst.ColumnSign}= '{card.Sign}',");
            builder.Append($"{SqliteConst.ColumnRare}= '{card.Rare}',");
            builder.Append($"{SqliteConst.ColumnJName}= '{card.JName}',");
            builder.Append($"{SqliteConst.ColumnIllust}= '{card.Illust}',");
            builder.Append($"{SqliteConst.ColumnCost}= '{cost}',");
            builder.Append($"{SqliteConst.ColumnPower}= '{power}',");
            builder.Append($"{SqliteConst.ColumnPack}= '{card.Pack}',");
            builder.Append($"{SqliteConst.ColumnLines}= '{card.Lines}',");
            builder.Append($"{SqliteConst.ColumnImage}= '{JsonUtils.Serializer(new List<string> {card.Number})}'");
            builder.Append($" WHERE {SqliteConst.ColumnNumber}='{card.Number}'");
            return builder.ToString();
        }

        public void AllChecked_Click(object obj)
        {
            foreach (var checkModel in CheckModels)
                checkModel.Checked = AllChecked;
        }

        public List<CardModel> GetChedkedCardModels()
        {
            return CheckModels.Where(model => model.Checked).Select(model => model.Model).ToList();
        }

        /// <summary>
        ///     同步卡包
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
                    var nodes =
                        frame.DocumentNode.SelectNodes(@"//select").First(x => x.Attributes["name"].Value.Equals("pn"));
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
            }, (s, e) => { });
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
            _pageCount = 1;
            await DialogHost.Show(new DialogProgress("解析中..."), (s, e) =>
            {
                AnalyseHint = string.Empty;
                var searchUrl = GetUrl(0);
                GetData(searchUrl, true).ObserveOnDispatcher().Subscribe(result =>
                {
                    var taskDic = new Dictionary<int, bool> {{1, true}};
                    CheckModels.Clear();
                    result.ForEach(cardModel => CheckModels.Add(new CheckModel<CardModel>(cardModel)));
                    e.Session.Close(false);
                    BaseDialogUtils.ShowDialogAuto("第1页解析成功");
                    AnalyseHint = $"开始执行后续解析...";
                    // 异步解析后续编号
                    for (var i = 2; i != _pageCount + 1; i++)
                    {
                        var page = i;
                        taskDic.Add(page, false);
                        Parallel.Invoke(() => GetData(GetUrl(page)).ObserveOnDispatcher().Subscribe(models =>
                        {
                            models.ForEach(cardModel => CheckModels.Add(new CheckModel<CardModel>(cardModel)));
                            AnalyseHint = $"第{page}页解析完成";
                            taskDic[page] = true;
                        }));
                    }
                    _dispose = Observable.Interval(TimeSpan.FromSeconds(1)).ObserveOnDispatcher().Subscribe(time =>
                    {
                        if (_pageCount != taskDic.Values.Count(flag => flag)) return;
                        var tempCardModels = CheckModels.OrderBy(m => m.Model.Number).ToList();
                        CheckModels.Clear();
                        tempCardModels.ForEach(CheckModels.Add);
                        AnalyseHint = $"全部解析完成";
                        _dispose.Dispose();
                    });
                });
            }, (s, e) => { });
        }

        private IObservable<List<CardModel>> GetData(string url, bool isPage = false)
        {
            return Task.Run(() =>
            {
                var doc = new HtmlWeb().Load(url);

                // 解析总数
                if (isPage)
                    _pageCount = AnalyseUtils.AnalysePageCount(doc.DocumentNode.SelectNodes(@"//div"))/30 + 1;
                // 解析卡片
                var body = doc.DocumentNode.SelectNodes(@"//tbody")[0];
                var childNodes = body.ChildNodes;
                var childNodesInnerHtml =
                (from childNode in childNodes
                    where !string.IsNullOrWhiteSpace(childNode.InnerHtml)
                    select childNode.InnerHtml).ToList();

                var cardModel = new CardModel();
                var cardModels = new List<CardModel>();
                var rowCount = 0;
                var tempNumber = "";
                foreach (var childNodeInnerHtml in childNodesInnerHtml)
                {
                    doc = new HtmlDocument();
                    doc.LoadHtml(childNodeInnerHtml);
                    HtmlNodeCollection nodes;
                    // 第一行
                    if (0 == rowCount%3)
                    {
                        nodes = doc.DocumentNode.SelectNodes(@"//td");
                        cardModel = new CardModel
                        {
                            JName = AnalyseUtils.AnalyseJName(nodes),
                            Cost = AnalyseUtils.AnalyseCost(nodes),
                            Power = AnalyseUtils.AnalysePower(nodes)
                        };
                        rowCount++;
                        continue;
                    }
                    // 第二行
                    if (1 == rowCount%3)
                    {
                        nodes = doc.DocumentNode.SelectNodes(@"//td");
                        // 卡编
                        cardModel.Number = AnalyseUtils.AnalyseNumber(nodes);
                        // 标记
                        cardModel.Sign = AnalyseUtils.AnalyseSign(nodes);
                        var web = new HtmlWeb();
                        var frame = web.Load($"https://www.zxtcg.com/card/card_detail.php?n={cardModel.Number}-00");
                        // 阵营
                        cardModel.Camp = AnalyseUtils.AnalyseCamp(frame);
                        // 罕贵
                        cardModel.Rare = AnalyseUtils.AnalyseRare(frame);
                        // 种类
                        cardModel.Type = AnalyseUtils.AnalyseType(frame);
                        // 台词
                        cardModel.Lines = AnalyseUtils.AnalyseLines(frame);
                        rowCount++;
                        continue;
                    }
                    // 第三行
                    if (2 == rowCount%3)
                    {
                        nodes = doc.DocumentNode.SelectNodes(@"//td");
                        // 种族
                        cardModel.Race = AnalyseUtils.AnalyseRace(nodes);
                        // 画师
                        cardModel.Illust = AnalyseUtils.AnalyseIllust(nodes);
                        if (tempNumber != cardModel.Number)
                            cardModels.Add(cardModel);
                        tempNumber = cardModel.Number;
                        rowCount++;
                    }
                }
                return cardModels;
            }).ToObservable();
        }

        private string GetUrl(int pageCount = 0)
        {
            return 0 == pageCount 
                ? $"https://www.zxtcg.com/card/?fwcn=1&fwil=1&fwct=1&fwft=1&pn={PackValue}" 
                : $"https://www.zxtcg.com/card/?page={pageCount}&fwcn=1&fwil=1&fwct=1&fwft=1&pn={PackValue}";
        }
    }
}