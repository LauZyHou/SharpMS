using Plat._C;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plat._VM
{
    public class MainWindow_VM : ViewModelBase
    {
        private readonly MetaInfo_VM metaInfo_VM = new MetaInfo_VM();
        private readonly ProcGraphPanel_VM procGraphPanel_VM = new ProcGraphPanel_VM();
        private string? tip = "Welcome to SharpMS!";

        public MainWindow_VM()
        {
            ResourceManager.mainWindow_VM = this;
        }

        public string? Tip { get => tip; set => this.RaiseAndSetIfChanged(ref tip, value); }
        public MetaInfo_VM MetaInfo_VM => metaInfo_VM;
        public ProcGraphPanel_VM ProcGraphPanel_VM => procGraphPanel_VM;
    }
}
