using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 类图顶端Anchor
    /// </summary>
    public class TopAnchor_VM : Anchor_VM
    {
        public TopAnchor_VM(double x, double y, DragDrop_VM hostVM)
            :base(x, y, hostVM)
        {
        }
    }
}
