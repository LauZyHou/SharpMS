using Plat._M;

namespace Plat._VM
{
    /// <summary>
    /// 类图面板
    /// </summary>
    public class ClassDiagram_P_VM : DragDrop_P_VM
    {
        public ClassDiagram_P_VM()
        {
            // 预定义Type的VM都放进来
            this.DragDrop_VMs.Add(new Type_VM(50, 50, this, Type.TYPE_INT));
            this.DragDrop_VMs.Add(new Type_VM(200, 50, this, Type.TYPE_BOOL));
            this.DragDrop_VMs.Add(new Type_VM(350, 50, this, Type.TYPE_MSG));
            this.DragDrop_VMs.Add(new Type_VM(500, 50, this, Type.TYPE_KEY));
            this.DragDrop_VMs.Add(new Type_VM(650, 50, this, Type.TYPE_PUB_KEY));
            this.DragDrop_VMs.Add(new Type_VM(820, 50, this, Type.TYPE_PVT_KEY));
        }

        /// <summary>
        /// 在类图上创建连线
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public override void CreateLinker(Anchor_VM src, Anchor_VM dst)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在类图上删除DragDrop元素
        /// </summary>
        /// <param name="item"></param>
        public override void DeleteDragDropItem(DragDrop_VM item)
        {
            throw new System.NotImplementedException();
        }
    }
}
