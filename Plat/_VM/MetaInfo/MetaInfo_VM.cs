using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class MetaInfo_VM
    {
        private readonly TypePanel_VM typePanel_VM = new TypePanel_VM();

        public string Greet => "meta info test";

        public TypePanel_VM TypePanel_VM => typePanel_VM;
    }
}
