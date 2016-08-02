/*
 * XClockEmbed.cs - Embed "xclock" in an Xsharp app using XC-APPGROUP.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

using System;
using Xsharp;

public class XClockEmbed : TopLevelWindow
{
#if CONFIG_EXTENDED_DIAGNOSTICS
	// Main entry point.
	public static void Main(String[] args)
	{
		Application app = new Application("XClockEmbed", args);
		XClockEmbed topLevel = new XClockEmbed("Embedded Clock", 200, 200);
		topLevel.Map();
		app.Run();
		app.Close();
	}

	// Internal state.
	private EmbeddedApplication embed;

	// Constructor.
	public XClockEmbed(String title, int width, int height)
		: base(title, width, height)
	{
		embed = new EmbeddedApplication(this, 0, 0, width, height);
		embed.Program = "xclock";
		embed.Launch();
	}

	// Handle resizes of the main application window.
	protected override void OnMoveResize(int x, int y, int width, int height)
	{
		embed.Resize(width, height);
	}
#else
	public XClockEmbed(String title, int width, int height)
		: base(title, width, height) {}
	public static void Main(String[] args) {}
#endif

}; // class XClockEmbed
