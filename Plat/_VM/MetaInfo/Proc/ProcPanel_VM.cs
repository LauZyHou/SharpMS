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
            this.procList = new ObservableCollection<Proc>();
            this.procList.Add(new Proc("test", "desc test"));
        }

        public Proc? CurrentProc { get => currentProc; set => this.RaiseAndSetIfChanged(ref currentProc, value); }
        public ObservableCollection<Proc> ProcList { get => procList; set => this.RaiseAndSetIfChanged(ref procList, value); }

        #region Button Command

        /// <summary>
        /// 删除进程模板
        /// </summary>
        /// <param name="proc"></param>
        private void DeleteProc(Proc proc)
        {
            if (proc is null)
            {
                return;
            }
            this.procList.Remove(proc);
            ResourceManager.UpdateTip($"Delete process [{proc.Identifier}].");
        }

        /// <summary>
        /// 创建新进程模板
        /// </summary>
        private void CreateProc()
        {
            this.procList.Add(new Proc("NewProc"));
            ResourceManager.UpdateTip("Create a new process.");
        }

        #endregion
    }
}
