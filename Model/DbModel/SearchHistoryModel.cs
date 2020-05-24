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
