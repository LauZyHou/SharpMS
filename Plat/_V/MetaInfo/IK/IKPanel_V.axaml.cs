using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Plat._V
{
    /// <summary>
    /// Initial Knowledge Panel View Model
    /// </summary>
    public partial class IKPanel_V : DragDrop_P_V
    {
        public IKPanel_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
