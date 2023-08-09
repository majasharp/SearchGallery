using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public static class ThumbnailResizer
{
    private const float imageResolution = 72;
    private const long compressionLevel = 80L;

    public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
    {
        int newWidth;
        int newHeight;

        foreach (var property in image.PropertyItems)
        {
            if (property.Id != 0x0112) continue;

            int orientationValue = image.GetPropertyItem(property.Id).Value[0];
            var rotateFlipType = GetRotateFlipType(orientationValue);
            image.RotateFlip(rotateFlipType);
            break;
        }

        if (image.Width > maxWidth || image.Height > maxHeight)
        {
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            newWidth = (int)(image.Width * ratio);
            newHeight = (int)(image.Height * ratio);
        }
        else
        {
            newWidth = image.Width;
            newHeight = image.Height;
        }

        Bitmap newImage = new Bitmap(newWidth, newHeight);
        newImage.SetResolution(imageResolution, imageResolution);

        using (var graphics = Graphics.FromImage(newImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        using (var ms = new MemoryStream())
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compressionLevel);

            newImage.Save(ms, GetEncoderInfo("image/jpeg"), encoderParameters);
        }

        return newImage;
    }

    public static Image ApplyPaddingToImage(Image image, Color backColor)
    {
        var maxSize = Math.Max(image.Height, image.Width);
        var squareSize = new Size(maxSize, maxSize);

        var squareImage = new Bitmap(squareSize.Width, squareSize.Height);
        using var graphics = Graphics.FromImage(squareImage);

        graphics.FillRectangle(new SolidBrush(backColor), 0, 0, squareSize.Width, squareSize.Height);
        graphics.DrawImage(image, (squareSize.Width / 2) - (image.Width / 2), (squareSize.Height / 2) - (image.Height / 2), image.Width, image.Height);

        return squareImage;
    }

    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        var encoders = ImageCodecInfo.GetImageEncoders();

        for (int j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType.ToLower() == mimeType.ToLower())
            {
                return encoders[j];
            }
        }

        return null;
    }

    private static RotateFlipType GetRotateFlipType(int rotateValue)
    {
        RotateFlipType flipType = RotateFlipType.RotateNoneFlipNone;

        switch (rotateValue)
        {
            case 1:
                flipType = RotateFlipType.RotateNoneFlipNone;
                break;
            case 2:
                flipType = RotateFlipType.RotateNoneFlipX;
                break;
            case 3:
                flipType = RotateFlipType.Rotate180FlipNone;
                break;
            case 4:
                flipType = RotateFlipType.Rotate180FlipX;
                break;
            case 5:
                flipType = RotateFlipType.Rotate90FlipX;
                break;
            case 6:
                flipType = RotateFlipType.Rotate90FlipNone;
                break;
            case 7:
                flipType = RotateFlipType.Rotate270FlipX;
                break;
            case 8:
                flipType = RotateFlipType.Rotate270FlipNone;
                break;
            default:
                flipType = RotateFlipType.RotateNoneFlipNone;
                break;
        }

        return flipType;
    }

    public static string ConvertImageToBase64(Image image)
    {
        using MemoryStream ms = new MemoryStream();
        image.Save(ms, ImageFormat.Jpeg);
        byte[] bin = ms.ToArray();

        return Convert.ToBase64String(bin);
    }
}