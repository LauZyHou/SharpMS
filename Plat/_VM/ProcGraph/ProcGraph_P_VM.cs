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
            Arrow_VM linker_VM = new Arrow_VM(src, dst, this);
            src.LinkerVM = dst.LinkerVM = linker_VM;
            this.DragDrop_VMs.Add(linker_VM);
            ResourceManager.UpdateTip($"Create a linker on process graph panel, from [{src.HostVM}] to [{dst.HostVM}].");
        }

        /// <summary>
        /// 删除元素（状态、锚点、边）
        /// </summary>
        /// <param name="item"></param>
        public override void DeleteDragDropItem(DragDrop_VM item)
        {
            if (item is Linker_VM)
            {
                Linker_VM linker_VM = (Linker_VM)item;
                linker_VM.Source.LinkerVM = null;
                linker_VM.Dest.LinkerVM = null;
                this.DragDrop_VMs.Remove(linker_VM);
                ResourceManager.UpdateTip("Remove a linker on process graph panel.");
                return;
            }
            else if (item is PureState_VM)
            {
                // todod
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
