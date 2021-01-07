using Plat._VM;
using System;
using System.Collections.Generic;
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
        public static MainWindow_VM? mainWindow_VM;

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
