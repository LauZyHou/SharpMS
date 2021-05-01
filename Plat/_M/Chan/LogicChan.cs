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

        /// <summary>
        /// 逻辑Channel的标识符，同Source和Dest通过这个标识符区分不同的逻辑Channel
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 反引该LogicChan所在的LogicChanGroup，在这里可以获取其Source和Dest
        /// </summary>
        public LogicChanGroup? LogicChanGroup { get => logicChanGroup; set => this.RaiseAndSetIfChanged(ref logicChanGroup, value); }
    }
}
