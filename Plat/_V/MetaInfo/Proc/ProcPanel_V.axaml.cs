using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Plat._V
{
    public partial class ProcPanel_V : UserControl
    {
        public ProcPanel_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
