using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using CardEditor.Model;
using CardEditor.Utils;
using Dialog;
using Wrapper;
using Wrapper.Constant;
using Wrapper.Model;
using Wrapper.Utils;

namespace CardEditor.ViewModel
{
    public class CardEditorVm : BaseModel
    {
        private readonly CardPictureVm _cardPictureVm;
        private readonly CardPreviewVm _cardPreviewVm;
        private readonly ExternQueryVm _externOpertaionVm;

        public CardEditorVm(ExternQueryVm externOpertaionVm, CardPreviewVm cardPreviewVm,
            CardPictureVm cardPictureVm)
        {
            _externOpertaionVm = externOpertaionVm;
            _cardPreviewVm = cardPreviewVm;
            _cardPictureVm = cardPictureVm;

            CmdQuery = new DelegateCommand {ExecuteCommand = Query_Click};
            CmdReset = new DelegateCommand {ExecuteCommand = Reset_Click};
            CmdAdd = new DelegateCommand {ExecuteCommand = Add_Click};
            CmdUpdate = new DelegateCommand {ExecuteCommand = Update_Click};
            CmdDelete = new DelegateCommand {ExecuteCommand = Delete_Click};
            CmdExport = new DelegateCommand {ExecuteCommand = Export_Click};

            RaceList = new ObservableCollection<string>();
            PackList = new ObservableCollection<string>();
            AbilityTypeModels = new ObservableCollection<AbilityModel>();
            CardUtils.GetPackList().ForEach(PackList.Add);
            CardUtils.GetPartRace(StringConst.NotApplicable).ForEach(RaceList.Add);

            CardEditorModel = new CardEditorModel();
            UpdateAbilityType(CardEditorModel.AbilityTypeModels);
        }

        public DelegateCommand CmdQuery { get; set; }
        public DelegateCommand CmdReset { get; set; }
        public DelegateCommand CmdAdd { get; set; }
        public DelegateCommand CmdUpdate { get; set; }
        public DelegateCommand CmdDelete { get; set; }
        public DelegateCommand CmdExport { get; set; }

        public CardEditorModel CardEditorModel { get; set; }
        public ObservableCollection<string> PackList { get; set; }
        public ObservableCollection<string> RaceList { get; set; }
        public ObservableCollection<AbilityModel> AbilityTypeModels { get; set; }

        public void Export_Click(object obj)
        {
            var pack = CardEditorModel.Pack;
            if (pack.Equals(StringConst.NotApplicable) || pack.Contains(StringConst.Series))
            {
                BaseDialogUtils.ShowDialogOk(StringConst.PackChoiceNone);
                return;
            }
            var exportPath = DialogUtils.ShowExport(pack);
            if (exportPath.Equals(string.Empty)) return;

            var sql = SqlUtils.GetExportSql(pack);
            var dataSet = new DataSet();
            if (!SqliteUtils.FillDataToDataSet(sql, dataSet)) return;

            var isExport = ExcelHelper.ExportPackToExcel(exportPath, dataSet);
            BaseDialogUtils.ShowDialogAuto(isExport ? StringConst.ExportSucceed : StringConst.ExportFailed);
        }

        /// <summary>
        ///     卡牌添加事件
        /// </summary>
        public async void Add_Click(object obj)
        {
            // 卡编是否重复判断
            if (CardUtils.IsNumberExist(CardEditorModel.Number))
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardIsExitst);
                return;
            }
            // 添加确认
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.AddConfirm)) return;
            // 数据库添加
            var addSql = GetAddSql(CardEditorModel);
            var isAdd = SqliteUtils.Execute(addSql);
            BaseDialogUtils.ShowDialogAuto(isAdd ? StringConst.AddSucceed : StringConst.AddFailed);
            // 数据库更新
            if (!isAdd) return;
            DataCache.DsAllCache.Clear();
            SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     卡牌删除事件
        /// </summary>
        public async void Delete_Click(object obj)
        {
            // 选择判空
            var selectedItem = _cardPreviewVm.SelectedItem;
            if (null == selectedItem)
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardChioceNone);
                return;
            }
            // 删除确认
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.DeleteConfirm)) return;
            // 数据库删除
            var deleteSql = GetDeleteSql(selectedItem.Number);
            var isDelete = SqliteUtils.Execute(deleteSql);
            BaseDialogUtils.ShowDialogAuto(isDelete ? StringConst.DeleteSucceed : StringConst.DeleteFailed);
            // 数据库更新
            if (!isDelete) return;
            DataCache.DsAllCache.Clear();
            SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
            // 删除数据模型
            _cardPreviewVm.MemoryQueryModel = null;
        }

        public void UpdateAbilityType(ObservableCollection<AbilityModel> abilityTypeModels)
        {
            AbilityTypeModels = abilityTypeModels;
            OnPropertyChanged(nameof(AbilityTypeModels));
        }

        /// <summary>
        ///     卡牌更新事件
        /// </summary>
        public async void Update_Click(object obj)
        {
            // 选择判空
            var selectedItem = _cardPreviewVm.SelectedItem;
            if (null == selectedItem)
            {
                BaseDialogUtils.ShowDialogAuto(StringConst.CardChioceNone);
                return;
            }
            // 修改确认
            if (!await BaseDialogUtils.ShowDialogConfirm(StringConst.UpdateConfirm)) return;
            // 数据库修改
            var updateSql = GetUpdateSql(CardEditorModel, selectedItem.Number);
            var isUpdate = SqliteUtils.Execute(updateSql);
            BaseDialogUtils.ShowDialogAuto(isUpdate ? StringConst.UpdateSucceed : StringConst.UpdateFailed);
            // 数据库更新
            if (!isUpdate) return;
            DataCache.DsAllCache.Clear();
            SqliteUtils.FillDataToDataSet(SqlUtils.GetQueryAllSql(), DataCache.DsAllCache);
            _cardPreviewVm.UpdateCardPreviewList(_cardPreviewVm.MemoryQueryModel);
        }

        /// <summary>
        ///     重置事件
        /// </summary>
        public void Reset_Click(object obj)
        {
            CardEditorModel = new CardEditorModel();
            OnPropertyChanged(nameof(CardEditorModel));
            var mode = _externOpertaionVm.ModeValue;
            if (CardUtils.GetModeType(mode).Equals(Enums.ModeType.Editor))
            {
                CardEditorModel.Pack = _cardPreviewVm.MemoryQueryModel.CardEditorModel.Pack;
                CardEditorModel.Number = _cardPreviewVm.MemoryQueryModel.CardEditorModel.Number;
            }
            UpdateAbilityType(CardEditorModel.AbilityTypeModels);
        }

        /// <summary>
        ///     查询事件
        /// </summary>
        public void Query_Click(object obj)
        {
            // 深拷贝查询模型
            var cardEditorModel = JsonUtils.Deserialize<CardEditorModel>(JsonUtils.Serializer(CardEditorModel));
            var mode = _externOpertaionVm.ModeValue;
            var restrict = _externOpertaionVm.RestrictValue.Equals(StringConst.NotApplicable)
                ? -1
                : int.Parse(_externOpertaionVm.RestrictValue);
            var cardQueryMdoel = new CardQueryMdoel
            {
                CardEditorModel = cardEditorModel,
                Restrict = restrict,
                ModeValue = mode
            };
            _cardPreviewVm.UpdateCardPreviewList(cardQueryMdoel);
        }

        /// <summary>
        ///     阵营、种族联动事件
        /// </summary>
        public void UpdateRaceList()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            // 类型判断,玩家、事件不改变种族默认值（此时种族处于不可编辑状态）
            if (CardEditorModel.Type.Equals(StringConst.TypePlayer) ||
                CardEditorModel.Type.Equals(StringConst.TypeEvent)) return;
            RaceList.Clear();
            CardUtils.GetPartRace(CardEditorModel.Camp).ForEach(RaceList.Add);
            OnPropertyChanged(nameof(RaceList));
            CardEditorModel.Race = StringConst.NotApplicable;
            OnPropertyChanged(nameof(CardEditorModel));
        }

        /// <summary>
        ///     种类改变联动事件
        /// </summary>
        public void UpdateTypeLinkage()
        {
            switch (CardEditorModel.Type)
            {
                case StringConst.NotApplicable:
                case StringConst.TypeZx:
                {
                    CardEditorModel.CostEnabled = true;
                    CardEditorModel.PowerEnabled = true;
                    CardEditorModel.RaceEnabled = true;
                    CardEditorModel.SignEnabled = true;
                    break;
                }
                case StringConst.TypeZxEx:
                {
                    CardEditorModel.CostEnabled = true;
                    CardEditorModel.PowerEnabled = true;
                    CardEditorModel.RaceEnabled = true;
                    CardEditorModel.SignEnabled = false;
                    CardEditorModel.Sign = StringConst.Hyphen;
                    break;
                }
                case StringConst.TypePlayer:
                {
                    CardEditorModel.CostEnabled = false;
                    CardEditorModel.PowerEnabled = false;
                    CardEditorModel.RaceEnabled = false;
                    CardEditorModel.SignEnabled = false;
                    CardEditorModel.Race = StringConst.Hyphen;
                    CardEditorModel.Sign = StringConst.Hyphen;
                    CardEditorModel.CostValue = string.Empty;
                    CardEditorModel.PowerValue = string.Empty;
                    break;
                }
                case StringConst.TypeEvent:
                {
                    CardEditorModel.CostEnabled = true;
                    CardEditorModel.PowerEnabled = false;
                    CardEditorModel.RaceEnabled = false;
                    CardEditorModel.SignEnabled = true;
                    CardEditorModel.Race = StringConst.Hyphen;
                    CardEditorModel.PowerValue = string.Empty;
                    break;
                }
            }
            OnPropertyChanged(nameof(CardEditorModel));
        }

        public void UpdateCardEditorModel(CardModel cardModel)
        {
            _cardPreviewVm.IsPreviewChanged = true;

            CardEditorModel.Md5 = cardModel.Md5;
            CardEditorModel.Type = cardModel.Type;
            CardEditorModel.Camp = cardModel.Camp;
            CardEditorModel.Race = cardModel.Race;
            CardEditorModel.Sign = cardModel.Sign;
            CardEditorModel.Rare = cardModel.Rare;
            CardEditorModel.Pack = cardModel.Pack;
            CardEditorModel.CName = cardModel.CName;
            CardEditorModel.JName = cardModel.JName;
            CardEditorModel.Number = cardModel.Number;
            CardEditorModel.Illust = cardModel.Illust;
            CardEditorModel.CostValue = cardModel.Cost.ToString();
            CardEditorModel.PowerValue = cardModel.Power.ToString();
            CardEditorModel.Ability = cardModel.Ability;
            CardEditorModel.Lines = cardModel.Lines;

            for (var i = 0; i != CardEditorModel.AbilityTypeModels.Count; i++)
            {
                var model = CardEditorModel.AbilityTypeModels[i];
                CardEditorModel.AbilityTypeModels[i] = new AbilityModel
                {
                    Checked = cardModel.Ability.Contains(model.Name),
                    Name = model.Name
                };
            }

            var abilityDetailModelList = JsonUtils.Deserialize<List<List<int>>>(cardModel.AbilityDetailJson);
            foreach (var pair in abilityDetailModelList)
                for (var i = 0; i != CardEditorModel.AbilityDetailModels.Count; i++)
                {
                    var model = CardEditorModel.AbilityDetailModels[i];
                    if (!model.Code.Equals(pair[0])) continue;
                    CardEditorModel.AbilityDetailModels[i] = new AbilityModel
                    {
                        Checked = pair[1] == 1,
                        Name = model.Name,
                        Code = pair[0]
                    };
                    break;
                }
            OnPropertyChanged(nameof(CardEditorModel));
            _externOpertaionVm.UpdateRestrictValue(cardModel.Restrict);
            _cardPictureVm.UpdatePicture(cardModel);

            _cardPreviewVm.IsPreviewChanged = false;
        }

        /// <summary>
        ///     卡包改变联动事件
        /// </summary>
        public void UpdatePackLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            var packNumber = CardUtils.GetPackNumber(CardEditorModel.Pack);
            if (CardEditorModel.Number.Contains(StringConst.Hyphen))
                packNumber +=
                    CardEditorModel.Number.Substring(
                        CardEditorModel.Number.IndexOf(StringConst.Hyphen, StringComparison.Ordinal) + 1);
            CardEditorModel.Number = packNumber;
            if (packNumber.IndexOf("P", StringComparison.Ordinal) == 0)
                CardEditorModel.Rare = StringConst.RarePr;
        }

        /// <summary>
        ///     能力改变联动事件
        /// </summary>
        public void UpdateAbilityLinkage()
        {
            if (_cardPreviewVm.IsPreviewChanged) return;
            var ability = CardEditorModel.Ability;
            if (ability.Contains("降临条件") || ability.Contains("觉醒条件"))
            {
                CardEditorModel.Type = StringConst.TypeZxEx;
                CardEditorModel.Sign = StringConst.Hyphen;
            }
            if (ability.Contains("【★】"))
            {
                CardEditorModel.Type = StringConst.TypeEvent;
                CardEditorModel.Race = StringConst.Hyphen;
                CardEditorModel.PowerValue = string.Empty;
            }
            if (ability.Contains("【常】生命恢复") || ability.Contains("【常】虚空使者"))
            {
                CardEditorModel.Type = StringConst.TypeZx;
                CardEditorModel.Sign = StringConst.SignIg;
            }
            if (ability.Contains("【常】起始卡"))
            {
                CardEditorModel.Type = StringConst.TypeZx;
                CardEditorModel.Sign = StringConst.Hyphen;
            }
        }

        public static string GetAddSql(CardEditorModel card)
        {
            var builder = new StringBuilder();
            builder.Append("INSERT INTO " + SqliteConst.TableName);
            builder.Append(SqliteConst.ColumnCard);
            builder.Append("VALUES(");
            builder.Append($"'{Md5Utils.GetMd5(card.JName + card.CostValue + card.PowerValue)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Type)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Camp)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Race)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Sign)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Rare)}',");
            builder.Append($"'{SqlUtils.GetAccurateValue(card.Pack)}',");
            builder.Append($"'{card.CName}',");
            builder.Append($"'{card.JName}',");
            builder.Append($"'{card.Illust}',");
            builder.Append($"'{card.Number}',");
            builder.Append($"'{card.CostValue}',");
            builder.Append($"'{card.PowerValue}',");
            builder.Append($"'{card.Ability}',");
            builder.Append($"'{card.Lines}',");
            builder.Append($"'{JsonUtils.Serializer(new List<string> {$"/{card.Number}.jpg"})}',");
            builder.Append($"'{GetAbilityDetailJson(card.AbilityDetailModels.ToList())}'");
            // 详细能力处理
            builder.Append(")");
            return builder.ToString();
        }

        public string GetDeleteSql(string number)
        {
            return $"DELETE FROM {SqliteConst.TableName} WHERE {SqliteConst.ColumnNumber}='{number}'";
        }

        public string GetUpdateSql(CardEditorModel card, string number)
        {
            var builder = new StringBuilder();
            builder.Append($"UPDATE {SqliteConst.TableName} SET ");
            builder.Append(
                $"{SqliteConst.ColumnMd5}='{Md5Utils.GetMd5(card.JName + card.CostValue + card.PowerValue)}',");
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
            builder.Append($"{SqliteConst.ColumnCost}= '{card.CostValue}',");
            builder.Append($"{SqliteConst.ColumnPower}= '{card.PowerValue}',");
            builder.Append($"{SqliteConst.ColumnAbility}= '{card.Ability}',");
            builder.Append($"{SqliteConst.ColumnLines}= '{card.Lines}',");
            builder.Append(
                $"{SqliteConst.ColumnImage}= '{JsonUtils.Serializer(new List<string> {$"/{card.Number}.jpg"})}',");
            builder.Append(
                $"{SqliteConst.ColumnAbilityDetail}= '{GetAbilityDetailJson(card.AbilityDetailModels.ToList())}'");
            // 详细能力处理
            builder.Append($" WHERE {SqliteConst.ColumnNumber}='{number}'");
            return builder.ToString();
        }

        private static string GetAbilityDetailJson(IEnumerable<AbilityModel> abilityModels)
        {
            var abilityDetailList =
                abilityModels.Select(abilityModel => new List<int> {abilityModel.Code, abilityModel.Checked ? 1 : 0})
                    .ToList();
            return JsonUtils.Serializer(abilityDetailList);
        }
    }
}