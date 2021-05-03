using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL location model, which can be trans to "<location/>" or "<init/>"
    /// </summary>
    public class UpLocation
    {
        private static int _id = 0;
        private readonly int id;
        private string name;
        private bool isInit;

        public UpLocation(string name, bool isInit = false)
        {
            this.id = _id++;
            this.name = name;
            this.isInit = isInit;
        }

        public string Name { get => name; set => name = value; }
        public bool IsInit { get => isInit; set => isInit = value; }
        public int Id => id;

        public override string ToString()
        {
            return $"<location id=\"id{id}\" x=\"0\" y=\"0\">\n<name x=\"0\" y=\"0\">{name}</name>\n</location>\n";
        }
    }
}
