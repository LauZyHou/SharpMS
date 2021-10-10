using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// 进程模板的实例
    /// </summary>
    public class ProcInst : TopoInst
    {
        private Proc? proc;

        public ProcInst()
            :base()
        {
        }

        /// <summary>
        /// 例化的进程模板
        /// </summary>
        public Proc? Proc { get => proc; set => this.RaiseAndSetIfChanged(ref proc, value); }
    }
}
