using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif code statement, general abstract class.
    /// </summary>
    public abstract class PvStatement
    {
    }

    #region outside of process template

    /// <summary>
    /// ProVerif type declaration statement.
    /// e.g. type Msg.
    /// </summary>
    public class PvTypeDeclaration : PvStatement
    {
        private readonly PvType type;

        public PvTypeDeclaration(PvType type)
        {
            this.type = type;
        }

        public PvType Type => type;

        public override string ToString()
        {
            return $"type {type.Name}.";
        }
    }

    /// <summary>
    /// ProVerif fuction declaration statemen.
    /// e.g. fun pk(skey): pkey.
    /// e.g. fun aenc(Msg, pkey): Msg.
    /// </summary>
    public class PvFuncDeclaration : PvStatement
    {
        private readonly string name;
        private readonly List<PvType> paramTypes;
        private readonly PvType returnType;

        public PvFuncDeclaration(string name, List<PvType> paramTypes, PvType returnType)
        {
            this.name = name;
            this.paramTypes = paramTypes;
            this.returnType = returnType;
        }

        public string Name => name;
        public List<PvType> ParamTypes => paramTypes;
        public PvType ReturnType => returnType;

        public override string ToString()
        {
            return $"fun {name}({string.Join(", ", paramTypes)}): {returnType}.";
        }
    }

    /// <summary>
    /// ProVerif axiom reduction declaration statement.
    /// </summary>
    public class PvReducDeclaration : PvStatement
    {
        /// <summary>
        /// params used in this axiom reduction.
        /// </summary>
        private readonly List<PvParam> parameters;
        /// <summary>
        /// axiom formula.
        /// </summary>
        private readonly string formula;

        public PvReducDeclaration(List<PvParam> parameters, string formula)
        {
            this.parameters = parameters;
            this.formula = formula;
        }

        public List<PvParam> Params => parameters;
        public string Formula => formula;

        public override string ToString()
        {
            return $"reduc forall {string.Join(", ", parameters)}; {formula}.";
        }
    }

    /// <summary>
    /// ProVerif channel declaration statement.
    /// e.g. free UES2: channel.
    /// e.g. free SN2HN: channel [private].
    /// </summary>
    public class PvChannelDeclaration : PvStatement
    {
        private readonly PvChannel channel;

        public PvChannelDeclaration(PvChannel channel)
        {
            this.channel = channel;
        }

        public PvChannel Channel => channel;

        public override string ToString()
        {
            return $"free {channel.Name}: channel{(channel.IsPrivate ? "[private]" : "")}.";
        }
    }

    /// <summary>
    /// ProVerif global variable declaration statement.
    /// e.g. free sk_HN_1: skey [private].
    /// </summary>
    public class PvGlobalVarDeclaration : PvStatement
    {
        private readonly PvGlobalVar var;

        public PvGlobalVarDeclaration(PvGlobalVar var)
        {
            this.var = var;
        }

        public PvGlobalVar Var => var;

        public override string ToString()
        {
            return $"free {var.Name}: {var.Type}{(var.IsPrivate ? "[private]" : "")}.";
        }
    }

    /// <summary>
    /// ProVerif event declaration.
    /// </summary>
    public class PvEventDeclaration : PvStatement
    {
        private readonly PvEvent @event;

        public PvEventDeclaration(PvEvent @event)
        {
            this.@event = @event;
        }

        public PvEvent Event => @event;

        public override string ToString()
        {
            return $"event {@event.Name}({(string.Join(", ", @event.ParamTypes))}).";
        }
    }

    #endregion

    #region inside of process template

    /// <summary>
    /// ProVerif process end, just a zero.
    /// </summary>
    public class PvEndOp : PvStatement
    {
        public override string ToString()
        {
            return "0.";
        }
    }

    /// <summary>
    /// ProVerif assign operate statement.
    /// e.g. let suci = Make__SUCI(bv, id_) in
    /// </summary>
    public class PvAssignment : PvStatement
    {
        private readonly string lh;
        private readonly string rh;

        public PvAssignment(string lh, string rh)
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
    /// ProVerif new local variable operate statement.
    /// e.g. new a: Msg;
    /// </summary>
    public class PvNewVar : PvStatement
    {
        private readonly PvParam param;

        public PvNewVar(PvParam param)
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
    /// ProVerif send message operate statement.
    /// e.g. out(SN2HN, (suci, idHN));
    /// </summary>
    public class PvSendMsg : PvStatement
    {
        private readonly PvChannel channel;
        private readonly List<string> vars;

        public PvSendMsg(PvChannel channel, List<string> vars)
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
    /// ProVerif receive message operate statement.
    /// e.g. in(SN2HN, (suci: SUCI, idHN: int));
    /// </summary>
    public class PvRecvMsg : PvStatement
    {
        private readonly PvChannel channel;
        private readonly List<PvParam> pattern;

        public PvRecvMsg(PvChannel channel, List<PvParam> pattern)
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
    /// ProVerif process instantiation operate statement.
    /// e.g. Proc__UE(id_UE_1, supi, rs, pk_HN_2) | Proc__SN(id_SN_1) | Proc__HN(id_HN_1, sk_HN_1) | Proc__HN(id_HN_2, sk_HN_2)
    /// </summary>
    public class PvConcurrency : PvStatement
    {
        /// <summary>
        /// e.g. {Proc__UE, Proc__SN, Proc__HN, Proc__HN}
        /// </summary>
        private readonly List<PvProcess> processes;
        /// <summary>
        /// e.g. {{id_UE_1, supi, rs, pk_HN_2}, {id_SN_1}, {id_HN_1, sk_HN_1}, {id_HN_2, sk_HN_2}}
        /// </summary>
        private readonly List<List<string>> procVars;

        public PvConcurrency(List<PvProcess> processes, List<List<string>> procParams)
        {
            Debug.Assert(processes.Count == procParams.Count);
            int procCnt = processes.Count;
            for (int i = 0; i < procCnt; i++)
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
            for (int i = 0; i < procCnt; i++)
            {
                resList.Add($"{processes[i].Name}({string.Join(", ", procVars[i])})");
            }
            return string.Join(" | ", resList);
        }
    }

    #endregion
}
