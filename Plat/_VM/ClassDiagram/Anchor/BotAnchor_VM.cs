using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 类图底端Anchor
    /// </summary>
    public class BotAnchor_VM : Anchor_VM
    {
        public BotAnchor_VM(double x, double y, DragDrop_VM hostVM)
            :base(x, y, hostVM)
        {
        }
    }
}
