using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// Process Graph，一个Proc对应这样一张状态机图
    /// </summary>
    public class ProcGraph : ReactiveObject
    {
        private readonly Proc proc;

        public ProcGraph(Proc proc)
        {
            this.proc = proc;
        }

        /// <summary>
        /// 对应的Proc，有且仅有一个
        /// Proc和ProcGraph是双射（单满射）关系
        /// </summary>
        public Proc Proc => proc;

        public override string ToString()
        {
            return proc.ToString();
        }
    }
}
