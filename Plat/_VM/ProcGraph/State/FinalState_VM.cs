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
        /// <summary>
        /// Just for design
        /// </summary>
        public FinalState_VM()
            :base(0, 0, null)
        {
            this.Anchor_VMs.Add(new Anchor_VM(0, 0, this));
        }

        public FinalState_VM(double x, double y, DragDrop_P_VM panelVM)
            :base(x, y, panelVM)
        {
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 20, Pos.Y + 4, this));
        }

        #region Command Callback

        private void OnDelete()
        {
            this.PanelVM.DeleteDragDropItem(this);
        }

        #endregion

        public override string ToString()
        {
            return "FinalState";
        }

        public string Tag { get => "Fin"; }
    }
}
