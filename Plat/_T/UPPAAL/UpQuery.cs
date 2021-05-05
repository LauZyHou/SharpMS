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
            return $"<query>\n<formula>{formula}</formula>\n<comment>{comment}</comment>\n</query>\n";
        }
    }
}
