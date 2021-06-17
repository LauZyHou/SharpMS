using Plat._M;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 拓扑结点ViewModel
    /// </summary>
    public class TopoNode_VM : DragDrop_VM
    {
        private readonly TopoNode topoNode;

        public TopoNode_VM(double x, double y, DragDrop_P_VM panelVM)
            : base(x, y, panelVM)
        {
            this.topoNode = new TopoNode(null);
            this.init_anchor();
        }

        public TopoNode TopoNode => topoNode;

        #region Init

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
            // todo
        }

        #endregion

        public override string ToString()
        {
            return topoNode.Proc is null ? "NullNode" : topoNode.Proc.Identifier;
        }
    }
}
