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
        private static int _id = 0;
        private string identifier;
        private bool pub;
        private int capacity;
        private string description;

        /// <summary>
        /// 无参构造
        /// </summary>
        public Channel()
        {
            this.identifier = $"c{++_id}";
            this.capacity = 1;
            this.pub = true;
            this.description = "";
        }

        /// <summary>
        /// 带有标识和容量的构造
        /// </summary>
        /// <param name="identifier">信道标识</param>
        /// <param name="capacity">容量</param>
        /// <param name="pub">是否公开</param>
        /// <param name="description">注解描述</param>
        public Channel(string identifier, int capacity, bool pub = true, string description = "")
        {
            this.identifier = identifier;
            this.capacity = capacity;
            this.pub = pub;
            this.description = description;
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
        /// <summary>
        /// 信道容量
        /// </summary>
        public int Capacity { get => capacity; set => this.RaiseAndSetIfChanged(ref capacity, value); }
        /// <summary>
        /// 信道描述
        /// </summary>
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        public string PubStr
        {
            get
            {
                return this.pub ? "+" : "-";
            }
        }
    }
}
