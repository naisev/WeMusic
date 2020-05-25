using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model.DbModel
{
    [SugarTable("SearchHistory")]
    public class SearchHistoryModel
    {
        public string SearchContent { get; set; }
        public DateTime SearchTime { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string UntilTime
        {
            get
            {
                TimeSpan ts = DateTime.Now - SearchTime;
                if (ts.TotalSeconds < 60) { return (int)ts.TotalSeconds + "秒前"; }
                else if (ts.TotalMinutes < 60) { return (int)ts.TotalMinutes + "分钟前"; }
                else if(ts.TotalHours < 24) { return (int)ts.TotalHours + "小时前"; }
                else { return (int)ts.TotalDays + "天前"; }
            }
        }
        public SearchHistoryModel()
        {

        }
        public SearchHistoryModel(string content)
        {
            SearchContent = content;
            SearchTime = DateTime.Now;
        }
    }
}
