using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageCrop;

public class ImageSharpTest
{
    public static void Test()
    {
        var outputPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        using var img1 = Image.Load<Rgba32>("C:\\Users\\33913\\Desktop\\1.jpg");
        using var img2 = Image.Load<Rgba32>("C:\\Users\\33913\\Desktop\\2.jpg");

        // 1. 裁切各种比例（以原图最大尺寸输出）
        var square = img1.CropToRatio("1:1");
        square.Save(Path.Combine(outputPath, "裁切_1比1.jpg"));
        var portrait = img1.CropToRatio("2:3");     // 2:3 竖图
        portrait.Save(Path.Combine(outputPath, "裁切_2比3.jpg"));
        var landscape = img1.CropToRatio("3:2");     // 3:2 横图
        landscape.Save(Path.Combine(outputPath, "裁切_3比2.jpg"));
        var landscape9x16 = img1.CropToRatio("9:16");
        landscape9x16.Save(Path.Combine(outputPath, "裁切_9比16.jpg"));
        var widescreen = img1.CropToRatio("16:9");
        widescreen.Save(Path.Combine(outputPath, "裁切_16比9.jpg"));

        // 想要固定宽度 600px 的 16:9 图：
        var fixed169 = img1.CropToRatio("1:1", outputWidth: 128);
        fixed169.Save(Path.Combine(outputPath, "固定128_1比1.jpg"));

        // 2. 左右拼接两张 1:1 图（自动转正方形，边长取较大值）
        using var concat2 = img1.ConcatLeftRightAutoHeight(img2);
        concat2.Save(Path.Combine(outputPath, "拼接_1比1.jpg"));
    }
}