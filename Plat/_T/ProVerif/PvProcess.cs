using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif process model in memory.
    /// todo: add more attribute
    /// </summary>
    public class PvProcess
    {
        private readonly string name;
        private readonly List<PvParam> parameters;
        private readonly PvDeclaration localDeclaration;

        public PvProcess(string name, List<PvParam> parameters, PvDeclaration localDeclaration)
        {
            this.name = name;
            this.parameters = parameters;
            this.localDeclaration = localDeclaration;
        }

        public string Name => name;
        public PvDeclaration LocalDeclaration => localDeclaration;
        public List<PvParam> Parameters => parameters;

        public override string ToString()
        {
            return $"let {name}({string.Join(", ", parameters)}) = \n{localDeclaration}";
        }
    }
}
