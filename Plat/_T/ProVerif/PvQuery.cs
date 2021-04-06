using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif property query statement.
    /// </summary>
    public abstract class PvQuery
    {
    }

    /// <summary>
    /// ProVerif confidentiality property query statement.
    /// </summary>
    public class PvConfidentiality : PvQuery
    {
        private readonly string globalVar;

        public PvConfidentiality(string globalVar)
        {
            this.globalVar = globalVar;
        }

        public string GlobalVar => globalVar;

        public override string ToString()
        {
            return $"query attacker({globalVar}).";
        }
    }


    /// <summary>
    /// ProVerif authentication property query statement.
    /// </summary>
    public class PvAuthentication : PvQuery
    {
        /// <summary>
        /// left hand event.
        /// </summary>
        private readonly PvEvent lhEvent;
        /// <summary>
        /// right hand event.
        /// </summary>
        private readonly PvEvent rhEvent;
        private readonly bool injective;

        public PvAuthentication(PvEvent lhEvent, PvEvent rhEvent, bool injective = false)
        {
            Debug.Assert(lhEvent.ParamTypes.Count == rhEvent.ParamTypes.Count);
            int typeCnt = lhEvent.ParamTypes.Count;
            for (int i = 0; i < typeCnt; i++)
            {
                // type == type, because of singleton, not use name equal
                Debug.Assert(lhEvent.ParamTypes[i] == rhEvent.ParamTypes[i]);
            }
            this.lhEvent = lhEvent;
            this.rhEvent = rhEvent;
            this.injective = injective;
        }

        public PvEvent LHEvent => lhEvent;
        public PvEvent RHEvent => rhEvent;
        public bool Injective => injective;

        public override string ToString()
        {
            List<string> locVars = new List<string>();
            int len = lhEvent.ParamTypes.Count;
            for (int i = 0; i < len; i++)
            {
                locVars.Add($"x{i}");
            }
            string res = "query ";
            for (int i = 0; i < len; i++)
            {
                res += $"{locVars[i]}: {lhEvent.ParamTypes[i].Name}, ";
            }
            // "query x: xx, y: yy, z: zz, " -> "query x: xx, y: yy, z: zz; "
            var sb = new StringBuilder(res);
            sb[sb.Length - 2] = ';';
            res = sb.ToString();
            // "query x: xx, y: yy, z: zz; event(name(x, y, z)) ==> [inj-]event(name(x, y, z))."
            string locVarsStr = string.Join(", ", locVars);
            return res + $"event({lhEvent.Name}({locVarsStr})) ==> {(injective ? "inj-" : "")}event({rhEvent.Name}({locVarsStr})).";
        }
    }
}
