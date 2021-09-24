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
        private readonly ObservableCollection<Type> typeList;
        private readonly ObservableCollection<IK> iKList;
        private Attribute? currentAttr;
        private AttrPair? currentAttrPair;
        private readonly ObservableCollection<Proc> procList;
        private readonly ObservableCollection<Env> envList;

        public IKPanel_VM()
        {
            this.typeList = ResourceManager.types;
            this.iKList = ResourceManager.iks;
            this.procList = ResourceManager.procs;
            this.envList = ResourceManager.envs;
        }

        /// <summary>
        /// 当前选中的IK
        /// </summary>
        public IK? CurrentIK { get => currentIK; set => this.RaiseAndSetIfChanged(ref currentIK, value); }
        /// <summary>
        /// 所有Type的表
        /// </summary>
        public ObservableCollection<Type> TypeList => typeList;
        /// <summary>
        /// 所有IK的表
        /// </summary>
        public ObservableCollection<IK> IKList => iKList;
        /// <summary>
        /// 当前选中的Attribute
        /// </summary>
        public Attribute? CurrentAttr { get => currentAttr; set => this.RaiseAndSetIfChanged(ref currentAttr, value); }
        /// <summary>
        /// 当前选中的AttrPair，用于Proc型IK
        /// </summary>
        public AttrPair? CurrentAttrPair { get => currentAttrPair; set => this.RaiseAndSetIfChanged(ref currentAttrPair, value); }
        /// <summary>
        /// 所有的Proc的表
        /// </summary>
        public ObservableCollection<Proc> ProcList => procList;
        /// <summary>
        /// 所有的Env的表
        /// </summary>
        public ObservableCollection<Env> EnvList => envList;

        #region Command Operation

        /// <summary>
        /// 创建IK
        /// </summary>
        private void OnCreateNewIK()
        {
            IK ik = new IK("NewIK");
            this.iKList.Add(ik);
            // 同步到Class Diagram
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            classDiagram_P_VM.DragDrop_VMs.Add(new IK_VM(100, 100, classDiagram_P_VM, ik));
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

        /// <summary>
        /// 删除指定的模板IK
        /// </summary>
        private void OnDeleteTemplateIK()
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (this.currentAttrPair is null)
            {
                ResourceManager.UpdateTip($"A template IK must be selected!");
                return;
            }
            this.currentIK.AttrPairs.Remove(this.currentAttrPair);
            ResourceManager.UpdateTip($"Remove template IK from IK [{this.currentIK.Identifier}].");
        }

        /// <summary>
        /// 上移一个Template IK
        /// </summary>
        /// <param name="vaPos"></param>
        private void OnMoveUpTemplateIK(int? apPos)
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (apPos is null)
            {
                ResourceManager.UpdateTip($"A template IK must be selected!");
                return;
            }
            if (apPos < 0 || apPos >= this.currentIK.AttrPairs.Count)
            {
                ResourceManager.UpdateTip($"Attr pair pos exceed!");
                return;
            }
            if (apPos == 0)
            {
                ResourceManager.UpdateTip($"The attr pair is the top one! No need to move up!");
                return;
            }
            int pos = (int)apPos;
            AttrPair attrPair = (AttrPair)this.currentIK.AttrPairs[pos];
            this.currentIK.AttrPairs.RemoveAt(pos);
            this.currentIK.AttrPairs.Insert(pos - 1, attrPair);
            ResourceManager.UpdateTip($"Move up template IK (attr pair) [{attrPair.ShowStr}] for IK [{this.currentIK.Identifier}].");
        }

        /// <summary>
        /// 下移一个Template IK
        /// </summary>
        /// <param name="vaPos"></param>
        private void OnMoveDownTemplateIK(int? apPos)
        {
            if (this.currentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (apPos is null)
            {
                ResourceManager.UpdateTip($"A template IK must be selected!");
                return;
            }
            if (apPos < 0 || apPos >= this.currentIK.AttrPairs.Count)
            {
                ResourceManager.UpdateTip($"Attr pair pos exceed!");
                return;
            }
            if (apPos == this.currentIK.AttrPairs.Count - 1)
            {
                ResourceManager.UpdateTip($"The attr pair is the bottom one! No need to move down!");
                return;
            }
            int pos = (int)apPos;
            AttrPair attrPair = (AttrPair)this.currentIK.AttrPairs[pos];
            this.currentIK.AttrPairs.RemoveAt(pos);
            this.currentIK.AttrPairs.Insert(pos + 1, attrPair);
            ResourceManager.UpdateTip($"Move down template IK (attr pair) [{attrPair.ShowStr}] for IK [{this.currentIK.Identifier}].");
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
