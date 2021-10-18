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
        private string identifier;
        private Type type;
        private bool isArray;
        private string description;

        public Attribute(string identifier, Type type, bool isArray = false, string description = "")
        {
            this.identifier = identifier;
            this.type = type;
            this.isArray = isArray;
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

        #region XXX Str

        public string ArrayStr
        {
            get
            {
                return this.isArray ? "[]" : "";
            }
        }

        #endregion
    }
}
