using Native.Tool.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;

namespace WeMusic.Model.MusicModel
{
    public class NeteaseMusic :IMusic,IApi
    {
        public string id { get; set; }
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string name { get; set; }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string[] artist { get; set; }
        public string Artists
        {
            get
            {
                string value = string.Empty;
                foreach (var tmp in artist) value += "、" + tmp;
                if (value != string.Empty) { value = value.Substring(1); }
                return value;
            }
            set
            {
                artist = value.Split('、');
            }
        }
        public string album { get; set; }
        public string Album
        {
            get { return album; }
            set { album = value; }
        }
        public string url_id { get; set; }
        public string pic_id { get; set; }
        public string lyric_id { get; set; }
        public string source { get; set; }
        public string SourceName { get { return "网易云"; } }
        public MusicSource Origin { get { return MusicSource.Netease; } }

        public string GetCoverUrl()
        {
            string pwd = CoreApi.EncryptNeteaseId(pic_id);
            return $"https://p3.music.126.net/{pwd}/{pic_id}.jpg?param=300y300";
        }

        public string GetLyric()
        {
            string url = "http://blog.ylz1.cn/page/music/api.php";
            byte[] commit = Encoding.UTF8.GetBytes($"types=lyric&id={Id}&source=netease");
            byte[] data = HttpWebClient.Post(url, commit);
            JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
            return json["lyric"].ToString();
        }

        public string GetMusicUrl()
        {
            string url = "http://blog.ylz1.cn/page/music/api.php";
            byte[] commit = Encoding.UTF8.GetBytes($"types=url&id={Id}&source=netease");
            byte[] data = HttpWebClient.Post(url, commit);
            JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
            return json["url"].ToString();
        }

        public NeteaseMusic(IMusic music)
        {
            Id = music.Id;
            Name = music.Name;
            Album = music.Album;
            Artists = music.Artists;
        }

        public NeteaseMusic() { }
    }
}
