using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._M
{
    /// <summary>
    /// Process和Environment连接时的Co-Tag
    /// </summary>
    public class ProcEnvInst : ReactiveObject
    {
        private ProcInst? procInst;
        private EnvInst? envInst;
        private readonly ObservableCollection<PortChanInst> portChanInsts;

        public ProcEnvInst()
        {
            this.portChanInsts = new ObservableCollection<PortChanInst>();
        }

        /// <summary>
        /// Porc实例引用
        /// </summary>
        public ProcInst? ProcInst { get => procInst; set => this.RaiseAndSetIfChanged(ref procInst, value); }
        /// <summary>
        /// Env实例引用
        /// </summary>
        public EnvInst? EnvInst { get => envInst; set => this.RaiseAndSetIfChanged(ref envInst, value); }
        /// <summary>
        /// Porc.Port-Env.Chan关联的所有列表
        /// </summary>
        public ObservableCollection<PortChanInst> PortChanInsts => portChanInsts;
    }
}
