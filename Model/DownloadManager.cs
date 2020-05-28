using Masuit.Tools.Net;
using Native.Tool.Http;
using Native.Tool.IniConfig;
using Native.Tool.IniConfig.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WeMusic.Model
{
    static class DownloadManager
    {
        public static string MusicCachePath
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Cache\\Music\\";
                Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string CoverCachePath
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Cache\\Cover\\";
                //if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string LyricCachePath
        {
            get
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Cache\\Lyric\\";
                //if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                Directory.CreateDirectory(path);
                return path;
            }
        }

        private static string _musicDownloadPath;
        public static string MusicDownloadPath
        {
            get
            {
                Directory.CreateDirectory(_musicDownloadPath);
                return _musicDownloadPath;
            }
            set
            {
                _musicDownloadPath = value;
                IniConfig ini = new IniConfig("Config.ini");
                ini.Load();
                ini.Object["System"]["DownloadPath"] = value;
                ini.Save();
            }
        }

        public static void DownloadFileAsync(string url, string path, string name,
            Action<object, EventArgs> progressChangedAction = null, Action<object, int> completedAction = null)
        {
            if (!Directory.Exists(path)) { return; }
            if (!path.EndsWith("\\")) { path += "\\"; }
            var mtd = new MultiThreadDownloader(url, path + name, 1);
            if (progressChangedAction != null)
            {
                mtd.TotalProgressChanged += (sender, e) => progressChangedAction.Invoke(sender, e);
            }
            if (completedAction != null)
            {
                mtd.FileMergeProgressChanged += (sender, e) => completedAction.Invoke(sender, e);
            }
            mtd.Start();
            /*new Task(new Action(() =>
            {
                if (!Directory.Exists(path)) { return; }
                if (!path.EndsWith("\\")) { path += "\\"; }
                try
                {
                    byte[] data = HttpWebClient.Get(url);
                    if (data.Length != 0)
                    {
                        FileStream fs = new FileStream(path + name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                        fs.Dispose();
                        fs.Close();
                    }
                }
                catch
                {

                }

            })).Start();*/

        }

        /*public static bool DownloadFile(string url, string path, string name, Action<object, int> action)
        {
            if (!Directory.Exists(path)) { return false; }
            if (!path.EndsWith("\\")) { path += "\\"; }

            var mtd = new MultiThreadDownloader(url, path + name, 8);
            if (action != null)
            {
                mtd.FileMergeProgressChanged += (sender, e) =>
                {
                    action.Invoke(sender, e);
                };
            }
            return true;

            try
            {
                byte[] data = HttpWebClient.Get(url);
                if (data.Length == 0) { return false; }
                FileStream fs = new FileStream(path + name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }*/

        public static void SaveFile(byte[] data, string path, string name)
        {
            FileStream fs = new FileStream(path + name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Dispose();
            fs.Close();
        }
    }
}
