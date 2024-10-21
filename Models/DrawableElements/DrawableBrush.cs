namespace screenerWpf.Models.DrawableElements;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Represents a drawable brush stroke that can be drawn on the canvas.
/// This element supports adding multiple points, smoothing, and creating a catmull-rom spline.
/// </summary>
internal class DrawableBrush : DrawableElement
{
    /// <summary>
    /// List of points that make up the brush stroke.
    /// </summary>
    public List<Point> points = new List<Point>();

    /// <summary>
    /// Thickness of the brush stroke.
    /// </summary>
    public double thickness;

    /// <summary>
    /// Transparency of the brush stroke, represented as a percentage (0-100).
    /// </summary>
    public double transparency;

    /// <summary>
    /// Color of the brush stroke.
    /// </summary>
    public Color color;

    /// <summary>
    /// Geometry that represents the brush path.
    /// </summary>
    private StreamGeometry geometry;

    /// <summary>
    /// Indicates if the brush stroke needs to be redrawn.
    /// </summary>
    public bool needsRedraw = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawableBrush"/> class with the specified color, thickness, and transparency.
    /// </summary>
    /// <param name="color">The color of the brush stroke.</param>
    /// <param name="thickness">The thickness of the brush stroke.</param>
    /// <param name="transparency">The transparency of the brush stroke (0-100).</param>
    public DrawableBrush(Color color, double thickness, double transparency)
    {
        this.transparency = transparency;
        this.color = Color.FromArgb((byte)(255 * transparency / 100), color.R, color.G, color.B);
        this.thickness = thickness;

        geometry = new StreamGeometry();
    }

    /// <summary>
    /// Adds a point to the brush stroke.
    /// </summary>
    /// <param name="point">The point to add.</param>
    public void AddPoint(Point point)
    {
        points.Add(point);
        needsRedraw = true;
    }

    /// <summary>
    /// Redraws the brush stroke if it has been updated.
    /// Applies smoothing and spline creation to create a visually pleasing path.
    /// </summary>
    public void Redraw()
    {
        if (needsRedraw && points.Count >= 4)
        {
            var smoothedPoints = SmoothPoints(points, 2); // Apply smoothing with window size of 2
            geometry.Clear();
            using (var ctx = geometry.Open())
            {
                var splinePoints = CreateCatmullRomPoints(smoothedPoints, 10); // Use smoothed points for the spline
                ctx.BeginFigure(splinePoints[0], false, false);
                ctx.PolyLineTo(splinePoints, true, false);
            }
            needsRedraw = false;
        }
    }

    /// <summary>
    /// Smooths the points by averaging their positions.
    /// </summary>
    /// <param name="originalPoints">The list of original points.</param>
    /// <param name="windowSize">The size of the averaging window.</param>
    /// <returns>A list of smoothed points.</returns>
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

    /// <summary>
    /// Creates Catmull-Rom spline points from the given points.
    /// </summary>
    /// <param name="points">The list of original points.</param>
    /// <param name="density">The density of points in the spline.</param>
    /// <returns>A list of points representing the Catmull-Rom spline.</returns>
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

    /// <summary>
    /// Draws the brush stroke on the canvas.
    /// </summary>
    /// <param name="context">The drawing context used for rendering the brush stroke.</param>
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

    /// <summary>
    /// Gets the bounding rectangle of the brush stroke.
    /// Currently returns an empty rectangle as it is not used for hit-testing.
    /// </summary>
    /// <returns>An empty rectangle.</returns>
    public override Rect GetBounds()
    {
        return new Rect();
    }

    /// <summary>
    /// Clones the brush element.
    /// </summary>
    /// <returns>A new instance of <see cref="DrawableBrush"/> with the same properties.</returns>
    public override DrawableElement Clone()
    {
        throw new System.NotImplementedException();
    }
}
