using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Plat._V
{
    /// <summary>
    /// �����õ�ê��View
    /// </summary>
    public partial class Anchor_V : DragDrop_V
    {
        public Anchor_V()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
