using Native.Tool.IniConfig;
using Native.Tool.IniConfig.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeMusic.Model;
using WeMusic.Model.DbModel;
using WeMusic.ViewModel;

namespace WeMusic
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Init();
            base.OnStartup(e);
        }

        public static void Init()
        {
            if (!File.Exists("data.db"))
            {
                SqlSugarClient conn = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\data.db;",
                    DbType = SqlSugar.DbType.Sqlite,
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true,
                });
                conn.CodeFirst.InitTables<CustomListModel>();
                conn.CodeFirst.InitTables<CustomTitleModel>();
                conn.CodeFirst.InitTables<DefaultListModel>();
                conn.CodeFirst.InitTables<MusicInfoModel>();
                conn.CodeFirst.InitTables<LocalListModel>();
                conn.CodeFirst.InitTables<SearchHistoryModel>();
                conn.Dispose();

            }
            IniConfig ini = new IniConfig("Config.ini");
            ini.Load();
            bool result = ini.Object["System"].TryGetValue("DownloadPath", out IValue v);
            if (!result || v?.ToString() == string.Empty)
            {
                DownloadManager.MusicDownloadPath = AppDomain.CurrentDomain.BaseDirectory + "Download\\";
            }
            else
            {
                DownloadManager.MusicDownloadPath = v.ObjToString();
            }
        }
    }
}
