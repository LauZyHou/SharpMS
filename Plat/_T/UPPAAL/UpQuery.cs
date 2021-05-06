using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL formal verification property query.
    /// </summary>
    public class UpQuery
    {
        private readonly string formula;
        private readonly string comment;

        public UpQuery(string formula, string comment = "")
        {
            this.formula = formula;
            this.comment = comment;
        }

        public string Formula => formula;
        public string Comment => comment;

        public override string ToString()
        {
            return $"\t\t<query>\n" +
                   $"\t\t\t<formula>{formula}</formula>\n" +
                   $"\t\t\t<comment>{comment}</comment>\n" +
                   $"\t\t</query>\n";
        }
    }
}
