using Plat._M;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class Axiom_VM : DragDrop_VM
    {
        private Axiom axiom;

        public Axiom_VM()
            :base(0, 0, null)
        {
            this.axiom = new Axiom("NewAx");
        }

        public Axiom_VM(double x, double y, DragDrop_P_VM panelVM, Axiom axiom)
            :base(x, y, panelVM)
        {
            this.axiom = axiom;
        }

        public Axiom Axiom { get => axiom; set => this.RaiseAndSetIfChanged(ref axiom, value); }
    }
}
