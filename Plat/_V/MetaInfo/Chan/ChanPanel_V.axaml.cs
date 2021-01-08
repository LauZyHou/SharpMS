using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Plat._V
{
    public partial class ChanPanel_V : UserControl
    {
        public ChanPanel_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
