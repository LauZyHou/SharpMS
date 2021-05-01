using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// Process，进程模板
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

        /// <summary>
        /// 进程的标识符，就是进程的唯一名字
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 进程的描述，就是一些注释文字
        /// </summary>
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
    }
}
