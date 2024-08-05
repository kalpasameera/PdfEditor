using System;
using System.IO;
using SkiaSharp;

namespace ImageQualityEnhancer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ImageQualityEnhancer <inputImagePath> <outputImagePath>");
                return;
            }

            string inputImagePath = args[0];
            string outputImagePath = args[1];

            EnhanceImageQuality(inputImagePath, outputImagePath);
        }

        static void EnhanceImageQuality(string inputPath, string outputPath)
        {
            using var inputStream = File.OpenRead(inputPath);
            using var original = SKBitmap.Decode(inputStream);

            // Set the desired output size
            int newWidth = original.Width * 2;
            int newHeight = original.Height * 2;

            using var resized = new SKBitmap(newWidth, newHeight);
            using var canvas = new SKCanvas(resized);
            canvas.DrawBitmap(original, new SKRect(0, 0, newWidth, newHeight));

            // Apply a simple blur to smooth out the pixels
            using var blurPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateBlur(1.5f, 1.5f)
            };
            canvas.DrawBitmap(resized, 0, 0, blurPaint);

            // Apply a sharpening filter
            using var sharpenPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateHighContrast(true, SKHighContrastConfigInvertStyle.NoInvert, 0.5f)
            };
            canvas.DrawBitmap(resized, 0, 0, sharpenPaint);

            canvas.Flush();

            using var outputStream = File.OpenWrite(outputPath);
            resized.Encode(outputStream, SKEncodedImageFormat.Jpeg, 100);
        }
    }
}
