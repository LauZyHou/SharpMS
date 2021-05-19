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
        private ObservableCollection<Type> typeList;

        public ProcPanel_VM()
        {
            this.procList = ResourceManager.procs;
            this.procList.Add(new Proc("test", "desc test")); // test
            this.typeList = ResourceManager.types; // 可以间接引用到TypePanel的TypeList
        }

        public Proc? CurrentProc { get => currentProc; set => this.RaiseAndSetIfChanged(ref currentProc, value); }
        public ObservableCollection<Proc> ProcList { get => procList; set => this.RaiseAndSetIfChanged(ref procList, value); }
        public ObservableCollection<Type> TypeList { get => typeList; set => this.RaiseAndSetIfChanged(ref typeList, value); }

        #region Command Callback

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
            foreach (ProcGraph_P_VM procGraph_P_VM in ResourceManager.procGraph_P_VMs)
            {
                if (procGraph_P_VM.ProcGraph.Proc == proc)
                {
                    ResourceManager.procGraph_P_VMs.Remove(procGraph_P_VM);
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
            ResourceManager.procGraph_P_VMs.Add(new ProcGraph_P_VM(new ProcGraph(proc)));
            ResourceManager.UpdateTip("Create a new process.");
        }

        /// <summary>
        /// 创建新的进程参数
        /// </summary>
        private void CreateAttribute(Proc proc)
        {
            if (proc is null)
            {
                return;
            }
            proc.Attributes.Add(new Attribute("newAttr", Type.TYPE_INT));
            ResourceManager.UpdateTip($"Create a new parameter for process [{proc.Identifier}].");
        }

        /// <summary>
        /// 从当前Proc中删除参数
        /// </summary>
        /// <param name="attribute"></param>
        private void DeleteAttribute(Attribute attribute)
        {
            if (currentProc is null || attribute is null)
            {
                return;
            }
            currentProc.Attributes.Remove(attribute);
            ResourceManager.UpdateTip($"Delete parameter [{attribute.Identifier}] for process [{currentProc.Identifier}].");
        }

        #endregion
    }
}
