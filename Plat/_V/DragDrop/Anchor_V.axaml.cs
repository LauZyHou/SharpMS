using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Plat._C;
using Plat._VM;

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
            init_binding();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// ��갴��
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            // �ҵ����ê�����ڵ�Drag Drop Panel
            DragDrop_P_VM panelVM = VM.PanelVM;

            // �����ǰ��ê���������ߣ���ôҪɾ������
            if (VM.LinkerVM is not null)
            {
                // ��������Panel��ɾ���������忴���Լ���ʵ��
                panelVM.DeleteDragDropItem(VM.LinkerVM);
            }
            // ����Ѿ��Ǹ�����ϵĻê�㣬��ôҪ����״̬
            else if (VM == panelVM.ActiveAnchorVM)
            {
                panelVM.ActiveAnchorVM = null;
                VM.IsActive = false;
            }
            // ���ˣ�˵����ǰ�ǿ���ê��
            // �������ϻ�û�лê�㣬��ô���ø�ê��Ϊ�ê��
            else if (panelVM.ActiveAnchorVM is null)
            {
                panelVM.ActiveAnchorVM = VM;
                VM.IsActive = true;
            }
            // ���ˣ�˵����ǰ�������һ���Ǹ�ê��Ļê�㣬��Ҫ��Linker�ӻê�㵽��ê��
            else
            {
                // ��������Panel�Ĵ����������忴���Լ���ʵ��
                panelVM.CreateLinker(panelVM.ActiveAnchorVM, VM);
                // ����ê��
                panelVM.ActiveAnchorVM.IsActive = false;
                panelVM.ActiveAnchorVM = null;
            }

            e.Handled = true;
        }

        /// <summary>
        /// ��ʼ�����ݰ�
        /// </summary>
        private void init_binding()
        {
            this.Bind(Anchor_V.IsVisibleProperty, ResourceManager.anchorVisible);
            // ��Ȼ������ˣ��������ڲ�ȷ������ʱ���״̬
            // ��Ҫ������Ѵ���ʱ��Ŀ���״̬�ٸ�ֵ��ȥ
            // ����������ʱ�Ĳ�һ�µ����
            // ���磺����״̬��false������ʱIsVisibleΪTrue�������˰�
            // �������ڿ���״̬û�б��������IsVisible�����Լ����False
            // �������ʱ����Ƕ��ݵĲ�һ�µ����
            this.IsVisible = ResourceManager.mainWindow_VM.AnchorVisible;
        }

        /// <summary>
        /// ��Ӧ��View Model
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
