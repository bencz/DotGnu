/*
 * ServiceResult.cs - matches the serviceResult struct returned by klauncher.
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

namespace Xsharp.Dcop
{

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ServiceResult 
{
	private int result;
	[NonSerializedAttribute]
	private string dcopName;
	[NonSerializedAttribute]
	private string errorMessage;
	private int pid; // *FIXME*

	public int Result
	{
		get { return result; }
		set { result = value; }
	}

	public string DcopName
	{
		get { return dcopName; }
		set { dcopName = value; }
	}

	public string ErrorMessage
	{
		get { return errorMessage; }
		set { errorMessage = value; }
	}

	public int PID
	{
		get { return pid; }
		set { pid = value; }
	}
}; // struct ServiceResult

} // namespace Xsharp.Dcop
