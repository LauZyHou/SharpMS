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

        private int id;
        private string name;

        /// <summary>
        /// 无参构造
        /// </summary>
        public State()
        {
            this.name = $"S{this.id = ++_id}";
        }

        /// <summary>
        /// 有名构造
        /// </summary>
        /// <param name="name"></param>
        public State(string name)
        {
            this.id = ++_id;
            this.name = name;
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
        /// 状态的自增id
        /// </summary>
        public int Id { get => id; set => id = value; }
        /// <summary>
        /// 状态的名字
        /// </summary>
        public string Name { get => name; set => this.RaiseAndSetIfChanged(ref name, value); }
        public override string ToString()
        {
            return name;
        }
    }
}
