using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class InitState_VM : DragDrop_VM
    {
        /// <summary>
        /// Just for design
        /// </summary>
        public InitState_VM()
            :base(0, 0, null)
        {
            this.Anchor_VMs.Add(new Anchor_VM(0, 0, this));
        }

        public InitState_VM(double x, double y, DragDrop_P_VM panelVM)
            :base(x, y, panelVM)
        {
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 34, Pos.Y + 37, this));
        }

        public override string ToString()
        {
            return "InitState";
        }

        public string Tag { get => "Init"; }

    }
}
