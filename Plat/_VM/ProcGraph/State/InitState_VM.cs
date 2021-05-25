using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class InitState_VM : DragDrop_VM
    {
        private readonly State state;

        public InitState_VM()
        {
            this.state = new State("InitState");
        }

        public State State => state;
    }
}
