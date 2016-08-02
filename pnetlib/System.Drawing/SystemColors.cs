/*
 * SystemColors.cs - Implementation of the "System.Drawing.SystemColors" class.
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

public sealed class SystemColors
{
	// Cannot instantiate this class.
	private SystemColors() {}

	// Define the system color values as properties.
	public static Color ActiveBorder
			{
				get
				{
					return new Color(KnownColor.ActiveBorder);
				}
			}
	public static Color ActiveCaption
			{
				get
				{
					return new Color(KnownColor.ActiveCaption);
				}
			}
	public static Color ActiveCaptionText
			{
				get
				{
					return new Color(KnownColor.ActiveCaptionText);
				}
			}
	public static Color AppWorkspace
			{
				get
				{
					return new Color(KnownColor.AppWorkspace);
				}
			}
	public static Color Control
			{
				get
				{
					return new Color(KnownColor.Control);
				}
			}
	public static Color ControlDark
			{
				get
				{
					return new Color(KnownColor.ControlDark);
				}
			}
	public static Color ControlDarkDark
			{
				get
				{
					return new Color(KnownColor.ControlDarkDark);
				}
			}
	public static Color ControlLight
			{
				get
				{
					return new Color(KnownColor.ControlLight);
				}
			}
	public static Color ControlLightLight
			{
				get
				{
					return new Color(KnownColor.ControlLightLight);
				}
			}
	public static Color ControlText
			{
				get
				{
					return new Color(KnownColor.ControlText);
				}
			}
	public static Color Desktop
			{
				get
				{
					return new Color(KnownColor.Desktop);
				}
			}
	public static Color GrayText
			{
				get
				{
					return new Color(KnownColor.GrayText);
				}
			}
	public static Color Highlight
			{
				get
				{
					return new Color(KnownColor.Highlight);
				}
			}
	public static Color HighlightText
			{
				get
				{
					return new Color(KnownColor.HighlightText);
				}
			}
	public static Color HotTrack
			{
				get
				{
					return new Color(KnownColor.HotTrack);
				}
			}
	public static Color InactiveBorder
			{
				get
				{
					return new Color(KnownColor.InactiveBorder);
				}
			}
	public static Color InactiveCaption
			{
				get
				{
					return new Color(KnownColor.InactiveCaption);
				}
			}
	public static Color InactiveCaptionText
			{
				get
				{
					return new Color(KnownColor.InactiveCaptionText);
				}
			}
	public static Color Info
			{
				get
				{
					return new Color(KnownColor.Info);
				}
			}
	public static Color InfoText
			{
				get
				{
					return new Color(KnownColor.InfoText);
				}
			}
	public static Color Menu
			{
				get
				{
					return new Color(KnownColor.Menu);
				}
			}
	public static Color MenuText
			{
				get
				{
					return new Color(KnownColor.MenuText);
				}
			}
	public static Color ScrollBar
			{
				get
				{
					return new Color(KnownColor.ScrollBar);
				}
			}
	public static Color Window
			{
				get
				{
					return new Color(KnownColor.Window);
				}
			}
	public static Color WindowFrame
			{
				get
				{
					return new Color(KnownColor.WindowFrame);
				}
			}
	public static Color WindowText
			{
				get
				{
					return new Color(KnownColor.WindowText);
				}
			}

}; // class SystemColors
		
}; // namespace System.Drawing
