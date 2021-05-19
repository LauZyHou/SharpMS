using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 状态机状态类，可以作为其它状态的父类
    /// </summary>
    public class State : ReactiveObject
    {
        private string name;
        public State(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 状态机的名字
        /// </summary>
        public string Name { get => name; set => this.RaiseAndSetIfChanged(ref name, value); }

        public override string ToString()
        {
            return name;
        }
    }
}
