using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 拓扑图面板
    /// </summary>
    public class TopoGraph_P_VM : DragDrop_P_VM
    {

        /// <summary>
        /// 创建拓扑连线
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public override void CreateLinker(Anchor_VM src, Anchor_VM dst)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除拓扑图上的DragDrop元素
        /// </summary>
        /// <param name="item"></param>
        public override void DeleteDragDropItem(DragDrop_VM item)
        {
            throw new NotImplementedException();
        }
    }
}
