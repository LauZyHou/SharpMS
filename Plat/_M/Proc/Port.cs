using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    public class Port : ReactiveObject
    {
        private static int _id = 0;
        private string identifier;
        private bool isOut;
        private string description;

        /// <summary>
        /// 无参构造
        /// </summary>
        public Port()
        {
            this.identifier = $"p{++_id}";
            this.isOut = true;
            this.description = "";
        }

        /// <summary>
        /// 带标识的构造
        /// </summary>
        /// <param name="identifier">端口标识</param>
        /// <param name="isOut">是否是出端口</param>
        public Port(string identifier, bool isOut = true, string description = "")
        {
            this.identifier = identifier;
            this.isOut = isOut;
            this.description = description;
        }

        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        public bool IsOut
        {
            get => isOut;
            set
            {
                this.RaiseAndSetIfChanged(ref isOut, value);
                this.RaisePropertyChanged(nameof(InOutString));
            }
        }
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        #region ToString

        public string InOutString
        {
            get
            {
                return this.isOut ? "OUT" : "IN";
            }
        }

        #endregion
    }
}
