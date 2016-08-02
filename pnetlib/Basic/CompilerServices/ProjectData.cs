/*
 * ProjectData.cs - Implementation of the
 *			"Microsoft.VisualBasic.ProjectData" class.
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

namespace Microsoft.VisualBasic.CompilerServices
{

using System;
using System.ComponentModel;
using System.Reflection;

#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class ProjectData
{
	// Cannot instantiate this class.
	private ProjectData() {}

	// Destructor - not used but must be declared.
	~ProjectData() {}

	// Clear the project error.
	public static void ClearProjectError()
			{
				Information.Err().Clear();
			}

	// Create a new project error.
	public static Exception CreateProjectError(int hr)
			{
				ClearProjectError();
				hr = ErrObject.HResultToNumber(hr);
				Exception e = ErrObject.CreateExceptionFromNumber(hr, null);
				Information.Err().Number = hr;
				return e;
			}

	// End the application.
	public static void EndApp()
			{
				File.CloseAll(Assembly.GetCallingAssembly());
				Environment.Exit(0);
			}

	// Set the project error.
	public static void SetProjectError(Exception ex)
			{
				Information.Err().SetException(ex);
			}
	public static void SetProjectError(Exception ex, int lErl)
			{
				Information.Err().SetException(ex, lErl);
			}

}; // class ProjectData

}; // namespace Microsoft.VisualBasic.CompilerServices
