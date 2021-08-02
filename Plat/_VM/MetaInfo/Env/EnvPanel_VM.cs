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
    /// <summary>
    /// 环境模板的Panel VM
    /// </summary>
    public class EnvPanel_VM : ViewModelBase
    {
        private Env? currentEnv;
        private ObservableCollection<Env> envList;

        public EnvPanel_VM()
        {
            this.envList = new ObservableCollection<Env>();
        }

        public Env? CurrentEnv { get => currentEnv; set => this.RaiseAndSetIfChanged(ref currentEnv, value); }
        public ObservableCollection<Env> EnvList { get => envList; set => this.RaiseAndSetIfChanged(ref envList, value); }

        #region Button Command

        public void CreateEnv()
        {

        }

        public void DeleteEnv()
        {

        }

        #endregion
    }
}
