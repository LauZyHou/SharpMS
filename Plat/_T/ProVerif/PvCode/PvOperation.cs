using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly PvParam param;

        public PvNewLocalVarOp(PvParam param)
        {
            this.param = param;
        }

        public PvParam Param => param;

        public override string ToString()
        {
            return $"new {param};";
        }
    }

    /// <summary>
    /// ProVerif send message operation.
    /// e.g. out(SN2HN, (suci, idHN));
    /// </summary>
    public class PvSendMsgOp : PvOperation
    {
        private readonly PvChannel channel;
        private readonly List<string> vars;

        public PvSendMsgOp(PvChannel channel, List<string> vars)
        {
            this.channel = channel;
            this.vars = vars;
        }

        public PvChannel Channel => channel;
        public List<string> Vars => vars;

        public override string ToString()
        {
            if (vars.Count == 1)
            {
                return $"out({channel.Name}, {vars[0]});";
            }
            return $"out({channel.Name}, ({(string.Join(", ", vars))}));";
        }
    }

    /// <summary>
    /// ProVerif receive message operation.
    /// e.g. in(SN2HN, (suci: SUCI, idHN: int));
    /// </summary>
    public class PvRecvMsgOp : PvOperation
    {
        private readonly PvChannel channel;
        private readonly List<PvParam> pattern;

        public PvRecvMsgOp(PvChannel channel, List<PvParam> pattern)
        {
            this.channel = channel;
            this.pattern = pattern;
        }

        public PvChannel Channel => channel;
        public List<PvParam> Pattern => pattern;

        public override string ToString()
        {
            if (pattern.Count == 1)
            {
                return $"in({channel.Name}, {pattern[0]});";
            }
            return $"in({channel.Name}, ({string.Join(", ", pattern)}));";
        }
    }

    /// <summary>
    /// ProVerif process instantiation operation.
    /// e.g. Proc__UE(id_UE_1, supi, rs, pk_HN_2) | Proc__SN(id_SN_1) | Proc__HN(id_HN_1, sk_HN_1) | Proc__HN(id_HN_2, sk_HN_2)
    /// </summary>
    public class PvInstantiateOp : PvOperation
    {
        /// <summary>
        /// e.g. {Proc__UE, Proc__SN, Proc__HN, Proc__HN}
        /// </summary>
        private readonly List<PvProcess> processes;
        /// <summary>
        /// e.g. {{id_UE_1, supi, rs, pk_HN_2}, {id_SN_1}, {id_HN_1, sk_HN_1}, {id_HN_2, sk_HN_2}}
        /// </summary>
        private readonly List<List<string>> procVars;

        public PvInstantiateOp(List<PvProcess> processes, List<List<string>> procParams)
        {
            Debug.Assert(processes.Count == procParams.Count);
            int procCnt = processes.Count;
            for (int i = 0; i < procCnt; i ++ )
            {
                Debug.Assert(processes[i].Parameters.Count == procParams[i].Count);
                // todo: type check
            }
            this.processes = processes;
            this.procVars = procParams;
        }

        public List<PvProcess> Processes => processes;
        public List<List<string>> ProcParams => procVars;

        public override string ToString()
        {
            List<string> resList = new List<string>();
            int procCnt = processes.Count;
            for (int i = 0; i < procCnt; i ++ )
            {
                resList.Add($"{processes[i].Name}({string.Join(", ", procVars[i])})");
            }
            return string.Join(" | ", resList);
        }
    }
}
