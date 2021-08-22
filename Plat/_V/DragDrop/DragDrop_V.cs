using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Plat._VM;

namespace Plat._V
{
    /// <summary>
    /// DragDrop的Item的父View
    /// </summary>
    public class DragDrop_V : UserControl
    {
        // 是否正在按下状态
        private bool isPressed = false;
        // 按下位置坐标
        private Point clkPos;
        // 可视化树上的祖先容器组件,NetworkItem会在它上面移动
        private IVisual? parentIVisual = null;

        #region DragDrop元素的拖拽

        // 鼠标按下
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            // 这里只管普通对象拖拽，Anchor和Linker一律不处理
            if (this is Anchor_V || this is Linker_V)
            {
                return;
            }

            //if (e.MouseButton != MouseButton.Left)
            //    return;
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonPressed)
                return;

            // 所在面板无法在构造函数里求得
            if (parentIVisual == null)
            {
                if (this.Parent is null)
                {
                    throw new System.NullReferenceException();
                }
                parentIVisual = this.Parent.Parent.Parent;
            }

            isPressed = true;
            clkPos = e.GetPosition(parentIVisual);
            VM.OldPos = VM.Pos;

            // 记录每个锚点的旧位置
            foreach (Anchor_VM anchor_VM in VM.Anchor_VMs)
            {
                anchor_VM.OldPos = anchor_VM.Pos;
                // 如果锚点的Linker上吸附了DragDrop_VM，也要记录其旧位置
                if (anchor_VM.LinkerVM is not null
                        && anchor_VM.LinkerVM.ExtMsg is not null
                        && anchor_VM.LinkerVM.ExtMsg is DragDrop_VM)
                {
                    DragDrop_VM attachedVM = (DragDrop_VM)anchor_VM.LinkerVM.ExtMsg;
                    attachedVM.OldPos = attachedVM.Pos;
                }
            }
            //ResourceManager.mainWindowVM.Tips = "鼠标按下，记录图形位置：" + oldLocation;

            e.Handled = true;
        }

        // 鼠标移动
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (isPressed)
            {
                // 计算X/Y方向的偏移量
                //Point pos = e.GetPosition(parentIVisual);
                //double deltaX = pos.X - clkPos.X;
                //double deltaY = pos.Y - clkPos.Y;
                //Point deltaPos = new Point(deltaX, deltaY);
                Point deltaPos = e.GetPosition(parentIVisual) - clkPos;

                // 更新元素新位置
                //VM.Pos = new Point(VM.OldPos.X + deltaX, VM.OldPos.Y + deltaY);
                VM.Pos = VM.OldPos + deltaPos;


                // 对所有锚点也要更新到新位置
                foreach (Anchor_VM anchor_VM in VM.Anchor_VMs)
                {
                    anchor_VM.Pos = anchor_VM.OldPos + deltaPos;
                    // 锚点位置更新时，还要刷新锚点的Linker上吸附的ExtMsg的位置
                    // 我们认为Linker上吸附的东西是放在Linker中央的，所以deltaPos要除以2来给它更新
                    // 注意只有吸附的ExtMsg是DragDrop_VM才对其X/Y作更新
                    if (anchor_VM.LinkerVM is not null
                        && anchor_VM.LinkerVM.ExtMsg is not null
                        && anchor_VM.LinkerVM.ExtMsg is DragDrop_VM)
                    {
                        DragDrop_VM attachedVM = (DragDrop_VM)anchor_VM.LinkerVM.ExtMsg;
                        attachedVM.Pos = attachedVM.OldPos + deltaPos / 2;
                    }
                }

                //ResourceManager.mainWindowVM.Tips = "拖拽图形，图形当前位置：" + NetworkItemVM.X + "," + NetworkItemVM.Y;
            }

            e.Handled = true;
        }

        // 鼠标释放
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);

            isPressed = false;

            //ResourceManager.mainWindowVM.Tips = "完成移动";

            e.Handled = true;
        }

        #endregion

        // 获取DataContext里的VM
        public DragDrop_VM? VM
        {
            get
            {
                if (DataContext is null)
                {
                    //throw new System.InvalidCastException();
                    return null;
                }
                return (DragDrop_VM)DataContext;
            }
        }
    }
}
