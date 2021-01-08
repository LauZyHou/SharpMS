using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 进程模板
    /// </summary>
    public class Proc : ReactiveObject
    {
        private string identifier;
        private string description;

        public Proc(string identifier, string description = "")
        {
            this.identifier = identifier;
            this.description = description;
        }

        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
    }
}
