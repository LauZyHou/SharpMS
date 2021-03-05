using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL declaration model, which can be trans to "<declaration/>".
    /// UPPAAL declaration is used on every process context declaration and global declaration.
    /// </summary>
    public class UpDeclaration
    {
        private readonly List<UpStatement> statements;

        public UpDeclaration()
        {
            this.statements = new List<UpStatement>();
        }

        public List<UpStatement> Statements => statements;

        public override string ToString()
        {
            string res = "<declaration>\n";
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
            res += "</declaration>\n";
            return res;
        }
    }
}
