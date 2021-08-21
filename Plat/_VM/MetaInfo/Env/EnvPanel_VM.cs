using Plat._C;
using Plat._M;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 环境模板的Panel VM
    /// </summary>
    public class EnvPanel_VM : ViewModelBase
    {
        private Env? currentEnv;
        private ObservableCollection<Env> envList;
        private readonly ObservableCollection<Type> typeList;

        public EnvPanel_VM()
        {
            this.envList = ResourceManager.envs;
            this.typeList = ResourceManager.types;
        }

        public Env? CurrentEnv { get => currentEnv; set => this.RaiseAndSetIfChanged(ref currentEnv, value); }
        public ObservableCollection<Env> EnvList { get => envList; set => this.RaiseAndSetIfChanged(ref envList, value); }
        public ObservableCollection<Type> TypeList => typeList;
        #region Button Command

        /// <summary>
        /// 创建Env
        /// </summary>
        private void CreateEnv()
        {
            Env env = new Env("NewEnv");
            this.envList.Add(env);
            // 同步操作结果到类图
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            classDiagram_P_VM.DragDrop_VMs.Add(new Env_VM(200, 200, classDiagram_P_VM, env));
            ResourceManager.UpdateTip($"Add new env, sync op to class diagram.");
        }

        /// <summary>
        /// 删除Env
        /// </summary>
        private void DeleteEnv()
        {
            if (this.currentEnv is null)
            {
                ResourceManager.UpdateTip("An env must be selected!");
                return;
            }
            var env = currentEnv; // 因为从列表里删了之后currentEnv就会变成null所以额外存一下
            this.envList.Remove(env);
            // 同步操作结果到类图
            ClassDiagram_P_VM classDiagram_P_VM = ResourceManager.mainWindow_VM.ClassDiagram_P_VM;
            foreach (DragDrop_VM item in classDiagram_P_VM.DragDrop_VMs)
            {
                if (item is Env_VM)
                {
                    Env_VM env_VM = (Env_VM)item;
                    if (env_VM.Env == env)
                    {
                        classDiagram_P_VM.DragDrop_VMs.Remove(item);
                        goto OVER;
                    }
                }
            }
        OVER:
            ResourceManager.UpdateTip($"Delet env [{env.Identifier}], sync op to class diagram.");
        }

        /// <summary>
        /// 创建新的Attr
        /// </summary>
        private void CreateAttribute()
        {
            if (currentEnv is null)
            {
                ResourceManager.UpdateTip("A env must be selected!");
                return;
            }
            currentEnv.Attributes.Add(new _M.VisAttr("NewAttr", _M.Type.TYPE_BOOL));
            ResourceManager.UpdateTip($"Create a new attr on env [{currentEnv.Identifier}].");
        }

        /// <summary>
        /// 删除Attr
        /// </summary>
        private void DeleteAttribute(VisAttr? attribute)
        {
            if (currentEnv is null)
            {
                ResourceManager.UpdateTip("A env must be selected!");
                return;
            }
            if (attribute is null)
            {
                ResourceManager.UpdateTip("An attr must be seleted!");
                return;
            }
            currentEnv.Attributes.Remove(attribute);
            ResourceManager.UpdateTip($"Remove attribute [{attribute}] on env [{currentEnv.Identifier}].");
        }

        /// <summary>
        /// 添加新的信道
        /// </summary>
        private void CreateChannel()
        {
            if (currentEnv is null)
            {
                ResourceManager.UpdateTip("A env must be selected!");
                return;
            }
            currentEnv.Channels.Add(new Channel("NewChan", 1, false));
            ResourceManager.UpdateTip($"Add new channel for env [{currentEnv.Identifier}].");
        }

        /// <summary>
        /// 删除选中的信道
        /// </summary>
        private void DeleteChannel(Channel? c)
        {
            if (currentEnv is null)
            {
                ResourceManager.UpdateTip("A env must be selected!");
                return;
            }
            if (c is null)
            {
                ResourceManager.UpdateTip("A channe must be seleted!");
                return;
            }
            var env = currentEnv;
            env.Channels.Remove(c);
            ResourceManager.UpdateTip($"Delete channel [{c.Identifier}] on env [{env.Identifier}].");
        }

        /// <summary>
        /// 清除父Env
        /// </summary>
        private void ClearParentEnv()
        {
            if (currentEnv is null)
            {
                ResourceManager.UpdateTip("A env must be selected!");
                return;
            }
            currentEnv.Parent = null;
            ResourceManager.UpdateTip($"Clear parent env for env [{currentEnv.Identifier}].");
        }

        #endregion
    }
}
