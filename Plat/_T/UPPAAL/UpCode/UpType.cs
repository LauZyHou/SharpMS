using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL type.
    /// </summary>
    public class UpType
    {
        private readonly string name;
        private readonly UpType? fromType;
        private readonly List<UpParam> props;

        public UpType(string name, UpType? fromType = null)
        {
            this.name = name;
            this.fromType = fromType;
            this.props = new List<UpParam>();
        }

        /// <summary>
        /// 类型名
        /// </summary>
        public string Name => name;
        /// <summary>
        /// 定义取自哪一数据类型，即
        /// typedef FromType.Name Name;
        /// 基本类型或结构体该字段为空
        /// </summary>
        public UpType? FromType => fromType;
        /// <summary>
        /// UPPAAL结构体的属性表
        /// </summary>
        public List<UpParam> Props => props;

        public static UpType INT = new UpType("int");
        public static UpType CLOCK = new UpType("clock");
        public static UpType CHAN = new UpType("chan");
        public static UpType MSG = new UpType("Msg", INT);
        public static UpType KEY = new UpType("Key", MSG);
        public static UpType PUBKEY = new UpType("PubKey", MSG);
        public static UpType PVTKEY = new UpType("PvtKey", MSG);

        /// <summary>
        /// 从一个数据类型生成其加密后的数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isAsymmetric"></param>
        /// <returns></returns>
        public static UpType GenEncryptedType(UpType type, bool isAsymmetric)
        {
            UpType res = new UpType($"{type.name}{(isAsymmetric ? "_A" : "_S")}");
            res.props.Add(new UpParam(type, "content"));
            if (isAsymmetric)
            {
                res.props.Add(new UpParam(PUBKEY, "pk"));
            }
            else
            {
                res.props.Add(new UpParam(KEY, "k"));
            }
            return res;
        }
    }
}
