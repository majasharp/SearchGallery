
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

public static class ThumbnailResizer
{
    private const float imageResolution = 72;
    private const long compressionLevel = 80L;


    public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
    {
        int newWidth;
        int newHeight;

        // first check if the image needs rotating
        foreach (var prop in image.PropertyItems)
        {
            if (prop.Id != 0x0112) continue;

            int orientationValue = image.GetPropertyItem(prop.Id).Value[0];
            var rotateFlipType = getRotateFlipType(orientationValue);
            image.RotateFlip(rotateFlipType);
            break;
        }

        // check if the with or height of the image exceeds the maximum specified, if so calculate the new dimensions
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

        // start the resize with a new image
        Bitmap newImage = new Bitmap(newWidth, newHeight);

        // set the new resolution
        newImage.SetResolution(imageResolution, imageResolution);

        // start the resizing
        using (var graphics = Graphics.FromImage(newImage))
        {
            // set some encoding specs
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        // save the image to a memorystream to apply the compression level
        using (var ms = new MemoryStream())
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compressionLevel);

            newImage.Save(ms, getEncoderInfo("image/jpeg"), encoderParameters);

            // save the image as byte array here if you want the return type to be a Byte Array instead of Image
            // byte[] imageAsByteArray = ms.ToArray();
        }

        // return the image
        return newImage;
    }


    // image padding
    public static Image applyPaddingToImage(Image image, Color backColor)
    {
        //get the maximum size of the image dimensions
        var maxSize = Math.Max(image.Height, image.Width);
        var squareSize = new Size(maxSize, maxSize);

        //create a new square image
        var squareImage = new Bitmap(squareSize.Width, squareSize.Height);
        using var graphics = Graphics.FromImage(squareImage);

        //fill the new square with a color
        graphics.FillRectangle(new SolidBrush(backColor), 0, 0, squareSize.Width, squareSize.Height);

        //put the original image on top of the new square
        graphics.DrawImage(image, (squareSize.Width / 2) - (image.Width / 2), (squareSize.Height / 2) - (image.Height / 2), image.Width, image.Height);

        //return the image
        return squareImage;
    }


    // get encoder info
    private static ImageCodecInfo getEncoderInfo(string mimeType)
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


    // determine image rotation
    private static RotateFlipType getRotateFlipType(int rotateValue)
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


    // convert image to base64
    public static string convertImageToBase64(Image image)
    {
        using MemoryStream ms = new MemoryStream();
        //convert the image to byte array
        image.Save(ms, ImageFormat.Jpeg);
        byte[] bin = ms.ToArray();

        //convert byte array to base64 string
        return Convert.ToBase64String(bin);
    }
}