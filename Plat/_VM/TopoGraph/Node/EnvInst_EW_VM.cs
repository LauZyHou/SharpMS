using Plat._C;
using Plat._M;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 拓扑图进程实例结点的编辑窗体VM
    /// </summary>
    public class EnvInst_EW_VM : ViewModelBase
    {
        private readonly EnvInst envInst;
        private readonly ObservableCollection<Env> envList;

        public EnvInst_EW_VM(EnvInst envInst)
        {
            this.envInst = envInst;
            this.envList = ResourceManager.envs;
        }

        public EnvInst EnvInst => envInst;
        public ObservableCollection<Env> EnvList => envList;

        #region Command

        /// <summary>
        /// 清除当前EnvInst例化的模板Env
        /// </summary>
        private void OnClearEnv()
        {
            this.envInst.Env = null;
            ResourceManager.UpdateTip($"Clear the template of environment instance.");
        }

        #endregion
    }
}
