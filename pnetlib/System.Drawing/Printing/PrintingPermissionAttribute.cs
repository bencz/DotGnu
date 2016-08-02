/*
 * PrintingPermissionAttribute.cs - Implementation of the
 *			"System.Drawing.Printing.PrintingPermissionAttribute" class.
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

namespace System.Drawing.Printing
{

#if CONFIG_PERMISSIONS

using System;
using System.Security;
using System.Security.Permissions;

[AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
public sealed class PrintingPermissionAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private PrintingPermissionLevel level;

	// Constructors.
	public PrintingPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the printing permission level.
	public PrintingPermissionLevel Level
			{
				get
				{
					return level;
				}
				set
				{
					level = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new PrintingPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new PrintingPermission(level);
				}
			}

}; // class PrintingPermissionAttribute

#endif // CONFIG_PERMISSIONS

}; // namespace System.Drawing.Printing
