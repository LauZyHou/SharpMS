using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL type.
    /// </summary>
    public class UpType
    {
        private readonly string name;

        public UpType(string name)
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
