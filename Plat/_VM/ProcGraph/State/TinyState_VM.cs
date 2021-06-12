using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 和PureState一样是普通的状态
    /// 只是TinyState在展示时像UPPAAL一样只展示一个小圆
    /// 圆的里面是这个State的id（因为是纯数字所以容易放下）
    /// 而State的name将被隐藏，可以在界面上主动要求显示
    /// </summary>
    public class TinyState_VM : DragDrop_VM
    {
        private readonly State state;

        public TinyState_VM()
            :base(0, 0, null)
        {
            this.state = new State("NewState");
            this.init_anchor();
        }

        /// <summary>
        /// 用户创建TinyState
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="panelVM"></param>
        public TinyState_VM(double x, double y, DragDrop_P_VM panelVM)
            : base(x, y, panelVM)
        {
            this.state = new State("NewState");
            this.init_anchor();
        }

        /// <summary>
        /// 导入模型中的TinyState
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="panelVM"></param>
        /// <param name="state"></param>
        public TinyState_VM(double x, double y, DragDrop_P_VM panelVM, State state)
            : base(x, y, panelVM)
        {
            this.state = state;
            this.init_anchor();
        }

        /// <summary>
        /// 状态模型
        /// </summary>
        public State State => state;

        #region Init

        /// <summary>
        /// 初始化锚点位置
        /// </summary>
        private void init_anchor()
        {
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 24, Pos.Y + 4, this));
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 44, Pos.Y + 24, this));
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 24, Pos.Y + 44, this));
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 4, Pos.Y + 24, this));
        }

        #endregion

        #region Command Callback

        private void OnDelete()
        {
            this.PanelVM.DeleteDragDropItem(this);
        }

        #endregion

        public override string ToString()
        {
            return $"{this.state.Id}-{this.state.Name}";
        }
    }
}
