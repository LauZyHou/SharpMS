using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif process model in memory.
    /// todo: add more attribute
    /// </summary>
    public class PvProcess
    {
        private readonly string name;
        private readonly List<PvParam> parameters;

        public PvProcess(string name, List<PvParam> parameters)
        {
            this.name = name;
            this.parameters = parameters;
        }

        public string Name => name;
        public List<PvParam> Parameters => parameters;
    }
}
