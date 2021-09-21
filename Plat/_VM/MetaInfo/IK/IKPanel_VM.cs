using Plat._C;
using Plat._M;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Plat._VM
{
    /// <summary>
    /// Initial Knowledge Panel View Model
    /// </summary>
    public class IKPanel_VM : DragDrop_P_VM
    {
        private IK? currentIK;
        private ObservableCollection<Type> typeList;
        private ObservableCollection<IK> iKList;
        private Attribute? currentAttr;

        public IKPanel_VM()
        {
            this.typeList = ResourceManager.types;
            this.iKList = ResourceManager.iks;
        }

        /// <summary>
        /// 当前选中的IK
        /// </summary>
        public IK? CurrentIK { get => currentIK; set => this.RaiseAndSetIfChanged(ref currentIK, value); }
        /// <summary>
        /// 所有Type的表
        /// </summary>
        public ObservableCollection<Type> TypeList { get => typeList; set => this.RaiseAndSetIfChanged(ref typeList, value); }
        /// <summary>
        /// 所有IK的表
        /// </summary>
        public ObservableCollection<IK> IKList { get => iKList; set => this.RaiseAndSetIfChanged(ref iKList, value); }
        /// <summary>
        /// 当前选中的Attribute
        /// </summary>
        public Attribute? CurrentAttr { get => currentAttr; set => this.RaiseAndSetIfChanged(ref currentAttr, value); }

        #region Command Operation

        /// <summary>
        /// 创建IK
        /// </summary>
        private void OnCreateNewIK()
        {
            this.iKList.Add(new IK("NewIK"));
            ResourceManager.UpdateTip($"Crate new IK.");
        }

        /// <summary>
        /// 删除IK
        /// </summary>
        private void OnDeleteSelectedIK()
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            this.IKList.Remove(this.currentIK);
            ResourceManager.UpdateTip($"Delete an IK.");
        }

        /// <summary>
        /// 创建一个Global属性
        /// </summary>
        private void OnCreateGlobalAttr()
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            this.currentIK.Attributes.Add(new Attribute("newAttr", Type.TYPE_BOOL));
            ResourceManager.UpdateTip($"Create new global attribute.");
        }

        /// <summary>
        /// 删除一个Global属性
        /// </summary>
        private void OnDeleteGlobalAttr()
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (this.currentAttr is null)
            {
                ResourceManager.UpdateTip($"A global attribute must be selected!");
                return;
            }
            this.currentIK.Attributes.Remove(this.currentAttr);
            ResourceManager.UpdateTip($"Remove a attrbite on IK [{this.currentIK.Identifier}].");
        }

        /// <summary>
        /// 向上移动一个Global Attribute
        /// </summary>
        /// <param name="attrPos"></param>
        private void OnMoveUpGlobalAttr(int? attrPos)
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (attrPos is null)
            {
                ResourceManager.UpdateTip($"A global attribute must be selected!");
                return;
            }
            if (attrPos < 0 || attrPos >= this.currentIK.Attributes.Count)
            {
                ResourceManager.UpdateTip($"The global attribute pos is exceed!");
                return;
            }
            if (attrPos == 0)
            {
                ResourceManager.UpdateTip($"The global attribute is the top one! No need to move up!");
                return;
            }
            int pos = (int)attrPos;
            Attribute attr = this.currentIK.Attributes[pos];
            this.currentIK.Attributes.RemoveAt(pos);
            this.currentIK.Attributes.Insert(pos - 1, attr);
            ResourceManager.UpdateTip($"Move up the global attribute [{attr.Identifier}] for IK [{this.currentIK.Identifier}].");
        }

        /// <summary>
        /// 向下移动一个Global Attribute
        /// </summary>
        /// <param name="attrPos"></param>
        private void OnMoveDownGlobalAttr(int? attrPos)
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (attrPos is null)
            {
                ResourceManager.UpdateTip($"A global attribute must be selected!");
                return;
            }
            if (attrPos < 0 || attrPos >= this.currentIK.Attributes.Count)
            {
                ResourceManager.UpdateTip($"The global attribute pos is exceed!");
                return;
            }
            if (attrPos == this.currentIK.Attributes.Count - 1)
            {
                ResourceManager.UpdateTip($"The global attribute is the bottom one! No need to move down!");
                return;
            }
            int pos = (int)attrPos;
            Attribute attr = this.currentIK.Attributes[pos];
            this.currentIK.Attributes.RemoveAt(pos);
            this.currentIK.Attributes.Insert(pos + 1, attr);
            ResourceManager.UpdateTip($"Move down the global attribute [{attr.Identifier}] for IK [{this.currentIK.Identifier}].");
        }

        #endregion


        public override void CreateLinker(Anchor_VM src, Anchor_VM dst)
        {
            throw new System.NotImplementedException();
        }

        public override void DeleteDragDropItem(DragDrop_VM item)
        {
            throw new System.NotImplementedException();
        }
    }
}
