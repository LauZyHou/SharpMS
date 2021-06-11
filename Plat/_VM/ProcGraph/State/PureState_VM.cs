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
        /// Just for design
        /// </summary>
        public PureState_VM()
            :base(0, 0, null)
        {
            this.state = new State("JustForDesign");
            this.init_anchor();
        }

        /// <summary>
        /// 仅需要位置，用于用户手动在界面上创建PureState
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PureState_VM(double x, double y, DragDrop_P_VM panelVM)
            :base(x, y, panelVM)
        {
            this.state = new State("NewState");
            this.init_anchor();
        }

        /// <summary>
        /// 需要位置和State对象，用于导入模型时的构造
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="state"></param>
        public PureState_VM(double x, double y, DragDrop_P_VM panelVM, State state)
            :base(x, y, panelVM)
        {
            this.state = state;
            this.init_anchor();
        }

        public State State => state;

        #region Init

        /// <summary>
        /// 初始化锚点位置
        /// </summary>
        private void init_anchor()
        {
            // 椭圆中心位置
            double centerX = Pos.X + 65;
            double centerY = Pos.Y + 40;

            // 根据V中Grid的布局，计算横纵的一个单位的长度
            int colSum = 1 + 2 + 6 + 6 + 8 + 6 + 6 + 2 + 1;
            int rowSum = 1 + 1 + 2 + 3 + 3 + 3 + 2 + 1 + 1;
            double c = 120 / colSum + 0.2; // 用Width除以份数得到列最小单位
            double r = 70 / rowSum + 0.5; // 用Height除以份数得到行最小单位
            // 这里加的是一个误差修复值

            // 14个锚点,从椭圆中心位置进行位置推算，这里顺序和V中一样
            Anchor_VMs.Add(new Anchor_VM(centerX - (4 + 6 + 3) * c, centerY - (1.5 + 3 + 1) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX - (4 + 3) * c, centerY - (1.5 + 3 + 2 + 0.5) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX, centerY - (1.5 + 3 + 2 + 1 + 1) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX + (4 + 3) * c, centerY - (1.5 + 3 + 2 + 0.5) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX + (4 + 6 + 3) * c, centerY - (1.5 + 3 + 1) * r, this));

            Anchor_VMs.Add(new Anchor_VM(centerX - (4 + 6 + 6 + 1) * c, centerY - (1.5 + 1.5) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX + (4 + 6 + 6 + 1) * c, centerY - (1.5 + 1.5) * r, this));

            Anchor_VMs.Add(new Anchor_VM(centerX - (4 + 6 + 6 + 1) * c, centerY + (1.5 + 1.5) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX + (4 + 6 + 6 + 1) * c, centerY + (1.5 + 1.5) * r, this));

            Anchor_VMs.Add(new Anchor_VM(centerX - (4 + 6 + 3) * c, centerY + (1.5 + 3 + 1) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX - (4 + 3) * c, centerY + (1.5 + 3 + 2 + 0.5) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX, centerY + (1.5 + 3 + 2 + 1 + 1) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX + (4 + 3) * c, centerY + (1.5 + 3 + 2 + 0.5) * r, this));
            Anchor_VMs.Add(new Anchor_VM(centerX + (4 + 6 + 3) * c, centerY + (1.5 + 3 + 1) * r, this));
        }

        #endregion

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
            return $"{this.state.Id}-{this.state.Name}";
        }
    }
}
