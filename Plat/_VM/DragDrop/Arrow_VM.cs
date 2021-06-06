using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 箭头VM，直接复用Linker
    /// </summary>
    public class Arrow_VM : Linker_VM
    {
        public Arrow_VM(Anchor_VM source, Anchor_VM dest, DragDrop_P_VM panelVM)
            :base(source, dest, panelVM)
        {
        }
    }
}
