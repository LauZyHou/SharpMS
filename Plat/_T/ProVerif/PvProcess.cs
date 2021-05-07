using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif process model in memory.
    /// </summary>
    public class PvProcess
    {
        private readonly string name;
        private readonly List<PvParam> parameters;
        private readonly List<PvActiveStmt> statements;

        /// <summary>
        /// Construct a new Pv process by name.
        /// </summary>
        /// <param name="name">process name</param>
        public PvProcess(string name)
        {
            this.name = name;
            this.parameters = new List<PvParam>();
            this.statements = new List<PvActiveStmt>();
        }

        /// <summary>
        /// Append a parameter to caller Pv process.
        /// </summary>
        /// <param name="param">process param</param>
        /// <returns></returns>
        public PvProcess WithParam(PvParam param)
        {
            this.parameters.Add(param);
            return this;
        }

        /// <summary>
        /// Append a statement to caller Pv process.
        /// </summary>
        /// <param name="stmt"></param>
        /// <returns></returns>
        public PvProcess WithStmt(PvActiveStmt stmt)
        {
            this.statements.Add(stmt);
            return this;
        }

        /// <summary>
        /// Pv process name.
        /// </summary>
        public string Name => name;
        /// <summary>
        /// Pv process parameters list.
        /// </summary>
        public List<PvParam> Parameters => parameters;
        /// <summary>
        /// Pv process statements list.
        /// </summary>
        public List<PvActiveStmt> Statements => statements;

        /// <summary>
        /// Observe the caller Pv process in the string perspective.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"let {name}({string.Join(", ", parameters)}) = \n{string.Join("\n\t", statements)}.";
        }
    }
}
