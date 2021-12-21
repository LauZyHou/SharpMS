using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif event.
    /// </summary>
    public class PvEvent
    {
        private readonly string name;
        private readonly List<PvType> paramTypes;

        public PvEvent(string name)
        {
            this.name = name;
            this.paramTypes = new List<PvType>();
        }

        public PvEvent(string name, List<PvType> paramTypes)
        {
            this.name = name;
            this.paramTypes = paramTypes;
        }

        public string Name => name;
        public List<PvType> ParamTypes => paramTypes;

        public override string ToString()
        {
            return name;
        }
    }
}
