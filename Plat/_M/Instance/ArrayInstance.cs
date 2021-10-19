using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 数组实例
    /// </summary>
    public class ArrayInstance : Instance
    {
        private readonly ObservableCollection<Instance> arrayItems;
        private int index = 0; // 用于产生数组下标

        public ArrayInstance(Type type, string identifier, bool isArray)
            : base(type, identifier, isArray)
        {
            // 只有数组才构造数组类型，需要由使用方保障
            Debug.Assert(isArray == true);
            this.arrayItems = new ObservableCollection<Instance>();
        }

        /// <summary>
        /// 数组项
        /// </summary>
        public ObservableCollection<Instance> ArrayItems => arrayItems;

        #region Command

        /// <summary>
        /// 添加一个数组项
        /// </summary>
        public void PushItem()
        {
            Instance instance;
            // 这一项数组项的标识符名称
            string idf = $"{this.Identifier}[{index++ }]";
            // 根据当前数组实例的类型type是否是基本类型来决定创建Value实例还是Ref实例
            if (this.Type.IsBase) // 值类型实例
            {
                instance = new ValueInstance(this.Type, idf, false);
            }
            else // 引用类型实例
            {
                instance = ReferenceInstance.build(this.Type, idf, false);
            }
            // 加入到数组项的表里
            this.arrayItems.Add(instance);
        }

        /// <summary>
        /// 弹出末尾的数组项
        /// </summary>
        public void PopItem()
        {
            int len = this.arrayItems.Count;
            if (len > 0)
            {
                this.arrayItems.RemoveAt(len - 1);
                this.index--;
            }
        }

        #endregion
    }
}
