using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace screenerWpf.Models.DrawableElements
{
    internal class DrawableBrush : DrawableElement
    {
        public List<Point> points = new List<Point>();
        public double thickness;
        public double transparency;
        public Color color;
        private StreamGeometry geometry;
        public bool needsRedraw = true;

        public DrawableBrush(Color color, double thickness, double transparency)
        {
            this.transparency = transparency;
            this.color = Color.FromArgb((byte)(255 * transparency / 100), color.R, color.G, color.B);
            this.thickness = thickness;

            geometry = new StreamGeometry();
        }

        public void AddPoint(Point point)
        {
            points.Add(point);
            needsRedraw = true;
        }

        public void Redraw()
        {
            if (needsRedraw && points.Count >= 4)
            {
                var smoothedPoints = SmoothPoints(points, 2); // Zastosowanie wygładzania z oknem 2
                geometry.Clear();
                using (var ctx = geometry.Open())
                {
                    var splinePoints = CreateCatmullRomPoints(smoothedPoints, 10); // Użycie wygładzonych punktów
                    ctx.BeginFigure(splinePoints[0], false, false);
                    ctx.PolyLineTo(splinePoints, true, false);
                }
                needsRedraw = false;
            }
        }

        private List<Point> SmoothPoints(List<Point> originalPoints, int windowSize)
        {
            if (windowSize < 1 || originalPoints.Count < 2)
                return new List<Point>(originalPoints);

            var smoothedPoints = new List<Point>();
            for (int i = 0; i < originalPoints.Count; i++)
            {
                double sumX = 0, sumY = 0;
                int count = 0;
                for (int j = -windowSize; j <= windowSize; j++)
                {
                    int index = i + j;
                    if (index >= 0 && index < originalPoints.Count)
                    {
                        sumX += originalPoints[index].X;
                        sumY += originalPoints[index].Y;
                        count++;
                    }
                }
                smoothedPoints.Add(new Point(sumX / count, sumY / count));
            }

            return smoothedPoints;
        }


        private List<Point> CreateCatmullRomPoints(List<Point> points, int density)
        {
            if (points.Count < 4)
                return new List<Point>(points);

            var splinePoints = new List<Point>();

            for (int i = 1; i < points.Count - 2; i++)
            {
                for (int j = 0; j < density; j++)
                {
                    double t = j / (double)density;
                    double t2 = t * t;
                    double t3 = t2 * t;

                    Point pi_1 = points[i - 1];
                    Point pi = points[i];
                    Point pi1 = points[i + 1];
                    Point pi2 = points[i + 2];

                    double x = 0.5 * (2 * pi.X + (pi1.X - pi_1.X) * t +
                        (2 * pi_1.X - 5 * pi.X + 4 * pi1.X - pi2.X) * t2 +
                        (3 * pi.X - pi_1.X - 3 * pi1.X + pi2.X) * t3);

                    double y = 0.5 * (2 * pi.Y + (pi1.Y - pi_1.Y) * t +
                        (2 * pi_1.Y - 5 * pi.Y + 4 * pi1.Y - pi2.Y) * t2 +
                        (3 * pi.Y - pi_1.Y - 3 * pi1.Y + pi2.Y) * t3);

                    splinePoints.Add(new Point(x, y));
                }
            }

            return splinePoints;
        }


        public override void Draw(DrawingContext context)
        {
            Redraw();

            var pen = new Pen(new SolidColorBrush(Color.FromArgb((byte)(255 * transparency / 100), color.R, color.G, color.B)), thickness)
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

        public override DrawableElement Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
