using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif type.
    /// </summary>
    public class PvType
    {
        private readonly string name;

        public PvType(string name)
        {
            this.name = name;
        }

        public string Name => name;

        public override string ToString()
        {
            return name;
        }
    }
}
