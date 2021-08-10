using Plat._M;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class Env_VM : DragDrop_VM
    {
        private readonly Env env;

        public Env_VM()
            :base(0, 0, null)
        {
            this.env = new Env("DesignEnv");
        }

        public Env_VM(double x, double y, DragDrop_P_VM panelVM, Env env)
            :base(x, y, panelVM)
        {
            this.env = env;
        }

        public Env Env => env;
    }
}
