using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plat._T;

namespace Plat._C
{
    /// <summary>
    /// 转储管理器，将内存中的内容dump到磁盘上
    /// </summary>
    public class DumpManager
    {
        /// <summary>
        /// 将UPPAAL内存模型dump到磁盘上
        /// </summary>
        /// <param name="upProject">UPPAAL内存模型</param>
        /// <param name="filePath">要dump到的文件路径</param>
        public static void OutUppalXml(UpProject upProject, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(upProject.ToString());
            }
        }
    }
}
