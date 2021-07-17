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
        /// 提示内容
        /// </summary>
        public string? Tip { get => tip; set => this.RaiseAndSetIfChanged(ref tip, value); }
        /// <summary>
        /// MetaInfo面板组
        /// </summary>
        public MetaInfo_PG_VM MetaInfo_PG_VM => metaInfo_PG_VM;
        /// <summary>
        /// ClassDiagram面板
        /// </summary>
        public ClassDiagram_P_VM ClassDiagram_P_VM => classDiagram_P_VM;
        /// <summary>
        /// ProcGraph面板组
        /// </summary>
        public ProcGraph_PG_VM ProcGraph_PG_VM => procGraph_PG_VM;
        /// <summary>
        /// TopoGraph面板
        /// </summary>
        public TopoGraph_P_VM TopoGraph_P_VM => topoGraph_P_VM;
        /// <summary>
        /// 用于绑定主面板上的AnchorVisible的CheckBox的IsChecked属性
        /// 在变更时这里触发，然后去触发ResourceManager的anchorVisible的订阅
        /// 然后所有的锚点Anchor_V在构造时候都绑定了那个订阅，所以可以收到通知
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
