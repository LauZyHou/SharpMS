using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Plat._V
{
    public partial class Env_V : DragDrop_V
    {
        public Env_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
