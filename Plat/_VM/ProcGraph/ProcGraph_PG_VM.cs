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
    public class ProcGraph_PG_VM : ViewModelBase
    {
        private readonly ObservableCollection<ProcGraph_VM> procGraph_VMs;

        public ProcGraph_PG_VM()
        {
            this.procGraph_VMs = ResourceManager.procGraph_VMs;
        }

        public ObservableCollection<ProcGraph_VM> ProcGraph_VMs => procGraph_VMs;
    }
}
