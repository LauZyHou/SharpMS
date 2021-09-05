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
        private Caller? currentMethod;
        private Type? wantParamType;

        public ProcPanel_VM()
        {
            this.procList = ResourceManager.procs;
            this.typeList = ResourceManager.types; // 可以间接引用到TypePanel的TypeList
        }

        public Proc? CurrentProc { get => currentProc; set => this.RaiseAndSetIfChanged(ref currentProc, value); }
        public ObservableCollection<Proc> ProcList { get => procList; set => this.RaiseAndSetIfChanged(ref procList, value); }
        public ObservableCollection<Type> TypeList { get => typeList; set => this.RaiseAndSetIfChanged(ref typeList, value); }
        public Caller? CurrentMethod { get => currentMethod; set => this.RaiseAndSetIfChanged(ref currentMethod, value); }
        /// <summary>
        /// 想变成的参数类型
        /// </summary>
        public Type? WantParamType { get => wantParamType; set => this.RaiseAndSetIfChanged(ref wantParamType, value); }

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
            // 同步到ProcGraph组
            ProcGraph_P_VM procGraph_P_VM = new ProcGraph_P_VM(new ProcGraph(proc));
            procGraph_P_VM.DragDrop_VMs.Add(new InitState_VM(50, 50, procGraph_P_VM)); // 自动添加一个初始状态
            ResourceManager.procGraph_P_VMs.Add(procGraph_P_VM);
            // 同步到ClassDiag
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            classDiagram_P_VM.DragDrop_VMs.Add(new Proc_VM(100, 100, classDiagram_P_VM, proc));
            ResourceManager.UpdateTip("Create a new process, op synced to process graph and class diagram.");
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

        /// <summary>
        /// 创建Method
        /// </summary>
        private void OnCreateMethod()
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            Caller c = new Caller("newMethod", Type.TYPE_BOOL);
            this.currentProc.Methods.Add(c);
            ResourceManager.UpdateTip($"Add a new method for process [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 删除选中的Method
        /// </summary>
        private void OnDeleteMethod()
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentMethod is null)
            {
                ResourceManager.UpdateTip($"A method must be selected!");
                return;
            }
            this.currentProc.Methods.Remove(this.currentMethod);
            ResourceManager.UpdateTip($"A method has been deleted on process [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 向指定Method中添加一个参数
        /// </summary>
        private void OnAddParam()
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentMethod is null)
            {
                ResourceManager.UpdateTip($"A method must be selected!");
                return;
            }
            this.currentMethod.ParamTypes.Add(Type.TYPE_BOOL);
            this.currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"A new param has been added to the method [{this.currentMethod.Identifier}].");
        }

        /// <summary>
        /// 在Method中删除指定位置的参数
        /// </summary>
        /// <param name="paramPos"></param>
        private void OnDeleteParam(int? paramPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentMethod is null)
            {
                ResourceManager.UpdateTip($"A method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip($"A param type must be selected!");
                return;
            }
            if (paramPos < 0 || paramPos >= this.currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip($"[ERROR] Wrong pos when [{nameof(OnDeleteMethod)}]! You need to select a param!");
                return;
            }
            this.currentMethod.ParamTypes.RemoveAt((int)paramPos);
            this.currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"The param at pos [{paramPos}] in method [{this.currentMethod.Identifier}] has been removed.");
        }

        /// <summary>
        /// 在Method中上移指定位置的参数
        /// </summary>
        /// <param name="paramPos"></param>
        private void OnMoveUpParamType(int? paramPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentMethod is null)
            {
                ResourceManager.UpdateTip($"A method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip($"A param type must be selected!");
                return;
            }
            if (paramPos == 0)
            {
                ResourceManager.UpdateTip($"The param is the top one! No need to move up!");
                return;
            }
            if (paramPos < 0 || paramPos >= this.currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip($"[ERROR] Wrong pos when [{nameof(OnMoveUpParamType)}]! You need to select a param!");
                return;
            }
            int pos = (int)paramPos;
            Type type = this.currentMethod.ParamTypes[pos];
            this.currentMethod.ParamTypes.RemoveAt(pos);
            this.currentMethod.ParamTypes.Insert(pos - 1, type);
            this.currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"The param at pos [{pos}] has been move up in method [{this.currentMethod.Identifier}].");
        }

        /// <summary>
        /// 在Method中下移指定位置的参数
        /// </summary>
        /// <param name="paramPos"></param>
        private void OnMoveDownParamType(int? paramPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentMethod is null)
            {
                ResourceManager.UpdateTip($"A method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip($"A param type must be selected!");
                return;
            }
            if (paramPos == this.currentMethod.ParamTypes.Count - 1)
            {
                ResourceManager.UpdateTip($"The param is the bottom one! No need to move down!");
                return;
            }
            if (paramPos < 0 || paramPos >= this.currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip($"[ERROR] Wrong pos when [{nameof(OnMoveDownParamType)}]! You need to select a param!");
                return;
            }
            int pos = (int)paramPos;
            Type type = this.currentMethod.ParamTypes[pos];
            this.currentMethod.ParamTypes.RemoveAt(pos);
            this.currentMethod.ParamTypes.Insert(pos + 1, type);
            this.currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"The param at pos [{pos}] has been move down in method [{this.currentMethod.Identifier}].");
        }

        /// <summary>
        /// 确认用想要的参数类型替换Method中指定位置的参数
        /// </summary>
        /// <param name="paramPos"></param>
        private void OnConfirmWantParamType(int? paramPos)
        {
            if(this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentMethod is null)
            {
                ResourceManager.UpdateTip($"A method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip($"A param type must be selected!");
                return;
            }
            if (this.wantParamType is null)
            {
                ResourceManager.UpdateTip($"A wanna type must be selected!");
                return;
            }
            if (paramPos < 0 || paramPos >= this.currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip($"[ERROR] Wrong pos when [{nameof(OnConfirmWantParamType)}]! You need to select a param!");
                return;
            }
            int pos = (int)paramPos;
            Type type = (Type)this.wantParamType;
            this.currentMethod.ParamTypes.RemoveAt(pos);
            this.currentMethod.ParamTypes.Insert(pos, type);
            this.currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"Use type [{type.Identifier}] replace the type at the [{pos}] pos of method [{this.currentMethod.Identifier}].");
        }

        #endregion
    }
}
