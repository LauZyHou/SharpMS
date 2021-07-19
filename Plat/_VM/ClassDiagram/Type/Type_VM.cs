using Plat._M;

namespace Plat._VM
{
    /// <summary>
    /// 类图上的Type
    /// </summary>
    public class Type_VM : DragDrop_VM
    {
        private readonly Type type;

        /// <summary>
        /// Design
        /// </summary>
        public Type_VM()
            : base(0, 0, null)
        {
            type = new Type("Design");
        }

        public Type_VM(double x, double y, DragDrop_P_VM panelVM, Type type)
            : base(x, y, panelVM)
        {
            this.type = type;
        }

        /// <summary>
        /// 数据类型Model
        /// </summary>
        public Type Type => type;
    }
}
