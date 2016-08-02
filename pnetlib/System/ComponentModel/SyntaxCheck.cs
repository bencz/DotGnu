/*
 * SyntaxCheck.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.SyntaxCheck" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;
using System.IO;

public class SyntaxCheck
{
	// Cannot instantiate this class.
	private SyntaxCheck() {}

	// Perform a syntax check on a machine name.
	public static bool CheckMachineName(String value)
			{
				if(value != null)
				{
					value = value.Trim();
					if(value.Length == 0)
					{
						return false;
					}
					if(value.IndexOf('\\') != -1 ||
					   value.IndexOf('/') != -1)
					{
						return false;
					}
					return true;
				}
				else
				{
					return false;
				}
			}

	// Perform a syntax check on a network share path.
	public static bool CheckPath(String value)
			{
				if(value != null)
				{
					value = value.Trim();
					if(value.Length >= 2 &&
					   (value[0] == '\\' || value[0] == '/') &&
					   (value[1] == '\\' || value[1] == '/'))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

	// Perform a syntax check on a rooted path.
	public static bool CheckRootedPath(String value)
			{
				if(value != null)
				{
					value = value.Trim();
					if(value.Length > 0)
					{
						return Path.IsPathRooted(value);
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

}; // class SyntaxCheck

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
