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
    /// ProcGraph����View
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
        /// �Ҽ��˵�����Create Pure State
        /// </summary>
        private void OnCreatePureState()
        {
            PureState_VM pureState_VM = new PureState_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(pureState_VM);
            ResourceManager.UpdateTip($"Create a pure state [{pureState_VM}] on process graph [{VM.ProcGraph}].");
        }

        /// <summary>
        /// �Ҽ��˵�����Create Final State
        /// </summary>
        private void OnCreateFinalState()
        {
            FinalState_VM finalState_VM = new FinalState_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(finalState_VM);
            ResourceManager.UpdateTip($"Create a final state on process graph [{VM.ProcGraph}].");
        }

        /// <summary>
        /// �Ҽ��˵�����Create Tiny State
        /// </summary>
        private void OnCreateTinyState()
        {
            TinyState_VM tinyState_VM = new TinyState_VM(clkPos.X, clkPos.Y, VM);
            VM.DragDrop_VMs.Add(tinyState_VM);
            ResourceManager.UpdateTip($"Create a tiny state [{tinyState_VM}] on process graph [{VM.ProcGraph}].");
        }

        #endregion

        /// <summary>
        /// ��View��Ӧ��ViewModel
        /// </summary>
        public ProcGraph_P_VM VM
        {
            get
            {
                if (DataContext is null)
                {
                    throw new System.InvalidCastException();
                }
                return (ProcGraph_P_VM)DataContext;
            }
        }
    }
}
