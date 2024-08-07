using System;
using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;

class Program
{
    static void Main()
    {
        // Load and preprocess the image
        string imagePath = "path_to_your_image.jpg";
        Bitmap bitmap = new Bitmap(imagePath);
        
        // Convert to grayscale
        Bitmap grayBitmap = ConvertToGrayscale(bitmap);
        
        // Optionally, perform other preprocessing steps like thresholding, resizing, etc.
        
        // Save the preprocessed image (for verification)
        grayBitmap.Save("preprocessed_image.png", ImageFormat.Png);
        
        // Perform OCR using Tesseract
        string tessDataPath = @"path_to_tessdata"; // Path to tessdata directory
        var ocrEngine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
        var img = PixConverter.ToPix(grayBitmap);
        var result = ocrEngine.Process(img);
        
        // Output the recognized text
        Console.WriteLine(result.GetText());
    }

    static Bitmap ConvertToGrayscale(Bitmap original)
    {
        // Create a blank bitmap the same size as the original
        Bitmap newBitmap = new Bitmap(original.Width, original.Height);
        
        // Get a graphics object from the new image
        Graphics g = Graphics.FromImage(newBitmap);
        
        // Create the grayscale ColorMatrix
        ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });
        
        // Create some image attributes
        ImageAttributes attributes = new ImageAttributes();
        
        // Set the color matrix attribute
        attributes.SetColorMatrix(colorMatrix);
        
        // Draw the original image on the new image
        // using the grayscale color matrix
        g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
            0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
        
        // Dispose the Graphics object
        g.Dispose();
        
        return newBitmap;
    }
}
