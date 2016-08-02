/*
 * UIPermissionAttribute.cs - Implementation of the
 *			"System.Security.Permissions.UIPermissionAttribute" class.
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

namespace System.Security.Permissions
{

#if CONFIG_PERMISSIONS && !ECMA_COMPAT

using System;
using System.Security;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class UIPermissionAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private UIPermissionWindow window;
	private UIPermissionClipboard clipboard;

	// Constructors.
	public UIPermissionAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the window flag.
	public UIPermissionWindow Window
			{
				get
				{
					return window;
				}
				set
				{
					window = value;
				}
			}

	// Get or set the clipboard flag.
	public UIPermissionClipboard Clipboard
			{
				get
				{
					return clipboard;
				}
				set
				{
					clipboard = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new UIPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new UIPermission(window, clipboard);
				}
			}

}; // class UIPermissionAttribute

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
