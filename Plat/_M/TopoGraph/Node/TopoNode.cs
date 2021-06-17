using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 拓扑结点Model
    /// </summary>
    public class TopoNode : ReactiveObject
    {
        public static int _id = 0;
        private readonly int id;
        private Proc? proc;

        /// <summary>
        /// 用户创建拓扑结点
        /// </summary>
        /// <param name="proc"></param>
        public TopoNode(Proc? proc)
        {
            this.id = _id++;
            this.proc = proc;
        }

        /// <summary>
        /// 从文件导入拓扑结点
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="id"></param>
        public TopoNode(Proc? proc, int id)
        {
            this.id = id;
            this.proc = proc;
        }

        /// <summary>
        /// 自增id
        /// </summary>
        public int Id => id;
        /// <summary>
        /// 拓扑图所例化的进程模板，null表示尚未设置
        /// </summary>
        public Proc? Proc { get => proc; set => this.RaiseAndSetIfChanged(ref proc, value); }
    }
}
