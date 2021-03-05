using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 拖拽面板父类VM
    /// </summary>
    public class DragDrop_P_VM : ViewModelBase
    {
        private readonly ObservableCollection<ViewModelBase> userControlVMs = new ObservableCollection<ViewModelBase>();

        /// <summary>
        /// 面板中的拖拽对象
        /// </summary>
        public ObservableCollection<ViewModelBase> UserControlVMs => userControlVMs;
    }
}
