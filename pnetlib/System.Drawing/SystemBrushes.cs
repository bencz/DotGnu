/*
 * SystemBrushes.cs - Implementation of the
 *			"System.Drawing.SystemBrushes" class.
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

namespace System.Drawing
{

public sealed class SystemBrushes
{
	// Internal state.
	private static Brush[] systemBrushes;

	// Cannot instantiate this class.
	private SystemBrushes() {}

	// Get a brush for a system color.
	public static Brush FromSystemColor(Color c)
			{
				if(c.IsSystemColor)
				{
					return GetOrCreateBrush(c.ToKnownColor());
				}
				else
				{
					throw new ArgumentException(S._("Arg_NotSystemColor"));
				}
			}

	// Get or create a system brush.
	private static Brush GetOrCreateBrush(KnownColor color)
			{
				lock(typeof(SystemBrushes))
				{
					if(systemBrushes == null)
					{
						systemBrushes = new Brush
							[((int)(KnownColor.WindowText)) -
							 ((int)(KnownColor.ActiveBorder)) + 1];
					}
					int index = ((int)color) - ((int)(KnownColor.ActiveBorder));
					if(systemBrushes[index] == null)
					{
						systemBrushes[index]
							= new SolidBrush(new Color(color));
					}
					return systemBrushes[index];
				}
			}

	// Standard system brush objects.
	public static Brush ActiveBorder
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ActiveBorder);
				}
			}
	public static Brush ActiveCaption
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ActiveCaption);
				}
			}
	public static Brush ActiveCaptionText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ActiveCaptionText);
				}
			}
	public static Brush AppWorkspace
			{
				get
				{
					return GetOrCreateBrush(KnownColor.AppWorkspace);
				}
			}
	public static Brush Control
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Control);
				}
			}
	public static Brush ControlDark
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ControlDark);
				}
			}
	public static Brush ControlDarkDark
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ControlDarkDark);
				}
			}
	public static Brush ControlLight
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ControlLight);
				}
			}
	public static Brush ControlLightLight
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ControlLightLight);
				}
			}
	public static Brush ControlText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ControlText);
				}
			}
	public static Brush Desktop
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Desktop);
				}
			}
	public static Brush GrayText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.GrayText);
				}
			}
	public static Brush Highlight
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Highlight);
				}
			}
	public static Brush HighlightText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.HighlightText);
				}
			}
	public static Brush HotTrack
			{
				get
				{
					return GetOrCreateBrush(KnownColor.HotTrack);
				}
			}
	public static Brush InactiveBorder
			{
				get
				{
					return GetOrCreateBrush(KnownColor.InactiveBorder);
				}
			}
	public static Brush InactiveCaption
			{
				get
				{
					return GetOrCreateBrush(KnownColor.InactiveCaption);
				}
			}
	public static Brush InactiveCaptionText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.InactiveCaptionText);
				}
			}
	public static Brush Info
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Info);
				}
			}
	public static Brush InfoText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.InfoText);
				}
			}
	public static Brush Menu
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Menu);
				}
			}
	public static Brush MenuText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.MenuText);
				}
			}
	public static Brush ScrollBar
			{
				get
				{
					return GetOrCreateBrush(KnownColor.ScrollBar);
				}
			}
	public static Brush Window
			{
				get
				{
					return GetOrCreateBrush(KnownColor.Window);
				}
			}
	public static Brush WindowFrame
			{
				get
				{
					return GetOrCreateBrush(KnownColor.WindowFrame);
				}
			}
	public static Brush WindowText
			{
				get
				{
					return GetOrCreateBrush(KnownColor.WindowText);
				}
			}

}; // class SystemBrushes

}; // namespace System.Drawing
