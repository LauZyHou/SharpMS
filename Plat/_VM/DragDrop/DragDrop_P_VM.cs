using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 拖拽面板（放置DragDrop_VM）父类VM
    /// </summary>
    public abstract class DragDrop_P_VM : ViewModelBase
    {
        private readonly ObservableCollection<DragDrop_VM> dragDrop_VMs = new ObservableCollection<DragDrop_VM>();
        private Anchor_VM? activeAnchorVM;

        /// <summary>
        /// 面板中的可拖拽对象VM表
        /// </summary>
        public ObservableCollection<DragDrop_VM> DragDrop_VMs => dragDrop_VMs;
        /// <summary>
        /// 面板上的活动锚点，没有时为null
        /// </summary>
        public Anchor_VM? ActiveAnchorVM { get => activeAnchorVM; set => this.RaiseAndSetIfChanged(ref activeAnchorVM, value); }

        /// <summary>
        /// 删除拖拽面板上的组件，具体的删除方法由实现类决定
        /// </summary>
        /// <param name="item">待删除的组件</param>
        public abstract void DeleteDragDropItem(DragDrop_VM item);
        /// <summary>
        /// 在拖拽面板上创建一个Linker，具体的创建方法由实现类决定
        /// </summary>
        /// <param name="src">创建源</param>
        /// <param name="dst">创建目标</param>
        public abstract void CreateLinker(Anchor_VM src, Anchor_VM dst);
    }
}
