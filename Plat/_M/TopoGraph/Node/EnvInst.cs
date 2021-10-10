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

        public Env? Env { get => env; set => this.RaiseAndSetIfChanged(ref env, value); }
    }
}
