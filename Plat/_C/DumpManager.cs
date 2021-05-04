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
    /// UPPAAL转储管理器，将内存中的内容dump到磁盘上
    /// </summary>
    public class UpDumpManager
    {
        //生成UPPAAL模型时，每个Location的排布形如
        // 0   1   2   3   4   5
        // 11  10  9   8   7   6
        // 12  13  14  15  16  17
        //           ...
        //即成Z字形排布，所以需要限定以下参数

        /// <summary>
        /// X方向的基础值
        /// </summary>
        public static int baseX = -300;

        /// <summary>
        /// Y方向的基础值
        /// </summary>
        public static int baseY = -300;

        /// <summary>
        /// X方向的步进（最大可延展点受baseX和rowNum限制）
        /// </summary>
        public static int deltaX = 150;

        /// <summary>
        /// 一行有多少个Location
        /// </summary>
        public static int rowNum = 5;

        /// <summary>
        /// Y方向的步进（最大可延展点无限制）
        /// </summary>
        public static int deltaY = 100;

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

    /// <summary>
    /// ProVerif转储管理器，将内存中的内容dump到磁盘上
    /// </summary>
    public class PvDumpManager
    {
        // todo
    }
}
