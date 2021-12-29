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

            // ��ͼ������ͼ����Top��Bot���ߵ����Ĵ����߼�
            if (panelVM is ClassDiagram_P_VM || panelVM is TopoGraph_P_VM)
            {
                // ��ǰ�����topê��
                if (VM is TopAnchor_VM)
                {
                    // ����ɾ��
                    if (VM.LinkerVM is not null)
                    {
                        panelVM.DeleteDragDropItem(VM.LinkerVM);
                    }
                    // �Լ��Ѽ�����
                    else if (VM == panelVM.ActiveAnchorVM)
                    {
                        panelVM.ActiveAnchorVM = null;
                        VM.IsActive = false;
                    }
                    // ������ê�㼤������Լ�
                    else if (panelVM.ActiveAnchorVM is not null)
                    {
                        panelVM.ActiveAnchorVM.IsActive = false;
                        panelVM.ActiveAnchorVM = VM;
                        VM.IsActive = true;
                    }
                    // ûê�㼤����Լ�����
                    else
                    {
                        panelVM.ActiveAnchorVM = VM;
                        VM.IsActive = true;
                    }
                }
                // ��ǰ�����botê��
                else
                {
                    // �Ѿ��м�����ˣ�������
                    if (panelVM.ActiveAnchorVM is not null)
                    {
                        panelVM.CreateLinker(panelVM.ActiveAnchorVM, VM);
                        panelVM.ActiveAnchorVM.IsActive = false;
                        panelVM.ActiveAnchorVM = null;
                    }
                    // ��������£������κ���Ӧ
                    else
                    {
                        ResourceManager.UpdateTip("Can not tap bot anchor unless you wanna link an active top anchor to it!");
                    }
                }
                goto OVER;
            }

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

        OVER:
            e.Handled = true;
        }

        /// <summary>
        /// ��ʼ�����ݰ�
        /// </summary>
        private void init_binding()
        {
            if (ResourceManager.anchorVisible is null || ResourceManager.mainWindow_VM is null)
            {
                return;
            }
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
                if (DataContext is null)
                {
                    throw new System.InvalidCastException();
                }
                return (Anchor_VM)DataContext;
            }
        }
    }
}
