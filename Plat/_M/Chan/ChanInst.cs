using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 信道实例
    /// </summary>
    public class ChanInst : ReactiveObject
    {
        private Channel? channel;
        private Type? type;
        private bool isEncrypted;
        private bool isAsymmetric;

        public ChanInst()
        {
            this.isEncrypted = this.isAsymmetric = false;
        }

        public ChanInst(Channel channel, Type type)
        {
            this.channel = channel;
            this.type = type;
            this.isEncrypted = this.isAsymmetric = false;
        }

        public Channel? Channel
        {
            get => channel;
            set
            {
                this.RaiseAndSetIfChanged(ref channel, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public Type? Type
        {
            get => type;
            set
            {
                this.RaiseAndSetIfChanged(ref type, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public bool IsEncrypted
        {
            get => isEncrypted;
            set
            {
                this.RaiseAndSetIfChanged(ref isEncrypted, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public bool IsAsymmetric
        {
            get => isAsymmetric;
            set
            {
                this.RaiseAndSetIfChanged(ref isAsymmetric, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }

        #region ShowXXX

        public string ShowStr
        {
            get
            {
                string suffix = "";
                if (this.isEncrypted)
                {
                    suffix = "^";
                    if (this.isAsymmetric)
                    {
                        suffix = "?";
                    }
                }
                return $"{channel?.Identifier} : {type?.Identifier}{suffix}";
            }
        }

        #endregion
    }
}
