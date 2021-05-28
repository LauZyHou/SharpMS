using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 终止状态结点
    /// </summary>
    public class FinalState_VM : DragDrop_VM
    {
        public FinalState_VM(double x, double y)
            :base(x, y)
        {
            this.Anchor_VMs.Add(new Anchor_VM(X + 20, Y + 4, this));
        }

        #region Command Callback

        private void OnDelete()
        {
        }

        #endregion
    }
}
