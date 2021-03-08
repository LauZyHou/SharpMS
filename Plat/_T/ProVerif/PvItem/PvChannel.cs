using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif channel item.
    /// </summary>
    public class PvChannel
    {
        private readonly string name;
        private readonly bool isPrivate;

        public PvChannel(string name, bool isPrivate = false)
        {
            this.name = name;
            this.isPrivate = isPrivate;
        }

        public string Name => name;
        public bool IsPrivate => isPrivate;
    }
}
