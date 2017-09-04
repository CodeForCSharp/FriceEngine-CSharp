using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriceEngine.Resource;
using FriceEngine.Utils.Misc;

namespace MusicGame
{
    class Constant
    {
        public static string Strikes = "ASDFJKL;";

        public static Dictionary<string, ImageResource> Images = Strikes
            .Select(x => new Pair<string, ImageResource>(x.ToString(), ImageResource.FromPath($"Res/Img/VK_{x}")))
            .ToDictionary(x => x.First, y => y.Second);
    }
}
