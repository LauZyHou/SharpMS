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
    public class DragDrop_P_VM : ViewModelBase
    {
        private readonly ObservableCollection<DragDrop_VM> dragDrop_VMs = new ObservableCollection<DragDrop_VM>();

        /// <summary>
        /// 面板中的可拖拽对象VM表
        /// </summary>
        public ObservableCollection<DragDrop_VM> DragDrop_VMs => dragDrop_VMs;
    }
}
