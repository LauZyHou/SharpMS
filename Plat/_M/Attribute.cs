using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 属性（Attr），或形式参数（Param），在SharpMS中共用这个类型
    /// </summary>
    public class Attribute : ReactiveObject
    {
        public static int _id = 0;

        private int id;
        private string identifier;
        private Type type;
        private bool isArray;
        private string description;
        private bool isEncrypted;
        private bool isAsymmetric;

        /// <summary>
        /// 无参构造
        /// </summary>
        public Attribute()
        {
            this.identifier = $"v{this.id = ++_id}";
            this.type = Type.TYPE_INT;
            this.isArray = false;
            this.description = "";
            this.isEncrypted = this.isAsymmetric = false;
        }

        /// <summary>
        /// 指定Attr标识和类型的构造
        /// </summary>
        /// <param name="identifier">Attr标识</param>
        /// <param name="type">所属类型</param>
        /// <param name="isArray">是否是数组</param>
        /// <param name="description">注解描述</param>
        public Attribute(string identifier, Type type, bool isArray = false, string description = "")
        {
            this.id = ++_id;
            this.identifier = identifier;
            this.type = type;
            this.isArray = isArray;
            this.description = description;
            this.isEncrypted = this.isAsymmetric = false;
        }

        public int Id { get => id; set => id = value; }
        public Type Type { get => type; set => this.RaiseAndSetIfChanged(ref type, value); }
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 是加密的
        /// </summary>
        public bool IsEncrypted
        {
            get => isEncrypted;
            set
            {
                this.RaiseAndSetIfChanged(ref isEncrypted, value);
                this.RaisePropertyChanged(nameof(EncryptStr));
            }
        }
        /// <summary>
        /// 是非对称的（仅当 IsEncrypted 为 true 时有效
        /// </summary>
        public bool IsAsymmetric
        {
            get => isAsymmetric;
            set
            {
                this.RaiseAndSetIfChanged(ref isAsymmetric, value);
                this.RaisePropertyChanged(nameof(EncryptStr));
            }
        }
        public bool IsArray
        {
            get => isArray;
            set
            {
                this.RaiseAndSetIfChanged(ref isArray, value);
                this.RaisePropertyChanged(nameof(IsArray));
            }
        }
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        #region XXX Str

        public string ArrayStr
        {
            get
            {
                return this.isArray ? "[]" : "";
            }
        }

        public string EncryptStr
        {
            get
            {
                if (this.isEncrypted)
                {
                    if (this.isAsymmetric)
                    {
                        return "?"; // 表示 非对称加密后的类型字段
                    }
                    return "^"; // 表示 对称加密后的类型字段
                }
                return ""; // 表示 不加密（原生）的类型字段
            }
        }

        #endregion
    }
}
