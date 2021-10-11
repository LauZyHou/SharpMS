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

        private void OnCreateTopoNode()
        {
            TopoNode_VM topoNode_VM = new TopoNode_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(topoNode_VM);
            ResourceManager.UpdateTip("Create a topology node.");
        }

        private void OnCreateProcInst()
        {
            ProcInst_VM procInst_VM = new ProcInst_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(procInst_VM);
            ResourceManager.UpdateTip("Create a process instance topo node.");
        }

        private void OnCreateEnvInst()
        {
            EnvInst_VM envInst_VM = new EnvInst_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(envInst_VM);
            ResourceManager.UpdateTip("Create an environment instance topo node.");
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
