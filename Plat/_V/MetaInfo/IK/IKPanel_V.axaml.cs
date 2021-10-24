using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Plat._C;
using Plat._M;
using Plat._VM;

namespace Plat._V
{
    /// <summary>
    /// Initial Knowledge Panel View Model
    /// </summary>
    public partial class IKPanel_V : UserControl
    {
        public IKPanel_V()
        {
            InitializeComponent();
            this.get_control_reference();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #region Command

        /// <summary>
        /// 创建Proc-Proc型模板IK
        /// </summary>
        private void OnCreateTemplateIK_ProcProc()
        {
            Proc? procA = (Proc?)this.ProcAList.SelectedItem;
            VisAttr? attrA = (VisAttr?)this.ProcAttrAList.SelectedItem;
            Proc? procB = (Proc?)this.ProcBList.SelectedItem;
            VisAttr? attrB = (VisAttr?)this.ProcAttrBList.SelectedItem;
            if (VM is null)
            {
                ResourceManager.UpdateTip($"Cannot get view model of IKPanel!");
                return;
            }
            if (VM.CurrentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (procA is null || attrA is null || procB is null || attrB is null)
            {
                ResourceManager.UpdateTip($"Cannot determin the op target!");
                return;
            }
            VM.CurrentIK.AttrPairs.Add(new AttrPair(procA, attrA, procB, attrB));
            ResourceManager.UpdateTip($"Create new Proc-Proc AttrPair for IK [{VM.CurrentIK.Identifier}].");
        }

        /// <summary>
        /// 创建Env-Env型模板IK
        /// </summary>
        private void OnCreateTemplateIK_EnvEnv()
        {
            Env? envA = (Env?)this.EnvAList.SelectedItem;
            VisAttr? attrA = (VisAttr?)this.EnvAttrAList.SelectedItem;
            Env? envB = (Env?)this.EnvBList.SelectedItem;
            VisAttr? attrB = (VisAttr?)this.EnvAttrBList.SelectedItem;
            if (VM is null)
            {
                ResourceManager.UpdateTip($"Cannot get view model of IKPanel!");
                return;
            }
            if (VM.CurrentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (envA is null || attrA is null || envB is null || attrB is null)
            {
                ResourceManager.UpdateTip($"Cannot determin the op target!");
                return;
            }
            VM.CurrentIK.AttrPairs.Add(new AttrPair(envA, attrA, envB, attrB));
            ResourceManager.UpdateTip($"Create new Env-Env AttrPair for IK [{VM.CurrentIK.Identifier}].");
        }
        
        /// <summary>
        /// 创建Proc-Env型模板IK
        /// </summary>
        private void OnCreateTemplateIK_ProcEnv()
        {
            Proc? proc1 = (Proc?)this.Proc1List.SelectedItem;
            VisAttr? attr1 = (VisAttr?)this.ProcAttr1List.SelectedItem;
            Env? env2 = (Env?)this.Env2List.SelectedItem;
            VisAttr? attr2 = (VisAttr?)this.EnvAttr2List.SelectedItem;
            if (VM is null)
            {
                ResourceManager.UpdateTip($"Cannot get view model of IKPanel!");
                return;
            }
            if (VM.CurrentIK is null)
            {
                ResourceManager.UpdateTip($"An IK must be selected!");
                return;
            }
            if (proc1 is null || attr1 is null || env2 is null || attr2 is null)
            {
                ResourceManager.UpdateTip($"Cannot determin the op target!");
                return;
            }
            VM.CurrentIK.AttrPairs.Add(new AttrPair(proc1, attr1, env2, attr2));
            ResourceManager.UpdateTip($"Create new Proc-Env AttrPair for IK [{VM.CurrentIK.Identifier}].");
        }

        #endregion

        private ComboBox ProcAList, ProcAttrAList, ProcBList, ProcAttrBList,
                         EnvAList, EnvAttrAList, EnvBList, EnvAttrBList,
                         Proc1List, ProcAttr1List, Env2List, EnvAttr2List;

        private void get_control_reference()
        {
            // for Proc-Proc
            this.ProcAList = ControlExtensions.FindControl<ComboBox>(this, nameof(ProcAList));
            this.ProcAttrAList = ControlExtensions.FindControl<ComboBox>(this, nameof(ProcAttrAList));
            this.ProcBList = ControlExtensions.FindControl<ComboBox>(this, nameof(ProcBList));
            this.ProcAttrBList = ControlExtensions.FindControl<ComboBox>(this, nameof(ProcAttrBList));
            // for Env-Env
            this.EnvAList = ControlExtensions.FindControl<ComboBox>(this, nameof(EnvAList));
            this.EnvAttrAList = ControlExtensions.FindControl<ComboBox>(this, nameof(EnvAttrAList));
            this.EnvBList = ControlExtensions.FindControl<ComboBox>(this, nameof(EnvBList));
            this.EnvAttrBList = ControlExtensions.FindControl<ComboBox>(this, nameof(EnvAttrBList));
            // for Proc-Env
            this.Proc1List = ControlExtensions.FindControl<ComboBox>(this, nameof(Proc1List));
            this.ProcAttr1List = ControlExtensions.FindControl<ComboBox>(this, nameof(ProcAttr1List));
            this.Env2List = ControlExtensions.FindControl<ComboBox>(this, nameof(Env2List));
            this.EnvAttr2List = ControlExtensions.FindControl<ComboBox>(this, nameof(EnvAttr2List));
        }

        public IKPanel_VM? VM
        {
            get
            {
                return (IKPanel_VM?)DataContext;
            }
        }
    }
}
