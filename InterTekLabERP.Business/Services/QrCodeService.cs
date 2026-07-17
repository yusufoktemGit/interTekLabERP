using QRCoder;

namespace interTekLabERP.Business.Services;

public class QrCodeService : IQrCodeService
{
    public string Generate(string text)
    {
        using var qrGenerator = new QRCodeGenerator();

        using var qrData = qrGenerator.CreateQrCode(
            text,
            QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrData);

        byte[] qrBytes = qrCode.GetGraphic(20);

        string folder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "qrcodes");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string fileName = $"{text}.png";

        string fullPath = Path.Combine(folder, fileName);

        File.WriteAllBytes(fullPath, qrBytes);

        return $"/qrcodes/{fileName}";
    }
}