using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// Process，进程模板
    /// </summary>
    public class Proc : ReactiveObject
    {
        private string identifier;
        private string description;
        private ObservableCollection<Attribute> attributes;

        public Proc(string identifier, string description = "")
        {
            this.identifier = identifier;
            this.description = description;
            this.attributes = new ObservableCollection<Attribute>();
        }

        /// <summary>
        /// 进程的标识符，就是进程的唯一名字
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 进程的描述，就是一些注释文字
        /// </summary>
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
        /// <summary>
        /// 进程的属性列表，实际上这里是指进程例化时候需要传递的参数表
        /// 请将这个字段理解为Params，在SharpMS中为了方便Param和Attr共用一个名为Attribute的class
        /// </summary>
        public ObservableCollection<Attribute> Attributes { get => attributes; set => attributes = value; }
    }
}
