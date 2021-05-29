using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using ReactiveUI;


namespace Plat._VM
{
    /// <summary>
    /// 可以拖拽移动、放缩、连线的通用VM
    /// 在这里将维护位置信息（用于拖拽移动）、宽高信息（用于放缩）、锚点表（用于连线）
    /// </summary>
    public class DragDrop_VM : ViewModelBase
    {
        private Point pos;
        private Point oldPos;
        private readonly ObservableCollection<Anchor_VM> anchor_VMs = new ObservableCollection<Anchor_VM>();

        public DragDrop_VM(double x, double y)
        {
            this.Pos = new Point(x, y);
        }

        /// <summary>
        /// 横纵坐标位置，其中包含X和Y分别表示横向坐标和纵向坐标位置
        /// </summary>
        public Point Pos { get => pos; set => this.RaiseAndSetIfChanged(ref pos, value); }
        /// <summary>
        /// 暂存拖拽前所在的旧位置，仅用于拖拽的新位置的计算
        /// </summary>
        public Point OldPos { get => oldPos; set => oldPos = value; }
        /// <summary>
        /// 该item身上的锚点表（注意锚点本身也是一种DragDrop_VM，但其锚点表为默认的空表）
        /// </summary>
        public ObservableCollection<Anchor_VM> Anchor_VMs => anchor_VMs;
    }
}
