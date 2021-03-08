using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif process operation (inside of process template), general abstract class.
    /// </summary>
    public abstract class PvOperation
    {
    }

    /// <summary>
    /// ProVerif process end, just a zero.
    /// </summary>
    public class PvEndOp : PvOperation
    {
        public override string ToString()
        {
            return "0.";
        }
    }

    /// <summary>
    /// ProVerif assign operation.
    /// e.g. let suci = Make__SUCI(bv, id_) in
    /// </summary>
    public class PvAssignOp : PvOperation
    {
        private readonly string lh;
        private readonly string rh;

        public PvAssignOp(string lh, string rh)
        {
            this.lh = lh;
            this.rh = rh;
        }

        public string LH => lh;
        public string RH => rh;

        public override string ToString()
        {
            return $"let {lh} = {rh} in";
        }
    }

    /// <summary>
    /// ProVerif new local variable operation.
    /// e.g. new a: Msg;
    /// </summary>
    public class PvNewLocalVarOp : PvOperation
    {
        // todo
    }

    // todo
}
