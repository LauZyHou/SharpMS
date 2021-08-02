﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 类型系统中的数据类型模型
    /// </summary>
    public class Type : ReactiveObject
    {
        private string identifier;
        private string description;
        private Type? parent;
        private ObservableCollection<Attribute> attributes;
        private ObservableCollection<Caller> methods;
        private readonly bool isBase;

        public Type(string identifier, string description = "", bool isBase = false)
        {
            this.identifier = identifier;
            this.description = description;
            this.isBase = isBase;
            this.parent = null;
            this.attributes = new ObservableCollection<Attribute>();
            this.methods = new ObservableCollection<Caller>();
        }

        /// <summary>
        /// 类型的标识符，如Int、Msg
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 类型的描述，如Int的描述是integer number，只是给用户注释用
        /// </summary>
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
        /// <summary>
        /// 类型的父类型，用于表达类型系统中的继承关系
        /// </summary>
        public Type? Parent { get => parent; set => this.RaiseAndSetIfChanged(ref parent, value); }
        /// <summary>
        /// 类型的属性列表，可以理解为C语言Struct里的一个个字段
        /// </summary>
        public ObservableCollection<Attribute> Attributes { get => attributes; set => attributes = value; }
        /// <summary>
        /// 类型的方法定义表
        /// </summary>
        public ObservableCollection<Caller> Methods { get => methods; set => methods = value; }
        /// <summary>
        /// 是否是内置（基本）类型，目前只有Int和Msg是
        /// </summary>
        public bool IsBase => isBase;

        /// <summary>
        /// Int类型，可以表达整数、枚举
        /// </summary>
        public static readonly Type TYPE_INT = new Type("Int", "integer number", true);
        /// <summary>
        /// Bool类型，逻辑量
        /// </summary>
        public static readonly Type TYPE_BOOL = new Type("Bool", "logical type", true);
        /// <summary>
        /// Msg类型，可以表达网络中传输的消息，当一个消息需要被传输时，一定直接或间接继承Msg
        /// </summary>
        public static readonly Type TYPE_MSG = new Type("Msg", "message in network", true);
        /// <summary>
        /// 对称密钥
        /// </summary>
        public static readonly Type TYPE_KEY = new Type("Key", "symmetric key", true) { parent = TYPE_MSG };
        /// <summary>
        /// 非对称公钥
        /// </summary>
        public static readonly Type TYPE_PUB_KEY = new Type("PubKey", "asymmetirc public key", true) { parent = TYPE_MSG };
        /// <summary>
        /// 非对称私钥
        /// </summary>
        public static readonly Type TYPE_PVT_KEY = new Type("PvtKey", "asymmetirc private key", true) { parent = TYPE_MSG };

        #region Have xxx 字段

        /// <summary>
        /// 有父类型
        /// </summary>
        public bool HaveParent
        {
            get
            {
                return this.parent is not null;
            }
        }

        /// <summary>
        /// 有属性
        /// </summary>
        public bool HaveAttr
        {
            get
            {
                return (this.attributes is not null) && (this.attributes.Count > 0);
            }
        }

        /// <summary>
        /// 有方法
        /// </summary>
        public bool HaveMethod
        {
            get
            {
                return (this.methods is not null) && (this.methods.Count > 0);
            }
        }

        #endregion

        public override string ToString()
        {
            return identifier;
        }
    }
}
