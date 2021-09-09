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
        private string identifier;
        private bool isOut;
        private string description;

        public Port(string identifier, bool isOut = true)
        {
            this.identifier = identifier;
            this.isOut = isOut;
            this.description = "";
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
