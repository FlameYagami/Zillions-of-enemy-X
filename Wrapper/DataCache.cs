using System.Collections.Generic;
using System.Data;
using DeckEditor.Entity;
using Wrapper.Entity;

namespace Wrapper
{
    public class DataCache
    {
        /// <summary>�����ݻ���</summary>
        public static DataSet DsAllCache = new DataSet();

        /// <summary>�б����ݻ���</summary>
        public static List<PreviewEntity> PreEntityList = new List<PreviewEntity>();

        /// <summary>�������໺��</summary>
        public static AbilityDetialEntity AbilityDetialEntity = new AbilityDetialEntity(); // ��ʼ����������ģ��

        /// <summary>������ݻ���</summary>
        public static List<DeckEntity> PlColl = new List<DeckEntity>();

        /// <summary>��ȼ���ݻ���</summary>
        public static List<DeckEntity> IgColl = new List<DeckEntity>();

        /// <summary>�ǵ�ȼ���ݻ���</summary>
        public static List<DeckEntity> UgColl = new List<DeckEntity>();

        /// <summary>�������ݻ���</summary>
        public static List<DeckEntity> ExColl = new List<DeckEntity>();
    }
}