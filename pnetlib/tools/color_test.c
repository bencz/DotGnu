
/*

This is a color test program that prints out some interesting information
about the color allocation policy and XImage format used by an X server.
We need to know this information to optimize the image handling in
XsharpSupport.c for particular pixel formats.

To compile, use a command-line such as the following:

  gcc -o color_test color_test.c -I/usr/X11R6/include -L/usr/X11R6/lib -lX11

Then run "./color_test" and send the output to "rweather@zip.com.au".
You should probably also send the output of the "xdpyinfo" command.

You must have your "DISPLAY" environment variable set correctly before
running the program.

*/

#include <stdio.h>
#include <stdlib.h>
#include <X11/Xlib.h>
#include <X11/Xutil.h>

int main(int argc, char *argv[])
{
	Display *dpy = XOpenDisplay(NULL);
	Screen *screen = DefaultScreenOfDisplay(dpy);
	Visual *visual = DefaultVisualOfScreen(screen);
	Colormap colormap = DefaultColormapOfScreen(screen);
	unsigned int depth = DefaultDepthOfScreen(screen);
	int format, posn;
	int bitmap_pad;
	XImage *image;
	unsigned char *imageData;
	static char *orders[] = {"LSBFirst", "MSBFirst"};
	static char *types[] = {"StaticGray", "GrayScale", "StaticColor",
							"PseudoColor", "TrueColor", "DirectColor"};
	int clientOrder;
	union
	{
		short volatile value;
		unsigned char volatile buf[2];
	} un;
	XColor color;

	/* Determine the client byte order */
	un.value = 0x0102;
	clientOrder = ((un.buf[0] == 0x02) ? LSBFirst : MSBFirst);

	/* Print interesting information about the display and visual */
	printf("depth             = %d\n", (int)depth);
	printf("class             = %s\n", types[visual->class]);
	printf("bitmap_unit       = %d\n", BitmapUnit(dpy));
	printf("bitmap_pad        = %d\n", BitmapPad(dpy));
	printf("bit_order         = %s\n", orders[BitmapBitOrder(dpy)]);
	printf("image_byte_order  = %s\n", orders[ImageByteOrder(dpy)]);
	printf("client_byte_order = %s\n", orders[clientOrder]);
	printf("red_mask          = 0x%08lX\n", visual->red_mask);
	printf("green_mask        = 0x%08lX\n", visual->green_mask);
	printf("blue_mask         = 0x%08lX\n", visual->blue_mask);
	printf("bits_per_rgb      = %d\n", visual->bits_per_rgb);

	/* Get the depth, format, and pad values */
	format = ZPixmap;
	if(depth <= 8)
	{
		bitmap_pad = 8;
	}
	else if(depth <= 16)
	{
		bitmap_pad = 16;
	}
	else
	{
		bitmap_pad = 32;
	}

	/* Create the main image structure */
	image = XCreateImage(dpy, visual, depth, format, 0, 0,
						 (unsigned int)2, (unsigned int)1,
						 bitmap_pad, 0);
	if(!image)
	{
		return 0;
	}
	imageData = (unsigned char *)malloc(image->bytes_per_line *
										image->height);
	if(!imageData)
	{
		XDestroyImage(image);
		return 0;
	}
	image->data = (char *)imageData;

	/* Allocate a specific color */
	color.flags = DoRed | DoGreen | DoBlue;
	color.red = 0xFF00;
	color.green = 0xAA00;
	color.blue = 0x3300;
	color.pad = 0;
	color.pixel = 0;
	XAllocColor(dpy, colormap, &color);
	printf("alloc(#FFAA33)    = 0x%08lX\n", color.pixel);

	/* Write the pixel to the image */
	XPutPixel(image, 0, 0, color.pixel);
	XPutPixel(image, 1, 0, color.pixel);

	/* Print the bytes that resulted */
	printf("image_bytes       =");
	for(posn = 0; posn < (int)(image->bytes_per_line); ++posn)
	{
		printf(" %02X", (int)(imageData[posn]));
	}
	printf("\n");

	/* Close the display and exit */
	XCloseDisplay(dpy);
	return 0;
}
