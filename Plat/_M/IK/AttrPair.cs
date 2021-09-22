using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 一个Proc/Env的Attr以及另一个Proc/Env的Attr
    /// </summary>
    public class AttrPair : ReactiveObject
    {
        public Proc ProcA { get; set; }
        public VisAttr AttrA { get; set; }
        // todo
    }
}
