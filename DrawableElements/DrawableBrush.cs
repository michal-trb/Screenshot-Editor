using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.DrawableElements
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
            if (originalPoints.Count < 2)
                return originalPoints;

            var smoothPoints = new List<Point>();
            for (int i = 0; i < originalPoints.Count - 1; i++)
            {
                if (i == 0) // Początek ścieżki
                {
                    smoothPoints.Add(originalPoints[i]);
                    smoothPoints.Add(MidPoint(originalPoints[i], originalPoints[i + 1]));
                }
                else
                {
                    var p0 = originalPoints[i];
                    var p1 = originalPoints[i + 1];

                    var midP0P1 = MidPoint(p0, p1);
                    smoothPoints.Add(midP0P1);
                    smoothPoints.Add(p1);
                }
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
