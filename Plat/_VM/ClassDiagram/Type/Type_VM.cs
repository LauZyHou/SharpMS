using Plat._M;

namespace Plat._VM
{
    /// <summary>
    /// 类图上的Type
    /// </summary>
    public class Type_VM : DragDrop_VM
    {
        private readonly Type type;

        /// <summary>
        /// Design
        /// </summary>
        public Type_VM()
            : base(0, 0, null)
        {
            this.type = new Type("Design");
            this.init_anchor();
        }

        public Type_VM(double x, double y, DragDrop_P_VM panelVM, Type type)
            : base(x, y, panelVM)
        {
            this.type = type;
            this.init_anchor();
        }

        /// <summary>
        /// 数据类型Model
        /// </summary>
        public Type Type => type;

        #region Init

        private void init_anchor()
        {
            this.Anchor_VMs.Add(new TopAnchor_VM(Pos.X + W / 2 + 2, Pos.Y, this));
            this.Anchor_VMs.Add(new BotAnchor_VM(Pos.X + W / 2 + 2, Pos.Y + H + 4, this));
        }

        #endregion

        #region 继承下来的功能

        public override void FlushAnchorPos()
        {
            Anchor_VMs[0].Pos = new Avalonia.Point(Pos.X + W / 2 + 2, Pos.Y);
            Anchor_VMs[1].Pos = new Avalonia.Point(Pos.X + W / 2 + 2, Pos.Y + H + 4);
        }

        #endregion
    }
}
