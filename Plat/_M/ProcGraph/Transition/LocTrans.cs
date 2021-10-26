using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// Location迁移
    /// </summary>
    public class LocTrans : ReactiveObject
    {
        public static int _id = 0;

        private int id;
        private readonly ObservableCollection<Formula> actions;
        private Formula guard;

        public LocTrans()
        {
            this.id = ++_id;
            this.guard = new Formula("");
            this.actions = new ObservableCollection<Formula>();
        }

        public int Id { get => id; set => id = value; }
        /// <summary>
        /// 迁移条件
        /// </summary>
        public Formula Guard { get => guard; set => this.RaiseAndSetIfChanged(ref guard, value); }
        /// <summary>
        /// 操作步
        /// </summary>
        public ObservableCollection<Formula> Actions => actions;
    }
}
