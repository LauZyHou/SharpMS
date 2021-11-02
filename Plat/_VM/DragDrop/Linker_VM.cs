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
        public static int _id = 0;
        private int id;
        private Anchor_VM source;
        private Anchor_VM dest;

        public Linker_VM(Anchor_VM source, Anchor_VM dest, DragDrop_P_VM panelVM)
            : base(0, 0, panelVM)
        {
            this.id = ++_id;
            this.source = source;
            this.dest = dest;
        }

        public int Id { get => id; set => id = value; }
        /// <summary>
        /// 源锚点引用
        /// </summary>
        public Anchor_VM Source { get => source; set => source = value; }
        /// <summary>
        /// 目标锚点引用
        /// </summary>
        public Anchor_VM Dest { get => dest; set => dest = value; }
    }
}
