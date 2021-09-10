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
        private ObservableCollection<Caller> methods;
        private ObservableCollection<Port> ports;
        private Proc? parent;

        public Proc(string identifier, string description = "")
        {
            this.identifier = identifier;
            this.description = description;
            this.attributes = new ObservableCollection<Attribute>();
            this.methods = new ObservableCollection<Caller>();
            this.ports = new ObservableCollection<Port>();
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
        /// <summary>
        /// 进程的方法列表
        /// </summary>
        public ObservableCollection<Caller> Methods { get => methods; set => methods = value; }
        /// <summary>
        /// 通信端口列表
        /// </summary>
        public ObservableCollection<Port> Ports { get => ports; set => ports = value; }
        /// <summary>
        /// 父进程模板（见进程模板的继承关系
        /// </summary>
        public Proc? Parent { get => parent; set => this.RaiseAndSetIfChanged(ref parent, value); }

        public override string ToString()
        {
            return identifier;
        }

        #region Have xxx 属性

        public bool HaveAttr
        {
            get
            {
                return this.attributes is not null && this.attributes.Count > 0;
            }
        }

        public bool HaveMethod
        {
            get
            {
                return this.methods is not null && this.methods.Count > 0;
            }
        }

        public bool HavePort
        {
            get
            {
                return this.ports is not null && this.ports.Count > 0;
            }
        }

        public bool HaveParent
        {
            get
            {
                return this.parent is not null;
            }
        }

        #endregion
    }
}
