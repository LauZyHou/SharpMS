using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif code segment declaration, for global variable declaration or process model.
    /// </summary>
    public class PvDeclaration
    {
        private readonly List<PvStatement> statements;

        public PvDeclaration()
        {
            this.statements = new List<PvStatement>();
        }

        public PvDeclaration(List<PvStatement> statements)
        {
            this.statements = statements;
        }

        public List<PvStatement> Statements => statements;

        public override string ToString()
        {
            string res = "";
            foreach (PvStatement statement in Statements)
            {
                if (statement is PvNewLineForDec)
                    res += "\n";
                else
                    res += $"{statement}\n";
            }
            return res;
        }
    }
}
