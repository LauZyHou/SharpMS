using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif parameter definition.
    /// </summary>
    public class PvParam
    {
        private readonly PvType type;
        private readonly string name;

        public PvParam(PvType type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public PvType Type => type;
        public string Name => name;

        public override string ToString()
        {
            return $"{name}: {type.Name}";
        }
    }
}
