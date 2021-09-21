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

        public IK(string identifier)
        {
            this.identifier = identifier;
            this.attributes = new ObservableCollection<Attribute>();
        }

        public string Identifier { get => identifier; set => this.RaiseAndSetIfChanged(ref identifier, value); }
        /// <summary>
        /// 作为全局初始知识时的属性
        /// </summary>
        public ObservableCollection<Attribute> Attributes => attributes;
    }
}
