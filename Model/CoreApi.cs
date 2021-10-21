using Native.Tool.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeMusic.Enum;
using WeMusic.Interface;
using WeMusic.Model.MusicModel;

namespace WeMusic.Model
{
    class CoreApi
    {
        public static IMusic[] SearchMusic(MusicSource type,string content)
        {
            try
            {
                if (type == MusicSource.Netease)
                {
                    string url = "https://tqlcode.com/page/music/api.php";
                    byte[] commit = Encoding.UTF8.GetBytes($"types=search&count=10&source=netease&pages=1&name={content}");
                    byte[] data = HttpWebClient.Post(url, commit);
                    NeteaseMusic[] infos = JsonConvert.DeserializeObject<NeteaseMusic[]>(Encoding.UTF8.GetString(data));
                    return infos;
                }
                else if (type == MusicSource.Kugou)
                {
                    string url = "https://tqlcode.com/page/music/api.php";
                    byte[] commit = Encoding.UTF8.GetBytes($"types=search&count=10&source=kugou&pages=1&name={content}");
                    byte[] data = HttpWebClient.Post(url, commit);
                    KugouMusic[] infos = JsonConvert.DeserializeObject<KugouMusic[]>(Encoding.UTF8.GetString(data));
                    return infos;
                }
                else
                {
                    string url = $"https://api.qq.jsososo.com/search?key={content}&pageNo=1&pageSize=10";
                    byte[] data = HttpWebClient.Get(url);
                    TencentMusic[] infos = JsonConvert.DeserializeObject<TencentMusic[]>(JObject.Parse(Encoding.UTF8.GetString(data))?["data"]?["list"].ToString());
                    return infos;
                }
            }
            catch (Exception)
            {
                return new IMusic[0];
            }
            
        }

        //加密网易云ID
        public static string EncryptNeteaseId(string id)
        {
            if (id == null) { return string.Empty; }
            string god = "3go8&$8*3*3h0k(2)2";
            string[] magic = new string[god.Length];
            for (int i = 0; i < god.Length; i++)
            {
                magic[i] = god.Substring(i, 1);
            }

            string[] sid = new string[id.Length];
            for (int i = 0; i < id.Length; i++)
            {
                sid[i] = id.Substring(i, 1);
            }

            for (int i = 0; i < sid.Length; i++)
            {
                int key1 = new ASCIIEncoding().GetBytes(sid[i])[0];
                int key2 = new ASCIIEncoding().GetBytes(magic[i % magic.Length])[0];
                sid[i] = ((char)(key1 ^ key2)).ToString();
            }
            byte[] md5 = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(string.Join("", sid)));
            string result = Convert.ToBase64String(md5);

            result = result.Replace("+", "-");
            result = result.Replace("/", "_");
            return result;
        }

        public static ISongList GetPlatformSongList(MusicSource type,string url)
        {
            //url正则取id分析
            ISongList result;

            try
            {
                if (type == MusicSource.Netease)
                {
                    string id = new Regex("(?<=playlist\\?id=)[0-9]+").Match(url).Value;
                    string requestUrl = "https://tqlcode.com/page/music/api.php";

                    byte[] data = HttpWebClient.Post(requestUrl, Encoding.UTF8.GetBytes($"types=playlist&id={id}"));
                    JObject jobj = JObject.Parse(Encoding.UTF8.GetString(data));
                    result = JsonConvert.DeserializeObject<NeteaseList>(jobj["playlist"].ToString());

                    //音乐写入
                    List<NeteaseMusic> musics = new List<NeteaseMusic>();
                    JArray playlist = JArray.Parse(jobj["playlist"]["tracks"].ToString());
                    foreach (var info in playlist)
                    {
                        //读取歌手数组
                        JArray ja = JArray.Parse(info["ar"].ToString());
                        List<string> artists = new List<string>();
                        foreach (var item in ja.Children()) { artists.Add(item["name"].ToString()); }

                        //写入返回结果
                        musics.Add(new NeteaseMusic
                        {
                            Id = info["id"].ToString(),
                            artist = artists.ToArray(),
                            Name = info["name"].ToString(),
                            Album = info["al"]["name"].ToString(),
                            CoverId = info["al"]["pic_str"].ToString()
                        });
                    }
                    result.Musics = musics.ToArray();
                }
                else if (type == MusicSource.Tencent)
                {
                    //获取重定向location
                    WebHeaderCollection whc = new WebHeaderCollection();
                    HttpWebClient.Get(url, "", ref whc, false);
                    //获取location中的id
                    string id = new Regex("(?<=id=)[0-9]+").Match(whc[HttpResponseHeader.Location]).Value;

                    //发出请求
                    string requestUrl = $"https://api.qq.jsososo.com/songlist?id={id}";
                    byte[] data = HttpWebClient.Get(requestUrl);
                    JObject jobj = JObject.Parse(Encoding.UTF8.GetString(data));
                    result = JsonConvert.DeserializeObject<TencentList>(jobj["data"].ToString());

                    //音乐写入
                    List<TencentMusic> musics = new List<TencentMusic>();
                    JArray playlist = JArray.Parse(jobj["data"]["songlist"].ToString());
                    foreach (var info in playlist)
                    {
                        musics.Add(JsonConvert.DeserializeObject<TencentMusic>(info.ToString()));
                    }
                    result.Musics = musics.ToArray();
                }
                else
                {
                    //获取歌单信息
                    HttpWebClient request = new HttpWebClient();
                    request.UserAgent = "Mozilla/5.0 (Linux; Android 4.4.2; Nexus 4 Build/KOT49H) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.114 Mobile Safari/537.36";
                    request.AllowAutoRedirect = true;
                    request.AutoCookieMerge = true;

                    byte[] tmpData = request.DownloadData(new Uri(url));
                    string tmpR = new Regex("(?<=<script>)([\\s\\S]*?)(?=<\\/script>)").Match(Encoding.UTF8.GetString(tmpData).Replace(" ", "")).Value;
                    string json = new Regex("(?<=phpParam=)([\\s\\S]*)(?=;)").Match(tmpR).Value;
                    result = JsonConvert.DeserializeObject<KugouList>(json);

                    //酷狗这歌单太乱了，需要多种方式实现
                    if (result is null)
                    {
                        //方式一
                        result = new KugouList
                        {
                            Id = Guid.NewGuid().ToString(),
                            CoverUrl = "",
                            Title = "酷狗歌单"
                        };
                        //获取htmldata
                        tmpData = HttpWebClient.Get(url);
                        tmpR = new Regex("(?<=<script>)([\\s\\S]*?)(?=<\\/script>)").Match(Encoding.UTF8.GetString(tmpData).Replace(" ", "")).Value;
                        json = new Regex("(?=\\[)([\\s\\S]*)(?<=\\])").Match(tmpR).Value;
                        JArray playlist = JArray.Parse(json);

                        //音乐写入
                        List<KugouMusic> musics = new List<KugouMusic>();
                        foreach (var info in playlist)
                        {
                            //写入返回结果
                            musics.Add(new KugouMusic
                            {
                                Id = info["hash"].ToString(),
                                Artists = info["author_name"].ToString(),
                                Name = info["song_name"].ToString(),
                                //该项无法获取
                                Album = ""
                            });
                        }
                        result.Musics = musics.ToArray();
                    }
                    else
                    {
                        //方式二
                        string reqUrl = $"http://m.kugou.com/plist/list/{result.Id}?json=true";
                        tmpData = HttpWebClient.Get(reqUrl);
                        JToken jobj = JObject.Parse(Encoding.UTF8.GetString(tmpData))["list"]["list"];
                        int.TryParse(jobj["total"].ToString(), out int total);
                        JArray playlist = JArray.Parse(jobj["info"].ToString());
                        List<KugouMusic> musics = new List<KugouMusic>();
                        foreach (var info in playlist)
                        {
                            string[] songName = info["filename"].ToString().Replace(" ", "").Split('-');
                            if (songName.Length != 2) { continue; }
                            //写入返回结果
                            musics.Add(new KugouMusic
                            {
                                Id = info["hash"].ToString(),
                                Artists = songName[0],
                                Name = songName[1],
                                Album = info["remark"].ToString()
                            });
                        }
                        //每一页只有30条，故循环获得
                        for (int i = 30; i < total; i += 30)
                        {
                            reqUrl = $"http://m.kugou.com/plist/list/{result.Id}?json=true&page={i / 30 + 1}";
                            tmpData = HttpWebClient.Get(reqUrl);
                            jobj = JObject.Parse(Encoding.UTF8.GetString(tmpData))["list"]["list"];
                            playlist = JArray.Parse(jobj["info"].ToString());
                            foreach (var info in playlist)
                            {
                                string[] songName = info["filename"].ToString().Replace(" ", "").Split('-');
                                if (songName.Length != 2) { continue; }
                                //写入返回结果
                                musics.Add(new KugouMusic
                                {
                                    Id = info["hash"].ToString(),
                                    Artists = songName[0],
                                    Name = songName[1],
                                    Album = info["remark"].ToString()
                                });
                            }
                        }
                        result.Musics = musics.ToArray();
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            if(result is null) { throw new Exception(); }
            return result;
        }
    }
}
