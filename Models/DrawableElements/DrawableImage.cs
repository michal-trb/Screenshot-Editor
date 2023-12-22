using System.Windows.Media;
using System.Windows;

namespace screenerWpf.Models.DrawableElements
{
    public class DrawableImage : DrawableElement
    {
        private ImageSource imageSource;

        public DrawableImage(ImageSource source, Point position)
        {
            imageSource = source;
            Position = position;
        }

        public override void Draw(DrawingContext context)
        {
            if (imageSource != null)
            {
                context.DrawImage(imageSource, new Rect(Position, new Size(imageSource.Width, imageSource.Height)));
            }
        }

        public override Rect GetBounds()
        {
            return new Rect(Position, new Size(imageSource.Width, imageSource.Height));
        }
    }
}
