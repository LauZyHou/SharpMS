using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// Channel 信道模板
    /// </summary>
    public class Chan : ReactiveObject
    {
        private string identifier;
        private bool pub;
        private int capacity;

        /// <summary>
        /// 信道模板名
        /// </summary>
        public string Identifier { get => identifier; set => identifier = value; }
        /// <summary>
        /// 是否为公有
        /// </summary>
        public bool Pub { get => pub; set => pub = value; }
        public int Capacity { get => capacity; set => capacity = value; }
    }
}
