using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// ProVerif 函数
    /// </summary>
    public class PvFun
    {
        private readonly string name;
        private readonly List<PvType> paramTypes;
        private readonly PvType returnType;

        public PvFun(string name, List<PvType> paramTypes, PvType returnType)
        {
            this.name = name;
            this.paramTypes = paramTypes;
            this.returnType = returnType;
        }

        public PvFun(string name, PvType returnType)
        {
            this.name = name;
            this.paramTypes = new List<PvType>();
            this.returnType = returnType;
        }

        public string Name => name;
        public List<PvType> ParamTypes => paramTypes;
        public PvType ReturnType => returnType;

        public override string ToString()
        {
            return this.name;
        }

        /// <summary>
        /// 非对称密钥生成（给出私钥返回公钥）
        /// </summary>
        public static PvFun PK = new PvFun(
            "PK",
            new List<PvType>()
            {
                PvType.PVTKEY
            },
            PvType.PUBKEY
        );

        /// <summary>
        /// 非对称加密
        /// </summary>
        public static PvFun ASYMENC = new PvFun(
            "AsymEnc",
            new List<PvType>()
            {
                PvType.BITSTRING,
                PvType.PUBKEY
            },
            PvType.BITSTRING
        );

        /// <summary>
        /// 非对称解密
        /// </summary>
        public static PvFun ASYMDEC = new PvFun(
            "AsymDec",
            new List<PvType>()
            {
                PvType.BITSTRING,
                PvType.PVTKEY
            },
            PvType.BITSTRING
        );
    }
}
