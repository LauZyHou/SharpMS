using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL transition model, which can be trans to "<transition/>"
    /// </summary>
    public class UpTransition
    {
        private readonly UpLocation source;
        private readonly UpLocation target;

        private UpSelect? upSelect;
        private UpGuard? upGurad;
        private UpSynchronisation? upSync;
        private UpAssignment? upAssign;

        private int x;
        private int y;

        public UpTransition(UpLocation source, UpLocation target)
        {
            this.source = source;
            this.target = target;
            this.x = (source.X + target.X) / 2;
            this.y = (source.Y + target.Y) / 2;
        }

        public UpLocation Source => source;
        public UpLocation Target => target;

        public UpSelect? UpSelect { get => upSelect; set => upSelect = value; }
        public UpGuard? UpGurad { get => upGurad; set => upGurad = value; }
        public UpSynchronisation? UpSync { get => upSync; set => upSync = value; }
        public UpAssignment? UpAssign { get => upAssign; set => upAssign = value; }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public override string ToString()
        {
            string res = $"\t\t<transition>\n" +
                         $"\t\t\t<source ref=\"id{source.Id}\"/>\n" +
                         $"\t\t\t<target ref=\"id{target.Id}\"/>\n";
            int yBias = 0; // Y方向每写一条，就要往下挪20的偏置以写下一条
            // 可能存在的select label
            if (upSelect is not null)
            {
                res += $"\t\t\t<label kind=\"select\" x=\"{x}\" y=\"{y + yBias}\">{upSelect}</label>\n";
                yBias += 20;
            }
            // 可能存在的guard label
            if (upGurad is not null)
            {
                res += $"\t\t\t<label kind=\"guard\" x=\"{x}\" y=\"{y + yBias}\">{upGurad}</label>\n";
                yBias += 20;
            }
            // 可能存在的synchronisation label
            if (upSync is not null)
            {
                res += $"\t\t\t<label kind=\"synchronisation\" x=\"{x}\" y=\"{y + yBias}\">{upSync}</label>\n";
                yBias += 20;
            }
            // 可能存在的assignment label
            if (upAssign is not null)
            {
                res += $"\t\t\t<label kind=\"assignment\" x=\"{x}\" y=\"{y + yBias}\">{upAssign}</label>\n";
                //yBias += 20;
            }
            // 如果是自己连自己的，考虑加上nail
            if (source == target)
            {
                res += $"\t\t\t<nail x=\"{x + 70}\" y=\"{y - 50}\"/>\n";
                res += $"\t\t\t<nail x=\"{x + 70}\" y=\"{y + 50}\"/>\n";
            }
            res += "\t\t</transition>\n";
            return res;
        }
    }
}
