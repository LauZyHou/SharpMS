using Plat._M;
using Plat._VM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._C
{
    /// <summary>
    /// 全局资源，挂过来
    /// </summary>
    public class ResourceManager
    {
        // 主窗体VM
        public static MainWindow_VM? mainWindow_VM;
        // 系统中存在的进程模板
        public static ObservableCollection<Proc> procs = new ObservableCollection<Proc>();

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
            mainWindow_VM.Tip = tip;
        }
    }
}
