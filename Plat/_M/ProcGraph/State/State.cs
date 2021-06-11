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
        public static int _id = 0;
        private string name;
        private readonly int id;

        /// <summary>
        /// 用户创建状态
        /// </summary>
        /// <param name="name"></param>
        public State(string name)
        {
            this.name = name;
            this.id = _id++;
        }

        /// <summary>
        /// 从文件导入状态
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public State(string name, int id)
        {
            this.name = name;
            this.id = id;
        }

        /// <summary>
        /// 状态的名字
        /// </summary>
        public string Name { get => name; set => this.RaiseAndSetIfChanged(ref name, value); }
        /// <summary>
        /// 状态的自增id
        /// </summary>
        public int Id => id;
        public override string ToString()
        {
            return name;
        }
    }
}
