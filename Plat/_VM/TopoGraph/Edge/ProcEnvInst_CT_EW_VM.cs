using Plat._C;
using Plat._M;
using ReactiveUI;

namespace Plat._VM
{
    public class ProcEnvInst_CT_EW_VM : ViewModelBase
    {
        private readonly ProcEnvInst procEnvInst;
        private PortChanInst? currentPortChanInst;

        public ProcEnvInst_CT_EW_VM(ProcEnvInst procEnvInst)
        {
            this.procEnvInst = procEnvInst;
        }

        /// <summary>
        /// ProcInst的协作实例Model
        /// </summary>
        public ProcEnvInst ProcEnvInst => procEnvInst;
        /// <summary>
        /// 当前选中的PortChanInst
        /// </summary>
        public PortChanInst? CurrentPortChanInst { get => currentPortChanInst; set => this.RaiseAndSetIfChanged(ref currentPortChanInst, value); }

        #region Command

        /// <summary>
        /// 创建新的PortChanInst
        /// </summary>
        private void OnCreateNewPortChanInst()
        {
            PortChanInst portChanInst = new PortChanInst();
            this.procEnvInst.PortChanInsts.Add(portChanInst);
            ResourceManager.UpdateTip("Create new port-channel instance.");
        }

        /// <summary>
        /// 删除选中的PortChanInst
        /// </summary>
        private void OnDeleteSelectedPortChanInst()
        {
            if (this.currentPortChanInst is null)
            {
                ResourceManager.UpdateTip("A port-channel instance must be selected!");
                return;
            }
            this.procEnvInst.PortChanInsts.Remove(this.currentPortChanInst);
            ResourceManager.UpdateTip("Delete a port-channel instance.");
        }

        /// <summary>
        /// 上移选中的PortChanInst 
        /// </summary>
        /// <param name="pcPos"></param>
        private void OnMoveUpPortChanInst(int? pcPos)
        {
            if (pcPos is null)
            {
                ResourceManager.UpdateTip("A port-channel instance must be selected!");
                return;
            }
            if (pcPos < 0 || pcPos >= this.procEnvInst.PortChanInsts.Count)
            {
                ResourceManager.UpdateTip("Port-channel instance position is exceed!");
                return;
            }
            if (pcPos == 0)
            {
                ResourceManager.UpdateTip("The selected port-channel instance is the top one! No need to move up!");
                return;
            }
            int pos = (int)pcPos;
            PortChanInst portChanInst = this.procEnvInst.PortChanInsts[pos];
            this.procEnvInst.PortChanInsts.Remove(portChanInst);
            this.procEnvInst.PortChanInsts.Insert(pos - 1, portChanInst);
            ResourceManager.UpdateTip($"Move up a port-channel instance [{portChanInst.Port?.InOutString} {portChanInst.Port?.Identifier}-{portChanInst.Chan?.Identifier}].");
        }

        /// <summary>
        /// 下移选中的PortChanInst
        /// </summary>
        /// <param name="pcPos"></param>
        private void OnMoveDownPortChanInst(int? pcPos)
        {
            if (pcPos is null)
            {
                ResourceManager.UpdateTip("A port-channel instance must be selected!");
                return;
            }
            if (pcPos < 0 || pcPos >= this.procEnvInst.PortChanInsts.Count)
            {
                ResourceManager.UpdateTip("Port-channel instance position is exceed!");
                return;
            }
            if (pcPos == this.procEnvInst.PortChanInsts.Count - 1)
            {
                ResourceManager.UpdateTip("The selected port-channel instance is the bottom one! No need to move down!");
                return;
            }
            int pos = (int)pcPos;
            PortChanInst portChanInst = this.procEnvInst.PortChanInsts[pos];
            this.procEnvInst.PortChanInsts.Remove(portChanInst);
            this.procEnvInst.PortChanInsts.Insert(pos + 1, portChanInst);
            ResourceManager.UpdateTip($"Move down a port-channel instance [{portChanInst.Port?.InOutString} {portChanInst.Port?.Identifier}-{portChanInst.Chan?.Identifier}].");
        }

        #endregion
    }
}
