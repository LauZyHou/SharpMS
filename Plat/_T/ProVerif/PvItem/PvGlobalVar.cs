using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    public class PvGlobalVar
    {
        private readonly PvType type;
        private readonly string name;
        private readonly bool isPrivate;

        public PvGlobalVar(PvType type, string name, bool isPrivate)
        {
            this.type = type;
            this.name = name;
            this.isPrivate = isPrivate;
        }

        public PvType Type => type;
        public string Name => name;
        public bool IsPrivate => isPrivate;
    }
}
