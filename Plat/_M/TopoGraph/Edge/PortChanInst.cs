using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// ProcInst的Port和EnvInst的Chan关联而成的实例
    /// </summary>
    public class PortChanInst : ReactiveObject
    {
        public static int _id = 0;
        private readonly int id;
        private Port? port;
        private Channel? chan;

        public PortChanInst()
        {
            this.id = ++_id;
        }

        public int Id => id;
        public Port? Port { get => port; set => this.RaiseAndSetIfChanged(ref port, value); }
        public Channel? Chan { get => chan; set => this.RaiseAndSetIfChanged(ref chan, value); }
    }
}
