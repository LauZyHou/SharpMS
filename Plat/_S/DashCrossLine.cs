using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plat._S
{
    /// <summary>
    /// 一头是Dash一头是Cross的箭头，形如
    /// --|--------------X--
    /// </summary>
    public class DashCrossLine : Line
    {
        private double vertRadius = 10;
        private double endDist = 25;

        /// <summary>
        /// 在此覆写方法中实现图形
        /// </summary>
        /// <returns></returns>
        protected override Geometry CreateDefiningGeometry()
        {
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext context = streamGeometry.Open())
            {
                double X1 = StartPoint.X;
                double X2 = EndPoint.X;
                double Y1 = StartPoint.Y;
                double Y2 = EndPoint.Y;

                double theta = Math.Atan2(Y2 - Y1, X2 - X1);
                double sint = Math.Sin(theta);
                double cost = Math.Cos(theta);

                // 长直线起点
                Point ptStart = new Point(X1, Y1);
                // 长直线终点
                Point ptEnd = new Point(X2, Y2);
                // 标记点A（Dash侧）
                Point ptTagA = new Point(X1 + cost * this.endDist, Y1 + sint * this.endDist);
                // 标记点B（Cross侧）
                Point ptTagB = new Point(X2 - cost * this.endDist, Y2 - sint * this.endDist);
                // 标记点A上方的边缘点
                Point ptTagAUp = new Point(ptTagA.X - sint * vertRadius, ptTagA.Y + cost * vertRadius);
                // 标记点A下方的边缘点
                Point ptTagADown = new Point(ptTagA.X + sint * vertRadius, ptTagA.Y - cost * vertRadius);
                // Theta - 45'
                double theta2 = theta - Math.PI / 4;
                double sint2 = Math.Sin(theta2);
                double cost2 = Math.Cos(theta2);
                // 标记点B右上角的边缘点
                Point ptTagBRightUp = new Point(ptTagB.X + cost2 * vertRadius, ptTagB.Y + sint2 * vertRadius);
                // 标记点B左下角的边缘点
                Point ptTagBLeftDown = new Point(ptTagB.X - cost2 * vertRadius, ptTagB.Y - sint2 * vertRadius);
                // 180' - Theta - 45'
                double theta3 = Math.PI - theta - Math.PI / 4;
                double sint3 = Math.Sin(theta3);
                double cost3 = Math.Cos(theta3);
                // 标记点B左上角的边缘点
                Point ptTagBLeftUp = new Point(ptTagB.X - cost3 * vertRadius, ptTagB.Y + sint3 * vertRadius);
                // 标记点B右下角的边缘点
                Point ptTagBRightDown = new Point(ptTagB.X + cost3 * vertRadius, ptTagB.Y - sint3 * vertRadius);

                // 绘制直线
                context.BeginFigure(ptStart, false);
                context.LineTo(ptEnd);
                context.EndFigure(false);
                // 绘制Dash
                context.BeginFigure(ptTagAUp, false);
                context.LineTo(ptTagADown);
                context.EndFigure(false);
                // 绘制Cross（左下到右上）
                context.BeginFigure(ptTagBLeftDown, false);
                context.LineTo(ptTagBRightUp);
                context.EndFigure(false);
                // 绘制Cross（右下到左上）
                context.BeginFigure(ptTagBRightDown, false);
                context.LineTo(ptTagBLeftUp);
                context.EndFigure(false);
            }
            return streamGeometry;
        }

        /// <summary>
        /// 与长直线垂直的最大半径
        /// </summary>
        public double VertRadius { get => vertRadius; set => vertRadius = value; }
        /// <summary>
        /// 标记点到端点的距离
        /// </summary>
        public double EndDist { get => endDist; set => endDist = value; }
    }
}
