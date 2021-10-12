using Plat._C;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// 创建Liner（拓扑边）
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public override void CreateLinker(Anchor_VM src, Anchor_VM dst)
        {
            Debug.Assert(src is TopAnchor_VM && dst is BotAnchor_VM);
            // 无箭头边
            Linker_VM linker_VM = new Linker_VM(src, dst, this);
            // src设linker
            src.LinkerVM = linker_VM;
            // dst加linker
            BotAnchor_VM botDst = (BotAnchor_VM)dst;
            botDst.AddLinker(linker_VM);
            // DD表里加linker
            this.DragDrop_VMs.Add(linker_VM);
            // todo extmsg
            ResourceManager.UpdateTip($"Create topology edge on topology graph, from [{src.HostVM}] to [{dst.HostVM}].");
        }

        /// <summary>
        /// 删除拓扑图上的DragDrop元素
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
                Debug.Assert(linker_VM.Source is TopAnchor_VM && linker_VM.Dest is BotAnchor_VM);
                // src删linker
                linker_VM.Source.LinkerVM = null;
                // dst移除linker
                BotAnchor_VM dstBot = (BotAnchor_VM)linker_VM.Dest;
                dstBot.RemoveLinker(linker_VM);
                // DD表里移除linker
                this.DragDrop_VMs.Remove(linker_VM);
                // todo extmsg
                ResourceManager.UpdateTip($"Remove a topology edge on topology graph panel.");
                return;
            }
            throw new NotImplementedException();
        }
    }
}
