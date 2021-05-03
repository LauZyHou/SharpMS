using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL program graph, which can be trans to <init/>, <location/> and <transition/>.
    /// </summary>
    public class UpPG
    {
        private readonly List<UpLocation> locations;
        private readonly List<UpTransition> transitions;

        public UpPG()
        {
            this.locations = new List<UpLocation>();
            this.transitions = new List<UpTransition>();
        }

        public List<UpLocation> Locations => locations;
        public List<UpTransition> Transitions => transitions;

        public override string ToString()
        {
            string res = "";
            int initId = 0; // init location的id号
            foreach (UpLocation location in locations)
            {
                res += location;
                if (location.IsInit)
                {
                    initId = location.Id;
                }
            }
            res += $"\n<init ref=\"id{initId}\"/>\n";
            foreach (UpTransition transition in transitions)
            {
                res += transition;
            }
            return res;
        }
    }
}
