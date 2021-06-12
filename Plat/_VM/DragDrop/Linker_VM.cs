using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// DragDrop实体上的连接线的父类
    /// Linker创建触发过程：先将源Anchor设置为活动锚点，再点击目标Anchor
    /// </summary>
    public class Linker_VM : DragDrop_VM
    {
        private Anchor_VM source;
        private Anchor_VM dest;
        private object? extMsg;

        public Linker_VM(Anchor_VM source, Anchor_VM dest, DragDrop_P_VM panelVM)
            : base(0, 0, panelVM)
        {
            this.source = source;
            this.dest = dest;
        }

        /// <summary>
        /// 源锚点引用
        /// </summary>
        public Anchor_VM Source { get => source; set => source = value; }
        /// <summary>
        /// 目标锚点引用
        /// </summary>
        public Anchor_VM Dest { get => dest; set => dest = value; }
        /// <summary>
        /// Linker携带的额外信息，可在具体实现时作不同用途而用
        /// </summary>
        public Object? ExtMsg { get => extMsg; set => extMsg = value; }
    }
}
