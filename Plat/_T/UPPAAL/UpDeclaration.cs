using Plat._C;
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
        private bool isLocal;

        public UpDeclaration(bool isLocal = false)
        {
            this.statements = new List<UpStatement>();
            this.isLocal = isLocal;
        }

        public List<UpStatement> Statements => statements;
        public bool IsLocal { get => isLocal; set => isLocal = value; }

        public override string ToString()
        {
            string res = $"\t{(isLocal?"\t":"")}<declaration>//{CommonDumpManager.AutoGenTips}\n";
            foreach (UpStatement statement in statements)
            {
                if (statement is null || statement is UpPass)
                {
                    res += "\n";
                }
                else if (statement is UpComment)
                {
                    res += $"{statement}\n";
                }
                else
                {
                    res += $"{statement};\n";
                }
            }
            res += $"\t{(isLocal?"\t":"")}</declaration>\n";
            return res;
        }
    }
}
