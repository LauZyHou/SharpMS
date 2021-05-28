using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 连线的锚点ViewModel
    /// </summary>
    public class Anchor_VM : DragDrop_VM
    {
        private readonly DragDrop_VM hostVM;
        private Point oldPos;

        /// <summary>
        /// 构造锚点时需要手动指定其X/Y位置
        /// </summary>
        /// <param name="x">锚点在面板中的x位置</param>
        /// <param name="y">锚点在面板中的y位置</param>
        public Anchor_VM(double x, double y, DragDrop_VM hostVM)
            : base(x, y)
        {
            this.hostVM = hostVM;
        }

        /// <summary>
        /// 锚点的宿主ViewModel，即该锚点吸附在的item组件VM
        /// </summary>
        public DragDrop_VM HostVM => hostVM;
        /// <summary>
        /// 锚点受item拖拽而被动移动的过程中，拖拽前所在的旧位置
        /// </summary>
        public Point OldPos { get => oldPos; set => oldPos = value; }
        /// <summary>
        /// 锚点颜色，反映 被占用/活动/空闲
        /// </summary>
        public ISolidColorBrush Color
        {
            get
            {
                return Brushes.White; // 空闲:白
            }
        }
    }
}
