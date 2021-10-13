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
    /// 拓扑图进程实例结点的编辑窗体VM
    /// </summary>
    public class ProcInst_EW_VM : ViewModelBase
    {
        private readonly ProcInst procInst;
        private readonly ObservableCollection<Proc> procList;

        public ProcInst_EW_VM(ProcInst procInst)
        {
            this.procInst = procInst;
            this.procList = ResourceManager.procs;
        }

        public ProcInst ProcInst => procInst;
        public ObservableCollection<Proc> ProcList => procList;
    }
}
