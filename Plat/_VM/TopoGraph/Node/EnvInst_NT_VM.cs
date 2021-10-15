using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 环境实例的Node Tag
    /// </summary>
    public class EnvInst_NT_VM : DragDrop_VM
    {
        private readonly EnvInst envInst;

        public EnvInst_NT_VM(double x, double y, DragDrop_P_VM panelVM, EnvInst envInst)
            : base(x, y, panelVM)
        {
            this.envInst = envInst;
        }

        public EnvInst EnvInst => envInst;
    }
}
