using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 带有可见性性质的Attribute
    /// </summary>
    public class VisAttr : ReactiveObject
    {
        private static int _id = 0;
        private Type type;
        private string identifier;
        private bool isArray;
        private string description;
        private bool pub;
        
        /// <summary>
        /// 无参构造
        /// </summary>
        public VisAttr()
        {
            this.identifier = $"v{++_id}";
            this.type = Type.TYPE_INT;
            this.isArray = false;
            this.pub = false;
            this.description = "";
        }

        /// <summary>
        /// 带标识符和类型的构造
        /// </summary>
        /// <param name="identifier">VisAttr标识</param>
        /// <param name="type">数据类型</param>
        /// <param name="isArray">是否是数组</param>
        /// <param name="pub">是否公开</param>
        /// <param name="description">注解描述</param>
        public VisAttr(string identifier, Type type, bool isArray = false, bool pub = false, string description = "")
        {
            this.identifier = identifier;
            this.type = type;
            this.isArray = isArray;
            this.pub = pub;
            this.description = description;
        }

        public Type Type { get => type; set => this.RaiseAndSetIfChanged(ref type, value); }
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
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
        public bool Pub
        {
            get => pub;
            set
            {
                this.RaiseAndSetIfChanged(ref pub, value);
                this.RaisePropertyChanged(nameof(PubStr));
            }
        }

        #region XXX Str

        public string ArrayStr
        {
            get
            {
                return this.isArray ? "[]" : "";
            }
        }

        public string PubStr
        {
            get
            {
                return this.pub ? "+" : "-";
            }
        }

        #endregion
    }
}
