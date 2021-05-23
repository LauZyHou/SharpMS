using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Plat._VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._V
{
    /// <summary>
    /// DragDrop的Panel的父View
    /// 我们约定每个DragDrop_P_V在实现时，里面一定有一个名为panel的ListBox，用来存里面的DragDrop控件
    /// </summary>
    public class DragDrop_P_V : UserControl
    {
        /// <summary>
        /// 记录用户鼠标右键按下时候的位置，用于在创建对象时在当前鼠标所在的位置创建
        /// </summary>
        public Point mousePos;

        // 无法直接获取到鼠标位置，必须在鼠标相关事件回调方法里取得
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            // 特别注意，要取得的不是相对这个DragDrop_P_V的位置，而是相对于里面的内容控件
            ListBox panel = ControlExtensions.FindControl<ListBox>(this, "panel");
            // 右键在这个面板上按下时
            if (e.GetCurrentPoint(panel).Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
            {
                // 更新位置
                mousePos = e.GetPosition(panel);
            }
        }
    }
}
