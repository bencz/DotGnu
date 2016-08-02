/*
 * BmpResizer.cs - Implementation of the "DotGNU.Images.BmpResizer" class.
 *
 * Copyright (C) 2003 Neil Cawse.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace DotGNU.Images
{
	using System;

	internal sealed class BmpResizer
	{
		public static Frame Resize(Frame originalFrame, int x, int y, int width, int height, int destWidth, int destHeight)
				{
					Frame newFrame = originalFrame.CloneFrameEmpty(destWidth, destHeight, originalFrame.pixelFormat);
					newFrame.data = new byte[newFrame.Height * newFrame.Stride];
					if (originalFrame.Palette != null)
					{
						newFrame.Palette = (int[])originalFrame.Palette.Clone();
					}
					if (destWidth <= width && destHeight <= height)
					{
						ShrinkBmp(originalFrame, newFrame, x, y, width, height);
					}
					else if (destWidth>= width && destHeight >= height)
					{
						ExpandBmp(originalFrame, newFrame, x, y, width, height);
					}
					else if (destWidth < width)
					{
						//TODO: Currently this is a two pass operation.
						// A temporary frame to hold the partially resized data
						Frame f = new Frame(null, destWidth, height, originalFrame.pixelFormat);
						ShrinkBmp(originalFrame, f, x, y, width, height);
						ExpandBmp(f, newFrame, 0, 0, destWidth, height);
					}
					else if (destHeight < height)
					{
						//TODO: currently this is a two pass operation.
						// A temporary frame to hold the partially resized data
						Frame f = new Frame(null, width, destHeight, originalFrame.pixelFormat);
						ShrinkBmp(originalFrame, f, x, y, width, height);
						ExpandBmp(f, newFrame, 0, 0, width, destHeight);
					}
					return newFrame;
				}

		// This is an integer, generic, low quality algorithm for expanding bitmaps.
		// A significant speed improvement could be optained by splitting out 1bpp & 4 bpp
		// Eliminating the many if's.
		private static void ExpandBmp(Frame oldFrame, Frame newFrame, int oldX, int oldY, int oldWidth, int oldHeight)
				{
					byte[] data = oldFrame.Data;
					byte[] dataOut = newFrame.Data;
					int sumY = 0;
					int lineStartNew = 0;

					int bytesPerPixel = Utils.FormatToBitCount(oldFrame.pixelFormat) / 8;
					int lineStartBit = 0;
					bool lineStartNibble = false;
					// Offset to the oldY.
					int lineStartOld = oldY * oldFrame.Stride;
					if (oldFrame.pixelFormat == PixelFormat.Format1bppIndexed)
					{
						lineStartOld = oldX / 8;
						lineStartBit = 7 - (oldX & 0x07);
					}
					else if (oldFrame.pixelFormat == PixelFormat.Format4bppIndexed)
					{
						lineStartOld = oldX / 2;
						lineStartNibble = ((oldX & 0x01) == 0);
					}
					else
					{
						lineStartOld =bytesPerPixel * oldX;
					}

					int y = 0;
					while(y < oldHeight)
					{
						int newPixel = lineStartNew;
						int newPixelBit = 7;
						bool newPixelNibble = true;
						int newPixelByte = 0;

						lineStartNew += newFrame.Stride;
						int oldPixelBit = lineStartBit;
						int oldPixel = lineStartOld;
						int oldPixelByte = -1;
						bool oldPixelNibble = lineStartNibble;
						int pix = 0;

						int x = 0;
						int sumX = 0;
						while(x < oldWidth)
						{
							sumX += oldWidth;
							// Write the pixel.
							// 1bpp format.
							if (oldFrame.pixelFormat == PixelFormat.Format1bppIndexed)
							{
								if (oldPixelByte == -1)
									oldPixelByte = data[oldPixel];
								if ( (oldPixelByte & 1<<oldPixelBit) != 0)
									newPixelByte |= 1<<newPixelBit;
								if (newPixelBit == 0)
								{
									dataOut[newPixel++] = (byte)(newPixelByte);
									newPixelBit = 7;
									newPixelByte = 0;
								}
								else
									newPixelBit--;
								if(sumX >= newFrame.Width)
								{
									x++;
									// Get the next nibble
									if (oldPixelBit==0)
									{
										oldPixelByte = -1;
										oldPixel++;
										oldPixelBit = 7;
									}
									else
										oldPixelBit--;
									sumX -= newFrame.Width;
								}
							}
							// 4bpp format.
							else if (oldFrame.pixelFormat == PixelFormat.Format4bppIndexed)
							{
								if (oldPixelByte == -1)
									oldPixelByte = data[oldPixel];
								if (oldPixelNibble)
									pix = oldPixelByte >> 4;
								else
									pix = oldPixelByte & 0x0F;
								
								if (newPixelNibble)
									newPixelByte = pix << 4;
								else
									dataOut[newPixel++] = (byte)(newPixelByte | pix);
								newPixelNibble = !newPixelNibble;
								if(sumX >= newFrame.Width)
								{
									x++;
									// Get the next nibble
									if (!oldPixelNibble)
									{
										oldPixelByte = -1;
										oldPixel++;
									}
									oldPixelNibble = !oldPixelNibble;
									sumX -= newFrame.Width;
								}
							}
							// All other formats.
							else
							{
								for(int i = 0; i < bytesPerPixel; i++, newPixel++)
									dataOut[newPixel] = data[oldPixel + i];
								if(sumX >= newFrame.Width)
								{
									x++;
									oldPixel += bytesPerPixel;
									sumX -= newFrame.Width;
								}
							}
						}
						// There maybe some bits left we need to write
						if (oldFrame.pixelFormat == PixelFormat.Format1bppIndexed && newPixelBit != 7)
							dataOut[newPixel++] = (byte)(newPixelByte);
						if (oldFrame.pixelFormat == PixelFormat.Format4bppIndexed && !newPixelNibble)
							dataOut[newPixel++] = (byte)(newPixelByte);
						sumY += oldHeight;
						if(sumY >= newFrame.Height)
						{
							y++;
							lineStartOld += oldFrame.Stride;
							oldPixel = lineStartOld;
							sumY -= newFrame.Height;
						}
					}
				} 

		// This is an integer, generic high quality algorithm for shrinking bitmaps.
		// A significant speed improvement could be optained by splitting out the formats
		// Eliminating the many if's.
		private static void ShrinkBmp(Frame oldFrame, Frame newFrame, int oldX, int oldY, int oldWidth, int oldHeight)
				{
					byte[] data = oldFrame.Data;
					byte[] dataOut = newFrame.Data;
					int[] palette = oldFrame.Palette;
					
					int lineStart = 0;
					int lineStartBit = 0;
					bool lineStartNibble = false;
					// Calculate the right line start based on the oldX
					if (oldFrame.pixelFormat == PixelFormat.Format1bppIndexed)
					{
						lineStart = oldX / 8;
						lineStartBit = 7 - (oldX & 0x07);
					}
					else if (oldFrame.pixelFormat == PixelFormat.Format4bppIndexed)
					{
						lineStart = oldX / 2;
						lineStartNibble = ((oldX & 0x01) == 0);
					}
					else
					{
						lineStart =Utils.FormatToBitCount(oldFrame.pixelFormat) / 8 * oldX;
					}

					// Offset to the right place based on oldY.
					lineStart += oldY * oldFrame.Stride;

					int lineStartOut = 0;
					int[] rowCoefficients = CreateCoefficients(oldWidth,  newFrame.Width);
					int[] columnCoefficients = CreateCoefficients(oldHeight,  newFrame.Height);
					byte pixelByte1 = 0;
					byte pixelByte2 = 0;
					byte pixelByte3 = 0;
					byte pixelAlpha = 255;
					byte byteData = 0;
					// Index for 1bpp format.
					int bit = lineStartBit;
					// Preread the byte if we have to.
					if (oldFrame.pixelFormat == PixelFormat.Format1bppIndexed && lineStartBit > 0)
						byteData = data[lineStart];
					// Index for 4bpp format.
					bool highNibble = lineStartNibble;
					// Preread the byte if we have to.
					if (oldFrame.pixelFormat == PixelFormat.Format4bppIndexed && !highNibble)
					{
						byteData = data[lineStart];
					}
					int bufWidth = 4;
					if (oldFrame.pixelFormat == PixelFormat.Format1bppIndexed)
					{
						bufWidth = 1;
					}
					int bufLine = bufWidth * newFrame.Width * 4;
					int bufNextLine =  bufWidth * newFrame.Width;
					uint[] buffer = new uint[bufWidth * 2 * newFrame.Width];
					int currentLine = 0;
					uint temp;

					int currentYCoeff = 0;
					int y = 0;
					while(y < newFrame.Height)
					{
						int currentPixel = lineStart;
						lineStart += oldFrame.Stride;

						int bufCurrentPixel = currentLine;
						int bufNextPixel = bufNextLine;

						int currentXCoeff = 0;
						int yCoefficient1 = columnCoefficients[currentYCoeff + 1];
						bool crossRow = yCoefficient1 > 0;
						int x = 0;
						while(x < newFrame.Width)
						{
							int yCoefficient = columnCoefficients[currentYCoeff];
							int xCoefficient = rowCoefficients[currentXCoeff];
						
							temp = (uint)(xCoefficient * yCoefficient);
							// Read the color from the particular format.
							// 1 bpp Format.
							if (oldFrame.pixelFormat==PixelFormat.Format1bppIndexed)
							{
								if (bit == 0)
								{
									byteData = data[currentPixel++];
									bit = 128;
								}
								if ((byteData & bit) > 0)
								{
									pixelByte1 = 255;
								}
								else
								{
									pixelByte1 = 0;
								}
								bit = (byte)(bit >>1);
								buffer[bufCurrentPixel] += temp * pixelByte1;
							}
							else
							{
								// 32 bpp Format.
								if (oldFrame.pixelFormat==PixelFormat.Format32bppArgb)
								{
									pixelByte1 = data[currentPixel++];
									pixelByte2 = data[currentPixel++];
									pixelByte3 = data[currentPixel++];
									pixelAlpha = data[currentPixel++];
								}
								// 24 bpp Format.
								else if (oldFrame.pixelFormat==PixelFormat.Format24bppRgb)
								{
									pixelByte1 = data[currentPixel++];
									pixelByte2 = data[currentPixel++];
									pixelByte3 = data[currentPixel++];
								}
								// 16 bpp 555 Format.
								else if (oldFrame.pixelFormat==PixelFormat.Format16bppRgb555)
								{
									pixelByte2 = data[currentPixel++];
									pixelByte1 = data[currentPixel++];
									pixelByte3 = (byte)(pixelByte2 & 0x1F);
									pixelByte2 = (byte)(pixelByte1 << 3 & 0x18  | pixelByte2 >> 5 & 0x07);
									pixelByte1 = (byte)(pixelByte1 >> 2 & 0x1f);
									pixelByte1 = (byte)((int)pixelByte1 * 255 / 31);
									pixelByte2 = (byte)((int)pixelByte2 * 255 / 31);
									pixelByte3 = (byte)((int)pixelByte3 * 255 / 31);
								}
								// 16 bpp 565 Format.
								else if (oldFrame.pixelFormat==PixelFormat.Format16bppRgb565)
								{
									pixelByte2 = data[currentPixel++];
									pixelByte1 = data[currentPixel++];
									pixelByte3 = (byte)(pixelByte2 & 0x1F);
									pixelByte2 = (byte)(pixelByte1 << 3 & 0x38  | pixelByte2 >> 5 & 0x07);
									pixelByte1 = (byte)(pixelByte1 >> 3);
									pixelByte1 = (byte)((int)pixelByte1 * 255 / 31);
									pixelByte2 = (byte)((int)pixelByte2 * 255 / 63);
									pixelByte3 = (byte)((int)pixelByte3 * 255 / 31);
								}
								// 8 bpp Format.
								else if (oldFrame.pixelFormat==PixelFormat.Format8bppIndexed)
								{
									int paletteColor = palette[data[currentPixel++]];
									pixelByte1 = (byte)(paletteColor>>16);
									pixelByte2 = (byte)(paletteColor>>8);
									pixelByte3 = (byte)paletteColor;
								}
								// 4 bpp Format.
								else if (oldFrame.pixelFormat==PixelFormat.Format4bppIndexed)
								{
									int paletteColor;
									if (highNibble)
									{
										byteData =  data[currentPixel++];
										paletteColor = palette[byteData >>4];
									}
									else
									{
										paletteColor = palette[byteData & 0x0F];
									}
									highNibble = !highNibble;
									pixelByte1 = (byte)(paletteColor>>16);
									pixelByte2 = (byte)(paletteColor>>8);
									pixelByte3 = (byte)paletteColor;
								}
								buffer[bufCurrentPixel] += temp * pixelByte1;
								buffer[bufCurrentPixel+1] += temp * pixelByte2;
								buffer[bufCurrentPixel+2] += temp * pixelByte3;
								buffer[bufCurrentPixel+3] += temp * pixelAlpha;
							}
							int xCoefficient1 = rowCoefficients[currentXCoeff + 1];
							bool crossColumn =  xCoefficient1> 0;
							if(crossColumn)
							{
								temp = (uint)(xCoefficient1 * yCoefficient);
								if (oldFrame.pixelFormat==PixelFormat.Format1bppIndexed)
									buffer[bufCurrentPixel + 1] += temp * pixelByte1;
								else
								{
									buffer[bufCurrentPixel + 4] += temp * pixelByte1;
									buffer[bufCurrentPixel + 5] += temp * pixelByte2;
									buffer[bufCurrentPixel + 6] += temp * pixelByte3;
									buffer[bufCurrentPixel + 7] += temp * pixelAlpha;
								}
							}
							if(crossRow)
							{
								temp = (uint)(xCoefficient * yCoefficient1);
								if (oldFrame.pixelFormat==PixelFormat.Format1bppIndexed)
										buffer[bufNextPixel] += temp * pixelByte1;
								else
								{
									buffer[bufNextPixel] += temp * pixelByte1;
									buffer[bufNextPixel + 1] += temp * pixelByte2;
									buffer[bufNextPixel + 2] += temp * pixelByte3;
									buffer[bufNextPixel + 3] += temp * pixelAlpha;
								}
								if(crossColumn)
								{
									temp = (uint)(xCoefficient1 * yCoefficient1);
									if (oldFrame.pixelFormat==PixelFormat.Format1bppIndexed)
										buffer[bufNextPixel + 1] += temp * pixelByte1;
									else
									{
										buffer[bufNextPixel + 4] += temp * pixelByte1;
										buffer[bufNextPixel + 5] += temp * pixelByte2;
										buffer[bufNextPixel + 6] += temp * pixelByte3;
										buffer[bufNextPixel + 7] += temp * pixelAlpha;
									}
								}
							}
							if(xCoefficient1 != 0)
							{
								x++;
								bufCurrentPixel += bufWidth;
								bufNextPixel += bufWidth;
							}
							currentXCoeff += 2;
						}
						if(yCoefficient1 != 0)
						{
							// set result line
							bufCurrentPixel = currentLine;
							currentPixel = lineStartOut;
							int endWriteBuffer = bufCurrentPixel + bufWidth * newFrame.Width;
							// Write the buffer.
							// 1 bpp format.
							if (oldFrame.pixelFormat==PixelFormat.Format1bppIndexed)
							{
								byte bit1 = 128;
								byte dataByte1 = 0;
								for(;bufCurrentPixel < endWriteBuffer; bufCurrentPixel++)
								{
									if (buffer[bufCurrentPixel] != 0)
									{
										dataByte1 |= bit1;
									}
									bit1 =(byte)(bit1 >> 1);
									if (bit1 == 0)
									{
										bit1 = 128;
										dataOut[currentPixel++] = dataByte1;
										dataByte1 = 0;
									}
								}
								// Write the last bits
								if (bit != 128)
								{
									dataOut[currentPixel] = dataByte1;
								}
							}
							// 32 bpp format.
							else if (oldFrame.pixelFormat==PixelFormat.Format32bppArgb)
							{
								for(; bufCurrentPixel < endWriteBuffer; bufCurrentPixel++)
								{
									dataOut[currentPixel++] = (byte)(buffer[bufCurrentPixel]>> 24);
								}
							}
							// 24 bpp format.
							else if (oldFrame.pixelFormat==PixelFormat.Format24bppRgb)
							{
								while( bufCurrentPixel < endWriteBuffer)
								{
									dataOut[currentPixel++] = (byte)(buffer[bufCurrentPixel++]>> 24);
									dataOut[currentPixel++] = (byte)(buffer[bufCurrentPixel++]>> 24);
									dataOut[currentPixel++] = (byte)(buffer[bufCurrentPixel++]>> 24);
									bufCurrentPixel++; // Skip alpha
								}
							}
							// 16 bpp 555 format.
							else if (oldFrame.pixelFormat==PixelFormat.Format16bppRgb555)
							{
								while( bufCurrentPixel < endWriteBuffer)
								{
									int r = (byte)(buffer[bufCurrentPixel++]>> 24);
									int g = (byte)(buffer[bufCurrentPixel++]>> 24);
									int b = (byte)(buffer[bufCurrentPixel++]>> 24);
									bufCurrentPixel++; // Skip alpha
									dataOut[currentPixel++] = (byte)((g<<2 & 0xE0) | (b>>3 & 0x1F));
									dataOut[currentPixel++] = (byte)((r>>1 & 0x7C) | (g >>6 & 0x03));
								}
							}
							// 16 bpp 565 format.
							else if (oldFrame.pixelFormat==PixelFormat.Format16bppRgb565)
							{
								while( bufCurrentPixel < endWriteBuffer)
								{
									int r = (byte)(buffer[bufCurrentPixel++]>> 24);
									int g = (byte)(buffer[bufCurrentPixel++]>> 24);
									int b = (byte)(buffer[bufCurrentPixel++]>> 24);
									bufCurrentPixel++; // Skip alpha
									dataOut[currentPixel++] = (byte)((g<<3 & 0xE0) | (b >> 3 & 0x1F)) ;
									dataOut[currentPixel++] = (byte)((r & 0xF8) | (g >>5 & 0x07));
								}
							}
							// 8 bpp format.
							else if (oldFrame.pixelFormat==PixelFormat.Format8bppIndexed)
							{
								while(bufCurrentPixel < endWriteBuffer)
								{
									int r = (byte)(buffer[bufCurrentPixel++]>> 24);
									int g = (byte)(buffer[bufCurrentPixel++]>> 24);
									int b = (byte)(buffer[bufCurrentPixel++]>> 24);
									bufCurrentPixel++; // Skip alpha
									dataOut[currentPixel++] = (byte)Utils.BestPaletteColor(palette, r, g, b);
								}
							}
							// 4 bpp format.
							else if (oldFrame.pixelFormat==PixelFormat.Format4bppIndexed)
							{
								bool highNibble1 = true;
								int dataByte1 = 0;
								while(bufCurrentPixel < endWriteBuffer)
								{
									int r = (byte)(buffer[bufCurrentPixel++]>> 24);
									int g = (byte)(buffer[bufCurrentPixel++]>> 24);
									int b = (byte)(buffer[bufCurrentPixel++]>> 24);
									bufCurrentPixel++; // Skip alpha
									int palettePos = (byte)Utils.BestPaletteColor(palette, r, g, b);
									if (highNibble1)
									{
										dataByte1 = palettePos << 4;
									}
									else
									{
										dataByte1 |= palettePos;
										dataOut[currentPixel++] = (byte)dataByte1;
									}
									highNibble1 = !highNibble1;
								}
								// Write the last bits
								if (!highNibble1)
								{
									dataOut[currentPixel] = (byte)dataByte1;
								}
							}
							
							bufCurrentPixel = bufNextLine;
							bufNextLine = currentLine;
							currentLine = bufCurrentPixel;
							int endClearBuffer = bufNextLine + bufLine/4;
							for (int c = bufNextLine; c < endClearBuffer; c++)
							{
								buffer[c] = 0;
							}
							y++;
							lineStartOut += newFrame.Stride;
						}
						currentYCoeff += 2;
					}
				} 

		private static int[] CreateCoefficients(int length, int newLength)
				{
					int sum = 0;
					int[] coefficients = new int[2 * length];
					int normalize = (newLength << 12) / length;
					int denominator = length;
					for(int i = 0; i < coefficients.Length; i += 2)
					{
						int sum2 = sum + newLength;
						if(sum2 > length)
						{
							coefficients[i] = ((length - sum) << 12) / denominator;
							coefficients[i+1] = ((sum2 - length) << 12) / denominator;
							sum2 -= length;
						}
						else
						{
							coefficients[i] = normalize;
							if(sum2 == length)
							{
								coefficients[i+1] = -1;
								sum2 = 0;
							}
						}
						sum = sum2;
					}
					return coefficients;
				}
	}
}
