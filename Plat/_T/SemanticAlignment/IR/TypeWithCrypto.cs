using Plat._M;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// 带有加密相关特性的类型，用于描述进程图状态机中的类型
    /// </summary>
    public class TypeWithCrypto
    {
        private Type type;
        private Crypto crypto;

        public TypeWithCrypto(Type type, Crypto crypto)
        {
            this.type = type;
            this.crypto = crypto;
        }

        /// <summary>
        /// 统一语义模型中的数据类型
        /// </summary>
        public Type Type { get => type; set => type = value; }
        /// <summary>
        /// 密码学附加特性
        /// </summary>
        public Crypto Crypto { get => crypto; set => crypto = value; }
    }
}
