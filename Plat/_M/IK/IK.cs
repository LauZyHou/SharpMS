using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._M
{
    /// <summary>
    /// Initial Knowledge
    /// </summary>
    public class IK : ReactiveObject
    {
        private static int _id = 0;
        private string identifier;
        private readonly ObservableCollection<ValAttr> attributes;
        private readonly ObservableCollection<AttrPair> attrPairs;
        private string description;

        /// <summary>
        /// 无参构造
        /// </summary>
        public IK()
        {
            this.identifier = $"Ik{++_id}";
            this.description = "";
            this.attributes = new ObservableCollection<ValAttr>();
            this.attrPairs = new ObservableCollection<AttrPair>();
        }

        /// <summary>
        /// 带有标识的构造
        /// </summary>
        /// <param name="identifier">初始知识标识</param>
        /// <param name="description">注解描述</param>
        public IK(string identifier, string description = "")
        {
            this.identifier = identifier;
            this.description = description;
            this.attributes = new ObservableCollection<ValAttr>();
            this.attrPairs = new ObservableCollection<AttrPair>();
        }

        /// <summary>
        /// 初始知识标识
        /// </summary>
        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 全局初始知识的属性
        /// </summary>
        public ObservableCollection<ValAttr> Attributes => attributes;
        /// <summary>
        /// 模板初始知识的<模板,属性>序偶
        /// </summary>
        public ObservableCollection<AttrPair> AttrPairs => attrPairs;
        /// <summary>
        /// 注解描述
        /// </summary>
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }

        #region Have xxx

        /// <summary>
        /// 有 全局知识
        /// </summary>
        public bool HaveGlbIni
        {
            get
            {
                return this.attributes is not null && this.attributes.Count > 0;
            }
        }

        /// <summary>
        /// 有 模板知识
        /// </summary>
        public bool HaveTmpIni
        {
            get
            {
                return this.attrPairs is not null && this.attrPairs.Count > 0;
            }
        }

        #endregion

        public override string ToString()
        {
            return this.identifier;
        }
    }
}
