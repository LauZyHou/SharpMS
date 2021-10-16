using ReactiveUI;

namespace Plat._VM
{
    /// <summary>
    /// Coordinate Tag View Model
    /// 用于Linker上吸附的可移动信息
    /// </summary>
    public abstract class CT_VM : DragDrop_VM
    {
        private Linker_VM attachedLinker;

        protected CT_VM(double x, double y, DragDrop_P_VM panelVM, Linker_VM linker_VM)
            :base(x, y, panelVM)
        {
            // 互相引用
            this.attachedLinker = linker_VM;
            linker_VM.ExtMsg = this;
        }

        /// <summary>
        /// 所吸附的linker
        /// </summary>
        public Linker_VM AttachedLinker { get => attachedLinker; set => this.RaiseAndSetIfChanged(ref attachedLinker, value); }
    }
}
