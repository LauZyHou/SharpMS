using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 引用类型实例（可以继续递归展开）
    /// </summary>
    public class ReferenceInstance : Instance
    {
        private readonly List<Instance> properties;

        /// <summary>
        /// 构造器私有化，由build工厂方法提供构造
        /// </summary>
        /// <param name="type">引用数据类型</param>
        /// <param name="identifier">实例标识</param>
        /// <param name="isArray">一定为false，表示非数组</param>
        private ReferenceInstance(Type type, string identifier, bool isArray)
            :base(type, identifier, isArray)
        {
            // 只有非基本、非数组类型才构造引用类型，需要由使用方保障
            Debug.Assert((type.IsBase | isArray) == false);
            this.properties = new List<Instance>();
        }

        /// <summary>
        /// 作为引用类型的属性列表
        /// </summary>
        public List<Instance> Properties => properties;

        /// <summary>
        /// 递归构造引用实例
        /// </summary>
        /// <param name="type">引用数据类型</param>
        /// <param name="identifier">实例标识</param>
        /// <param name="isArray">一定为false，表示非数组</param>
        /// <returns></returns>
        public static ReferenceInstance build(Type type, string identifier, bool isArray)
        {
            // 先把外壳建出来
            ReferenceInstance referenceInstance = new ReferenceInstance(type, identifier, isArray);
            // 对type及其所有祖先的所有子属性都要构造一下
            Type? hyperType = type;
            do
            {
                foreach (Attribute attr in hyperType.Attributes)
                {
                    Instance instance;
                    if (attr.IsArray) // 数组类型
                    {
                        instance = new ArrayInstance(attr.Type, attr.Identifier, attr.IsArray);
                    }
                    else if (attr.Type.IsBase) // 基本类型
                    {
                        instance = new ValueInstance(attr.Type, attr.Identifier, attr.IsArray);
                    }
                    else // 引用类型（递归构造）
                    {
                        instance = build(attr.Type, attr.Identifier, attr.IsArray);
                    }
                    // 加到壳里
                    referenceInstance.properties.Add(instance);
                }
                // 往祖先走
                hyperType = hyperType.Parent;
            } while (hyperType is not null);
            // 至此，[子属性]和[祖先类型的子属性]已经全部完成，返回
            return referenceInstance;
        }
    }
}
