using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Initial Knowledge View Model on Class Diagram
    /// </summary>
    public class IK_VM : DragDrop_VM
    {
        private readonly IK ik;

        public IK_VM()
            :base(0, 0, null)
        {
            this.ik = new IK("Design");
        }

        public IK_VM(double x, double y, DragDrop_P_VM panelVM, IK ik)
            :base(x, y, panelVM)
        {
            this.ik = ik;
        }

        public IK IK => ik;
    }
}
