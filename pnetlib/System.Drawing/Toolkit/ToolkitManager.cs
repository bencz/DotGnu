/*
 * ToolkitManager.cs - Implementation of the
 *			"System.Drawing.Toolkit.ToolkitManager" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Drawing.Toolkit
{

using System.Drawing.Printing;
using System.Reflection;
using System.IO;
using DotGNU.Images;

[NonStandardExtra]
public sealed class ToolkitManager
{
	// Helper values to allow a user override prior to initialization
	// with the Toolkit and PrintingSystem property setters.
	// The initialization happens on first call to the getters of these
	// properties,
	// After that point no overriding is possible.
	// NOTE: Due to possible inlining Setting the value to override with
	// and accessing the getter in the same function might not give the
	// desired result.
	internal static IToolkit toolkitOverride;
	internal static IToolkitPrintingSystem printingSystemOverride;

	public ToolkitManager() {}

	// Get or set the active graphical display toolkit.
	public static IToolkit Toolkit
			{
				get
				{
					return ToolkitHandler.toolkit;
				}
				set
				{
					toolkitOverride = value;
				}
			}

	// Determine if we currently have a graphical toolkit.
	public static bool HasToolkit
			{
				get
				{
					try
					{
						return (ToolkitHandler.toolkit != null);
					}
					catch (TypeInitializationException e)
					{
						return false;
					}
				}
			}

	// Get or set the active printing system.
	public static IToolkitPrintingSystem PrintingSystem
			{
				get
				{
					return PrintingSystemHandler.printingSystem;
				}
				set
				{
					printingSystemOverride = value;
				}
			}

	// Get a standard paper size object.
	public static PaperSize GetStandardPaperSize(PaperKind kind)
			{
				return new PaperSize(kind);
			}

	// Get a standard paper source object.
	public static PaperSource GetStandardPaperSource(PaperSourceKind kind)
			{
				return new PaperSource(kind, null);
			}

	// Get a standard printer resolution object.
	public static PrinterResolution GetStandardPrinterResolution
				(PrinterResolutionKind kind, int x, int y)
			{
				return new PrinterResolution(kind, x, y);
			}

	// Create a "Graphics" object from an "IToolkitGraphics" handler.
	public static Graphics CreateGraphics(IToolkitGraphics graphics)
			{
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics");
				}
				return new Graphics(graphics);
			}

	// Create a "Graphics" object from an "IToolkitGraphics" handler.
	public static Graphics CreateGraphics(IToolkitGraphics graphics, Rectangle baseWindow)
			{
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics");
				}
				return new Graphics(graphics, baseWindow);
			}
	
	// Create a "Graphics" object from an "IToolkitGraphics" handler.
	public static Graphics CreateGraphics(Graphics graphics, Rectangle baseWindow)
			{
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics");
				}
				return new Graphics(graphics, baseWindow);
			}

	// Create a "Graphics" object from an "IToolkitGraphics" handler.
	// Start with a clip that has already been set in the underlying IToolkitGraphics.
	public static Graphics CreateGraphics(IToolkitGraphics graphics, Region clip)
			{
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics");
				}
				Graphics createdGraphics = new Graphics(graphics);
				createdGraphics.SetClipInternal(clip);
				return createdGraphics;
			}

	// Draw a bitmap-based glyph to a "Graphics" object.  "bits" must be
	// in the form of an xbm bitmap.
	public static void DrawGlyph(Graphics graphics, int x, int y,
								 byte[] bits, int bitsWidth, int bitsHeight,
								 Color color)
			{
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics", "Argument cannot be null");
				}

				graphics.DrawGlyph(x, y, bits, bitsWidth, bitsHeight, color);
			}

	// Get the raw frame data for an "Image" or "Icon" object.
	public static DotGNU.Images.Frame GetImageFrame(Object image)
			{
				if(image is System.Drawing.Image)
				{
					return ((System.Drawing.Image)image).dgImage.GetFrame(0);
				}
				else if(image is System.Drawing.Icon)
				{
					return ((System.Drawing.Icon)image).frame;
				}
				else
				{
					return null;
				}
			}

	// Convert a brush into an exclusive-OR brush which XOR's the brush
	// against a drawing surface, and also includes inferior child
	// windows in the draw operation.
	public static Brush CreateXorBrush(Brush brush)
			{
				return new XorBrush(brush);
			}

	//
	// This is a helper class for the active toolkit.
	// It will be initialized on the first access to the
	// ToolkitManager.Toolkit property getter.
	//
	private sealed class ToolkitHandler
	{
		public static IToolkit toolkit;

		static ToolkitHandler()
				{
					if(ToolkitManager.toolkitOverride != null)
					{
						toolkit = ToolkitManager.toolkitOverride;
					}
					else
					{
						toolkit = CreateDefaultToolkit();
					}
				}

		// Determine if this platform appears to be Unix-ish.
		private static bool IsUnix()
				{
				#if !ECMA_COMPAT || CONFIG_FRAMEWORK_2_0
					return (Environment.OSVersion.Platform == PlatformID.Unix);
				#else
					if(Path.DirectorySeparatorChar == '\\' ||
					   Path.AltDirectorySeparatorChar == '\\')
					{
						return false;
					}
					else
					{
						return true;
					}
				#endif
				}

		// Get the override toolkit name.
		private static String GetToolkitOverride()
				{
					String name;

					// Search for "--toolkit" in the command-line options.
					String[] args = Environment.GetCommandLineArgs();
					int index;
					name = null;
					for(index = 1; index < args.Length; ++index)
					{
						if(args[index] == "--toolkit")
						{
							if((index + 1) < args.Length)
							{
								name = args[index + 1];
								break;
							}
						}
						else if(args[index].StartsWith("--toolkit="))
						{
							name = args[index].Substring(10);
							break;
						}
					}

					// Check the environment next.
					if(name == null)
					{
						name = Environment.GetEnvironmentVariable
							("PNET_WINFORMS_TOOLKIT");
					}

					// Bail out if no toolkit name specified.
					if(name == null || name.Length == 0)
					{
						return null;
					}

					// Prepend "System.Drawing." if necessary.
					if(name.IndexOf('.') == -1)
					{
						name = "System.Drawing." + name;
					}
					return name;
				}

		// Create the default toolkit.
		private static IToolkit CreateDefaultToolkit()
				{
				#if CONFIG_REFLECTION
					// Determine the name of the toolkit we wish to use.
					String name = GetToolkitOverride();
					if(name == null)
					{
						if(IsUnix())
						{
							name = "System.Drawing.Xsharp";
						}
						else
						{
							name = "System.Drawing.Win32";
						}
					}

					// Load the toolkit's assembly.
					Assembly assembly = Assembly.Load(name);

					// Find the "System.Drawing.Toolkit.DrawingToolkit" class.
					Type type = assembly.GetType
						("System.Drawing.Toolkit.DrawingToolkit");
					if(type == null)
					{
						throw new NotSupportedException();
					}

					// Instantiate "DrawingToolkit" and return it.
					ConstructorInfo ctor = type.GetConstructor(new Type [0]);
					return (IToolkit)(ctor.Invoke(new Object [0]));
				#else
					return new NullToolkit();
				#endif
			}
	}

	private sealed class PrintingSystemHandler
	{
		public static IToolkitPrintingSystem printingSystem;

		static PrintingSystemHandler()
				{
					if(ToolkitManager.printingSystemOverride != null)
					{
						printingSystem = ToolkitManager.printingSystemOverride;
					}
					else
					{
						if(IsUnix())
						{
							printingSystem = new UnixPrintingSystem();
						}
						else
						{
							// TODO: Win32 printing system.
							printingSystem = new NullPrintingSystem();
						}
					}
				}

		// Determine if this platform appears to be Unix-ish.
		private static bool IsUnix()
				{
				#if !ECMA_COMPAT || CONFIG_FRAMEWORK_2_0
					return (Environment.OSVersion.Platform == PlatformID.Unix);
				#else
					if(Path.DirectorySeparatorChar == '\\' ||
					   Path.AltDirectorySeparatorChar == '\\')
					{
						return false;
					}
					else
					{
						return true;
					}
				#endif
				}
	}

}; // class ToolkitManager

}; // namespace System.Drawing.Toolkit
