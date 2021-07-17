using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Plat._VM;

namespace Plat._V
{
    public partial class ClassDiagram_P_V : DragDrop_P_V
    {
        public ClassDiagram_P_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// 该View对应的ViewModel
        /// </summary>
        public ClassDiagram_P_VM VM
        {
            get
            {
                if (DataContext is null)
                {
                    throw new System.InvalidCastException();
                }
                return (ClassDiagram_P_VM)DataContext;
            }
        }
    }
}
