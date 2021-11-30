using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plat._C;

namespace Plat._T
{
    /// <summary>
    /// UPPAAL location model, which can be trans to "<location/>" or "<init/>"
    /// </summary>
    public class UpLocation
    {
        public static int _id = 0;
        private readonly int id;
        private string? name;
        private bool isCommitted;
        private bool isInit;
        private string? invariant;
        private int x;
        private int y;

        /// <summary>
        /// 自动计算坐标位置的构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isCommitted"></param>
        /// <param name="isInit"></param>
        public UpLocation(string? name, bool isCommitted = false, bool isInit = false)
        {
            this.id = _id++;
            this.name = name;
            this.isCommitted = isCommitted;
            this.isInit = isInit;
            // 计算Z字形绘制时，Location具体应放在哪个位置
            // id / rowNum 即是放在的行号，从0开始
            // 所以，baseY + (id / rowNum) * deltaY 即是Y方向的值
            // id % rowNum 即表明这个Location是这一行的第几个绘制的，从0开始
            // 如果 id / rowNum 是偶数，说明从左往右排布
            // 此时，baseX + (id % rowNum) * deltaX 即是X方向的值
            // 如果 id / rowNum 是奇数，说明是从右往左排布
            // 此时，相当于从左往右排布的第 rowNum - 1 - (id % rowNum) 个Location
            // 因此，baseX + (rowNum - 1 - id % rowNum) * deltaX 即是X方向的值
            int curRowNum = this.id / UpDumpManager.rowNum;
            int curRowCnt = this.id % UpDumpManager.rowNum;
            this.y = UpDumpManager.baseY + curRowNum * UpDumpManager.deltaY;
            if (curRowNum % 2 == 0)
            {
                this.x = UpDumpManager.baseX + curRowCnt * UpDumpManager.deltaX;
            }
            else
            {
                this.x = UpDumpManager.baseX + (UpDumpManager.rowNum - 1 - curRowCnt) * UpDumpManager.deltaX;
            }
        }

        /// <summary>
        /// 带坐标位置的构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isCommitted"></param>
        /// <param name="isInit"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public UpLocation(string name, bool isCommitted, bool isInit, int x, int y)
        {
            this.id = _id++;
            this.name = name;
            this.isCommitted = isCommitted;
            this.isInit = isInit;
            this.x = x;
            this.y = y;
        }

        public string? Name { get => name; set => name = value; }
        public bool IsCommitted { get => isCommitted; set => isCommitted = value; }
        public bool IsInit { get => isInit; set => isInit = value; }
        public int Id => id;
        public string? Invariant { get => invariant; set => invariant = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public override string ToString()
        {
            return $"\t\t<location id=\"id{id}\" x=\"{x}\" y=\"{y}\">\n" +
                   (name is not null ? $"\t\t\t<name x=\"{x - 8}\" y=\"{y + 10}\">{name}</name>\n" : "") +
                   (invariant is not null ? $"\t\t\t<label kind=\"invariant\" x=\"{x}\" y=\"{y + 80}\">{invariant}</label>\n" : "") +
                   (isCommitted ? "\t\t\t<committed/>\n" : "") +
                   "\t\t</location>\n";
        }
    }
}
