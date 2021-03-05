using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL param, used in function definition.
    /// i.e. int a
    /// i.e. bool& b
    /// </summary>
    public class UpParam
    {
        /// <summary>
        /// param type
        /// </summary>
        private readonly UpType type;
        /// <summary>
        /// param name
        /// </summary>
        private readonly string name;
        /// <summary>
        /// identify if this param passed by reference, "false" default
        /// </summary>
        private readonly bool passByRef;

        public UpParam(UpType type, string name, bool passByRef = false)
        {
            this.type = type;
            this.name = name;
            this.passByRef = passByRef;
        }

        public UpType Type => type;
        public string Name => name;
        public bool PassByRef => passByRef;

        public override string ToString()
        {
            return $"{type.Name}{(passByRef ? "&amp;" : "")} {name}";
        }
    }
}
