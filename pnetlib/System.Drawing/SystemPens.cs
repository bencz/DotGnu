/*
 * SystemPens.cs - Implementation of the
 *			"System.Drawing.SystemPens" class.
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

public sealed class SystemPens
{
	// Internal state.
	private static Pen[] systemPens;

	// Cannot instantiate this class.
	private SystemPens() {}

	// Get a pen for a system color.
	public static Pen FromSystemColor(Color c)
			{
				if(c.IsSystemColor)
				{
					return GetOrCreatePen(c.ToKnownColor());
				}
				else
				{
					throw new ArgumentException(S._("Arg_NotSystemColor"));
				}
			}

	// Get or create a system pen.
	private static Pen GetOrCreatePen(KnownColor color)
			{
				lock(typeof(SystemPens))
				{
					if(systemPens == null)
					{
						systemPens = new Pen
							[((int)(KnownColor.WindowText)) -
							 ((int)(KnownColor.ActiveBorder)) + 1];
					}
					int index = ((int)color) - ((int)(KnownColor.ActiveBorder));
					if(systemPens[index] == null)
					{
						systemPens[index] = new Pen(new Color(color));
					}
					return systemPens[index];
				}
			}

	// Standard system brush objects.
	public static Pen ActiveBorder
			{
				get
				{
					return GetOrCreatePen(KnownColor.ActiveBorder);
				}
			}
	public static Pen ActiveCaption
			{
				get
				{
					return GetOrCreatePen(KnownColor.ActiveCaption);
				}
			}
	public static Pen ActiveCaptionText
			{
				get
				{
					return GetOrCreatePen(KnownColor.ActiveCaptionText);
				}
			}
	public static Pen AppWorkspace
			{
				get
				{
					return GetOrCreatePen(KnownColor.AppWorkspace);
				}
			}
	public static Pen Control
			{
				get
				{
					return GetOrCreatePen(KnownColor.Control);
				}
			}
	public static Pen ControlDark
			{
				get
				{
					return GetOrCreatePen(KnownColor.ControlDark);
				}
			}
	public static Pen ControlDarkDark
			{
				get
				{
					return GetOrCreatePen(KnownColor.ControlDarkDark);
				}
			}
	public static Pen ControlLight
			{
				get
				{
					return GetOrCreatePen(KnownColor.ControlLight);
				}
			}
	public static Pen ControlLightLight
			{
				get
				{
					return GetOrCreatePen(KnownColor.ControlLightLight);
				}
			}
	public static Pen ControlText
			{
				get
				{
					return GetOrCreatePen(KnownColor.ControlText);
				}
			}
	public static Pen Desktop
			{
				get
				{
					return GetOrCreatePen(KnownColor.Desktop);
				}
			}
	public static Pen GrayText
			{
				get
				{
					return GetOrCreatePen(KnownColor.GrayText);
				}
			}
	public static Pen Highlight
			{
				get
				{
					return GetOrCreatePen(KnownColor.Highlight);
				}
			}
	public static Pen HighlightText
			{
				get
				{
					return GetOrCreatePen(KnownColor.HighlightText);
				}
			}
	public static Pen HotTrack
			{
				get
				{
					return GetOrCreatePen(KnownColor.HotTrack);
				}
			}
	public static Pen InactiveBorder
			{
				get
				{
					return GetOrCreatePen(KnownColor.InactiveBorder);
				}
			}
	public static Pen InactiveCaption
			{
				get
				{
					return GetOrCreatePen(KnownColor.InactiveCaption);
				}
			}
	public static Pen InactiveCaptionText
			{
				get
				{
					return GetOrCreatePen(KnownColor.InactiveCaptionText);
				}
			}
	public static Pen Info
			{
				get
				{
					return GetOrCreatePen(KnownColor.Info);
				}
			}
	public static Pen InfoText
			{
				get
				{
					return GetOrCreatePen(KnownColor.InfoText);
				}
			}
	public static Pen Menu
			{
				get
				{
					return GetOrCreatePen(KnownColor.Menu);
				}
			}
	public static Pen MenuText
			{
				get
				{
					return GetOrCreatePen(KnownColor.MenuText);
				}
			}
	public static Pen ScrollBar
			{
				get
				{
					return GetOrCreatePen(KnownColor.ScrollBar);
				}
			}
	public static Pen Window
			{
				get
				{
					return GetOrCreatePen(KnownColor.Window);
				}
			}
	public static Pen WindowFrame
			{
				get
				{
					return GetOrCreatePen(KnownColor.WindowFrame);
				}
			}
	public static Pen WindowText
			{
				get
				{
					return GetOrCreatePen(KnownColor.WindowText);
				}
			}

}; // class SystemPens

}; // namespace System.Drawing
