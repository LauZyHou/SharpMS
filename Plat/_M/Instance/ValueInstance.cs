using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 值类型实例
    /// </summary>
    public class ValueInstance : Instance
    {
        private string val;

        public ValueInstance(Type type, string identifier, bool isArray)
            :base(type, identifier, isArray)
        {
            // 基本类型实例一定非数组，需要由使用方保障
            Debug.Assert(isArray == false);
            this.val = "";
        }

        /// <summary>
        /// 用字符串模拟基本类型的取值
        /// </summary>
        public string Value { get => val; set => this.RaiseAndSetIfChanged(ref val, value); }
    }
}
