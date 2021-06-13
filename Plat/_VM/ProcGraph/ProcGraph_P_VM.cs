using Plat._C;
using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Process Graph面板的View Model
    /// </summary>
    public class ProcGraph_P_VM : DragDrop_P_VM
    {
        private readonly ProcGraph procGraph;

        public ProcGraph_P_VM(ProcGraph procGraph)
        {
            this.procGraph = procGraph;
        }

        public ProcGraph ProcGraph => procGraph;

        /// <summary>
        /// 创建Linker（状态机边）
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public override void CreateLinker(Anchor_VM src, Anchor_VM dst)
        {
            // 线型箭头
            Arrow_VM linker_VM = new Arrow_VM(src, dst, this);
            src.LinkerVM = dst.LinkerVM = linker_VM;
            this.DragDrop_VMs.Add(linker_VM);
            // 转移结点
            TransNode_VM transNode_VM = new TransNode_VM(
                (src.Pos.X + dst.Pos.X) / 2,
                (src.Pos.Y + dst.Pos.Y) /2,
                this,
                linker_VM
            );
            this.DragDrop_VMs.Add(transNode_VM);
            ResourceManager.UpdateTip($"Create a linker on process graph panel, from [{src.HostVM}] to [{dst.HostVM}].");
        }

        /// <summary>
        /// 删除元素（状态、锚点、边）
        /// </summary>
        /// <param name="item"></param>
        public override void DeleteDragDropItem(DragDrop_VM item)
        {
            // 因为有可能删除active anchor所在的item，特判一下
            if (this.ActiveAnchorVM != null && this.ActiveAnchorVM.HostVM == item)
            {
                this.ActiveAnchorVM = null;
            }
            // 视item类别做不同处理
            if (item is Linker_VM)
            {
                Linker_VM linker_VM = (Linker_VM)item;
                linker_VM.Source.LinkerVM = null;
                linker_VM.Dest.LinkerVM = null;
                this.DragDrop_VMs.Remove(linker_VM);
                if (linker_VM.ExtMsg is not null && linker_VM.ExtMsg is TransNode_VM)
                {
                    this.DragDrop_VMs.Remove((TransNode_VM)linker_VM.ExtMsg);
                }
                ResourceManager.UpdateTip("Remove a linker on process graph panel.");
                return;
            }
            else if (item is TinyState_VM || item is PureState_VM)
            {
                foreach (Anchor_VM anchor_VM in item.Anchor_VMs)
                {
                    if (anchor_VM.LinkerVM is not null)
                    {
                        this.DeleteDragDropItem(anchor_VM.LinkerVM);
                    }
                }
                this.DragDrop_VMs.Remove(item);
                ResourceManager.UpdateTip($"Remove the state [{item}] on process graph panel.");
                return;
            }
            else if (item is FinalState_VM)
            {
                // todo
                return;
            }
            throw new NotImplementedException();
        }
    }
}
