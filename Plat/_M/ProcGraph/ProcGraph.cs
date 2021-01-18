using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// Process Graph
    /// </summary>
    public class ProcGraph : ReactiveObject
    {
        private readonly Proc proc;

        public ProcGraph(Proc proc)
        {
            this.proc = proc;
        }

        public Proc Proc => proc;
    }
}
