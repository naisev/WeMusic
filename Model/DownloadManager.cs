using Native.Tool.Http;
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
        public static void DownloadFileAsync(string url, string path, string name)
        {
            new Task(new Action(() =>
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

            })).Start();

        }

        public static bool DownloadFile(string url, string path, string name)
        {
            if (!Directory.Exists(path)) { return false; }
            if (!path.EndsWith("\\")) { path += "\\"; }

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
        }

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
