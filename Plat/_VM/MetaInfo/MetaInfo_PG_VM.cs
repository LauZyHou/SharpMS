using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class MetaInfo_PG_VM
    {
        private readonly TypePanel_VM typePanel_VM = new TypePanel_VM();
        private readonly ProcPanel_VM procPanel_VM = new ProcPanel_VM();
        private readonly ChanPanel_VM chanPanel_VM = new ChanPanel_VM();

        public string Greet => "meta info test";

        public TypePanel_VM TypePanel_VM => typePanel_VM;
        public ProcPanel_VM ProcPanel_VM => procPanel_VM;
        public ChanPanel_VM ChanPanel_VM => chanPanel_VM;
    }
}
