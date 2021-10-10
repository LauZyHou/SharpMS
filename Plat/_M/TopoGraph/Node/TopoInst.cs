using ReactiveUI;

namespace Plat._M
{
    /// <summary>
    /// 拓扑图的抽象实例
    /// </summary>
    public abstract class TopoInst : ReactiveObject
    {
        public static int _id = 0;
        private readonly int id;

        public TopoInst()
        {
            this.id = ++_id;
        }

        public int Id => id;
    }
}
