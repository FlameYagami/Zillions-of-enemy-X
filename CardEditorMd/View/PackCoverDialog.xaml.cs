using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using CardEditor.Model;
using Common;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;
using CheckBox = System.Windows.Controls.CheckBox;

namespace CardEditor.View
{
    /// <summary>
    ///     PackCoverWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PackCoverWindow
    {
        public PackCoverWindow()
        {
            InitializeComponent();
            DialogHost.Identifier = StringConst.SecondaryDialogHost;
        }

        private async void BtnCover_Click(object sender, RoutedEventArgs e)
        {
            var filePath = TxtFilePath.Text.Trim();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                BaseDialogUtils.ShowDialogOk("源文件不存在", StringConst.SecondaryDialogHost);
                return;
            }
            var packName = TxtPackName.Text.Trim();
            if (string.IsNullOrWhiteSpace(packName))
            {
                BaseDialogUtils.ShowDialogOk("请输入卡包名称", StringConst.SecondaryDialogHost);
                return;
            }
            // 获取源文件所有的信息
            var dtSource = new DataSet();
            var isImport = ExcelHelper.ImportExcelToDataTable(filePath, packName, dtSource);
            if (!isImport)
            {
                BaseDialogUtils.ShowDialogOk("文件中数据异常", StringConst.SecondaryDialogHost);
                return;
            }
            // 确认状态
            if (!await BaseDialogUtils.ShowDialogConfirm("确认覆写?", StringConst.SecondaryDialogHost))
                return;
            // 获取源文件编号
            var dtNumberList =
                dtSource.Tables[0].Rows.Cast<DataRow>().Select(column => column["编号"].ToString()).ToList();
            var sourceList = GetSourceCardModelList(dtSource);
            // 获取覆写所有的信息
            var dataList =
                dtNumberList.Select(CardUtils.GetCardModel).ToList();
            var selectColumnList = GetSelectColumnList();
            // 填充覆写的数据
            for (var i = 0; i != dataList.Count; i++)
            {
                var dataNumber = dataList[i].Number;
                foreach (var editorModel in sourceList)
                {
                    if (!editorModel.Number.Equals(dataNumber)) continue;
                    if (selectColumnList.Contains("种类"))
                        dataList[i].Type = editorModel.Type;
                    if (selectColumnList.Contains("色"))
                        dataList[i].Camp = editorModel.Camp;
                    if (selectColumnList.Contains("种族"))
                        dataList[i].Race = editorModel.Race;
                    if (selectColumnList.Contains("标记"))
                        dataList[i].Sign = editorModel.Sign;
                    if (selectColumnList.Contains("罕贵度"))
                        dataList[i].Race = editorModel.Race;
                    if (selectColumnList.Contains("卡片名_中"))
                        dataList[i].CName = editorModel.CName;
                    if (selectColumnList.Contains("COST"))
                        dataList[i].Cost = int.Parse(editorModel.CostValue);
                    if (selectColumnList.Contains("力量"))
                        dataList[i].Power = int.Parse(editorModel.PowerValue);
                    if (selectColumnList.Contains("能力_中"))
                        dataList[i].Ability = editorModel.Ability;
                }
            }
            // 生成覆写的数据库语句集合
            var updateSqlList = dataList
                .Select(cardEntity => GetUpdateSql(cardEntity, cardEntity.Number))
                .ToList();
            // 数据库覆写
            var isExecute = DataManager.Execute(updateSqlList);
            BaseDialogUtils.ShowDialogAuto(isExecute ? StringConst.UpdateSucceed : StringConst.UpdateFailed, StringConst.SecondaryDialogHost);
        }

        private string GetUpdateSql(CardModel card, string number)
        {
            var builder = new StringBuilder();
            builder.Append($"UPDATE {SqliteConst.TableName} SET ");
            builder.Append($"{SqliteConst.ColumnMd5}='{Md5Utils.GetMd5(card.JName + card.Cost + card.Power)}',");
            builder.Append($"{SqliteConst.ColumnType}='{card.Type}',");
            builder.Append($"{SqliteConst.ColumnCamp}= '{card.Camp}',");
            builder.Append($"{SqliteConst.ColumnRace}= '{card.Race}',");
            builder.Append($"{SqliteConst.ColumnSign}= '{card.Sign}',");
            builder.Append($"{SqliteConst.ColumnRare}= '{card.Rare}',");
            builder.Append($"{SqliteConst.ColumnPack}= '{card.Pack}',");
            builder.Append($"{SqliteConst.ColumnCName}= '{card.CName}',");
            builder.Append($"{SqliteConst.ColumnJName}= '{card.JName}',");
            builder.Append($"{SqliteConst.ColumnIllust}= '{card.Illust}',");
            builder.Append($"{SqliteConst.ColumnNumber}= '{card.Number}',");
            builder.Append($"{SqliteConst.ColumnCost}= '{card.Cost}',");
            builder.Append($"{SqliteConst.ColumnPower}= '{card.Power}',");
            builder.Append($"{SqliteConst.ColumnAbility}= '{card.Ability}',");
            builder.Append($"{SqliteConst.ColumnLines}= '{card.Lines}',");
            builder.Append($"{SqliteConst.ColumnImage}= '{card.ImageJson}'");
            // 详细能力处理
            builder.Append($" WHERE {SqliteConst.ColumnNumber}='{number}'");
            return builder.ToString();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = @"xls文件(*.xls)|*.xls|xlsm文件(*.xlsm)|*.xlsm"
            };
            TxtFilePath.Text = ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK ? string.Empty : ofd.FileName;
        }

        private List<string> GetSelectColumnList()
        {
            var columnList = new List<string>();
            for (var i = 0; i != LstColumn.Items.Count; i++)
            {
                var isChecked = ((CheckBox) LstColumn.Items[i]).IsChecked;
                if ((isChecked != null) && (bool) isChecked)
                    columnList.Add(((CheckBox) LstColumn.Items[i]).Tag.ToString());
            }
            return columnList;
        }

        private static List<CeSearchModel> GetSourceCardModelList(DataSet dataSet)
        {
            return dataSet.Tables[0].Rows.Cast<DataRow>().Select(row => new CeSearchModel
            {
                Type = row["种类"].ToString(),
                Camp = row["色"].ToString(),
                Race = row["种族"].ToString(),
                Sign = row["标记"].ToString().Equals("IG") ? "点燃" : row["标记"].ToString(),
                Rare = row["罕贵度"].ToString(),
                CName = row["卡片名_中"].ToString(),
                CostValue = row["COST"].ToString().Equals("-") ? "0" : row["COST"].ToString(),
                PowerValue = row["力量"].ToString().Equals("-") ? "0" : row["力量"].ToString(),
                Number = row["编号"].ToString(),
                Ability = row["能力_中"].ToString()
            }).ToList();
        }
    }
}