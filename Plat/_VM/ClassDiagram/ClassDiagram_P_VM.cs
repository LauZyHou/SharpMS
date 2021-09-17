using Plat._C;
using Plat._M;
using System.Collections.Generic;
using System.Diagnostics;

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
            Debug.Assert(src is TopAnchor_VM && dst is BotAnchor_VM);
            Linker_VM arrow_VM = new Linker_VM(src, dst, this);
            // 只记录src的linker
            src.LinkerVM = arrow_VM;
            this.DragDrop_VMs.Add(arrow_VM);
            ResourceManager.UpdateTip($"Create a linker on class diag.");
        }

        /// <summary>
        /// 在类图上删除DragDrop元素（级联删除）
        /// </summary>
        /// <param name="item">要删掉的元素</param>
        public override void DeleteDragDropItem(DragDrop_VM item)
        {
            if (item is Linker_VM)
            {
                Linker_VM linker_VM = (Linker_VM)item;
                this.DragDrop_VMs.Remove(linker_VM);
                // 只清理src的linker
                Debug.Assert(linker_VM.Source is TopAnchor_VM && linker_VM.Dest is BotAnchor_VM);
                TopAnchor_VM src = (TopAnchor_VM)linker_VM.Source;
                src.LinkerVM = null;
                ResourceManager.UpdateTip("Delete a linker on class diagram.");
                return;
            }
            else if (item is Type_VM || item is Env_VM || item is Proc_VM)
            {
                // 所有连到上面的线都删掉
                HashSet<Linker_VM> delSet = new HashSet<Linker_VM>();
                foreach (DragDrop_VM vm in this.DragDrop_VMs)
                {
                    if (vm is Linker_VM)
                    {
                        Linker_VM linker_VM = (Linker_VM)vm;
                        if (linker_VM.Source == item.Anchor_VMs[0] || linker_VM.Dest == item.Anchor_VMs[1])
                        {
                            delSet.Add(linker_VM);
                        }
                    }
                }
                foreach (Linker_VM vm in delSet)
                {
                    DeleteDragDropItem(vm);
                }
                // 然后删掉自己
                this.DragDrop_VMs.Remove(item);
                ResourceManager.UpdateTip($"Delete the component [{item}] on class diagram.");
                return;
            }
            throw new System.NotImplementedException();
        }
    }
}
