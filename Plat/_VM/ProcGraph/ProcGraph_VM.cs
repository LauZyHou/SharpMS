using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Process Graph View Model
    /// </summary>
    public class ProcGraph_VM : DragDrop_P_VM
    {
        private readonly ProcGraph procGraph;

        public ProcGraph_VM(ProcGraph procGraph)
        {
            this.procGraph = procGraph;
        }

        public ProcGraph ProcGraph => procGraph;
    }
}
