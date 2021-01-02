using System;
using System.Collections.Generic;
using System.Text;

namespace Plat._VM
{
    public class MainWindow_VM : ViewModelBase
    {
        private readonly MetaInfo_VM metaInfo_VM = new MetaInfo_VM();
        private readonly ProcGraph_VM procGraph_VM = new ProcGraph_VM();

        public string Greeting => "Welcome to SharpMS!";

        public MetaInfo_VM MetaInfo_VM => metaInfo_VM;
        public ProcGraph_VM ProcGraph_VM => procGraph_VM;
    }
}
