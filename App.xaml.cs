using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
                conn.Dispose();
            }
        }
    }
}
