using Plat._C;
using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Process Graph面板的View Model
    /// </summary>
    public class ProcGraph_P_VM : DragDrop_P_VM
    {
        private readonly ProcGraph procGraph;

        public ProcGraph_P_VM(ProcGraph procGraph)
        {
            this.procGraph = procGraph;
        }

        public ProcGraph ProcGraph => procGraph;

        #region Command Callback

        private void OnCreatePureState()
        {
            PureState_VM pureState_VM = new PureState_VM()
            {
                X = 300,
                Y = 400
            };
            this.DragDrop_VMs.Add(pureState_VM);
            ResourceManager.UpdateTip($"Create a pure state [{pureState_VM}] on process graph [{procGraph}].");
        }

        #endregion
    }
}
