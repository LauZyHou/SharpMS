using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 逻辑Channel
    /// </summary>
    public class LogicChan : ReactiveObject
    {
        private string identifier;
        private LogicChanGroup? logicChanGroup;

        public LogicChan(string identifier, LogicChanGroup logicChanGroup)
        {
            this.identifier = identifier;
            this.LogicChanGroup = logicChanGroup;
        }

        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        public LogicChanGroup? LogicChanGroup { get => logicChanGroup; set => this.RaiseAndSetIfChanged(ref logicChanGroup, value); }
    }
}
