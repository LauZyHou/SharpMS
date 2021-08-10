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
    public class Channel : ReactiveObject
    {
        private string identifier;
        private bool pub;
        private int capacity;

        public Channel(string identifier, int capacity, bool pub = true)
        {
            this.identifier = identifier;
            this.capacity = capacity;
            this.pub = pub;
        }

        /// <summary>
        /// 信道模板名
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 是否为公有
        /// </summary>
        public bool Pub
        {
            get => pub;
            set
            {
                this.RaiseAndSetIfChanged(ref pub, value);
                this.RaisePropertyChanged(nameof(PubStr));
            }
        }
        public int Capacity { get => capacity; set => this.RaiseAndSetIfChanged(ref capacity, value); }

        public string PubStr
        {
            get
            {
                return this.pub ? "+" : "-";
            }
        }
    }
}
