﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ConfigUtils
    {
        /// <summary>配置文件设置 @"name=节点名字,cs=节点值"</summary>
        public static void Set(string name, string cs)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings[name].ConnectionString = cs;
            config.Save();
        }

        /// <summary>配置文件读取</summary>
        public static string Get(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
