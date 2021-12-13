using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Plat._VM;
using System;

namespace Plat._V
{
    public partial class Env_V : DragDrop_V
    {
        public Env_V()
        {
            InitializeComponent();
            this.init_binding();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void init_binding()
        {
            // ��������Grid��̬�仯ʱͬʱ�ı��NetworkItem_VM�̳������Ŀ������
            // ��������ʱ�̴�VM��ֱ�ӻ�ȡ�����
            Grid root_grid = ControlExtensions.FindControl<Grid>(this, "RootGrid");
            var observable = root_grid.GetObservable(Grid.BoundsProperty);
            observable.Subscribe(value =>
            {
                if (VM != null)
                {
                    // �������ж���Height��Width��û�б䣬û������û�����ק�ƶ�
                    // ʵ���ϲ��ж�Ҳ����������Ƕ����˼�����ܻ��������ܱ��
                    if (VM.H == value.Height && VM.W == value.Width)
                        return;
                    VM.H = value.Height; // RootGrid.Bounds.Height
                    VM.W = value.Width;
                    // ˢ��ê��λ��
                    VM.FlushAnchorPos();
                }
            });
        }
    }
}
