using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model.DbModel
{
    [SugarTable("CustomTitle")]
    public class CustomTitleModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Title { get; set; }
        public CustomTitleModel()
        {

        }
        public CustomTitleModel(string name)
        {
            Title = name;
        }
    }
}
