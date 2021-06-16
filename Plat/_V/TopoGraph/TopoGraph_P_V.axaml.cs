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
        /// 右键菜单按下Create Topo Node
        /// </summary>
        private void OnCreateTopoNode()
        {
            TopoNode_VM topoNode_VM = new TopoNode_VM(clkPos.X, clkPos.Y, VM, null);
            VM.DragDrop_VMs.Add(topoNode_VM);
            ResourceManager.UpdateTip("Create a topology node.");
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
