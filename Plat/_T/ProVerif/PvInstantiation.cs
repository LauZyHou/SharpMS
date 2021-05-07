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
        private readonly List<PvActiveStmt> statements;

        public PvInstantiation()
        {
            this.statements = new List<PvActiveStmt>();
        }

        public PvInstantiation(List<PvActiveStmt> statements)
        {
            this.statements = statements;
        }

        public List<PvActiveStmt> Statements => statements;

        public override string ToString()
        {
            string res = "process\n";
            foreach (PvActiveStmt statement in statements)
            {
                res += $"\t{statement}\n";
            }
            return res;
        }
    }
}
