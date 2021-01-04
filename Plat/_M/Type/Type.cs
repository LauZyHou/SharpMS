using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    public class Type : ReactiveObject
    {
        private string identifier;
        private string description;
        private Type? parent;
        private ObservableCollection<Attribute> attributes;
        private readonly bool isBase;

        public Type(string identifier, string description = "", bool isBase = false)
        {
            this.identifier = identifier;
            this.description = description;
            this.isBase = isBase;
            this.parent = null;
            this.attributes = new ObservableCollection<Attribute>();
        }

        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
        public Type? Parent { get => parent; set => parent = value; }
        public ObservableCollection<Attribute> Attributes { get => attributes; set => attributes = value; }
        public bool IsBase => isBase;

        public static readonly Type TYPE_INT = new Type("Int", "integer number", true);
        public static readonly Type TYPE_MSG = new Type("Msg", "message in network", true);

    }
}
