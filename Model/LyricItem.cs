using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeMusic.Model
{
    public class LyricItem
    {
        public string Text { get; set; }
        public int Time { get; set; }
        public static List<LyricItem> Parse(string lyric)
        {
            List<LyricItem> LyricsList = new List<LyricItem>();

            List<string> Timespans = new List<string>();
            string[] arr = lyric.Split(new char[] { '\r', '\n' });
            Regex r = new Regex(@"\[\d{2}:\d{2}(.\d{2})*\]|\[\d{2}:\d{2}(.\d{3})*\]");

            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var matches = r.Matches(arr[i]);
                    if (matches != null && matches.Count > 0)
                    {
                        for (int j = 0; j < matches.Count; j++)
                        {
                            Timespans.Add(matches[j].Value);
                        }
                    }
                }
            }
            Timespans.Sort();
            Timespans.Distinct();

            for (int i = 0; i < Timespans.Count; i++)
            {
                for (int j = 0; j < arr.Length; j++)
                {
                    var index = arr[j].IndexOf(Timespans[i]);
                    if (index > -1)
                    {
                        LyricsList.Add(new LyricItem
                        {
                            Text = arr[j].Substring(arr[j].LastIndexOf(']') + 1).TrimEnd('\r').Trim(),
                            Time = GetSeconds(Timespans[i])
                        });
                        break;
                    }
                }
            }

            return LyricsList;
        }
        private static int GetSeconds(string input)
        {
            input = input.Replace("[", "").Replace("]", "");
            string[] str = input.Split(':');
            int seconds = 0;
            for (int i = 0; i < str.Length; i++)
            {
                double.TryParse(str[i],out double s);
                seconds += (int)(s * Math.Pow(60, str.Length - i - 1));
            }
            return seconds;
        }
    }


}
