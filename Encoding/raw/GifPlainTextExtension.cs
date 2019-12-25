namespace Orikivo.Drawing
{
    public class GifPlainTextExtension
    {
        byte Introducer;
        byte Label; // 01 == Plain Text Extension
        byte BlockSize;
        ushort LeftTextGrid; // x pos
        ushort TopTextGrid; // y pos
        ushort TextGridWidth;
        ushort TextGridHeight;
        byte CellWidth;
        byte CellHeight;
        byte TextForegroundColorIndex;
        byte TextBackgroundColorIndex;
        GifSubBlock[] PlainTextData; // infinite
        byte Terminator = 0x00;
    }
}
