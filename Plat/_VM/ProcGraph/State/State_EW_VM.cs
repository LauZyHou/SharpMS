using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// Sate编辑窗体
    /// </summary>
    public class State_EW_VM : ViewModelBase
    {
        private readonly State state;

        public State_EW_VM(State state)
        {
            this.state = state;
        }

        public State State => state;
    }
}
