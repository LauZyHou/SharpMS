using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// 通用的公式类（公理公式、操作步等
    /// </summary>
    public class Formula : ReactiveObject
    {
        public static int _id = 0;

        private int id;
        private string content;
        private string description;

        public Formula(string content, string description = "")
        {
            this.id = ++_id;
            this.content = content;
            this.description = description;
        }

        public int Id { get => id; set => id = value; }
        public string Content { get => content; set => this.RaiseAndSetIfChanged(ref content, value); }
        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
    }
}
