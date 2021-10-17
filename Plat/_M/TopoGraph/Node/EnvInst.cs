using Plat._C;
using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// 环境模板的实例
    /// </summary>
    public class EnvInst : TopoInst
    {
        private Env? env;

        public EnvInst()
            :base()
        {
        }

        /// <summary>
        /// 例化的环境模板
        /// </summary>
        public Env? Env
        {
            get => env;
            set
            {
                this.RaiseAndSetIfChanged(ref env, value);
                // 当对EnvInst设置新Env时，要让其连接的所有ProcEnvInst设定的关联信息全失效
                foreach (ProcEnvInst procEnvInst in ResourceManager.procEnvInsts)
                {
                    if (procEnvInst.EnvInst == this)
                    {
                        procEnvInst.PortChanInsts.Clear();
                    }
                }
            }
        }
    }
}
