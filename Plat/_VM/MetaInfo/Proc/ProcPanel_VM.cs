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
        private Port? currentPort;

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
        public Port? CurrentPort { get => currentPort; set => this.RaiseAndSetIfChanged(ref currentPort, value); }

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
            // 同步到ProcGraph组
            foreach (ProcGraph_P_VM procGraph_P_VM in ResourceManager.procGraph_P_VMs)
            {
                if (procGraph_P_VM.ProcGraph.Proc == proc)
                {
                    ResourceManager.procGraph_P_VMs.Remove(procGraph_P_VM);
                    break;
                }
            }
            // 同步到ClassDiag
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            foreach (DragDrop_VM item in classDiagram_P_VM.DragDrop_VMs)
            {
                if (item is Proc_VM)
                {
                    Proc_VM proc_VM = (Proc_VM)item;
                    if (proc_VM.Proc == proc)
                    {
                        classDiagram_P_VM.DeleteDragDropItem(item);
                        goto OVER;
                    }
                }
            }
        OVER:
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
            proc.Attributes.Add(new VisAttr("newAttr", Type.TYPE_INT));
            ResourceManager.UpdateTip($"Create a new parameter for process [{proc.Identifier}].");
        }

        /// <summary>
        /// 从当前Proc中删除参数
        /// </summary>
        /// <param name="attribute"></param>
        private void DeleteAttribute(VisAttr attribute)
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

        /// <summary>
        /// 创建一个Port
        /// </summary>
        private void OnCreateNewPort()
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            this.currentProc.Ports.Add(new Port("NewPort"));
            ResourceManager.UpdateTip($"Create a new port on process [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 删除选中的Port
        /// </summary>
        private void OnDeleteSelectedPort()
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentPort is null)
            {
                ResourceManager.UpdateTip($"A port must be selected!");
                return;
            }
            this.currentProc.Ports.Remove(this.currentPort);
            ResourceManager.UpdateTip($"Remove a port on process [{this.currentProc.Identifier}]");
        }

        /// <summary>
        /// 清除继承关系
        /// </summary>
        private void ClearParent()
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"A process must be selected!");
                return;
            }
            if (this.currentProc.Parent is null)
            {
                ResourceManager.UpdateTip($"No parent proc so do not need to clear!");
                return;
            }
            this.currentProc.Parent = null;
            ResourceManager.UpdateTip($"Clear parent for process [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 向上移动一个Attr
        /// </summary>
        /// <param name="attrPos"></param>
        private void OnMoveUpAttr(int? attrPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"An proc must be selected!");
                return;
            }
            if (attrPos is null)
            {
                ResourceManager.UpdateTip($"An attr must be selected!");
                return;
            }
            if (attrPos < 0 || attrPos >= this.currentProc.Attributes.Count)
            {
                ResourceManager.UpdateTip($"Attr pos exceed!");
                return;
            }
            if (attrPos == 0)
            {
                ResourceManager.UpdateTip($"The attr is the top one! No need to move up!");
                return;
            }
            int pos = (int)attrPos;
            VisAttr attr = this.currentProc.Attributes[pos];
            this.currentProc.Attributes.RemoveAt(pos);
            this.currentProc.Attributes.Insert(pos - 1, attr);
            ResourceManager.UpdateTip($"Move up attr [{attr.Identifier}] for proc [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 向下移动一个Attr
        /// </summary>
        /// <param name="attrPos"></param>
        private void OnMoveDownAttr(int? attrPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"An proc must be selected!");
                return;
            }
            if (attrPos is null)
            {
                ResourceManager.UpdateTip($"An attr must be selected!");
                return;
            }
            if (attrPos < 0 || attrPos >= this.currentProc.Attributes.Count)
            {
                ResourceManager.UpdateTip($"Attr pos exceed!");
                return;
            }
            if (attrPos == this.currentProc.Attributes.Count - 1)
            {
                ResourceManager.UpdateTip($"The attr is the bottom one! No need to move down!");
                return;
            }
            int pos = (int)attrPos;
            VisAttr attr = this.currentProc.Attributes[pos];
            this.currentProc.Attributes.RemoveAt(pos);
            this.currentProc.Attributes.Insert(pos + 1, attr);
            ResourceManager.UpdateTip($"Move down attr [{attr.Identifier}] for proc [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 向上移动Method
        /// </summary>
        /// <param name="methodPos"></param>
        private void OnMoveUpMethod(int? methodPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"An proc must be selected!");
                return;
            }
            if (methodPos is null)
            {
                ResourceManager.UpdateTip($"An method must be selected!");
                return;
            }
            if (methodPos < 0 || methodPos >= this.currentProc.Methods.Count)
            {
                ResourceManager.UpdateTip($"Method pos exceed!");
                return;
            }
            if (methodPos == 0)
            {
                ResourceManager.UpdateTip($"The method is the top one! No need to move up!");
                return;
            }
            int pos = (int)methodPos;
            Caller method = this.currentProc.Methods[pos];
            this.currentProc.Methods.RemoveAt(pos);
            this.currentProc.Methods.Insert(pos - 1, method);
            ResourceManager.UpdateTip($"Move up method [{method.Identifier}] for proc [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 向下移动Method
        /// </summary>
        /// <param name="methodPos"></param>
        private void OnMoveDownMethod(int? methodPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"An proc must be selected!");
                return;
            }
            if (methodPos is null)
            {
                ResourceManager.UpdateTip($"An method must be selected!");
                return;
            }
            if (methodPos < 0 || methodPos >= this.currentProc.Methods.Count)
            {
                ResourceManager.UpdateTip($"Method pos exceed!");
                return;
            }
            if (methodPos == this.currentProc.Methods.Count - 1)
            {
                ResourceManager.UpdateTip($"The method is the bottom one! No need to move down!");
                return;
            }
            int pos = (int)methodPos;
            Caller method = this.currentProc.Methods[pos];
            this.currentProc.Methods.RemoveAt(pos);
            this.currentProc.Methods.Insert(pos + 1, method);
            ResourceManager.UpdateTip($"Move down method [{method.Identifier}] for proc [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 上移一个Port
        /// </summary>
        /// <param name="portPos"></param>
        private void OnMoveUpPort(int? portPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"An proc must be selected!");
                return;
            }
            if (portPos is null)
            {
                ResourceManager.UpdateTip($"An port must be selected!");
                return;
            }
            if (portPos < 0 || portPos >= this.currentProc.Ports.Count)
            {
                ResourceManager.UpdateTip($"Port pos exceed!");
                return;
            }
            if (portPos == 0)
            {
                ResourceManager.UpdateTip($"The port is the top one! No need to move up!");
                return;
            }
            int pos = (int)portPos;
            Port port = this.currentProc.Ports[pos];
            this.currentProc.Ports.RemoveAt(pos);
            this.currentProc.Ports.Insert(pos - 1, port);
            ResourceManager.UpdateTip($"Move up port [{port.Identifier}] for proc [{this.currentProc.Identifier}].");
        }

        /// <summary>
        /// 下移一个Port
        /// </summary>
        /// <param name="portPos"></param>
        private void OnMoveDownPort(int? portPos)
        {
            if (this.currentProc is null)
            {
                ResourceManager.UpdateTip($"An proc must be selected!");
                return;
            }
            if (portPos is null)
            {
                ResourceManager.UpdateTip($"An port must be selected!");
                return;
            }
            if (portPos < 0 || portPos >= this.currentProc.Ports.Count)
            {
                ResourceManager.UpdateTip($"Port pos exceed!");
                return;
            }
            if (portPos == this.currentProc.Ports.Count - 1)
            {
                ResourceManager.UpdateTip($"The port is the bottom one! No need to move down!");
                return;
            }
            int pos = (int)portPos;
            Port port = this.currentProc.Ports[pos];
            this.currentProc.Ports.RemoveAt(pos);
            this.currentProc.Ports.Insert(pos + 1, port);
            ResourceManager.UpdateTip($"Move down port [{port.Identifier}] for proc [{this.currentProc.Identifier}].");
        }


        #endregion
    }
}
