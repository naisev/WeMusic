using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeMusic.Interface;
using Newtonsoft.Json;
using System.Net;
using Native.Tool.Http;
using WeMusic.Enum;

namespace WeMusic.Model.MusicModel
{
    public class KugouMusic : IMusic, IApi
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
        public string SourceName { get { return "酷狗音乐"; } }
        public MusicSource Origin { get { return MusicSource.Kugou; } }

        public string CoverId { get => Id; set => Id = value; }

        public string GetCoverUrl()
        {
            try
            {
                string url = $"https://wwwapi.kugou.com/yy/index.php?r=play/getdata&hash={Id}";
                CookieCollection cookies = new CookieCollection();
                cookies.Add(new Cookie("kg_mid", "emmmm"));
                byte[] data = HttpWebClient.Get(url, "", ref cookies);
                return JObject.Parse(Encoding.UTF8.GetString(data))["data"]["img"].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetLyric()
        {
            string url = "https://tqlcode.com/page/music/api.php";
            byte[] commit = Encoding.UTF8.GetBytes($"types=lyric&id={Id}&source=kugou");
            byte[]data=HttpWebClient.Post(url, commit);
            JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
            return json["lyric"].ToString();
        }

        public string GetMusicUrl()
        {
            string url = $"https://tqlcode.com/page/music/kgapi.php?id={Id}";
            byte[] data = HttpWebClient.Get(url);
            JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
            return json["url"].ToString();
        }

        public KugouMusic(IMusic music)
        {
            Id = music.Id;
            Name = music.Name;
            Album = music.Album;
            Artists = music.Artists;
        }

        public KugouMusic() { }
    }
}
