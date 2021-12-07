using Plat._C;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plat._VM
{
    public class MainWindow_VM : ViewModelBase
    {
        private string? tip = "Welcome to SharpMS!";
        private readonly MetaInfo_PG_VM metaInfo_PG_VM = new MetaInfo_PG_VM();
        private readonly ClassDiagram_P_VM classDiagram_P_VM = new ClassDiagram_P_VM();
        private readonly ProcGraph_PG_VM procGraph_PG_VM = new ProcGraph_PG_VM();
        private readonly TopoGraph_P_VM topoGraph_P_VM = new TopoGraph_P_VM();
        private bool anchorVisible;

        public MainWindow_VM()
        {
            ResourceManager.mainWindow_VM = this;
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        public string? Tip { get => tip; set => this.RaiseAndSetIfChanged(ref tip, value); }
        /// <summary>
        /// MetaInfo�����
        /// </summary>
        public MetaInfo_PG_VM MetaInfo_PG_VM => metaInfo_PG_VM;
        /// <summary>
        /// ClassDiagram���
        /// </summary>
        public ClassDiagram_P_VM ClassDiagram_P_VM => classDiagram_P_VM;
        /// <summary>
        /// ProcGraph�����
        /// </summary>
        public ProcGraph_PG_VM ProcGraph_PG_VM => procGraph_PG_VM;
        /// <summary>
        /// TopoGraph���
        /// </summary>
        public TopoGraph_P_VM TopoGraph_P_VM => topoGraph_P_VM;
        /// <summary>
        /// ���ڰ�������ϵ�AnchorVisible��CheckBox��IsChecked����
        /// �ڱ��ʱ���ﴥ����Ȼ��ȥ����ResourceManager��anchorVisible�Ķ���
        /// Ȼ�����е�ê��Anchor_V�ڹ���ʱ�򶼰����Ǹ����ģ����Կ����յ�֪ͨ
        /// </summary>
        public bool AnchorVisible
        {
            get => anchorVisible;
            set
            {
                anchorVisible = value;
                ResourceManager.anchorVisible.OnNext(value);
            }
        }
    }
}
