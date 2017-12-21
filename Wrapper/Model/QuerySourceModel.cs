using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Wrapper.Constant;
using Wrapper.Utils;

namespace Wrapper.Model
{
    /// <summary>
    ///     搜索界面控件资源模型
    /// </summary>
    public class QuerySourceModel
    {
        public QuerySourceModel()
        {
            TypeList = Dic.TypeDic.Keys.ToList();
            CampList = Dic.CampDic.Keys.ToList();
            SignList = Dic.SignDic.Keys.ToList();
            RareList = Dic.RareDic.Keys.ToList();
            IllustList = CardUtils.GetIllustList();
            PackList = CardUtils.GetPackList();
            RaceList = new ObservableCollection<string>();
            CardUtils.GetPartRace(StringConst.NotApplicable).ForEach(RaceList.Add);
        }

        public List<string> TypeList { get; set; }
        public List<string> CampList { get; set; }
        public List<string> SignList { get; set; }
        public List<string> RareList { get; set; }
        public List<string> IllustList { get; set; }
        public List<string> PackList { get; set; }
        public ObservableCollection<string> RaceList { get; set; }

        public void UpdateRaceList(string camp)
        {
            RaceList.Clear();
            CardUtils.GetPartRace(camp).ForEach(RaceList.Add);
        }
    }
}