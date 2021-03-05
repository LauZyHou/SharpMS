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

        public UpTransition(UpLocation source, UpLocation target)
        {
            this.source = source;
            this.target = target;
        }

        public UpLocation Source => source;
        public UpLocation Target => target;
        public UpSyncAction? SyncAction => syncAction;
        public List<UpAssignment> AssignActions => assignActions;

        public override string ToString()
        {
            string res = $"<transition>\n<source ref=\"id{source.Id}\"/>\n<target ref=\"id{target.Id}\"/>\n";
            if (syncAction is not null)
            {
                res += $"<label kind=\"synchronisation\" x=\"0\" y=\"0\">\n{syncAction}\n</label>\n";
            }
            foreach (UpAssignment assignAction in assignActions)
            {
                res += $"<label kind=\"assignment\" x=\"0\" y=\"0\">\n{assignAction}\n</label>\n";
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
