using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._T
{
    /// <summary>
    /// 标识符映射器
    /// </summary>
    public class IdentifierMapper
    {
        /// <summary>
        /// 将进程图状态机中的类型名映射到UPPAAL类型名上
        /// </summary>
        /// <param name="sharpMsTypeInFSM"></param>
        /// <returns></returns>
        public static string MapToUpTypeName(string sharpMsTypeInFSM)
        {
            if (sharpMsTypeInFSM == "Int") return "int";
            if (sharpMsTypeInFSM == "Bool") return "bool";
            if (sharpMsTypeInFSM.EndsWith("?"))
            {
                return sharpMsTypeInFSM.Replace("?", "_A");
            }
            else if (sharpMsTypeInFSM.EndsWith("^"))
            {
                return sharpMsTypeInFSM.Replace("^", "_S");
            }
            return sharpMsTypeInFSM;
        }
    }
}
