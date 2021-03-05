using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL instantiation is the "model declaration".
    /// </summary>
    public class UpInstantiation
    {
        private readonly List<UpStatement> statements;

        public UpInstantiation()
        {
            this.statements = new List<UpStatement>();
        }

        public List<UpStatement> Statements => statements;

        public override string ToString()
        {
            string res = "<system>\n";
            foreach (UpStatement statement in statements)
            {
                if (statement is null || statement is UpPass)
                {
                    res += "\n";
                }
                else
                {
                    res += $"{statement};\n";
                }
            }
            res += "</system>\n";
            return res;
        }
    }
}
