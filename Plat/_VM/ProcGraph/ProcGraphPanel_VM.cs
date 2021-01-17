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
    public class ProcGraphPanel_VM : ViewModelBase
    {
        private readonly ObservableCollection<Proc> procs;

        public string Test => "Welcome to Proc Graph!";

        public ProcGraphPanel_VM()
        {
            this.procs = ResourceManager.procs;
        }

        public ObservableCollection<Proc> Procs => procs;
    }
}
