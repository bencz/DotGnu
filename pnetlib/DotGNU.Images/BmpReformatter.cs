/*
 * BmpConvert.cs - Implementation of the "DotGNU.Images.Convert" class.
 *
 * Copyright (C) 2003  Neil Cawse.
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

	public class BmpReformatter
	{
		public static Frame Reformat(Frame oldFrame, PixelFormat format)
				{
					if (oldFrame.pixelFormat == format)
					{
						return oldFrame.CloneFrame(null);
					}
					Frame newFrame = oldFrame.CloneFrameEmpty(oldFrame.width, oldFrame.height, format);
					newFrame.data = new byte[newFrame.height * newFrame.stride];
					if (oldFrame.Mask != null)
					{
						newFrame.mask = (byte[])oldFrame.Mask.Clone();
					}

					// alpha 32 bpp Format.
					if (oldFrame.pixelFormat == PixelFormat.Format32bppArgb)
					{
						switch(format)
						{
							case(PixelFormat.Format32bppRgb):
								Reformat32bppRemoveAlpha(oldFrame, newFrame);
								break;
							case(PixelFormat.Format24bppRgb):
								Reformat32bppTo24bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format16bppRgb555):
								Reformat32bppTo16bpp(oldFrame, newFrame, true);
								break;
							case(PixelFormat.Format16bppRgb565):
								Reformat32bppTo16bpp(oldFrame, newFrame, false);
								break;
							case(PixelFormat.Format8bppIndexed):
								Reformat32bppTo8bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format4bppIndexed):
								Reformat32bppTo1bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format1bppIndexed):
								Reformat32bppTo1bpp(oldFrame, newFrame);
								break;
							default:
								throw new NotSupportedException();
						}
					}

					// no alpha 32 bpp Format.
					else if (oldFrame.pixelFormat == PixelFormat.Format32bppRgb)
					{
						switch(format)
						{
							case(PixelFormat.Format32bppArgb):
								Array.Copy(oldFrame.data, newFrame.data, oldFrame.data.Length);
								break;
							case(PixelFormat.Format24bppRgb):
								Reformat32bppTo24bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format16bppRgb555):
								Reformat32bppTo16bpp(oldFrame, newFrame, true);
								break;
							case(PixelFormat.Format16bppRgb565):
								Reformat32bppTo16bpp(oldFrame, newFrame, false);
								break;
							case(PixelFormat.Format8bppIndexed):
								Reformat32bppTo8bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format4bppIndexed):
								Reformat32bppTo1bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format1bppIndexed):
								Reformat32bppTo1bpp(oldFrame, newFrame);
								break;
							default:
								throw new NotSupportedException();
						}
					}
								
					// 24 bpp Format.
					else if (oldFrame.pixelFormat == PixelFormat.Format24bppRgb)
					{
						switch(format)
						{
							case(PixelFormat.Format32bppArgb):
							case(PixelFormat.Format32bppRgb):
								Reformat24bppTo32bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format16bppRgb555):
								Reformat24bppTo16bpp(oldFrame, newFrame, true);
								break;
							case(PixelFormat.Format16bppRgb565):
								Reformat24bppTo16bpp(oldFrame, newFrame, false);
								break;
							case(PixelFormat.Format8bppIndexed):
								Reformat24bppTo8bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format4bppIndexed):
								Reformat24bppTo4bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format1bppIndexed):
								Reformat24bppTo1bpp(oldFrame, newFrame);
								break;
							default:
								throw new NotSupportedException();
						}
					}
					// 16 bpp Format.
					else if (oldFrame.pixelFormat == PixelFormat.Format16bppRgb565 || oldFrame.pixelFormat == PixelFormat.Format16bppRgb555)
					{
						switch (format)
						{
							case(PixelFormat.Format32bppArgb):
							case(PixelFormat.Format32bppRgb):
								Reformat16bppTo32bpp(oldFrame, oldFrame.pixelFormat == PixelFormat.Format16bppRgb555, newFrame);
								break;
							case(PixelFormat.Format24bppRgb):
								Reformat16bppTo24bpp(oldFrame, oldFrame.pixelFormat == PixelFormat.Format16bppRgb555, newFrame);
								break;
							case(PixelFormat.Format16bppRgb555):
								Reformat16bpp(oldFrame, newFrame, true);
								break;
							case(PixelFormat.Format16bppRgb565):
								Reformat16bpp(oldFrame, newFrame, false);
								break;
							case(PixelFormat.Format8bppIndexed):
								//TODO: Not efficient.
								Reformat32bppTo8bpp(Reformat(oldFrame, PixelFormat.Format32bppRgb), newFrame);
								break;
							case(PixelFormat.Format4bppIndexed):
								//TODO: Not efficient.
								Reformat32bppTo4bpp(Reformat(oldFrame, PixelFormat.Format32bppRgb), newFrame);
								break;
							case(PixelFormat.Format1bppIndexed):
								//TODO: Not efficient.
								Reformat32bppTo1bpp(Reformat(oldFrame, PixelFormat.Format32bppRgb), newFrame);
								break;
							default:
								throw new NotSupportedException();
						}
					}
					// 8 bpp Format.
					else if (oldFrame.pixelFormat == PixelFormat.Format8bppIndexed)
					{
						switch (format)
						{
							case(PixelFormat.Format32bppArgb):
								Reformat8bppTo32bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format32bppRgb):
								Reformat8bppTo32bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format24bppRgb):
								Reformat8bppTo24bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format16bppRgb555):
								Reformat8bppTo16bpp(oldFrame, newFrame, true);
								break;
							case(PixelFormat.Format16bppRgb565):
								Reformat8bppTo16bpp(oldFrame, newFrame, false);
								break;
							case(PixelFormat.Format4bppIndexed):
								//TODO: Not efficient.
								Reformat32bppTo4bpp(Reformat(oldFrame, PixelFormat.Format32bppRgb), newFrame);
								break;
							case(PixelFormat.Format1bppIndexed):
								//TODO: Not efficient.
								Reformat32bppTo1bpp(Reformat(oldFrame, PixelFormat.Format32bppRgb), newFrame);
								break;
							default:
								throw new NotSupportedException();
						}
					}
					// 4 bpp Format.
					else if (oldFrame.pixelFormat == PixelFormat.Format4bppIndexed)
					{
						switch (format)
						{
							case(PixelFormat.Format32bppArgb):
								Reformat4bppTo32bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format32bppRgb):
								Reformat4bppTo32bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format24bppRgb):
								Reformat4bppTo24bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format16bppRgb555):
								Reformat4bppTo16bpp(oldFrame, newFrame, true);
								break;
							case(PixelFormat.Format16bppRgb565):
								Reformat4bppTo16bpp(oldFrame, newFrame, false);
								break;
							case(PixelFormat.Format8bppIndexed):
								Reformat4bppTo8bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format1bppIndexed):
								//TODO: Not efficient.
								Reformat32bppTo1bpp(Reformat(oldFrame, PixelFormat.Format32bppRgb), newFrame);
								break;
							default:
								throw new NotSupportedException();
						}
					}
					// 1bpp Format.
					else if (oldFrame.pixelFormat == PixelFormat.Format1bppIndexed)
					{
						switch (format)
						{
							case(PixelFormat.Format32bppArgb):
							case(PixelFormat.Format32bppRgb):
								Reformat1bppTo32bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format24bppRgb):
								Reformat1bppTo24bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format16bppRgb555):
								Reformat1bppTo16bpp(oldFrame, newFrame, true);
								break;
							case(PixelFormat.Format16bppRgb565):
								Reformat1bppTo16bpp(oldFrame, newFrame, false);
								break;
							case(PixelFormat.Format8bppIndexed):
								Reformat1bppTo8bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format4bppIndexed):
								Reformat1bppTo4bpp(oldFrame, newFrame);
								break;
							case(PixelFormat.Format1bppIndexed):
								//TODO: Not efficient.
								Reformat32bppTo1bpp(Reformat(oldFrame, PixelFormat.Format32bppRgb), newFrame);
								break;
							default:
								throw new NotSupportedException();
						}
					}
					else
					{
						throw new NotSupportedException();
					}
					return newFrame;
				}

		private static void Reformat16bppTo32bpp(Frame oldFrame, bool format555, Frame newFrame)
				{
					byte alphafiller = 0;
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;

					if(newFrame.PixelFormat == PixelFormat.Format32bppArgb)
					{
						alphafiller = 255; // full opacity
					}
					
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 4;
						while(ptrNew < newByteEnd)
						{
							byte g = oldData[ptrOld++];
							byte r = oldData[ptrOld++];
							byte b = (byte)(g & 0x1F);
							if (format555)
							{
								g = (byte)(r << 3 & 0x18  | g >> 5 & 0x07);
								g = (byte)((int)g * 255 / 31);
								r = (byte)(r >> 2 & 0x1F);
							}
							else
							{
								g = (byte)(r << 3 & 0x38  | g >> 5 & 0x07);
								g = (byte)((int)g * 255 / 63);
								r = (byte)(r >> 3);
							}
							r = (byte)((int)r * 255 / 31);
							b = (byte)((int)b * 255 / 31);
							newData[ptrNew++] = b;
							newData[ptrNew++] = g;
							newData[ptrNew++] = r;
							newData[ptrNew++] = alphafiller;
						}
					}
				}

		private static void Reformat24bppTo32bpp(Frame oldFrame, Frame newFrame)
				{
					byte alphafiller = 0;
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
								
					int ptrNew = 0;
					byte[] newData = newFrame.Data;

					if(newFrame.PixelFormat == PixelFormat.Format32bppArgb)
					{
						alphafiller = 255; // full opacity
					}
					
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 4;
						while(ptrNew < newByteEnd)
						{
							newData[ptrNew++] = oldData[ptrOld++];
							newData[ptrNew++] = oldData[ptrOld++];
							newData[ptrNew++] = oldData[ptrOld++];
							// alpha component is 0xff for 32bppArgb
							newData[ptrNew++] = alphafiller;
						}
					}
				}

		private static void Reformat8bppTo32bpp(Frame oldFrame, Frame newFrame)
				{
					byte alphafiller = 0;
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					
					if(newFrame.PixelFormat == PixelFormat.Format32bppArgb)
					{
						alphafiller = 255; // full opacity
					}

					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 4;
						while(ptrNew < newByteEnd)
						{
							int color = palette[oldData[ptrOld++]];
							newData[ptrNew++] = (byte)color;
							newData[ptrNew++] = (byte)(color>>8);
							newData[ptrNew++] = (byte)(color>>16);
							newData[ptrNew++] = alphafiller;
						}
					}
				}

		private static void Reformat4bppTo32bpp(Frame oldFrame, Frame newFrame)
				{
					byte alphafiller = 0;
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					
					if(newFrame.PixelFormat == PixelFormat.Format32bppArgb)
					{
						alphafiller = 255; // full opacity
					}

					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						bool firstNibble = true;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width * 4;
						while(ptrNew < newByteEnd)
						{
							int color;
							if (firstNibble)
							{
								byteData = oldData[ptrOld];
								color = palette[(byteData & 0xF0) >> 4];
							}
							else
							{
								color = palette[byteData & 0x0F];
								ptrOld++;
							}
							firstNibble = !firstNibble;
							newData[ptrNew++] = (byte)color;
							newData[ptrNew++] = (byte)(color>>8);
							newData[ptrNew++] = (byte)(color>>16);
							newData[ptrNew++] = alphafiller;
						}
					}
				}

		private static void Reformat1bppTo32bpp(Frame oldFrame, Frame newFrame)
				{
					byte alphafiller = 0;
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int colorBlack = oldFrame.Palette[0];
					int colorWhite = oldFrame.Palette[1];
					byte colorBlackR = (byte)(colorBlack>>16);
					byte colorBlackG = (byte)(colorBlack>>8);
					byte colorBlackB = (byte)(colorBlack);
					byte colorWhiteR = (byte)(colorWhite>>16);
					byte colorWhiteG = (byte)(colorWhite>>8);
					byte colorWhiteB = (byte)(colorWhite);
					int ptrNew = 0;
					byte[] newData = newFrame.Data;

					if(newFrame.PixelFormat == PixelFormat.Format32bppArgb)
					{
						alphafiller = 255; // full opacity
					}

					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int bit = 128;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width * 4;
						if (ptrNew < newByteEnd)
							byteData = oldData[ptrOld];
						while(ptrNew < newByteEnd)
						{
							if ((byteData & bit) != 0)
							{
								newData[ptrNew++] = colorWhiteB;
								newData[ptrNew++] = colorWhiteG;
								newData[ptrNew++] = colorWhiteR;
								newData[ptrNew++] = alphafiller;
							}
							else
							{
								newData[ptrNew++] = colorBlackB;
								newData[ptrNew++] = colorBlackG;
								newData[ptrNew++] = colorBlackR;
								newData[ptrNew++] = alphafiller;
							}
							bit = bit>>1;
							if (bit == 0 && ptrNew < newByteEnd)
							{
								bit = 128;
								ptrOld++;
								byteData = oldData[ptrOld];
							}
						}
					}
				}

		private static void Reformat32bppTo24bpp(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
						
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 3;
						while(ptrNew < newByteEnd)
						{
							newData[ptrNew++] = oldData[ptrOld++];
							newData[ptrNew++] = oldData[ptrOld++];
							newData[ptrNew++] = oldData[ptrOld++];
							// Skip the alpha component.
							ptrOld++;
						}
					}
				}

		private static void Reformat32bppTo16bpp(Frame oldFrame, Frame newFrame, bool format555)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
							
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 2;
						while(ptrNew < newByteEnd)
						{
							int color = oldData[ptrOld++] | oldData[ptrOld++] <<8 | oldData[ptrOld++] << 16;
							// Skip the alpha
							ptrOld++;
							if (format555)
							{
								newData[ptrNew++] =  (byte)((color>>6 & 0xE0) | (color>>3 & 0x1F));
								newData[ptrNew++] =  (byte)((color>>17 & 0x7C) | (color >>14 & 0x03));
							}
							else
							{
								newData[ptrNew++] = (byte)((color>>5 & 0xE0) | (color >> 3 & 0x1F)) ;
								newData[ptrNew++] = (byte)((color>>16 & 0xF8) | (color >>13 & 0x07));
							}
						}
					}
				}

		private static void Reformat32bppTo8bpp(Frame oldFrame, Frame newFrame)
				{
					Octree octree = new Octree (Utils.FormatToBitCount(newFrame.pixelFormat));
					octree.Process(oldFrame, newFrame);
				}

		private static void Reformat24bppTo8bpp(Frame oldFrame, Frame newFrame)
				{
					Octree octree = new Octree (Utils.FormatToBitCount(newFrame.pixelFormat));
					octree.Process(oldFrame, newFrame);
				}

		private static void Reformat32bppTo4bpp(Frame oldFrame, Frame newFrame)
				{
					Octree octree = new Octree (Utils.FormatToBitCount(newFrame.pixelFormat));
					octree.Process(oldFrame, newFrame);
				}

		private static void Reformat24bppTo4bpp(Frame oldFrame, Frame newFrame)
				{
					Octree octree = new Octree (Utils.FormatToBitCount(newFrame.pixelFormat));
					octree.Process(oldFrame, newFrame);
				}

		private static void Reformat32bppTo1bpp(Frame oldFrame, Frame newFrame)
				{
					Octree octree = new Octree (Utils.FormatToBitCount(newFrame.pixelFormat));
					octree.Process(oldFrame, newFrame);
				}

		private static void Reformat24bppTo1bpp(Frame oldFrame, Frame newFrame)
				{
					Octree octree = new Octree (Utils.FormatToBitCount(newFrame.pixelFormat));
					octree.Process(oldFrame, newFrame);
				}
		
		private static void Reformat32bppRemoveAlpha(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 4;
						while(ptrNew < newByteEnd)
						{
							newData[ptrNew++] = oldData[ptrOld++];
							newData[ptrNew++] = oldData[ptrOld++];
							newData[ptrNew++] = oldData[ptrOld++];
							// Skip the alpha component.
							ptrOld++;
							ptrNew++;
						}
					}
				}

		// Not supported by .NET
		private static void Reformat4bppTo8bpp(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					// Copy palette.
					newFrame.Palette = (int[])oldFrame.Palette.Clone();
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						bool firstNibble = true;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width;
						for(; ptrNew < newByteEnd; ptrNew++)
						{
							int palettePos;
							if (firstNibble)
							{
								byteData = oldData[ptrOld];
								palettePos = (byteData & 0xF0) >> 4;
							}
							else
							{
								palettePos = byteData & 0x0F;
								ptrOld++;
							}
							firstNibble = !firstNibble;
							newData[ptrNew] = (byte)palettePos;
						}
					}
				}
		private static void Reformat4bppTo24bpp(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						bool firstNibble = true;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width * 3;
						while(ptrNew < newByteEnd)
						{
							int color;
							if (firstNibble)
							{
								byteData = oldData[ptrOld];
								color = palette[(byteData & 0xF0) >> 4];
							}
							else
							{
								color = palette[byteData & 0x0F];
								ptrOld++;
							}
							firstNibble = !firstNibble;
							newData[ptrNew++] = (byte)color;
							newData[ptrNew++] = (byte)(color>>8);
							newData[ptrNew++] = (byte)(color>>16);
						}
					}
				}
		
		private static void Reformat4bppTo16bpp(Frame oldFrame, Frame newFrame, bool format555)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						bool firstNibble = true;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width * 2;
						while(ptrNew < newByteEnd)
						{
							int color;
							if (firstNibble)
							{
								byteData = oldData[ptrOld];
								color = palette[(byteData & 0xF0) >> 4];
							}
							else
							{
								color = palette[byteData & 0x0F];
								ptrOld++;
							}
							firstNibble = !firstNibble;
							if (format555)
							{
								newData[ptrNew++] = (byte)((color>>6 & 0xE0) | (color>>3 & 0x1F));
								newData[ptrNew++] = (byte)((color>>17 & 0x7C) | (color >>14 & 0x03));
							}
							else
							{
								newData[ptrNew++] = (byte)((color>>5 & 0xE0) | (color >> 3 & 0x1F)) ;
								newData[ptrNew++] = (byte)((color>>16 & 0xF8) | (color >>13 & 0x07));
							}
						}
					}
				}
		
		private static void Reformat1bppTo24bpp(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int colorBlack = oldFrame.Palette[0];
					int colorWhite = oldFrame.Palette[1];
					byte colorBlackR = (byte)(colorBlack>>16);
					byte colorBlackG = (byte)(colorBlack>>8);
					byte colorBlackB = (byte)(colorBlack);
					byte colorWhiteR = (byte)(colorWhite>>16);
					byte colorWhiteG = (byte)(colorWhite>>8);
					byte colorWhiteB = (byte)(colorWhite);
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int bit = 128;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width * 3;
						if (ptrNew < newByteEnd)
							byteData = oldData[ptrOld];
						while(ptrNew < newByteEnd)
						{
							if ((byteData & bit) != 0)
							{
								newData[ptrNew++] = colorWhiteB;
								newData[ptrNew++] = colorWhiteG;
								newData[ptrNew++] = colorWhiteR;
							}
							else
							{
								newData[ptrNew++] = colorBlackB;
								newData[ptrNew++] = colorBlackG;
								newData[ptrNew++] = colorBlackR;
							}
							bit = bit>>1;
							if (bit == 0 && ptrNew < newByteEnd)
							{
								bit = 128;
								ptrOld++;
								byteData = oldData[ptrOld];
							}
						}
					}
				}
		
		// Not supported by .NET
		private static void Reformat1bppTo4bpp(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					// Copy palette.
					newFrame.Palette = (int[])oldFrame.Palette.Clone();
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int bit = 0;
						bool firstNibble = true;
						byte byteNewData = 0;
						byte byteOldData = 0;
						for(int x = 0; x < oldFrame.width; x++)
						{
							if (bit == 0)
							{
								bit = 128;
								byteOldData = oldData[ptrOld++];
							}
							if (firstNibble)
							{
								if ((byteOldData & bit) != 0)
									byteNewData =  16;
								else
									byteNewData = 0;
							}
							else
							{
								if ((byteOldData & bit) != 0)
									byteNewData |= 0x01;
								newData[ptrNew++] = byteNewData;
							}
							bit = bit>>1;
							firstNibble = !firstNibble;
							
						}
					}
				}
		// Not supported by .NET
		private static void Reformat1bppTo8bpp(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					// Copy palette.
					newFrame.Palette = (int[])oldFrame.Palette.Clone();
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int bit = 0;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width;
						for(; ptrNew < newByteEnd; ptrNew++)
						{
							if (bit == 0)
							{
								bit = 128;
								byteData = oldData[ptrOld++];
							}
							if ((byteData & bit) != 0)
								newData[ptrNew] = 1;
							else
								newData[ptrNew] = 0;
							bit = bit>>1;
						}
					}
				}
		private static void Reformat1bppTo16bpp(Frame oldFrame, Frame newFrame, bool format555)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int colorBlack = oldFrame.Palette[0];
					int colorWhite = oldFrame.Palette[1];
					byte colorBlack1, colorBlack2, colorWhite1, colorWhite2;
					if (format555)
					{
						colorBlack1 = (byte)((colorBlack>>6 & 0xE0) | (colorBlack>>3 & 0x1F));
						colorBlack2 = (byte)((colorBlack>>17 & 0x7C) | (colorBlack >>14 & 0x03));
						colorWhite1 = (byte)((colorWhite>>6 & 0xE0) | (colorWhite>>3 & 0x1F));
						colorWhite2 = (byte)((colorWhite>>17 & 0x7C) | (colorWhite >>14 & 0x03));
					}
					else
					{
						colorBlack1 = (byte)((colorBlack>>5 & 0xE0) | (colorBlack >> 3 & 0x1F)) ;
						colorBlack2 = (byte)((colorBlack>>16 & 0xF8) | (colorBlack >>13 & 0x07));
						colorWhite1 = (byte)((colorWhite>>5 & 0xE0) | (colorWhite >> 3 & 0x1F)) ;
						colorWhite2 = (byte)((colorWhite>>16 & 0xF8) | (colorWhite >>13 & 0x07));
					}
					
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int bit = 0;
						byte byteData = 0;
						int newByteEnd = ptrNew + newFrame.width * 2;
						while(ptrNew < newByteEnd)
						{
							if (bit == 0)
							{
								bit = 128;
								byteData = oldData[ptrOld++];
							}
							if ((byteData & bit) != 0)
							{
								newData[ptrNew++] =  colorWhite1;
								newData[ptrNew++] =  colorWhite2;
							}
							else
							{
								newData[ptrNew++] = colorBlack1;
								newData[ptrNew++] = colorBlack2;
							}
							bit = bit>>1;
						}
					}
				}
		
		private static void Reformat8bppTo24bpp(Frame oldFrame, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 3;
						while(ptrNew < newByteEnd)
						{
							int color = palette[oldData[ptrOld++]];
							newData[ptrNew++] = (byte)color;
							newData[ptrNew++] = (byte)(color>>8);
							newData[ptrNew++] = (byte)(color>>16);
						}
					}
				}

		private static void Reformat8bppTo16bpp(Frame oldFrame, Frame newFrame, bool format555)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 2;
						while(ptrNew < newByteEnd)
						{
							int color = palette[oldData[ptrOld++]];
							if (format555)
							{
								newData[ptrNew++] = (byte)((color>>6 & 0xE0) | (color>>3 & 0x1F));
								newData[ptrNew++] = (byte)((color>>17 & 0x7C) | (color >>14 & 0x03));
							}
							else
							{
								newData[ptrNew++] = (byte)((color>>5 & 0xE0) | (color >> 3 & 0x1F)) ;
								newData[ptrNew++] = (byte)((color>>16 & 0xF8) | (color >>13 & 0x07));
							}
						}
					}
				}

		private static void Reformat16bppTo24bpp(Frame oldFrame, bool format555, Frame newFrame)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 3;
						while(ptrNew < newByteEnd)
						{
							byte g = oldData[ptrOld++];
							byte r = oldData[ptrOld++];
							byte b = (byte)(g & 0x1F);
							if (format555)
							{
								g = (byte)(r << 3 & 0x18  | g >> 5 & 0x07);
								g = (byte)((int)g * 255 / 31);
								r = (byte)(r >> 2 & 0x1F);
							}
							else
							{
								g = (byte)(r << 3 & 0x38  | g >> 5 & 0x07);
								g = (byte)((int)g * 255 / 63);
								r = (byte)(r >> 3);
							}
							r = (byte)((int)r * 255 / 31);
							b = (byte)((int)b * 255 / 31);
							newData[ptrNew++] = b;
							newData[ptrNew++] = g;
							newData[ptrNew++] = r;
						}
					}
				}
		private static void Reformat16bpp(Frame oldFrame, Frame newFrame, bool formatTo555)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 2;
						while(ptrNew < newByteEnd)
						{
							byte b1 = oldData[ptrOld++];
							byte b2 = oldData[ptrOld++];
							if (formatTo555)
							{
								b1 = (byte)((b2 & 0x01) << 7 | (b1 & 0xC0) >> 1 | (b1 & 0x1F));
								b2 = (byte)((b2 & 0xF8) >> 1 | (b2 & 0x07)>>1);
							}
							else
							{
								b2 = (byte)((b2 & 0x7C) << 1 | (b2 & 0x03)<<1 | (b1 & 0x80)>> 7);
								b1 = (byte)((b1 & 0xD0) << 1 | (b1 & 0x1F));
							}
							newData[ptrNew++] = b1;
							newData[ptrNew++] = b2;
						}
					}
				}
		private static void Reformat24bppTo16bpp(Frame oldFrame, Frame newFrame, bool format555)
				{
					int ptrOld = 0;
					byte[] oldData = oldFrame.Data;
					
					int[] palette = oldFrame.Palette;
					int ptrNew = 0;
					byte[] newData = newFrame.Data;
					for(int y = 0; y < oldFrame.height; y++)
					{
						ptrOld = y * oldFrame.stride;
						ptrNew = y * newFrame.stride;
						int newByteEnd = ptrNew + newFrame.width * 2;
						while(ptrNew < newByteEnd)
						{
							int color = oldData[ptrOld++] | oldData[ptrOld++] <<8 | oldData[ptrOld++] << 16;
							if (format555)
							{
								newData[ptrNew++] =  (byte)((color>>6 & 0xE0) | (color>>3 & 0x1F));
								newData[ptrNew++] =  (byte)((color>>17 & 0x7C) | (color >>14 & 0x03));
							}
							else
							{
								newData[ptrNew++] = (byte)((color>>5 & 0xE0) | (color >> 3 & 0x1F)) ;
								newData[ptrNew++] = (byte)((color>>16 & 0xF8) | (color >>13 & 0x07));
							}
						}
					}
				}
			
		private class Octree
		{
			// The root of the octree.
			private	OctreeNode root;
			private int leafCount = 0;
			private OctreeNode[] reducibleNodes;
			private int maxColorBits;
			// Store the last node quantized.
			private OctreeNode previousNode = null;
			// Cache the previous color quantized.
			private int previousColor;
			// Mask used when getting the appropriate pixels for a given node.
			private static int[] mask = new int[8] { 0x80 , 0x40 , 0x20 , 0x10 , 0x08 , 0x04 , 0x02 , 0x01 };
			public Octree (int maxColorBits)
					{
						this.maxColorBits = maxColorBits;
						reducibleNodes = new OctreeNode[9];
						root = new OctreeNode (0 , maxColorBits , this); 
					}

			// Reduce the depth of the tree.
			private void Reduce (ref int childrenNeedingReduction)
					{
						int i;
						// Find the deepest level containing at least one reducible node.
						for (i = maxColorBits - 1; i > 0 && reducibleNodes[i] == null; i--);

						// Reduce the node most recently added to the list at level 'index'.
						OctreeNode node = reducibleNodes[i];
						reducibleNodes[i] = node.nextReducible;
						leafCount -= node.Reduce(ref childrenNeedingReduction);

						// Just in case we have reduced the last color to be added, and the next color to
						// be added is the same, invalidate the previousNode.
						previousNode = null;
					}

			public void Process(Frame sourceFrame, Frame destFrame)
					{
						previousColor = -1;
						byte[] destData = destFrame.Data;
						byte[] sourceData = sourceFrame.Data;
						int color;
						bool is32 = sourceFrame.pixelFormat == PixelFormat.Format32bppArgb || sourceFrame.pixelFormat == PixelFormat.Format32bppRgb;
						if (!is32 && sourceFrame.pixelFormat != PixelFormat.Format24bppRgb)
						{
							throw new NotSupportedException();
						}

						int pSourceRow = 0;
						int pSourcePixel;
								
						// Loop through each row.
						for (int row = 0 ; row <  sourceFrame.height ; row++)
						{
							// Set the source pixel to the first pixel in this row
							pSourcePixel = pSourceRow ;
							for (int col = 0 ; col < sourceFrame.width ; col++)
							{
								int b;
								int g;
								int r;
								if (is32)
								{
									// Add the color to the octree
									b = sourceData[pSourcePixel++];
									g = sourceData[pSourcePixel++];
									r = sourceData[pSourcePixel];
									pSourcePixel += 2;
								}
								else
								{
									// Add the color to the octree
									b = sourceData[pSourcePixel++];
									g = sourceData[pSourcePixel++];
									r = sourceData[pSourcePixel++];
								}
								
								color = b | g << 8 | r << 16;
								// Add a given color value to the octree
								// Check if this request is for the same color as the last
								if (previousColor == color)
								{
									// Just update the previous node
									previousNode.Increment (r, g, b);
								}
								else
								{
									previousColor = color;
									root.AddColor (maxColorBits , 0 , this, r, g, b);
								}
							}
							pSourceRow += sourceFrame.stride;
						}

						destFrame.Palette = CreatePalette(1 << maxColorBits);

						// Do the second pass.
						pSourceRow = 0;
						pSourcePixel = pSourceRow;
						int pDestinationRow =0;
						int pDestinationPixel = pDestinationRow;
						byte destByte = 0;

						// And convert the first pixel, so that I have values going into the loop
						int prevColor = -1;
						byte prevPixelValue = 0;

						if (destFrame.pixelFormat == PixelFormat.Format8bppIndexed)
						{
							for (int row = 0 ; row < sourceFrame.height; row++)
							{
								pSourcePixel = pSourceRow;
								pDestinationPixel = pDestinationRow;

								// Loop through each pixel on this scan line
								for (int col = 0; col < sourceFrame.width; col++)
								{
									if (is32)
									{
										color = sourceData[pSourcePixel++] | sourceData[pSourcePixel++] << 8 | sourceData[pSourcePixel] << 16;
										pSourcePixel += 2;
									}
									else
									{
										color = sourceData[pSourcePixel++] | sourceData[pSourcePixel++] << 8 | sourceData[pSourcePixel++] << 16;
									}
									if (color == prevColor)
									{
										destData[pDestinationPixel] = prevPixelValue;
									}
									else
									{
										prevColor = color;
										prevPixelValue = (byte)root.GetPaletteIndex (color , 0);
										destData[pDestinationPixel] = prevPixelValue;
									}
									pDestinationPixel++;
								}
								pSourceRow += sourceFrame.stride;
								pDestinationRow += destFrame.stride;
							}		
						}
						else if(destFrame.pixelFormat == PixelFormat.Format4bppIndexed)
						{
							for (int row = 0 ; row < sourceFrame.height; row++)
							{
								pSourcePixel = pSourceRow;
								pDestinationPixel = pDestinationRow;

								// Loop through each pixel on this scan line
								for (int col = 0 ; col < sourceFrame.width ; col++)
								{
									// Get the color from the source.
									if (is32)
									{
										color = sourceData[pSourcePixel++] | sourceData[pSourcePixel++] << 8 | sourceData[pSourcePixel] << 16;
										pSourcePixel += 2;
									}
									else
									{
										color = sourceData[pSourcePixel++] | sourceData[pSourcePixel++] << 8 | sourceData[pSourcePixel++] << 16;
									}

									// Use the cached color if the source color hasnt changed.
									byte pixelValue;
									if (color == prevColor)
									{
										pixelValue = prevPixelValue;
									}
									else
									{
										pixelValue = (byte)root.GetPaletteIndex (color , 0);
										prevColor = color;
										prevPixelValue = pixelValue;
										destData[pDestinationPixel] = prevPixelValue;
									}
									if ((col & 0x1) == 0)
									{
										destByte = (byte)(pixelValue << 4);
									}
									else
									{
										destByte |= pixelValue;
										destData[pDestinationPixel++] = destByte;
									}
								}
								// In case we are not on a nibble boundary
								if ((destFrame.width & 0x1) > 0)
								{
									destData[pDestinationPixel] = destByte;
								}

								pSourceRow += sourceFrame.stride;
								pDestinationRow += destFrame.stride;
							}		
						}
						else if(destFrame.pixelFormat == PixelFormat.Format1bppIndexed)
						{
							for (int row = 0 ; row < sourceFrame.height; row++)
							{
								pSourcePixel = pSourceRow;
								pDestinationPixel = pDestinationRow;

								// Loop through each pixel on this scan line
								for (int col = 0 ; col < sourceFrame.width ; col++)
								{
									// Get the color from the source.
									if (is32)
									{
										color = sourceData[pSourcePixel++] | sourceData[pSourcePixel++] << 8 | sourceData[pSourcePixel] << 16;
										pSourcePixel += 2;
									}
									else
									{
										color = sourceData[pSourcePixel++] | sourceData[pSourcePixel++] << 8 | sourceData[pSourcePixel++] << 16;
									}
									
									// Use the cached color if the source color hasnt changed.
									byte pixelValue;
									if (color == prevColor)
									{
										pixelValue = prevPixelValue;
									}
									else
									{
										pixelValue = (byte)root.GetPaletteIndex (color , 0);
										prevColor = color;
										prevPixelValue = pixelValue;
										destData[pDestinationPixel] = prevPixelValue;
									}
									// Which bit must be written?
									int pos = 7 - col & 0x7;
									
									if (pixelValue > 0)
									{
										destByte = (byte)(destByte | 1 << pos);
									}
									// If we are at the end of the byte then write it to the destination.
									if (pos == 0)
									{
										destData[pDestinationPixel++] = destByte;
										destByte = 0;
									}
								}
								// In case we are not on an 8 bit boundary
								if ((destFrame.width & 0x7) > 0)
								{
									destData[pDestinationPixel] = destByte;
								}
								pSourceRow += sourceFrame.stride;
								pDestinationRow += destFrame.stride;
							}
						}
						else
						{
							throw new NotSupportedException();
						}
					}

			// Convert the nodes in the octree to a palette with a maximum of colorCount colors
			public int[] CreatePalette (int colorCount)
					{
						int childrenNeedingReduction = leafCount - colorCount;
						while (leafCount > colorCount)
						{
							Reduce(ref childrenNeedingReduction);
						}
						int[] palette = new int[leafCount];
						int paletteIndex = 0;
						root.CreatePalette (palette , ref paletteIndex);
						return palette;
					}

			// Class which encapsulates each node in the tree
			private class OctreeNode
			{
				private	int pixelCount = 0;
				private	int red = 0;
				private	int green = 0;
				private int blue= 0;
				// if the children are null then we are a leaf node.
				private OctreeNode[] children;
				// Next reducible node
				internal OctreeNode nextReducible;
				// The index of this node in the palette
				private	int paletteIndex;

				// Construct the node
				public OctreeNode (int level , int colorBits , Octree octree)
						{
							// If a leaf, increment the leaf count
							if (level == colorBits)
							{
								octree.leafCount++;
							}
							else
							{
								// Otherwise add this to the reducible nodes
								nextReducible = octree.reducibleNodes[level];
								octree.reducibleNodes[level] = this;
								children = new OctreeNode[8];
							}
						}

				// Add a color into the tree
				public void AddColor (int colorBits , int level , Octree octree, int r, int g, int b)
						{
							// Update the color information if this is a leaf
							if (children == null)
							{
								Increment(r, g, b);
								// Setup the previous node
								octree.previousNode = this;
							}
							else
							{
								// Go to the next level down in the tree
								int index = (r & mask[level]) >> (5 - level) | (g & mask[level]) >> (6 - level) | (b & mask[level]) >> (7 - level);

								OctreeNode child = children[index];
								if (child == null)
								{
									// Create a new child node & store in the array
									child = new OctreeNode(level + 1 , colorBits , octree); 
									children[index] = child;
								}
								// Add the color to the child node
								child.AddColor (colorBits , level + 1 , octree, r, g, b);
							}
						}

				// Reduce this node by removing children leaving a minimum number 
				public int Reduce (ref int childrenNeedingReduction)
						{
							red = green = blue = 0;
							int childrenReduced = 0;
							if (childrenNeedingReduction >= 8)
							{
								// Loop through all children and add their information to this node
								for (int index = 0; index < 8; index++)
								{
									OctreeNode node = children[index];
									if (node != null)
									{
										red += node.red;
										green += node.green;
										blue += node.blue;
										pixelCount += node.pixelCount;
										childrenReduced++;
										children[index] = null;
									}
								}
								// Add one more node because this node is becoming a leaf.
								childrenReduced--;
								childrenNeedingReduction -= childrenReduced;
							}
							else
							{
								while (childrenNeedingReduction > 0)
								{
									// Find the node with the least items.
									int leastItems = int.MaxValue;
									int leastItemIndex = -1;
									OctreeNode node;
									for (int index = 0; index < 8; index++)
									{
										node = children[index];
										if (node != null && node.pixelCount < leastItems)
										{
											leastItems = node.pixelCount;
											leastItemIndex = index;
										}
									}
									if (leastItemIndex == -1)
									{
										break;	
									}
									node = children[leastItemIndex];
									red += node.red;
									green += node.green;
									blue += node.blue;
									pixelCount += node.pixelCount;
									children[leastItemIndex] = null;
									childrenReduced++;
									childrenNeedingReduction--;
								}
								for (int i = 0; i < 8; i++)
								{
									if (children[i] != null)
									{
										return childrenReduced;
									}
								}
								// Add one more node because this node is becoming a leaf.
								childrenReduced--;
								childrenNeedingReduction++;
							}
							children = null;
							// Return the number of nodes to decrement the leaf count by
							return childrenReduced;
						}

				// Traverse the tree, building up the color palette
				public void CreatePalette (int[] palette , ref int paletteIndex)
						{
							if (children == null)
							{
								this.paletteIndex = paletteIndex++;
								// Set the color of the palette entry
								palette[this.paletteIndex] = (red / pixelCount) << 16 | (green / pixelCount) << 8 |(blue / pixelCount);
							}
							else
							{
								// Loop through children looking for leaves
								for (int index = 0; index < 8; index++)
								{
									if (null != children[index])
									{
										children[index].CreatePalette (palette , ref paletteIndex);
									}
								}
							}
						}

				// Return the palette index for the passed color
				public int GetPaletteIndex (int pixel , int level)
						{
							if (children != null)
							{
								int red = pixel >> 16;
								int green = (byte)(pixel >> 8);
								int blue = (byte)pixel;
								int index = (red & mask[level]) >> (5 - level) | (green & mask[level]) >> (6 - level) | (blue & mask[level]) >> (7 - level);
								if (children[index] != null)
								{
									return children[index].GetPaletteIndex (pixel , level + 1);
								}
								else
								{
									// Find the closest palette.
									for (int i = 1; i <= 7; i++)
									{
										int tryPos = index - i;
										if (tryPos >= 0)
										{
											if (children[tryPos] != null)
											{
												return children[tryPos].GetPaletteIndex (pixel , level + 1);
											}
										}
										tryPos = index + i;
										if (tryPos <= 7)
										{
											if (children[tryPos] != null)
											{
												return children[tryPos].GetPaletteIndex (pixel , level + 1);
											}
										}
									}
									throw new InvalidOperationException();
								}
							}
							return paletteIndex;
						}

				// Increment the pixel count and add to the color information
				public void Increment (int r, int g, int b)
						{
							pixelCount++;
							red += r;
							green += g;
							blue += b;
						}
			}
		}

	}
}
