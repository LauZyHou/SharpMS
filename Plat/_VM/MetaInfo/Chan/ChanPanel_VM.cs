using System;
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
    /// Channel面板VM
    /// </summary>
    public class ChanPanel_VM: ViewModelBase
    {
        private LogicChanGroup? currentLogicChanGroup;
        private ObservableCollection<LogicChanGroup> logicChanGroupList;

        public ChanPanel_VM()
        {
            this.logicChanGroupList = new ObservableCollection<LogicChanGroup>();
        }

        public LogicChanGroup? CurrentLogicChanGroup { get => currentLogicChanGroup; set => this.RaiseAndSetIfChanged(ref currentLogicChanGroup, value); }
        public ObservableCollection<LogicChanGroup> LogicChanGroupList { get => logicChanGroupList; set => this.RaiseAndSetIfChanged(ref logicChanGroupList, value); }

        #region Button Command

        public void DeleteLogicChanGroup(LogicChanGroup logicChanGroup)
        {
            if (logicChanGroup is null)
            {
                return;
            }
            this.logicChanGroupList.Remove(logicChanGroup);
            ResourceManager.UpdateTip($"Delete logic channel group [{logicChanGroup.Source} -> {logicChanGroup.Dest}].");
        }

        public void CreateLogicChanGroup()
        {
            this.LogicChanGroupList.Add(new LogicChanGroup(null, null));
            ResourceManager.UpdateTip("Create a new logic channel group.");
        }

        #endregion
    }
}
