using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 逻辑Channel组，即有相同的{source proc, dest proc}的LogicChan
    /// </summary>
    public class LogicChanGroup : ReactiveObject
    {
        private readonly ObservableCollection<LogicChan> logicChanList;
        private Proc source;
        private Proc dest;

        public LogicChanGroup(Proc source, Proc dest)
        {
            this.source = source;
            this.dest = dest;
            this.logicChanList = new ObservableCollection<LogicChan>();
        }

        /// <summary>
        /// 源Process
        /// </summary>
        public Proc Source { get => source; set => this.RaiseAndSetIfChanged(ref source, value); }
        /// <summary>
        /// 目标Process
        /// </summary>
        public Proc Dest { get => dest; set => this.RaiseAndSetIfChanged(ref dest, value); }
        /// <summary>
        /// 逻辑Channel组中的所有LogicChan，形成一个列表
        /// </summary>
        public ObservableCollection<LogicChan> LogicChanList => logicChanList;
    }
}
