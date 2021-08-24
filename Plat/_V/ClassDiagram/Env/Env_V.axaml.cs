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
            // 在最外层的Grid形态变化时同时改变从NetworkItem_VM继承下来的宽高属性
            // 这样就能时刻从VM里直接获取宽高了
            Grid root_grid = ControlExtensions.FindControl<Grid>(this, "RootGrid");
            var observable = root_grid.GetObservable(Grid.BoundsProperty);
            observable.Subscribe(value =>
            {
                if (VM != null)
                {
                    // 这里是判断下Height和Width有没有变，没变就是用户在拖拽移动
                    // 实际上不判断也不会出错，但是多算了几遍可能会引起性能变差
                    if (VM.H == value.Height && VM.W == value.Width)
                        return;
                    VM.H = value.Height; // RootGrid.Bounds.Height
                    VM.W = value.Width;
                    // 刷新锚点位置
                    VM.FlushAnchorPos();
                }
            });
        }
    }
}
