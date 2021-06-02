using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Plat._C;
using Plat._VM;

namespace Plat._V
{
    /// <summary>
    /// 连线用的锚点View
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

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            // 找到这个锚点所在的Drag Drop Panel
            DragDrop_P_VM panelVM = VM.PanelVM;

            // 如果当前的锚点上有连线，那么要删除连线
            if (VM.LinkerVM is not null)
            {
                // 调用所在Panel的删除法，具体看其自己的实现
                panelVM.DeleteDragDropItem(VM.LinkerVM);
            }
            // 如果已经是该面板上的活动锚点，那么要清除活动状态
            else if (VM == panelVM.ActiveAnchorVM)
            {
                panelVM.ActiveAnchorVM = null;
                VM.IsActive = false;
            }
            // 至此，说明当前是空闲锚点
            // 如果面板上还没有活动锚点，那么设置该锚点为活动锚点
            else if (panelVM.ActiveAnchorVM is null)
            {
                panelVM.ActiveAnchorVM = VM;
                VM.IsActive = true;
            }
            // 至此，说明当前面板上有一个非该锚点的活动锚点，需要连Linker从活动锚点到该锚点
            else
            {
                // 调用所在Panel的创建法，具体看其自己的实现
                panelVM.CreateLinker(panelVM.ActiveAnchorVM, VM);
                // 清除活动锚点
                panelVM.ActiveAnchorVM.IsActive = false;
                panelVM.ActiveAnchorVM = null;
            }

            e.Handled = true;
        }


        /// <summary>
        /// 对应的View Model
        /// </summary>
        public new Anchor_VM VM
        {
            get
            {
                return (Anchor_VM)DataContext;
            }
        }
    }
}
