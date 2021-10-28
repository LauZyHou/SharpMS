using Plat._M;
using Plat._V;
using Plat._VM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Plat._C
{
    /// <summary>
    /// 全局资源，挂过来
    /// </summary>
    public class ResourceManager
    {
        // 主窗体View Model
        public static MainWindow_VM mainWindow_VM;
        // 主窗体View
        public static MainWindow_V mainWindow_V;
        // 系统中所有的数据类型
        public static ObservableCollection<_M.Type> types = new ObservableCollection<_M.Type>();
        // 系统中所有的环境模板
        public static ObservableCollection<_M.Env> envs = new ObservableCollection<_M.Env>();
        // 系统中所有的进程模板
        public static ObservableCollection<Proc> procs = new ObservableCollection<Proc>();
        // 系统中所有的初始知识
        public static ObservableCollection<IK> iks = new ObservableCollection<IK>();
        // 系统中所有的公理
        public static ObservableCollection<Axiom> axioms = new ObservableCollection<Axiom>();
        // 系统中所有的逻辑Channel组
        public static ObservableCollection<LogicChanGroup> logicChanGroups = new ObservableCollection<LogicChanGroup>();
        // 系统中所有的Process Graph面板的VM（在创建Proc时同步创建一个Process Graph面板）
        public static ObservableCollection<ProcGraph_P_VM> procGraph_P_VMs = new ObservableCollection<ProcGraph_P_VM>();
        // 所有ProcEnvInst
        public static ObservableCollection<ProcEnvInst> procEnvInsts = new ObservableCollection<ProcEnvInst>();
        /// <summary>
        /// 锚点是否可见
        /// </summary>
        public static Subject<bool> anchorVisible = new Subject<bool>();
        /// <summary>
        /// Tip的刷新次数，每次刷新都+1
        /// </summary>
        public static uint tipFlushNum = 0;

        /// <summary>
        /// 更新主页最下方的提示内容
        /// </summary>
        /// <param name="tip"></param>
        public static void UpdateTip(string tip)
        {
            if (mainWindow_VM is null)
            {
                return;
            }
            mainWindow_VM.Tip = $"#{++tipFlushNum} | {tip}";
        }
    }
}
