using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif进程实例
    /// </summary>
    public class PvProcInst
    {
        private PvProcess proc;
        private List<string> @params = new List<string>();
        private bool unlimited = false;

        public PvProcInst(PvProcess proc, bool unlimited = false)
        {
            this.proc = proc;
            this.unlimited = unlimited;
        }

        /// <summary>
        /// 实例化的是哪个进程模板
        /// </summary>
        public PvProcess Proc { get => proc; set => proc = value; }
        /// <summary>
        /// 对进程实例化时候的参数表
        /// </summary>
        public List<string> Params { get => @params; set => @params = value; }
        /// <summary>
        /// 无限数量实例化，默认为flase，即只实例化一个
        /// </summary>
        public bool Unlimited { get => unlimited; set => unlimited = value; }

        public override string ToString()
        {
            if (unlimited)
            {
                return $"(!{proc.Name}({string.Join(", ", @params)}))";
            }
            return $"{proc.Name}({string.Join(", ", @params)})";
        }
    }
}
