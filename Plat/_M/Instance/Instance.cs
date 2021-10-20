using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 数据实例抽象类
    /// </summary>
    public abstract class Instance : ReactiveObject
    {
        private readonly Type type;
        private readonly string identifier;
        private readonly bool isArray;

        protected Instance(Type type, string identifier, bool isArray)
        {
            this.type = type;
            this.identifier = identifier;
            this.isArray = isArray;
        }

        /// <summary>
        /// 实例的类型
        /// </summary>
        public Type Type => type;
        /// <summary>
        /// 实例的标识符
        /// </summary>
        public string Identifier => identifier;
        /// <summary>
        /// 是否是数组
        /// </summary>
        public bool IsArray => isArray;

        public override string ToString()
        {
            return $"{this.identifier}: {(this.isArray ? "[]" : "")}{this.type.Identifier} ";
        }
    }
}
