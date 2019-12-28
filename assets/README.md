# Assets
Orikivo.Drawing utilizes FontFace to draw pixelated strings using spritesheets as references.
As of now, there is not an easier way (that I know of) to keep required font faces within the namespace.
When using FontFace, make sure to place a copy of this folder within your project's build directory.

## Upcoming
There will soon be a file format made for the FontFace called .OCF (Orikivo Compact Font)
This will be used to store an indexed sprite with custom X, Y, Width, and Height info for any sprite.
This also includes the ability to easily map multiple characters to one sprite.
