using Plat._C;
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
        public Proc? Proc
        {
            get => proc;
            set
            {
                this.RaiseAndSetIfChanged(ref proc, value);
                // 当对ProcInst设置新Proc时，要让其连接的所有ProcEnvInst设定的关联信息全失效
                foreach (ProcEnvInst procEnvInst in ResourceManager.procEnvInsts)
                {
                    if (procEnvInst.ProcInst == this)
                    {
                        procEnvInst.PortChanInsts.Clear();
                    }
                }
            }
        }
    }
}
