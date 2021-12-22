using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif type.
    /// </summary>
    public class PvType
    {
        private readonly string name;

        public PvType(string name)
        {
            this.name = name;
        }

        public string Name => name;

        public override string ToString()
        {
            return name;
        }

        public static PvType BITSTRING = new PvType("bitstring");
        public static PvType CHANNEL = new PvType("channel"); // just for port
        public static PvType INT = new PvType("Int");
        public static PvType BOOL = new PvType("Bool");
        public static PvType MSG = new PvType("Msg"); // unused type (use bitstring
        public static PvType KEY = new PvType("Key");
        public static PvType PUBKEY = new PvType("PubKey");
        public static PvType PVTKEY = new PvType("PvtKey");
    }
}
