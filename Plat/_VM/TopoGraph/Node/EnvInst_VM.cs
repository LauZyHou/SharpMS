using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 环境模板的实例的VM
    /// </summary>
    public class EnvInst_VM : DragDrop_VM
    {
        private readonly EnvInst envInst;

        public EnvInst_VM(double x, double y, DragDrop_P_VM panelVM)
            :base(x, y, panelVM)
        {
            this.envInst = new EnvInst();
        }

        public EnvInst EnvInst => envInst;
    }
}
