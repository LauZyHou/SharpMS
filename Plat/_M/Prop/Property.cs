using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 待验证的性质
    /// </summary>
    public class Property : ReactiveObject
    {
        private Prop prop;
        private string content;
        private string description;

        public Property()
        {
            this.prop = Prop.INVAR;
            this.content = "";
            this.description = "";
        }

        public Prop Prop { get => prop; set => this.RaiseAndSetIfChanged(ref prop, value); }
        public string Content { get => content; set => this.RaiseAndSetIfChanged(ref content, value); }
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        public override string ToString()
        {
            return $"{prop}: {content}";
        }
    }
}
