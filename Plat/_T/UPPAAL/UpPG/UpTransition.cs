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
        private readonly UpSyncAction? syncAction;
        private readonly List<UpAssignment> assignActions = new List<UpAssignment>();
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
        public UpSyncAction? SyncAction => syncAction;
        public List<UpAssignment> AssignActions => assignActions;
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public override string ToString()
        {
            string res = $"<transition>\n<source ref=\"id{source.Id}\"/>\n<target ref=\"id{target.Id}\"/>\n";
            int yBias = 0; // Y方向每写一条，就要往下挪20的偏置以写下一条
            if (syncAction is not null)
            {
                res += $"<label kind=\"synchronisation\" x=\"{x}\" y=\"{y}\">\n{syncAction}\n</label>\n";
                yBias += 20;
            }
            foreach (UpAssignment assignAction in assignActions)
            {
                res += $"<label kind=\"assignment\" x=\"{x}\" y=\"{y + yBias}\">\n{assignAction}\n</label>\n";
                yBias += 20;
            }
            res += "</transition>\n";
            return res;
        }
    }

    /// <summary>
    /// UPPAAL sync action, which can be trans to content of "<label/>" with attr kind="synchronisation".
    /// [todo] channel definition use a new class.
    /// </summary>
    public class UpSyncAction
    {
        /// <summary>
        /// "true" if it is send sync, otherwise "false" to identify receive sync.
        /// </summary>
        private readonly bool isSend;
        private readonly string channelName;

        public UpSyncAction(bool isSend, string channelName)
        {
            this.isSend = isSend;
            this.channelName = channelName;
        }

        public bool IsSend => isSend;
        public string ChannelName => channelName;

        public override string ToString()
        {
            return $"{channelName}{(isSend ? "!" : "?")}";
        }
    }

}
