using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using ZXing.SkiaSharp.Rendering;

namespace interTekLabERP.Business.Services;

public class BarcodeService : IBarcodeService
{
    public string Generate(string text)
    {
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = BarcodeFormat.CODE_128,

            Options = new EncodingOptions
            {
                Width = 600,
                Height = 160,
                Margin = 10
            },

            Renderer = new SKBitmapRenderer()
        };

        SKBitmap bitmap = writer.Write(text);

        string folder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "barcodes");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string fileName = $"{text}.png";

        string fullPath = Path.Combine(folder, fileName);

        using var image = SKImage.FromBitmap(bitmap);

        using var data = image.Encode(
            SKEncodedImageFormat.Png,
            100);

        using var stream = File.OpenWrite(fullPath);

        data.SaveTo(stream);

        return $"/barcodes/{fileName}";
    }
}