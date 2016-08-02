/*
 * DcopSample.cs - Simple Dcop sample to open a URL in Konqueror
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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


using System;
using Xsharp;
using Xsharp.Dcop;

public class DcopSample
{
	public static void Main(String[] args)
	{
		String url = "http://dotgnu.org/";
		if(args.Length != 0 && args[0].StartsWith("http://"))
		{
			url = args[0];
		}
		Application app = new Application("DcopSample", args);
		DcopClient dc = new DcopClient(app.Display, null);
		DcopRef dr = new DcopRef();
		dr.DiscoverApplication("konqueror", true, true);
		dr.Obj = "KonquerorIface";
		dr.Initialise();
		dr.Call("DCOPRef openBrowserWindow(QString url)", url);
	}
}


