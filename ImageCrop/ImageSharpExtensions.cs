using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

public static class ImageSharpExtensions
{
    // 1. 裁切到指定比例（支持 1:1、2:3、3:2、9:16、16:9 等）
    public static Image CropToRatio(this Image image, string ratio, int? outputWidth = null)
    {
        if (!TryParseRatio(ratio, out float targetRatio))
            throw new ArgumentException("不支持的比例格式，支持如 1:1、16:9 等");

        return image.Clone(ctx =>
        {
            Size size = ctx.GetCurrentSize();

            int cropWidth, cropHeight;
            if (targetRatio < (float)size.Width / size.Height)
            {
                cropHeight = size.Height;
                cropWidth = (int)(size.Height * targetRatio);
            }
            else
            {
                cropWidth = size.Width;
                cropHeight = (int)(size.Width / targetRatio);
            }

            int x = (size.Width - cropWidth) / 2;
            int y = (size.Height - cropHeight) / 2;

            ctx.Crop(new Rectangle(x, y, cropWidth, cropHeight));

            if (outputWidth.HasValue)
            {
                int outputHeight = (int)(outputWidth.Value / targetRatio);
                ctx.Resize(outputWidth.Value, outputHeight);
            }
        });
    }

    private static bool TryParseRatio(string ratio, out float targetRatio)
    {
        targetRatio = 1f;
        var parts = ratio.Trim().Split(':');
        if (parts.Length != 2) return false;
        if (!float.TryParse(parts[0], out float w) || !float.TryParse(parts[1], out float h) || h == 0)
            return false;
        targetRatio = w / h;
        return true;
    }

    // 修改拼接函数，传入的图片定高，按中心先才成1:2，再把两张1:2的图拼成1：1
    public static Image ConcatLeftRightAutoHeight(this Image img1, Image img2)
    {
        int targetHeight = Math.Max(img1.Height, img2.Height);
        var croppedImg1 = img1.CropToRatio("1:2", outputWidth: targetHeight / 2);
        var croppedImg2 = img2.CropToRatio("1:2", outputWidth: targetHeight / 2);
        int totalWidth = croppedImg1.Width + croppedImg2.Width;
        var resultImage = new Image<Rgba32>(totalWidth, targetHeight);
        resultImage.Mutate(ctx =>
        {
            ctx.DrawImage(croppedImg1, new Point(0, 0), 1f);
            ctx.DrawImage(croppedImg2, new Point(croppedImg1.Width, 0), 1f);
        });
        croppedImg1.Dispose();
        croppedImg2.Dispose();
        return resultImage;
    }
}