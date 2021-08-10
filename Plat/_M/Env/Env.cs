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
    /// 环境模板（或者就叫环境）
    /// 要注意，环境模板和环境实例之间是"拷贝"关系，而进程模板和进程实例之间则是"例化"关系
    /// </summary>
    public class Env : ReactiveObject
    {
        private string identifier;
        private ObservableCollection<VisAttr> attributes;
        private ObservableCollection<Channel> channels;
        private bool pub;
        private string description;

        public Env(string identifier, bool pub = true)
        {
            this.identifier = identifier;
            this.pub = pub;
            this.attributes = new ObservableCollection<VisAttr>();
            this.channels = new ObservableCollection<Channel>();
            this.description = "";
        }

        /// <summary>
        /// 环境的标识符（在拓扑图里需要唯一
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 环境模板中的所有属性（这里是不带值的，只有定义
        /// </summary>
        public ObservableCollection<VisAttr> Attributes { get => attributes; }
        /// <summary>
        /// 环境模板中的所有信道模板
        /// </summary>
        public ObservableCollection<Channel> Channels { get => channels; }
        /// <summary>
        /// 是否是公开环境
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
        /// 环境的自然语言描述（相当于是一个注释
        /// </summary>
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        #region Have xxx 属性

        public bool HaveAttr
        {
            get
            {
                return this.attributes is not null && this.attributes.Count > 0;
            }
        }

        public bool HaveChan
        {
            get
            {
                return this.channels is not null && this.channels.Count > 0;
            }
        }

        public string PubStr
        {
            get
            {
                return this.pub ? "+" : "-";
            }
        }

        #endregion
    }
}
