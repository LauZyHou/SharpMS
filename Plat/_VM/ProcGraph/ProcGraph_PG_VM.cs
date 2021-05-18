using Plat._C;
using Plat._M;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Process Graph面板组的ViewModel，组织多个Process Graph面板
    /// </summary>
    public class ProcGraph_PG_VM : ViewModelBase
    {
        private readonly ObservableCollection<ProcGraph_P_VM> procGraph_P_VMs;

        public ProcGraph_PG_VM()
        {
            this.procGraph_P_VMs = ResourceManager.procGraph_P_VMs;
        }

        /// <summary>
        /// Process Graph面板组中组织的若干Process Graph面板
        /// </summary>
        public ObservableCollection<ProcGraph_P_VM> ProcGraph_P_VMs => procGraph_P_VMs;
    }
}
