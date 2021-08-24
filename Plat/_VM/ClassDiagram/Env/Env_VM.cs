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
            this.init_anchor();
        }

        public Env_VM(double x, double y, DragDrop_P_VM panelVM, Env env)
            :base(x, y, panelVM)
        {
            this.env = env;
            this.init_anchor();
        }

        public Env Env => env;

        #region Init

        private void init_anchor()
        {
            this.Anchor_VMs.Add(new TopAnchor_VM(Pos.X + W / 2 + 2, Pos.Y, this));
            this.Anchor_VMs.Add(new BotAnchor_VM(Pos.X + W / 2 + 2, Pos.Y + H + 4, this));
        }

        #endregion

        #region 继承下来的功能

        public override void FlushAnchorPos()
        {
            Anchor_VMs[0].Pos = new Avalonia.Point(Pos.X + W / 2 + 2, Pos.Y);
            Anchor_VMs[1].Pos = new Avalonia.Point(Pos.X + W / 2 + 2, Pos.Y + H + 4);
        }

        #endregion
    }
}
