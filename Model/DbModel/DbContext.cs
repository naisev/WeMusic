using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model.DbModel
{
    public class DbContext<T> where T : class, new()
    {
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source="+AppDomain.CurrentDomain.BaseDirectory+"\\data.db;",
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了

            });

        }
        //注意：不能写成静态的
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SimpleClient<MusicInfoModel> MusicInfoDb { get { return new SimpleClient<MusicInfoModel>(Db); } }
        public SimpleClient<DefaultListModel> DefaultListDb { get { return new SimpleClient<DefaultListModel>(Db); } }
        public SimpleClient<LocalListModel> LocalListDb { get { return new SimpleClient<LocalListModel>(Db); } }
        public SimpleClient<CustomListModel> CustomListDb { get { return new SimpleClient<CustomListModel>(Db); } }
        public SimpleClient<CustomTitleModel> CustomTitleDb { get { return new SimpleClient<CustomTitleModel>(Db); } }
        public SimpleClient<SearchHistoryModel> SearchHistoryDb { get { return new SimpleClient<SearchHistoryModel>(Db); } }
        public SimpleClient<PlatformInfoModel> PlatformInfoDb { get { return new SimpleClient<PlatformInfoModel>(Db); } }
        public SimpleClient<PlatformListModel> PlatformListDb { get { return new SimpleClient<PlatformListModel>(Db); } }
        public SimpleClient<T> CurrentDb { get { return new SimpleClient<T>(Db); } }//用来处理T表的常用操作


        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList()
        {
            return CurrentDb.GetList();
        }

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(dynamic id)
        {
            try
            {
                return CurrentDb.DeleteById(id);
            }
            catch { return false; }
        }

        /// <summary>
        /// 根据内容删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Delete(T obj)
        {
            try
            {
                return CurrentDb.Delete(obj);
            }
            catch { return false; }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Update(T obj)
        {
            return CurrentDb.Update(obj);
        }

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Insert(T obj)
        {
            try
            {
                return CurrentDb.Insert(obj);
            }
            catch (System.Data.SQLite.SQLiteException)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T Find(dynamic id)
        {
            return CurrentDb.GetById(id);
        }
    }
}
