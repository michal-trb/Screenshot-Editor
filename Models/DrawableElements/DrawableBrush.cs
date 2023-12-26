using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    internal class DrawableBrush : DrawableElement
    {
        private List<Point> points = new List<Point>();
        private double thickness;
        private Color color;
        private StreamGeometry geometry;
        private bool needsRedraw = true;

        public DrawableBrush(Color color, double thickness, double transparency)
        {
            this.color = Color.FromArgb((byte)(255 * transparency / 100), color.R, color.G, color.B);
            this.thickness = thickness;

            geometry = new StreamGeometry();
        }

        public void AddPoint(Point point)
        {
            points.Add(point);
            needsRedraw = true;
        }

        private void Redraw()
        {
            if (needsRedraw && points.Count >= 2)
            {
                geometry.Clear();
                using (var ctx = geometry.Open())
                {
                    ctx.BeginFigure(points[0], false, false);
                    ctx.PolyBezierTo(CreateSmoothCurvePoints(points), true, false);
                }
                needsRedraw = false;
            }
        }

        private List<Point> CreateSmoothCurvePoints(List<Point> originalPoints)
        {
            var smoothPoints = new List<Point>();
            if (originalPoints.Count < 2)
                return originalPoints;

            // Dodanie pierwszego punktu
            smoothPoints.Add(originalPoints[0]);

            for (int i = 0; i < originalPoints.Count - 1; i++)
            {
                var p0 = originalPoints[i];
                var p1 = originalPoints[i + 1];
                var p2 = (i + 2 < originalPoints.Count) ? originalPoints[i + 2] : p1;

                var controlPoint1 = new Point(p0.X + (p1.X - p0.X) / 3, p0.Y + (p1.Y - p0.Y) / 3);
                var controlPoint2 = new Point(p1.X - (p2.X - p0.X) / 3, p1.Y - (p2.Y - p0.Y) / 3);

                smoothPoints.Add(controlPoint1);
                smoothPoints.Add(controlPoint2);
                smoothPoints.Add(p1);
            }

            return smoothPoints;
        }


        private Point MidPoint(Point p0, Point p1)
        {
            return new Point((p0.X + p1.X) / 2.0, (p0.Y + p1.Y) / 2.0);
        }

        public override void Draw(DrawingContext context)
        {
            Redraw();

            var pen = new Pen(new SolidColorBrush(color), thickness)
            {
                LineJoin = PenLineJoin.Round,
                EndLineCap = PenLineCap.Round,
                StartLineCap = PenLineCap.Round
            };

            context.DrawGeometry(null, pen, geometry);
        }


        public override Rect GetBounds()
        {
            return new();
        }
    }
}
