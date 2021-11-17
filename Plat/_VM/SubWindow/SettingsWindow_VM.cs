using Plat._C;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._VM
{
    /// <summary>
    /// 设置窗体
    /// </summary>
    public class SettingsWindow_VM : ViewModelBase
    {
        public string TransPath
        {
            get => ResourceManager.transPath;
            set => ResourceManager.transPath = value;
        }
    }
}
