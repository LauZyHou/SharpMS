using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;


namespace Plat._VM
{
    /// <summary>
    /// 可以拖拽移动、放缩、连线的通用VM
    /// 在这里将维护位置信息（用于拖拽移动）、宽高信息（用于放缩）、锚点表（用于连线）
    /// </summary>
    public class DragDrop_VM : ViewModelBase
    {
        private double x;
        private double y;

        /// <summary>
        /// 横坐标位置
        /// </summary>
        public double X { get => x; set => this.RaiseAndSetIfChanged(ref x, value); }
        /// <summary>
        /// 纵坐标位置
        /// </summary>
        public double Y { get => y; set => this.RaiseAndSetIfChanged(ref y, value); }
    }
}
