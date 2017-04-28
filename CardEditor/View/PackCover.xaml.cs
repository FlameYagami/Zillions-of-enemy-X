﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using CardEditor.Constant;
using CardEditor.Entity;
using CardEditor.Model;
using CardEditor.Utils;
using Dialog;
using CheckBox = System.Windows.Controls.CheckBox;

namespace CardEditor.View
{
    /// <summary>
    ///     PackCover.xaml 的交互逻辑
    /// </summary>
    public partial class PackCover : Window
    {
        public PackCover()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnCover_Click(object sender, RoutedEventArgs e)
        {
            var filePath = TxtFilePath.Text.Trim();
            if (filePath.Equals(""))
            {
                BaseDialogUtils.ShowDlg("源文件不存在");
                return;
            }
            var packName = TxtPackName.Text.Trim();
            if (packName.Equals(""))
            {
                BaseDialogUtils.ShowDlg("请输入卡包名称");
                return;
            }
            // 获取源文件所有的信息
            var dtSource = new DataSet();
            var isImport = ExcelHelper.ImportExcelToDataTable(filePath, packName, dtSource);
            if (!isImport)
            {
                BaseDialogUtils.ShowDlg("文件中数据异常");
                return;
            }
            // 确认状态
            if (!BaseDialogUtils.ShowDlgOkCancel("确认覆写?"))
                return;
            // 获取源文件编号
            var dtNumberList =
                dtSource.Tables[0].Rows.Cast<DataRow>().Select(column => column["编号"].ToString()).ToList();
            var sourceCardEntitys = GetSourceCardEntities(dtSource);
            // 获取覆写所有的信息
            var dataCardEntitys =
                dtNumberList.Select(CardUtils.GetCardEntity).ToList();
            var selectColumnList = GetSelectColumnList();
            // 填充覆写的数据
            for (var i = 0; i != dataCardEntitys.Count; i++)
            {
                var dataNumber = dataCardEntitys[i].Number;
                foreach (var dtSourceCardEntity in sourceCardEntitys)
                {
                    if (!dtSourceCardEntity.Number.Equals(dataNumber)) continue;
                    if (selectColumnList.Contains("种类"))
                        dataCardEntitys[i].Type = dtSourceCardEntity.Type;
                    if (selectColumnList.Contains("色"))
                        dataCardEntitys[i].Camp = dtSourceCardEntity.Camp;
                    if (selectColumnList.Contains("种族"))
                        dataCardEntitys[i].Race = dtSourceCardEntity.Race;
                    if (selectColumnList.Contains("标记"))
                        dataCardEntitys[i].Sign = dtSourceCardEntity.Sign;
                    if (selectColumnList.Contains("罕贵度"))
                        dataCardEntitys[i].Race = dtSourceCardEntity.Race;
                    if (selectColumnList.Contains("卡片名_中"))
                        dataCardEntitys[i].CName = dtSourceCardEntity.CName;
                    if (selectColumnList.Contains("COST"))
                        dataCardEntitys[i].Cost = dtSourceCardEntity.Cost;
                    if (selectColumnList.Contains("力量"))
                        dataCardEntitys[i].Power = dtSourceCardEntity.Power;
                    if (selectColumnList.Contains("能力_中"))
                        dataCardEntitys[i].Ability = dtSourceCardEntity.Ability;
                }
            }
            // 生成覆写的数据库语句集合
            var queryModel = new Query();
            var updateSqlList = dataCardEntitys
                .Select(cardEntity => queryModel.GetUpdateSql(cardEntity, cardEntity.Number))
                .ToList();
            // 数据库覆写
            var isExecute = SqliteUtils.Execute(updateSqlList);
            BaseDialogUtils.ShowDlg(isExecute ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
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

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
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

        private List<CardEntity> GetSourceCardEntities(DataSet dataSet)
        {
            return dataSet.Tables[0].Rows.Cast<DataRow>().Select(row => new CardEntity
            {
                Type = row["种类"].ToString(),
                Camp = row["色"].ToString(),
                Race = row["种族"].ToString(),
                Sign = row["标记"].ToString().Equals("IG") ? "点燃" : row["标记"].ToString(),
                Rare = row["罕贵度"].ToString(),
                CName = row["卡片名_中"].ToString(),
                Cost = row["COST"].ToString().Equals("-") ? string.Empty : row["COST"].ToString(),
                Power = row["力量"].ToString().Equals("-") ? string.Empty : row["力量"].ToString(),
                Number = row["编号"].ToString(),
                Ability = row["能力_中"].ToString()
            }).ToList();
        }
    }
}