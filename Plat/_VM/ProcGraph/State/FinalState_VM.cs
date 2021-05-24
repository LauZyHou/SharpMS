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

        /// <summary>
        /// 空参构造FinalState的VM，用于用户手动在界面上创建FinalState
        /// </summary>
        public FinalState_VM()
        {
            this.state = new State("FinalState");
        }

        /// <summary>
        /// 通过State对象构造其FinalState的VM，用于导入模型时的构造
        /// </summary>
        /// <param name="state"></param>
        public FinalState_VM(State state)
        {
            this.state = state;
        }

        public State State => state;

        #region Command Callback

        private void OnDelete()
        {
        }

        #endregion
    }
}
