/*
 * DrawingRootTopLevelWindow.cs - This is the main windows form for the application
 * Copyright (C) 2003  Neil Cawse.
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

using System;

internal class DrawingRootTopLevelWindow : DrawingTopLevelWindow
{
	public DrawingRootTopLevelWindow(DrawingToolkit toolkit, String name,
		int width, int height, IToolkitEventSink sink) : base (toolkit, name, width, height, sink) {}

	//TODO
	/*wParam 
When the system sends this message as a result of a SystemParametersInfo call, wParam is a flag that indicates the system parameter that was changed. For a list of values, see SystemParametersInfo. 
When the system sends this message as a result of a change in policy settings, this parameter indicates the type of policy that was applied. This value is 1 if computer policy was applied or zero if user policy was applied.

When the system sends this message as a result of a change in locale settings, this parameter is zero.

When an application sends this message, this parameter must be NULL.
*/
	//TODO
	internal override void SettingsChange(int wParam)
	{
		
	}
}
}
