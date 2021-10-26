using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// 拓扑图的抽象实例
    /// </summary>
    public abstract class TopoInst : ReactiveObject
    {
        public static int _id = 0;

        private int id;
        private string desc;

        public TopoInst()
        {
            this.id = ++_id;
            this.desc = "";
        }

        public int Id { get => id; set => id = value; }
        public string Desc { get => desc; set => this.RaiseAndSetIfChanged(ref desc, value); }

        #region As String

        public string IdStr
        {
            get
            {
                return $"{id}";
            }
        }

        #endregion
    }
}
