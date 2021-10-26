using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// 带有取值的Attribute
    /// </summary>
    public class ValAttr : Attribute
    {
        private string val;

        /// <summary>
        /// 无参构造
        /// </summary>
        public ValAttr()
            :base()
        {
            this.val = "";
        }

        /// <summary>
        /// 带标识和类型的构造
        /// </summary>
        /// <param name="identifier">ValAttr标识</param>
        /// <param name="type">数据类型</param>
        /// <param name="isArray">是否是数组</param>
        /// <param name="description">注解描述</param>
        public ValAttr(string identifier, Type type, bool isArray = false, string val = "", string description = "")
            :base(identifier, type, isArray, description)
        {
            this.val = val;
        }

        public string Value { get => val; set => this.RaiseAndSetIfChanged(ref val, value); }
    }
}
