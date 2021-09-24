using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._M
{
    /// <summary>
    /// Initial Knowledge
    /// </summary>
    public class IK : ReactiveObject
    {
        private string identifier;
        private readonly ObservableCollection<Attribute> attributes;
        private readonly ObservableCollection<AttrPair> attrPairs;

        public IK(string identifier)
        {
            this.identifier = identifier;
            this.attributes = new ObservableCollection<Attribute>();
            this.attrPairs = new ObservableCollection<AttrPair>();
        }

        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 全局初始知识的属性
        /// </summary>
        public ObservableCollection<Attribute> Attributes => attributes;
        /// <summary>
        /// 模板初始知识的<模板,属性>序偶
        /// </summary>
        public ObservableCollection<AttrPair> AttrPairs => attrPairs;

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
    }
}
