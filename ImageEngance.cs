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

            // Apply a sharpening filter
            var kernel = new float[]
            {
                -1, -1, -1,
                -1, 9, -1,
                -1, -1, -1
            };

            using var sharpenPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateMatrixConvolution(new SKSizeI(3, 3), kernel, 1.0f, 0.0f, new SKPointI(1, 1), SKMatrixConvolutionTileMode.Clamp, true)
            };
            canvas.DrawBitmap(resized, 0, 0, sharpenPaint);

            canvas.Flush();

            using var outputStream = File.OpenWrite(outputPath);
            resized.Encode(outputStream, SKEncodedImageFormat.Jpeg, 100);
        }
    }
}
