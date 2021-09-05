using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 进程模板在类图上的VM
    /// </summary>
    public class Proc_VM : DragDrop_VM
    {
        private readonly Proc proc;

        public Proc_VM()
            :base(0, 0, null)
        {
            this.proc = new Proc("Design");
        }

        public Proc_VM(double x, double y, DragDrop_P_VM panelVM, Proc proc)
            :base(x, y, panelVM)
        {
            this.proc = proc;
        }

        public Proc Proc => proc;
    }
}
