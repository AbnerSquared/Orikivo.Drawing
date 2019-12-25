namespace Orikivo.Drawing
{
    /*
        GIF87a Layout
        - Header and Color Table Information
            - Header
            - Logical Screen Descriptor
            - Global Color Table
        - Image 1
            - Local Image Descriptor
            - Local Color Table
            - Image Data
        - Trailer
     
        
        GIF89a Layout
        - Header and Color Table Information
            - Header
            - Logical Screen Descriptor
            - Global Color Table
        - Extension Information
            - Comment Extension
            - Application Extension
            - Graphic Control Extension
        - Image 1
            - Local Image Descriptor
            - Local Color Table
            - Image Data
        - Extension Information
            - Comment Extension
            - Plain Text Extension
        - Trailer     
     */

    public class GifPtr
    {
        GifHeader Header { get; set; }
        GifScreenDescriptor LogicalScreenDescriptor { get; set; }
        GifImageBlock[] Images { get; set; }

        byte Trailer;
    }
}
