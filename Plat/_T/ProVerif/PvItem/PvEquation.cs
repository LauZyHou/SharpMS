using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif equation
    /// </summary>
    public class PvEquation
    {
        /// <summary>
        /// params used in this axiom equation.
        /// </summary>
        private readonly List<PvParam> parameters;
        /// <summary>
        /// axiom formula.
        /// </summary>
        private readonly string formula;

        public PvEquation(List<PvParam> parameters, string formula)
        {
            this.parameters = parameters;
            this.formula = formula;
        }

        public List<PvParam> Params => parameters;
        public string Formula => formula;

        public override string ToString()
        {
            return this.formula;
        }

        public static PvEquation ASYM_ENC_DEC = new PvEquation(
            new List<PvParam>()
            {
                new PvParam(PvType.BITSTRING, "m"),
                new PvParam(PvType.PVTKEY, "k")
            },
            "AsymDec(AsymEnc(m, PK(k)),k)=m"
        );
    }
}
