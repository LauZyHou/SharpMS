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
        private List<PvParam> @params;
        private PvSeqStmt rootStmt;

        /// <summary>
        /// Construct a new Pv process by name.
        /// </summary>
        /// <param name="name">进程名</param>
        public PvProcess(string name)
        {
            this.name = name;
            this.@params = new List<PvParam>();
            this.rootStmt = new PvSeqStmt();
        }

        /// <summary>
        /// Pv process name.
        /// </summary>
        public string Name => name;
        /// <summary>
        /// Pv process parameters list.
        /// </summary>
        public List<PvParam> Params { get => @params; set => @params = value; }

        public PvSeqStmt RootStmt { get => rootStmt; set => rootStmt = value; }

        /// <summary>
        /// Observe the caller Pv process in the string perspective.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (rootStmt is null)
            {
                return "[Error in PvProcess]";
            }
            return $"let {name}({string.Join(", ", @params)}) = \n{rootStmt}.";
        }
    }
}
