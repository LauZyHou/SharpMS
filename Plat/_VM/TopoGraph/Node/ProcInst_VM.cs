using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 进程模板的实例的VM
    /// </summary>
    public class ProcInst_VM : DragDrop_VM
    {
        private readonly ProcInst procInst;

        public ProcInst_VM(double x, double y, DragDrop_P_VM panelVM)
            :base(x, y, panelVM)
        {
            this.procInst = new ProcInst();
            this.init_anchor();
        }

        public ProcInst ProcInst => procInst;

        #region Init

        private void init_anchor()
        {
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 24, Pos.Y + 4, this)); // 0
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 37, Pos.Y + 11, this)); // 1
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 44, Pos.Y + 24, this)); // 2
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 37, Pos.Y + 38, this)); // 3
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 24, Pos.Y + 44, this)); // 4
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 10, Pos.Y + 38, this)); // 5
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 4, Pos.Y + 24, this)); // 6
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 10, Pos.Y + 10, this)); // 7
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
            return procInst.Proc is null ? "NullNode" : procInst.Proc.Identifier;
        }
    }
}
