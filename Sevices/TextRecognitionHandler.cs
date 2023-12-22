using Tesseract;
using System.Drawing; // Może wymagać dodatkowego pakietu System.Drawing.Common
using System.Windows.Media.Imaging;
using System;
using System.Windows;
using System.Windows.Controls;
using screenerWpf.Helpers;
using screenerWpf.Controls;

namespace screenerWpf.Sevices
{
    public class TextRecognitionHandler
    {
        private DrawableCanvas canvas;

        public TextRecognitionHandler(DrawableCanvas canvas)
        {
            this.canvas = canvas;
        }

        internal void StartRecognizeFromImage()
        {
            BitmapSource canvasBitmap = CanvasBitmapGenerator.GetCanvasBitmap(canvas);
            if (canvasBitmap != null)
            {
                string recognizedText = RecognizeTextFromImage(canvasBitmap);
                Clipboard.SetText(recognizedText);
            }
        }


        public string RecognizeTextFromImage(BitmapSource bitmapSource)
        {
            // Konwersja BitmapSource do Bitmap
            Bitmap bitmap;
            using (var outStream = new System.IO.MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapSource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }

            // Użycie Tesseract do rozpoznania tekstu
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = PixConverter.ToPix(bitmap))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }
    }
}