using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plat._C;
using Plat._M;
using ReactiveUI;

namespace Plat._VM
{
    /// <summary>
    /// 数据类型面板VM
    /// </summary>
    public class TypePanel_VM : ViewModelBase
    {

        private Type? currentType;
        private ObservableCollection<Type> typeList;

        public TypePanel_VM()
        {
            this.typeList = new ObservableCollection<Type>();
            this.typeList.Add(Type.TYPE_INT);
            this.typeList.Add(Type.TYPE_MSG);
        }

        public ObservableCollection<Type> TypeList { get => typeList; set => this.RaiseAndSetIfChanged(ref typeList, value); }
        public Type? CurrentType { get => currentType; set => this.RaiseAndSetIfChanged(ref currentType, value); }

        #region Button Command

        /// <summary>
        /// 删除类型
        /// </summary>
        /// <param name="type"></param>
        private void DeleteType(Type type)
        {
            if (type is null || type.IsBase)
            {
                return;
            }
            this.typeList.Remove(type);
            ResourceManager.UpdateTip($"Delete type [{type.Identifier}].");
        }

        /// <summary>
        /// 创建新类型
        /// </summary>
        private void CreateType()
        {
            this.typeList.Add(new Type("NewType"));
            ResourceManager.UpdateTip("Create a new type.");
        }

        /// <summary>
        /// 清除类型的继承关系
        /// </summary>
        /// <param name="type"></param>
        private void ClearParentType(Type type)
        {
            if (type is null)
            {
                return;
            }
            type.Parent = null;
        }

        /// <summary>
        /// 创建新的类型字段
        /// </summary>
        private void CreateAttribute(Type type)
        {
            if (type is null || type.IsBase)
            {
                return;
            }
            type.Attributes.Add(new Attribute("newAttr", Type.TYPE_INT));
            ResourceManager.UpdateTip($"Create a new attribute for type [{type.Identifier}].");
        }

        /// <summary>
        /// 从当前Type中删除字段
        /// </summary>
        /// <param name="attribute"></param>
        private void DeleteAttribute(Attribute attribute)
        {
            if (currentType is null || currentType.IsBase || attribute is null)
            {
                return;
            }
            currentType.Attributes.Remove(attribute);
            ResourceManager.UpdateTip($"Delete attribute [{attribute.Identifier}] for type [{currentType.Identifier}].");
        }

        #endregion
    }
}
