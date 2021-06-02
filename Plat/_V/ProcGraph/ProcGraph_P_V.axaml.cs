using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Plat._C;
using Plat._VM;

namespace Plat._V
{
    /// <summary>
    /// ProcGraph面板的View
    /// </summary>
    public partial class ProcGraph_P_V : DragDrop_P_V
    {
        public ProcGraph_P_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #region Command Callback

        /// <summary>
        /// 右键菜单按下Create Pure State
        /// </summary>
        private void OnCreatePureState()
        {
            PureState_VM pureState_VM = new PureState_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(pureState_VM);
            ResourceManager.UpdateTip($"Create a pure state [{pureState_VM}] on process graph [{VM.ProcGraph}].");
        }

        /// <summary>
        /// 右键菜单按下Create Final State
        /// </summary>
        private void OnCreateFinalState()
        {
            FinalState_VM finalState_VM = new FinalState_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(finalState_VM);
            ResourceManager.UpdateTip($"Create a final state on process graph [{VM.ProcGraph}].");
        }

        #endregion

        /// <summary>
        /// 该View对应的ViewModel
        /// </summary>
        public ProcGraph_P_VM VM { get => (ProcGraph_P_VM)DataContext; }
    }
}
