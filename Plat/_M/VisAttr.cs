using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 带有可见性性质的Attribute
    /// </summary>
    public class VisAttr : ReactiveObject
    {
        private string identifier;
        private Type type;
        private string description;
        private bool pub;

        public VisAttr(string identifier, Type type, string description = "", bool pub = false)
        {
            this.identifier = identifier;
            this.type = type;
            this.description = description;
            this.pub = pub;
        }

        public Type Type { get => type; set => this.RaiseAndSetIfChanged(ref type, value); }
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
        public bool Pub
        {
            get => pub;
            set
            {
                this.RaiseAndSetIfChanged(ref pub, value);
                this.RaisePropertyChanged(nameof(PubStr));
            }
        }

        public string PubStr
        {
            get
            {
                return this.pub ? "+" : "-";
            }
        }
    }
}
