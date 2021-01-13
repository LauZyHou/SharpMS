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
    public class ChanPanel_VM : ViewModelBase
    {
        private LogicChanGroup? currentLogicChanGroup;
        private ObservableCollection<LogicChanGroup> logicChanGroupList;
        private LogicChan? currentLogicChan;
        private readonly ObservableCollection<Proc> procs;

        public ChanPanel_VM()
        {
            this.procs = ResourceManager.procs;
            this.logicChanGroupList = new ObservableCollection<LogicChanGroup>();
        }

        // 引用系统中所有的进程模板，这里只作读取用
        public ObservableCollection<Proc> Procs => procs;
        public LogicChanGroup? CurrentLogicChanGroup { get => currentLogicChanGroup; set => this.RaiseAndSetIfChanged(ref currentLogicChanGroup, value); }
        public ObservableCollection<LogicChanGroup> LogicChanGroupList { get => logicChanGroupList; set => this.RaiseAndSetIfChanged(ref logicChanGroupList, value); }
        public LogicChan? CurrentLogicChan { get => currentLogicChan; set => this.RaiseAndSetIfChanged(ref currentLogicChan, value); }

        #region Button Command

        /// <summary>
        /// 删除逻辑Channel组
        /// </summary>
        /// <param name="logicChanGroup"></param>
        public void DeleteLogicChanGroup(LogicChanGroup logicChanGroup)
        {
            if (logicChanGroup is null)
            {
                return;
            }
            this.logicChanGroupList.Remove(logicChanGroup);
            ResourceManager.UpdateTip($"Delete logic channel group [{logicChanGroup.Source} -> {logicChanGroup.Dest}].");
        }

        /// <summary>
        /// 创建逻辑Channel组
        /// </summary>
        public void CreateLogicChanGroup()
        {
            this.LogicChanGroupList.Add(new LogicChanGroup(null, null));
            ResourceManager.UpdateTip("Create a new logic channel group.");
        }

        /// <summary>
        /// 删除逻辑Channel
        /// </summary>
        /// <param name="logicChan"></param>
        public void DeleteLogicChan(LogicChan logicChan)
        {
            if (logicChan is null || this.currentLogicChanGroup is null)
            {
                return;
            }
            this.currentLogicChanGroup.LogicChanList.Remove(logicChan);
            ResourceManager.UpdateTip($"Delete logic channel [{logicChan.Identifier}].");
        }

        /// <summary>
        /// 创建逻辑Channel
        /// </summary>
        public void CreateLogicChan()
        {
            if (this.currentLogicChanGroup is null)
            {
                return;
            }
            this.currentLogicChanGroup.LogicChanList.Add(new LogicChan("NewLogicChan", this.currentLogicChanGroup));
            ResourceManager.UpdateTip($"Create a new logic channel in group [{this.currentLogicChanGroup.Source} -> {this.currentLogicChanGroup.Dest}].");
        }

        #endregion
    }
}
