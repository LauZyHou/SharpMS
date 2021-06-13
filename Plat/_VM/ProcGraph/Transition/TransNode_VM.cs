using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Transition结点的ViewModel
    /// </summary>
    public class TransNode_VM : DragDrop_VM
    {
        private Linker_VM attachedLinker;

        /// <summary>
        /// 用户创建TransNode，或导入的TransNode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="panelVM"></param>
        /// <param name="linker_VM">所吸附的Linker</param>
        public TransNode_VM(double x, double y, DragDrop_P_VM panelVM, Linker_VM linker_VM)
            :base(x, y, panelVM)
        {
            // 互相引用
            this.attachedLinker = linker_VM;
            linker_VM.ExtMsg = this;
        }

        /// <summary>
        /// 所吸附的Linker
        /// </summary>
        public Linker_VM AttachedLinker { get => attachedLinker; set => this.RaiseAndSetIfChanged(ref attachedLinker, value); }
    }
}
