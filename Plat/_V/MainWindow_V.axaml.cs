using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Plat._C;

namespace Plat._V
{
    public partial class MainWindow_V : Window
    {
        public MainWindow_V()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ResourceManager.mainWindow_V = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
