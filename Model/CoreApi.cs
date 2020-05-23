using Native.Tool.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                    string url = "http://blog.ylz1.cn/page/music/api.php";
                    byte[] commit = Encoding.UTF8.GetBytes($"types=search&count=10&source=netease&pages=1&name={content}");
                    byte[] data = HttpWebClient.Post(url, commit);
                    NeteaseMusic[] infos = JsonConvert.DeserializeObject<NeteaseMusic[]>(Encoding.UTF8.GetString(data));
                    return infos;
                }
                else if (type == MusicSource.Kugou)
                {
                    string url = "http://blog.ylz1.cn/page/music/api.php";
                    byte[] commit = Encoding.UTF8.GetBytes($"types=search&count=10&source=kugou&pages=1&name={content}");
                    byte[] data = HttpWebClient.Post(url, commit);
                    KugouMusic[] infos = JsonConvert.DeserializeObject<KugouMusic[]>(Encoding.UTF8.GetString(data));
                    return infos;
                }
                else
                {
                    string url = $"https://api.qq.jsososo.com/search?key={content}&pageNo=1&pageSize=10";
                    byte[] data = HttpWebClient.Get(url);
                    TencentMusic[] infos = JsonConvert.DeserializeObject<TencentMusic[]>(JObject.Parse(Encoding.UTF8.GetString(data))["data"]["list"].ToString());
                    return infos;
                }
            }catch(Exception e)
            {
                throw e;
            }
            
        }

        //加密网易云ID
        public static string EncryptNeteaseId(string id)
        {
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

    }
}
