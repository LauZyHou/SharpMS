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
            return $"free {channel.Name}: channel{(channel.IsPrivate ? " [private]" : "")}.";
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
            return $"free {var.Name}: {var.Type}{(var.IsPrivate ? " [private]" : "")}.";
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
            if (@event.ParamTypes.Count == 0)
            {
                return $"event {@event.Name}.";
            }
            return $"event {@event.Name}({(string.Join(", ", @event.ParamTypes))}).";
        }
    }
}
