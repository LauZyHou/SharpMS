using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 进程实例的Node Tag
    /// </summary>
    public class ProcInst_NT_VM : DragDrop_VM
    {
        private readonly ProcInst procInst;

        public ProcInst_NT_VM(double x, double y, DragDrop_P_VM panelVM, ProcInst procInst)
            :base(x, y, panelVM)
        {
            this.procInst = procInst;
        }

        public ProcInst ProcInst => procInst;
    }
}
