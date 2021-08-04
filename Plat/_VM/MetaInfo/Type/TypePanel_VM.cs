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
        private Caller? currentMethod;
        private Type? wantParamType;

        public TypePanel_VM()
        {
            this.typeList = ResourceManager.types;
            this.typeList.Add(Type.TYPE_INT);
            this.typeList.Add(Type.TYPE_BOOL);
            this.typeList.Add(Type.TYPE_MSG);
            this.typeList.Add(Type.TYPE_KEY);
            this.typeList.Add(Type.TYPE_PUB_KEY);
            this.typeList.Add(Type.TYPE_PVT_KEY);
            // 因为不能保证TypePanel_VM和ClassDragram_P_VM的构造顺序，所以不能在这去往后者里塞东西
            // 只能到后者的构造里也一个一个地把VM塞进去
        }

        public ObservableCollection<Type> TypeList { get => typeList; set => this.RaiseAndSetIfChanged(ref typeList, value); }
        public Type? CurrentType { get => currentType; set => this.RaiseAndSetIfChanged(ref currentType, value); }
        public Caller? CurrentMethod { get => currentMethod; set => this.RaiseAndSetIfChanged(ref currentMethod, value); }
        /// <summary>
        /// 想变成的参数类型
        /// </summary>
        public Type? WantParamType { get => wantParamType; set => this.RaiseAndSetIfChanged(ref wantParamType, value); }

        #region Button Command

        /// <summary>
        /// 删除类型
        /// </summary>
        /// <param name="type"></param>
        private void DeleteType(Type type)
        {
            if (type is null || type.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            this.typeList.Remove(type);
            // 同步操作结果到类图
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            foreach (DragDrop_VM item in classDiagram_P_VM.DragDrop_VMs)
            {
                if (item is Type_VM)
                {
                    Type_VM type_VM = (Type_VM)item;
                    if (type_VM.Type == type)
                    {
                        classDiagram_P_VM.DragDrop_VMs.Remove(item);
                        goto OVER;
                    }
                }
            }
            OVER:
            ResourceManager.UpdateTip($"Delete type [{type.Identifier}], sync op to class diagram.");
        }

        /// <summary>
        /// 创建新类型
        /// </summary>
        private void CreateType()
        {
            Type type = new Type("NewType");
            this.typeList.Add(type);
            // 同步操作结果到类图
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            classDiagram_P_VM.DragDrop_VMs.Add(new Type_VM(100, 100, classDiagram_P_VM, type));
            ResourceManager.UpdateTip("Create a new type, sync op to class diagram.");
        }

        /// <summary>
        /// 清除类型的继承关系
        /// </summary>
        /// <param name="type"></param>
        private void ClearParentType(Type type)
        {
            if (type is null)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
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
                ResourceManager.UpdateTip("A custom data type must be selected!");
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
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            currentType.Attributes.Remove(attribute);
            ResourceManager.UpdateTip($"Delete attribute [{attribute.Identifier}] for type [{currentType.Identifier}].");
        }

        /// <summary>
        /// 创建新的类型方法
        /// </summary>
        private void OnCreateMethod()
        {
            if (this.currentType is null || this.currentType.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            currentType.Methods.Add(new Caller("NewMethod", Type.TYPE_BOOL));
            ResourceManager.UpdateTip($"Create a new method on type [{currentType.Identifier}].");
        }

        /// <summary>
        /// 删除类型方法
        /// </summary>
        private void OnDeleteMethod(Caller c)
        {
            if (this.currentType is null || this.currentType.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            currentType.Methods.Remove(c);
            ResourceManager.UpdateTip($"Delete the method [{c.Identifier}] on type [{currentType.Identifier}].");
        }

        /// <summary>
        /// 给Method添加参数
        /// </summary>
        private void OnAddParam()
        {
            if (this.currentType is null || this.currentType.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            if (currentMethod is null)
            {
                ResourceManager.UpdateTip("A custom method must be selected!");
                return;
            }
            currentMethod.ParamTypes.Add(Type.TYPE_BOOL);
            currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"Add new param type for method [{currentMethod.Identifier}].");
        }

        /// <summary>
        /// 给Method删除参数
        /// </summary>
        private void OnDeleteParam(int? paramPos)
        {
            if (this.currentType is null || this.currentType.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            if (currentMethod is null)
            {
                ResourceManager.UpdateTip("A custom method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip("A param must be selected!");
                return;
            }
            if (paramPos < 0 || paramPos >= currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip("The pos of the param to be delete is in wrong value!");
                return;
            }
            currentMethod.ParamTypes.RemoveAt((int)paramPos);
            currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"Delete a param type for method [{currentMethod.Identifier}].");
        }

        /// <summary>
        /// 给Method表确认改参数类型
        /// </summary>
        private void OnConfirmWantParamType(int? paramPos)
        {
            if (this.currentType is null || this.currentType.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            if (currentMethod is null)
            {
                ResourceManager.UpdateTip("A custom method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip("A param must be selected!");
                return;
            }
            if (wantParamType is null)
            {
                ResourceManager.UpdateTip("A wanted param type must be selected!");
                return;
            }
            if (paramPos < 0 || paramPos >= currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip("The pos of the param to be delete is in wrong value!");
                return;
            }
            currentMethod.ParamTypes.RemoveAt((int)paramPos);
            currentMethod.ParamTypes.Insert((int)paramPos, wantParamType);
            currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"Update a param type for method [{currentMethod.Identifier}] at pos [{paramPos}].");
        }

        /// <summary>
        /// 向上移动一个参数
        /// </summary>
        private void OnMoveUpParamType(int? paramPos)
        {
            if (this.currentType is null || this.currentType.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            if (currentMethod is null)
            {
                ResourceManager.UpdateTip("A custom method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip("A param must be selected!");
                return;
            }
            if (paramPos < 0 || paramPos >= currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip("The pos of the param to be delete is in wrong value!");
                return;
            }
            if (paramPos == 0)
            {
                ResourceManager.UpdateTip("It is the first param!");
                return;
            }
            Type t = currentMethod.ParamTypes[(int)paramPos];
            currentMethod.ParamTypes.RemoveAt((int)paramPos);
            currentMethod.ParamTypes.Insert((int)paramPos - 1, t);
            currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"Move up a param type for method [{currentMethod.Identifier}] at pos [{paramPos}].");
        }

        /// <summary>
        /// 向下移动一个参数
        /// </summary>
        private void OnMoveDownParamType(int? paramPos)
        {
            if (this.currentType is null || this.currentType.IsBase)
            {
                ResourceManager.UpdateTip("A custom data type must be selected!");
                return;
            }
            if (currentMethod is null)
            {
                ResourceManager.UpdateTip("A custom method must be selected!");
                return;
            }
            if (paramPos is null)
            {
                ResourceManager.UpdateTip("A param must be selected!");
                return;
            }
            if (paramPos < 0 || paramPos >= currentMethod.ParamTypes.Count)
            {
                ResourceManager.UpdateTip("The pos of the param to be delete is in wrong value!");
                return;
            }
            if (paramPos == currentMethod.ParamTypes.Count - 1)
            {
                ResourceManager.UpdateTip("It is the last param!");
                return;
            }
            Type t = currentMethod.ParamTypes[(int)paramPos];
            currentMethod.ParamTypes.RemoveAt((int)paramPos);
            currentMethod.ParamTypes.Insert((int)paramPos + 1, t);
            currentMethod.RaisePropertyChanged("ParamTypeString");
            ResourceManager.UpdateTip($"Move down a param type for method [{currentMethod.Identifier}] at pos [{paramPos}].");
        }

        #endregion
    }
}
