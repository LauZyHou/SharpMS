using Plat._C;
using Plat._M;
using Plat._V;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 进程实例和环境实例协同Tag
    /// </summary>
    public class ProcEnvInst_CT_VM : CT_VM
    {
        private readonly ProcEnvInst procEnvInst;

        public ProcEnvInst_CT_VM(double x, double y, DragDrop_P_VM panelVM, Linker_VM linker_VM)
            :base(x, y, panelVM, linker_VM)
        {
            // 解析linker两端的ProcInst_VM和EnvInst_VM
            Anchor_VM src = linker_VM.Source;
            Anchor_VM dst = linker_VM.Dest;
            Debug.Assert(src.HostVM is ProcInst_VM && dst.HostVM is EnvInst_VM);
            ProcInst_VM procInst_VM = (ProcInst_VM)src.HostVM;
            EnvInst_VM envInst_VM = (EnvInst_VM)dst.HostVM;
            // 拿出里面Model来构造这个ProcEnvInst
            this.procEnvInst = new ProcEnvInst(procInst_VM.ProcInst, envInst_VM.EnvInst);
        }

        /// <summary>
        /// ProcInst的协作实例Model
        /// </summary>
        public ProcEnvInst ProcEnvInst => procEnvInst;

        #region Command

        /// <summary>
        /// 打开编辑窗体
        /// </summary>
        private void OnEdit()
        {
            ProcEnvInst_CT_EW_V procEnvInst_CT_EW_V = new ProcEnvInst_CT_EW_V()
            {
                DataContext = new ProcEnvInst_CT_EW_VM(this.procEnvInst)
            };
            procEnvInst_CT_EW_V.ShowDialog(ResourceManager.mainWindow_V);
            ResourceManager.UpdateTip($"Open the edit window for the process-environment coordinate tag.");
        }

        #endregion
    }
}
