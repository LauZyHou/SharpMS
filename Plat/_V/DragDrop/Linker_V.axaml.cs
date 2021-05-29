using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Plat._V
{
    public partial class Linker_V : DragDrop_V
    {
        public Linker_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
