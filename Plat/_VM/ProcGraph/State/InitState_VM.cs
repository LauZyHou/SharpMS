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
        public InitState_VM(double x, double y)
            :base(x, y)
        {
            this.Anchor_VMs.Add(new Anchor_VM(Pos.X + 20, Pos.Y + 40, this));
        }
    }
}
