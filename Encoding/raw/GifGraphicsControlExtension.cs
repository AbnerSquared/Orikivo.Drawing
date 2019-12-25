namespace Orikivo.Drawing
{
    public enum GifDisposalMethod : byte
    {
        Unspecified = 0x00,
        DoNotDispose = 0x01,
        OverrideWithForegroundColor = 0x02,
        OverrideWithPrevious = 0x04
    }

    public class GifGraphicsControlExtension
    {
        byte Introducer;
        byte Label; // F9 == Graphics Control Extension
        byte BlockSize;
        byte Packed;
        // 01234567
        // 7 = Transparent Color Flag
        // 1 if the color index contains a color transparency index
        // 6 = User Input Flag
        // 1 if user input is expected before contining
        // 543 = Disposal Method
        // 00 => Disposal method not specified
        // 01 => Do not dispose of graphics
        // 02 => Override graphics with bg color
        // 04 => override graphics with previous graphics

        // 210 = Reserved
        // unused in 89a, set to 0

        ushort DelayTime;
        // if 0, no delay is used
        // byte DelayHighByte
        // byte DelayLowByte

        byte ColorIndex;
        // color transparency index
        // ONLY if Transparent color Flag subfield is set to 1.

        byte Terminator = 0x00;
    }
}
