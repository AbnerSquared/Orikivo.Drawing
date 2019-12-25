namespace Orikivo.Drawing
{
    public class GifImageBlock
    {
        byte Separator = (byte)',';
        GifImageDescriptor LocalImageDescriptor { get; }
        GifColorTable[] LocalColorTable { get; } // 256 colors max
        byte[] ImageData { get; }
    }
}
