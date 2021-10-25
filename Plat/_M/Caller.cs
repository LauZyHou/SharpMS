using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 函数/方法声明
    /// </summary>
    public class Caller : ReactiveObject
    {
        private static int _id = 0;
        private readonly ObservableCollection<Type> paramTypes;
        private string identifier;
        private Type returnType;
        private string description;

        /// <summary>
        /// 无参构造
        /// </summary>
        public Caller()
        {
            this.identifier = $"Fun{++_id}";
            this.returnType = Type.TYPE_BOOL;
            this.description = "";
            this.paramTypes = new ObservableCollection<Type>();
        }

        /// <summary>
        /// 指定调用名和返回类型的构造
        /// </summary>
        /// <param name="identifier">函数或方法名</param>
        /// <param name="returnType">返回值类型</param>
        /// <param name="description">注解描述</param>
        public Caller(string identifier, Type returnType, string description = "")
        {
            this.identifier = identifier;
            this.returnType = returnType;
            this.description = description;
            this.paramTypes = new ObservableCollection<Type>();
        }

        /// <summary>
        /// 函数/方法名
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 参数数据类型表
        /// </summary>
        public ObservableCollection<Type> ParamTypes => paramTypes;
        /// <summary>
        /// 返回值类型
        /// </summary>
        public Type ReturnType { get => returnType; set => this.RaiseAndSetIfChanged(ref returnType, value); }
        /// <summary>
        /// 自然语言描述
        /// </summary>
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        #region 变更prop

        public string ParamTypeString
        {
            get
            {
                return string.Join(", ", paramTypes);
            }
        }

        #endregion
    }
}
