using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 终止状态结点
    /// </summary>
    public class FinalState_VM : DragDrop_VM
    {
        private readonly State state;

        public FinalState_VM()
        {
            this.state = new State("FinalState");
        }

        public State State => state;

        #region Command Callback

        private void OnDelete()
        {
        }

        #endregion
    }
}
