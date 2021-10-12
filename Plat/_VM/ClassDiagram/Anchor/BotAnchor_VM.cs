using Avalonia.Media;
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
    /// 类图底端Anchor
    /// </summary>
    public class BotAnchor_VM : Anchor_VM
    {
        /// <summary>
        /// 由于一个Bot可以被多个Top连接，所以这里用一个Set来维护所有的Linker
        /// 每连一个就加一个进来
        /// 当这里全删光的时候自己才算删光了
        /// </summary>
        private readonly HashSet<Linker_VM> linker_VMs;

        public BotAnchor_VM(double x, double y, DragDrop_VM hostVM)
            :base(x, y, hostVM)
        {
            this.linker_VMs = new HashSet<Linker_VM>();
        }
        
        // 有了Wrapper不再需要对外直接访问容器的接口
        // public HashSet<Linker_VM> Linker_VMs => linker_VMs;

        #region Wrapper

        /// <summary>
        /// 包一下往表里加linker的动作
        /// </summary>
        /// <param name="linker_VM"></param>
        public void AddLinker(Linker_VM linker_VM)
        {
            this.linker_VMs.Add(linker_VM);
            this.RaisePropertyChanged(nameof(Color)); // 为了加这个刷新颜色
        }

        /// <summary>
        /// 包一下从表里删linker的动作
        /// </summary>
        /// <param name="linker_VM"></param>
        public void RemoveLinker(Linker_VM linker_VM)
        {
            this.linker_VMs.Remove(linker_VM);
            this.RaisePropertyChanged(nameof(Color)); // 为了加这个刷新颜色
        }

        #endregion

        /// <summary>
        /// 锚点颜色，反映 被占用/活动/空闲
        /// </summary>
        public new ISolidColorBrush Color
        {
            get
            {
                // 被占用
                if (this.linker_VMs.Count > 0)
                    return Brushes.Red;
                // 活动（在目前的Top-Bot锚点用法里永远不会让Bot锚点变成活动的
                else if (this.IsActive)
                    return Brushes.LightGreen;
                // 空闲
                return Brushes.White;
            }
        }
    }
}
