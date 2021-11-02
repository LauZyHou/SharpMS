using Avalonia;
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

        // 有了Wrapper对增减而言不再需要对外直接访问容器的接口
        // 但是在锚点移动时需要更新这些Linker的ExtMsg
        // 干脆由这里来刷新所有Linker的Pos之类的
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

        /// <summary>
        /// 刷新所有linker的ExtMsg的OldPos为其Pos
        /// </summary>
        public void FlushLinkersExtMsgOldPos()
        {
            foreach (Linker_VM linker_VM in this.linker_VMs)
            {
                if (linker_VM.ExtMsg is null)
                    continue;
                DragDrop_VM attachedVM = (DragDrop_VM)linker_VM.ExtMsg;
                attachedVM.OldPos = attachedVM.Pos;
            }
        }

        /// <summary>
        /// 根据一个加偏置来更新所有linker吸附的ExtMsg的Pos
        /// </summary>
        /// <param name="bais"></param>
        public void MoveLinkersExtMsgPosByOldPosPlusBais(Point bais)
        {
            foreach (Linker_VM linker_VM in this.linker_VMs)
            {
                if (linker_VM.ExtMsg is null)
                    continue;
                DragDrop_VM attachedVM = (DragDrop_VM)linker_VM.ExtMsg;
                attachedVM.Pos = attachedVM.OldPos + bais;
            }
        }

        /// <summary>
        /// 获取所有linker_VM的id
        /// </summary>
        /// <returns></returns>
        public List<int> FetchLinkersIds()
        {
            List<int> res = new List<int>();
            foreach (Linker_VM linker_VM in linker_VMs)
            {
                res.Add(linker_VM.Id);
            }
            return res;
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
