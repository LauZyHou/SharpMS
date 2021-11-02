using Avalonia;
using Avalonia.Media;
using ReactiveUI;
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
        public static int _id = 0;
        private int id;
        private readonly DragDrop_VM hostVM;
        private Linker_VM? linkerVM;
        private bool isActive;

        /// <summary>
        /// for Design
        /// </summary>
        public Anchor_VM()
            : base(0, 0, new ClassDiagram_P_VM())
        {
            this.id = ++_id;
            this.hostVM = new Type_VM();
        }

        /// <summary>
        /// 构造锚点时需要手动指定其X/Y位置
        /// </summary>
        /// <param name="x">锚点在面板中的x位置</param>
        /// <param name="y">锚点在面板中的y位置</param>
        public Anchor_VM(double x, double y, DragDrop_VM hostVM)
            : base(x, y, hostVM.PanelVM)
        {
            this.id = ++_id;
            this.hostVM = hostVM;
        }

        public int Id { get => id; set => id = value; }
        /// <summary>
        /// 锚点的宿主ViewModel，即该锚点吸附在的item组件VM
        /// </summary>
        public DragDrop_VM HostVM => hostVM;
        /// <summary>
        /// 锚点上的连线，没有时为null
        /// </summary>
        public Linker_VM? LinkerVM
        {
            get => linkerVM; set
            {
                this.RaiseAndSetIfChanged(ref linkerVM, value);
                this.RaisePropertyChanged(nameof(Color));
            }
        }
        /// <summary>
        /// 是否是活动锚点
        /// </summary>
        public bool IsActive
        {
            get => isActive; set
            {
                this.RaiseAndSetIfChanged(ref isActive, value);
                this.RaisePropertyChanged(nameof(Color));
            }
        }
        /// <summary>
        /// 锚点颜色，反映 被占用/活动/空闲
        /// </summary>
        public ISolidColorBrush Color
        {
            get
            {
                // 被占用
                if (linkerVM is not null)
                    return Brushes.Red;
                // 活动
                else if (isActive)
                    return Brushes.LightGreen;
                // 空闲
                return Brushes.White;
            }
        }
    }
}
