using System;
using System.Drawing;

namespace Metalogic.UI
{
    public class DrawPolarCoodinationTools
    {
        public static float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }

        public static float DegreeToRadian(float angle)
        {
            return (float)(Math.PI * angle / 180.0);
        }

        public static void DrawCicle(Graphics g, Pen pen, Point centre, float radius)
        {
            try
            {
                g.DrawArc(pen, centre.X - radius, centre.Y - radius, radius * 2, radius * 2, 0, 360);

            }
            catch (Exception)
            {

            }
        }

        public static void DrawArc(Graphics g, Pen pen, Point centre, float radius, float radianStart, float radianEnd)
        {
            var degreestart = RadianToDegree(radianStart) + 270;
            var degreeEnd = RadianToDegree(radianEnd) + 270;

            //IndicationHolderComponent.SwapSmallestToFirst(ref degreestart, ref degreeEnd);
            g.DrawArc(pen, centre.X - radius, centre.Y - radius, radius * 2, radius * 2, degreestart, degreeEnd - degreestart);
        }



        public static void DrawLine(Graphics g, Pen pen, Point centre, float radian, float radiusStart, float radiusEnd)
        {

            g.DrawLine(pen, centre.X + (float)(Math.Sin(radian) * radiusStart), centre.Y - (float)(Math.Cos(radian) * radiusStart), centre.X + (float)(Math.Sin(radian) * radiusEnd), centre.Y - (float)(Math.Cos(radian) * radiusEnd));
        }

        public static void DrawText(Graphics g, Font f, Brush brush, Point centre, float radian, float radius, string text, int xMax, bool isInner = false)
        {
            var textSize = g.MeasureString(text, f);
            var radianMod = radian % (Math.PI * 2);
            var pointBase = new PointF(centre.X + (float)(Math.Sin(radian) * radius),
                                       centre.Y - (float)(Math.Cos(radian) * radius));

            if (isInner)
            {
                if (radianMod <= 0.25 * Math.PI || radianMod > 1.75 * Math.PI)
                {
                    pointBase.X = pointBase.X - textSize.Width / 2;
                }
                else if (radianMod <= 0.75 * Math.PI)
                {
                    pointBase.X = pointBase.X - textSize.Width;
                    pointBase.Y = pointBase.Y - textSize.Height / 2;
                }

                else if (radianMod <= 1.25 * Math.PI)
                {
                    pointBase.X = pointBase.X - textSize.Width / 2;
                    pointBase.Y = pointBase.Y - textSize.Height;
                }
                else
                {
                    pointBase.Y = pointBase.Y - textSize.Height / 2;
                }
            }
            else
            {
                if (radianMod <= 0.25 * Math.PI || radianMod > 1.75 * Math.PI)
                {
                    pointBase.X = pointBase.X - textSize.Width / 2;
                    pointBase.Y = pointBase.Y - textSize.Height;
                }
                else if (radianMod <= 0.75 * Math.PI)
                {
                    pointBase.X = Math.Min(pointBase.X, xMax - textSize.Width);
                    pointBase.Y = pointBase.Y - textSize.Height / 2;
                }

                else if (radianMod <= 1.25 * Math.PI)
                {
                    pointBase.X = pointBase.X - textSize.Width / 2;
                }
                else
                {
                    pointBase.X = Math.Max(pointBase.X - textSize.Width, 0);
                    pointBase.Y = pointBase.Y - textSize.Height / 2;
                }
            }

            g.DrawString(text, f, brush, pointBase.X,
                pointBase.Y);
        }
    }
}
