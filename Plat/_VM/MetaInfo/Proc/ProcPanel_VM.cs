using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plat._C;
using Plat._M;
using ReactiveUI;

namespace Plat._VM
{
    /// <summary>
    /// 进程模板面板VM
    /// </summary>
    public class ProcPanel_VM : ViewModelBase
    {
        private Proc? currentProc;
        private ObservableCollection<Proc> procList;

        public ProcPanel_VM()
        {
            this.procList = ResourceManager.procs;
            this.procList.Add(new Proc("test", "desc test")); // test
        }

        public Proc? CurrentProc { get => currentProc; set => this.RaiseAndSetIfChanged(ref currentProc, value); }
        public ObservableCollection<Proc> ProcList { get => procList; set => this.RaiseAndSetIfChanged(ref procList, value); }

        #region Button Command

        /// <summary>
        /// 删除进程模板，注意同时还要删除Process Graph相关对象
        /// </summary>
        /// <param name="proc"></param>
        private void DeleteProc(Proc proc)
        {
            if (proc is null)
            {
                return;
            }
            this.procList.Remove(proc);
            foreach (ProcGraph_VM procGraph_VM in ResourceManager.procGraph_VMs)
            {
                if (procGraph_VM.ProcGraph.Proc == proc)
                {
                    ResourceManager.procGraph_VMs.Remove(procGraph_VM);
                    break;
                }
            }
            ResourceManager.UpdateTip($"Delete process [{proc.Identifier}].");
        }

        /// <summary>
        /// 创建新进程模板，注意同时还要创建Process Graph相关对象
        /// </summary>
        private void CreateProc()
        {
            Proc proc = new Proc("NewProc");
            this.procList.Add(proc);
            ResourceManager.procGraph_VMs.Add(new ProcGraph_VM(new ProcGraph(proc)));
            ResourceManager.UpdateTip("Create a new process.");
        }

        #endregion
    }
}
