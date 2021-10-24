using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// 带有取值的Attribute
    /// </summary>
    public class ValAttr : Attribute
    {
        private string val;

        public ValAttr(string identifier, Type type, bool isArray = false, string description = "")
            :base(identifier, type, isArray, description)
        {
            this.val = "";
        }

        public string Value { get => val; set => this.RaiseAndSetIfChanged(ref val, value); }
    }
}
