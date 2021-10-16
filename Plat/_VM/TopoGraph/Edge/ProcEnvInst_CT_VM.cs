using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 进程实例和环境实例协同Tag
    /// </summary>
    public class ProcEnvInst_CT_VM : CT_VM
    {
        private readonly ProcEnvInst procEnvInst;

        public ProcEnvInst_CT_VM(double x, double y, DragDrop_P_VM panelVM, Linker_VM linker_VM)
            :base(x, y, panelVM, linker_VM)
        {
            this.procEnvInst = new ProcEnvInst();
        }

        /// <summary>
        /// ProcInst的协作实例Model
        /// </summary>
        public ProcEnvInst ProcEnvInst => procEnvInst;
    }
}
