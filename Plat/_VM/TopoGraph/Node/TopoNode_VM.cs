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
        private Proc? proc;

        public TopoNode_VM(double x, double y, DragDrop_P_VM panelVM, Proc? proc)
            : base(x, y, panelVM)
        {
            this.proc = proc;
            this.init_anchor();
        }

        /// <summary>
        /// 拓扑图所例化的进程模板，null表示尚未设置
        /// </summary>
        public Proc? Proc { get => proc; set => this.RaiseAndSetIfChanged(ref proc, value); }

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
            return proc is not null ? proc.ToString() : "NullProc";
        }
    }
}
