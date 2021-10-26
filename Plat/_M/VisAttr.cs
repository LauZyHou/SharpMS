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
    public class VisAttr : Attribute
    {
        private bool pub;

        /// <summary>
        /// 无参构造
        /// </summary>
        public VisAttr()
            : base()
        {
            this.pub = false;
        }

        /// <summary>
        /// 带标识和类型的构造
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="type"></param>
        /// <param name="isArray"></param>
        /// <param name="pub"></param>
        /// <param name="description"></param>
        public VisAttr(string identifier, Type type, bool isArray = false, bool pub = false, string description = "")
            :base(identifier, type, isArray, description)
        {
            this.pub = pub;
        }

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
