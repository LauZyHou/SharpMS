using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 一个Proc/Env的Attr以及另一个Proc/Env的Attr
    /// </summary>
    public class AttrPair : ReactiveObject
    {
        public static int _id = 0;

        private int id;
        private Proc? procA;
        private VisAttr? procAttrA;
        private Proc? procB;
        private VisAttr? procAttrB;
        private Env? envA;
        private VisAttr? envAttrA;
        private Env? envB;
        private VisAttr? envAttrB;

        /// <summary>
        /// 构造Proc-Proc型
        /// </summary>
        /// <param name="procA"></param>
        /// <param name="procAttrA"></param>
        /// <param name="procB"></param>
        /// <param name="procAttrB"></param>
        public AttrPair(Proc procA, VisAttr procAttrA, Proc procB, VisAttr procAttrB)
        {
            this.id = ++_id;
            this.procA = procA;
            this.procAttrA = procAttrA;
            this.procB = procB;
            this.procAttrB = procAttrB;
        }

        /// <summary>
        /// 构造Env-Env型
        /// </summary>
        /// <param name="envA"></param>
        /// <param name="envAttrA"></param>
        /// <param name="envB"></param>
        /// <param name="envAttrB"></param>
        public AttrPair(Env envA, VisAttr envAttrA, Env envB, VisAttr envAttrB)
        {
            this.id = ++_id;
            this.envA = envA;
            this.envAttrA = envAttrA;
            this.envB = envB;
            this.envAttrB = envAttrB;
        }

        /// <summary>
        /// 构造Proc-Env型
        /// </summary>
        /// <param name="procA"></param>
        /// <param name="procAttrA"></param>
        /// <param name="envB"></param>
        /// <param name="envAttrB"></param>
        public AttrPair(Proc procA, VisAttr procAttrA, Env envB, VisAttr envAttrB)
        {
            this.id = ++_id;
            this.procA = procA;
            this.procAttrA = procAttrA;
            this.envB = envB;
            this.envAttrB = envAttrB;
        }

        public int Id { get => id; set => id = value; }
        public Proc? ProcA
        {
            get => procA;
            set
            {
                this.RaiseAndSetIfChanged(ref procA, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public VisAttr? ProcAttrA
        {
            get => procAttrA;
            set
            {
                this.RaiseAndSetIfChanged(ref procAttrA, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public Proc? ProcB
        {
            get => procB;
            set
            {
                this.RaiseAndSetIfChanged(ref procB, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public VisAttr? ProcAttrB
        {
            get => procAttrB;
            set
            {
                this.RaiseAndSetIfChanged(ref procAttrB, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public Env? EnvA
        {
            get => envA;
            set
            {
                this.RaiseAndSetIfChanged(ref envA, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public VisAttr? EnvAttrA
        {
            get => envAttrA;
            set
            {
                this.RaiseAndSetIfChanged(ref envAttrA, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public Env? EnvB
        {
            get => envB;
            set
            {
                this.RaiseAndSetIfChanged(ref envB, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }
        public VisAttr? EnvAttrB
        {
            get => envAttrB;
            set
            {
                this.RaiseAndSetIfChanged(ref envAttrB, value);
                this.RaisePropertyChanged(nameof(ShowStr));
            }
        }

        #region 展示属性

        public string ShowStr
        {
            get
            {
                if (this.procA is not null && this.procAttrA is not null && this.procB is not null && this.procAttrB is not null)
                {
                    return $"[PP] {this.procA.Identifier}.{this.procAttrA.Identifier}={this.procB.Identifier}.{this.procAttrB.Identifier}";
                }
                if (this.envA is not null && this.envAttrA is not null && this.envB is not null && this.envAttrB is not null)
                {
                    return $"[EE] {this.envA.Identifier}.{this.envAttrA.Identifier}={this.envB.Identifier}.{this.envAttrB.Identifier}";
                }
                if (this.procA is not null && this.procAttrA is not null && this.envB is not null && this.envAttrB is not null)
                {
                    return $"[PE] {this.procA.Identifier}.{this.procAttrA.Identifier}={this.envB.Identifier}.{this.envAttrB.Identifier}";
                }
                return "[ERROR] not a regular AttrPair!";
            }
        }

        #endregion
    }
}
