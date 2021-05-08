using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif process instantiation.
    /// </summary>
    public class PvInstantiation
    {
        private PvSeqStmt rootStmt;

        public PvInstantiation()
        {
            this.rootStmt = new PvSeqStmt();
        }

        public PvInstantiation(PvSeqStmt rootStmt)
        {
            this.rootStmt = rootStmt;
        }

        /// <summary>
        /// 实例化的根是一个顺序语句
        /// </summary>
        public PvSeqStmt RootStmt { get => rootStmt; set => rootStmt = value; }

        public override string ToString()
        {
            return $"process\n{rootStmt}";
        }
    }
}
