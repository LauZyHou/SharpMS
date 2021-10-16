using Plat._C;
using Plat._M;
using Plat._V;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Transition结点的ViewModel
    /// </summary>
    public class TransNode_VM : CT_VM
    {
        private readonly LocTrans locTrans;

        /// <summary>
        /// 用户创建TransNode，或导入的TransNode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="panelVM"></param>
        /// <param name="linker_VM">所吸附的Linker</param>
        public TransNode_VM(double x, double y, DragDrop_P_VM panelVM, Linker_VM linker_VM)
            : base(x, y, panelVM, linker_VM)
        {
            // 初始化迁移信息
            this.locTrans = new LocTrans();
        }

        /// <summary>
        /// Location迁移信息
        /// </summary>
        public LocTrans LocTrans => locTrans;

        #region Command

        /// <summary>
        /// 打开编辑窗体
        /// </summary>
        private void OnEdit()
        {
            TransNode_EW_V transNode_EW_V = new TransNode_EW_V()
            {
                DataContext = new TransNode_EW_VM(locTrans)
            };
            transNode_EW_V.ShowDialog(ResourceManager.mainWindow_V);
            ResourceManager.UpdateTip($"Open the edit window for the location transition.");
        }

        #endregion
    }
}
