using Plat._C;
using Plat._M;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    public class PropVerify_P_VM : ViewModelBase
    {
        private readonly ObservableCollection<Property> properties;
        private readonly List<Prop> propList;
        private Property? curProperty;

        public PropVerify_P_VM()
        {
            this.properties = ResourceManager.props;
            this.propList = ResourceManager.propEnumList;
        }

        /// <summary>
        /// 所有的功能安全和信息安全性质
        /// </summary>
        public ObservableCollection<Property> Properties => properties;
        /// <summary>
        /// Prop的枚举表
        /// </summary>
        public List<Prop> PropList => propList;
        /// <summary>
        /// 当前选中的Property
        /// </summary>
        public Property? CurProperty { get => curProperty; set => this.RaiseAndSetIfChanged(ref curProperty, value); }

        #region Command

        /// <summary>
        /// 创建新的安全性质
        /// </summary>
        public void CreateNewProp()
        {
            Property property = new Property();
            this.properties.Add(property);
            ResourceManager.UpdateTip("Create new property.");
        }

        /// <summary>
        /// 删除选中的安全性质
        /// </summary>
        public void DeleteSelectedProp()
        {
            if (this.curProperty is null)
            {
                ResourceManager.UpdateTip("Nothing selected.");
                return;
            }
            this.properties.Remove(this.curProperty);
            ResourceManager.UpdateTip("Delete selected property.");
        }

        /// <summary>
        /// 下移一个Property
        /// </summary>
        /// <param name="pIndex">当前选中位置</param>
        public void MoveUpProperty(int? pIndex)
        {
            if (pIndex is null) return;
            if (pIndex < 0 || pIndex >= this.properties.Count) return;
            if (pIndex == 0)
            {
                ResourceManager.UpdateTip("Cur as top one!");
                return;
            }
            Property property = this.properties[(int)pIndex];
            this.properties.RemoveAt((int)pIndex);
            this.properties.Insert((int)pIndex - 1, property);
            ResourceManager.UpdateTip($"Move up property [{property}].");
        }

        /// <summary>
        /// 上移一个Property
        /// </summary>
        /// <param name="pIndex">当前选中位置</param>
        public void MoveDownProperty(int? pIndex)
        {
            if (pIndex is null) return;
            if (pIndex < 0 || pIndex >= this.properties.Count) return;
            if (pIndex == this.properties.Count - 1)
            {
                ResourceManager.UpdateTip("Cur as bottom one!");
                return;
            }
            Property property = this.properties[(int)pIndex];
            this.properties.RemoveAt((int)pIndex);
            this.properties.Insert((int)pIndex + 1, property);
            ResourceManager.UpdateTip($"Move down property [{property}].");
        }

        #endregion
    }
}
