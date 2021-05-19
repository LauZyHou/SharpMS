using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 最朴素的状态结点
    /// </summary>
    public class PureState_VM : DragDrop_VM
    {
        private readonly State state;

        /// <summary>
        /// 空参构造PureState的VM，用于用户手动在界面上创建PureState
        /// </summary>
        public PureState_VM()
        {
            this.state = new State("NewState");
        }

        /// <summary>
        /// 通过State对象构造其PureState的VM，用于导入模型时的构造
        /// </summary>
        /// <param name="state">State对象</param>
        public PureState_VM(State state)
        {
            this.state = state;
        }

        public State State => state;

        #region Command Callback

        public void OnEdit()
        {

        }

        public void OnDelete()
        {

        }

        #endregion

        public override string ToString()
        {
            return state.ToString();
        }
    }
}
