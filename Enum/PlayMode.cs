using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeMusic.Enum
{
    public enum PlayMode
    {
        //单曲播放
        SinglePlay = 0,

        //单曲循环
        SingleLoop = 1,

        //列表循环
        ListLoop = 2,

        //随机播放
        RandomLoop = 3
    }
}
