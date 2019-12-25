namespace Orikivo.Drawing
{
    // varies from 5 to 259 bytes in length.
    public class GifCommentExtension
    {
        byte Introducer;
        byte Label; // FE == Identity of Comment Extension
        GifSubBlock CommentData;
        byte Terminator = 0x00;
    }
}
