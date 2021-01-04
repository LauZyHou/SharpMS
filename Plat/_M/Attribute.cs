using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    public class Attribute
    {
        private string identifier;
        private Type type;

        public Attribute(string identifier, Type type)
        {
            this.identifier = identifier;
            this.type = type;
        }

        public Type Type { get => type; set => type = value; }
        public string Identifier { get => identifier; set => identifier = value; }
    }
}
