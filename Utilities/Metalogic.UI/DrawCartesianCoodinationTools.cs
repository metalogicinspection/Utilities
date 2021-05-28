using System.Drawing;

namespace Metalogic.UI
{
    public class DrawCartesianCoodinationTools
    {
        public enum TextPositions
        {
            RightTop, RightMiddle, RightBottom, LeftTop, LeftMiddle, LeftButtom, BottomMiddle, BottomRight, BottomLeft
        }

        public static void DrawText(Graphics g, Font f, Brush brush, Point location, TextPositions position, string text)
        {
            var textSize = g.MeasureString(text, f);
            switch (position)
            {
                case TextPositions.RightTop:
                    g.DrawString(text, f, brush, location.X, location.Y - textSize.Height);
                    break;
                case TextPositions.RightMiddle:
                    g.DrawString(text, f, brush, location.X, location.Y - textSize.Height / 2);
                    break;
                case TextPositions.RightBottom:
                    g.DrawString(text, f, brush, location.X, location.Y);
                    break;
                case TextPositions.LeftTop:
                    g.DrawString(text, f, brush, location.X - textSize.Width, location.Y - textSize.Height);
                    break;
                case TextPositions.LeftMiddle:
                    g.DrawString(text, f, brush, location.X - textSize.Width, location.Y - textSize.Height / 2);
                    break;
                case TextPositions.LeftButtom:
                    g.DrawString(text, f, brush, location.X - textSize.Width, location.Y);
                    break;
                case TextPositions.BottomMiddle:
                    g.DrawString(text, f, brush, location.X - textSize.Width / 2, location.Y);
                    break;
                case TextPositions.BottomRight:
                    g.DrawString(text, f, brush, location.X, location.Y);
                    break;
                case TextPositions.BottomLeft:
                    g.DrawString(text, f, brush, location.X - textSize.Width, location.Y);
                    break;
            }

        }

        public static void DrawText(Graphics g, Font f, Brush brush, Point zeroLocation, Point location,
            TextPositions position, string text)
        {
            var textSize = g.MeasureString(text, f);

            var locationAdjust = new Point(zeroLocation.X + location.X, zeroLocation.Y + location.Y);
            switch (position)
            {
                case TextPositions.RightTop:
                    g.DrawString(text, f, brush, locationAdjust.X, locationAdjust.Y - textSize.Height);
                    break;
                case TextPositions.RightMiddle:
                    g.DrawString(text, f, brush, locationAdjust.X, locationAdjust.Y - textSize.Height / 2);
                    break;
                case TextPositions.RightBottom:
                    g.DrawString(text, f, brush, locationAdjust.X, locationAdjust.Y);
                    break;
                case TextPositions.LeftTop:
                    g.DrawString(text, f, brush, locationAdjust.X - textSize.Width, locationAdjust.Y - textSize.Height);
                    break;
                case TextPositions.LeftMiddle:
                    g.DrawString(text, f, brush, locationAdjust.X - textSize.Width, locationAdjust.Y - textSize.Height / 2);
                    break;
                case TextPositions.LeftButtom:
                    g.DrawString(text, f, brush, locationAdjust.X - textSize.Width, locationAdjust.Y);
                    break;
            }
        }
    }
}
