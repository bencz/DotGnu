/*
 * XHello.cs - Sample program for Xsharp.
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

public class XHello : TopLevelWindow
{
	// Main entry point.
	public static void Main(String[] args)
	{
		Application app = new Application("XHello", args);
		Image image = new Image("dotgnu-logo.bmp");
		XHello topLevel = new XHello
			("Hello DotGNU!", image.Width, image.Height, image);
		topLevel.Map();
		app.Run();
		app.Close();
	}

	// Internal state.
	private Image image;

	// Constructor.
	public XHello(String title, int width, int height, Image image)
		: base(title, width, height)
	{
		this.image = image;
		this.Background = new Color(0, 0, 0);
	}

	// Handle paint requests.
	protected override void OnPaint(Graphics graphics)
	{
		graphics.DrawImage(0, 0, image);
	}

}; // class XHello
