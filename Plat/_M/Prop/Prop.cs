using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._M
{
    /// <summary>
    /// 性质枚举
    /// </summary>
    public enum Prop
    {
        INVAR, // 不变性
        CTL, // 非嵌套CTL
        SEC, // 保密性
        FSEC, // 前向保密性
        INTE, // 完整性
        IINTE, // 单射完整性
        AUTH, // 认证性
        IAUTH // 单射认证性
    }
}
