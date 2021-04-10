using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif process instantiation.
    /// </summary>
    public class PvInstantiation
    {
        private readonly List<PvStatement> statements;

        public PvInstantiation(List<PvStatement> statements)
        {
            this.statements = statements;
        }

        public List<PvStatement> Statements => statements;

        public override string ToString()
        {
            string res = "process\n";
            foreach (PvStatement statement in statements)
            {
                res += $"\t{statement}\n";
            }
            return res;
        }
    }
}
