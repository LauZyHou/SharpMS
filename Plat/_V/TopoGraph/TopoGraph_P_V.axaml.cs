using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Plat._C;
using Plat._VM;

namespace Plat._V
{
    public partial class TopoGraph_P_V : DragDrop_P_V
    {
        public TopoGraph_P_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #region Command Callback

        /// <summary>
        /// [deprecated]
        /// </summary>
        private void OnCreateTopoNode()
        {
            TopoNode_VM topoNode_VM = new TopoNode_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(topoNode_VM);
            ResourceManager.UpdateTip("Create a topology node.");
        }

        private void OnCreateProcInst()
        {
            // 创建ProcInst
            ProcInst_VM procInst_VM = new ProcInst_VM(clkPos.X, clkPos.Y, VM);
            // 创建相应的NodeTag
            ProcInst_NT_VM procInst_NT_VM = new ProcInst_NT_VM(clkPos.X, clkPos.Y + 60, VM, procInst_VM.ProcInst);
            // 通过ExtMsg关联
            procInst_VM.ExtMsg = procInst_NT_VM;
            // 加到DD表里
            VM.DragDrop_VMs.Add(procInst_VM);
            VM.DragDrop_VMs.Add(procInst_NT_VM);
            ResourceManager.UpdateTip("Create a process instance topo node (with corresponding node tag).");
        }

        private void OnCreateEnvInst()
        {
            // 创建EnvInst
            EnvInst_VM envInst_VM = new EnvInst_VM(clkPos.X, clkPos.Y, VM);
            // 创建相应的NodeTag
            EnvInst_NT_VM envInst_NT_VM = new EnvInst_NT_VM(clkPos.X, clkPos.Y + 60, VM, envInst_VM.EnvInst);
            // 通过ExtMsg关联
            envInst_VM.ExtMsg = envInst_NT_VM;
            // 加到DD表里
            VM.DragDrop_VMs.Add(envInst_VM);
            VM.DragDrop_VMs.Add(envInst_NT_VM);
            ResourceManager.UpdateTip("Create an environment instance topo node (with corresponding node tag).");
        }

        #endregion

        /// <summary>
        /// 该View对应的ViewModel
        /// </summary>
        public TopoGraph_P_VM VM
        {
            get
            {
                if (DataContext is null)
                {
                    throw new System.InvalidCastException();
                }
                return (TopoGraph_P_VM)DataContext;
            }
        }
    }
}
