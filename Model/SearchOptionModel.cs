using Native.Tool.IniConfig.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Model
{
    [IniSection(SectionName = "SearchOption")]
    public class SearchOptionModel
    {
        public bool Netease { get; set; }
        public bool Kugou { get; set; }
        public bool Tencent { get; set; }
    }
}
