using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    // 注意，此文件下的都是UPPAAL活动语句
    // 即在UPPAAL状态机转移边上的语句
    // 其中，UpAssignment既可以在转移边上作为活动语句，也可以作为普通的声明中的语句
    // 所以它被放在UpStatement.cs中，没有放在这里


    /// <summary>
    /// UPPAAL转移边上的Select语句
    /// </summary>
    public class UpSelect
    {
        // todo
    }

    /// <summary>
    /// UPPAAL转移边上的的Guard条件语句
    /// </summary>
    public class UpGuard
    {
        private string itemA;
        private string guardOp;
        private string itemB;

        public UpGuard(string itemA, string guardOp, string itemB)
        {
            this.itemA = itemA;
            this.guardOp = guardOp;
            this.itemB = itemB;
        }

        public string ItemA { get => itemA; set => itemA = value; }
        public string GuardOp { get => guardOp; set => guardOp = value; }
        public string ItemB { get => itemB; set => itemB = value; }

        public override string ToString()
        {
            return $"{itemA} {guardOp} {itemB}";
        }
    }

    /// <summary>
    /// UPPAAL转移边上的同步语句
    /// </summary>
    public class UpSynchronisation
    {
        /// <summary>
        /// "true" if it is send sync, otherwise "false" to identify receive sync.
        /// </summary>
        private readonly bool isSend;
        private readonly string channelName;

        public UpSynchronisation(bool isSend, string channelName)
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
