using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL code statement, general abstract class.
    /// </summary>
    public abstract class UpStatement
    {
    }

    /// <summary>
    /// UPPAAL assign statement, i.e. left hand := right hand
    /// e.g. a[1].b.c[0] := myFun(e, f[0])
    /// [todo] class definition need to be improve.
    /// </summary>
    public class UpAssignment : UpStatement
    {
        private readonly string lh;
        private readonly string rh;

        public UpAssignment(string lh, string rh)
        {
            this.lh = lh;
            this.rh = rh;
        }

        public string LH => lh;
        public string RH => rh;

        public override string ToString()
        {
            return $"{lh} := {rh}";
        }
    }

    /// <summary>
    /// UPPAAL pass statement, just an empty code line.
    /// </summary>
    public class UpPass : UpStatement
    {
        public override string ToString()
        {
            return string.Empty;
        }
    }
}
