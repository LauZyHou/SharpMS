using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 一边Dash一边Cross的连接线VM
    /// </summary>
    public class DashCrossLine_VM : Linker_VM
    {
        public DashCrossLine_VM(Anchor_VM source, Anchor_VM dest, DragDrop_P_VM panelVM)
            :base(source, dest, panelVM)
        {
        }
    }
}
