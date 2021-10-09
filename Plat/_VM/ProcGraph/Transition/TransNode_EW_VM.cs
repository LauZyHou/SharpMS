using Plat._C;
using Plat._M;
using ReactiveUI;

namespace Plat._VM
{
    /// <summary>
    /// 编辑TransNode的窗体的VM
    /// </summary>
    public class TransNode_EW_VM : ViewModelBase
    {
        private LocTrans locTrans;
        private Formula? currentAction;

        public TransNode_EW_VM(LocTrans locTrans)
        {
            this.locTrans = locTrans;
        }

        /// <summary>
        /// 要编辑的sLocation迁移信息
        /// </summary>
        public LocTrans LocTrans => locTrans;
        /// <summary>
        /// 当前选中的Action
        /// </summary>
        public Formula? CurrentAction { get => currentAction; set => this.RaiseAndSetIfChanged(ref currentAction, value); }

        #region Command

        private void OnAddNewAction()
        {
            this.locTrans.Actions.Add(new Formula("New Action"));
            ResourceManager.UpdateTip($"Add new action.");
        }

        private void OnDeleteSelectedAction()
        {
            if (this.currentAction is null)
            {
                ResourceManager.UpdateTip($"An action must be selected!");
                return;
            }
            locTrans.Actions.Remove(this.currentAction);
            ResourceManager.UpdateTip($"Delete selected action.");
        }

        private void OnMoveUpAction(int? acPos)
        {
            if (acPos is null)
            {
                ResourceManager.UpdateTip($"An action must be selected!");
                return;
            }
            if (acPos < 0 || acPos >= this.locTrans.Actions.Count)
            {
                ResourceManager.UpdateTip($"The action pos is exceed!");
                return;
            }
            if (acPos == 0)
            {
                ResourceManager.UpdateTip($"The action is the top one, no need to move up!");
                return;
            }
            int p = (int)acPos;
            Formula action = locTrans.Actions[p];
            locTrans.Actions.RemoveAt(p);
            locTrans.Actions.Insert(p - 1, action);
            ResourceManager.UpdateTip($"Move up the action [{action.Content}].");
        }

        private void OnMoveDownAction(int? acPos)
        {
            if (acPos is null)
            {
                ResourceManager.UpdateTip($"An action must be selected!");
                return;
            }
            if (acPos < 0 || acPos >= this.locTrans.Actions.Count)
            {
                ResourceManager.UpdateTip($"The action pos is exceed!");
                return;
            }
            if (acPos == this.locTrans.Actions.Count - 1)
            {
                ResourceManager.UpdateTip($"The action is the bottom one, no need to move down!");
                return;
            }
            int p = (int)acPos;
            Formula action = locTrans.Actions[p];
            locTrans.Actions.RemoveAt(p);
            locTrans.Actions.Insert(p + 1, action);
            ResourceManager.UpdateTip($"Move down the action [{action.Content}].");
        }

        #endregion
    }
}
