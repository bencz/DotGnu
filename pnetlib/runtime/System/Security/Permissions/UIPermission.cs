/*
 * UIPermission.cs - Implementation of the
 *		"System.Security.Permissions.UIPermission" class.
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

public sealed class UIPermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private UIPermissionWindow window;
	private UIPermissionClipboard clipboard;

	// Constructors.
	public UIPermission(PermissionState state)
			{
				if(state == PermissionState.None)
				{
					window = UIPermissionWindow.NoWindows;
					clipboard = UIPermissionClipboard.NoClipboard;
				}
				else if(state == PermissionState.Unrestricted)
				{
					window = UIPermissionWindow.AllWindows;
					clipboard = UIPermissionClipboard.AllClipboard;
				}
				else
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
			}
	public UIPermission(UIPermissionWindow windowFlag)
			{
				if(windowFlag < UIPermissionWindow.NoWindows ||
				   windowFlag > UIPermissionWindow.AllWindows)
				{
					throw new ArgumentException(_("Arg_WindowFlag"));
				}
				window = windowFlag;
				clipboard = UIPermissionClipboard.NoClipboard;
			}
	public UIPermission(UIPermissionClipboard clipboardFlag)
			{
				if(clipboardFlag < UIPermissionClipboard.NoClipboard ||
				   clipboardFlag > UIPermissionClipboard.AllClipboard)
				{
					throw new ArgumentException(_("Arg_ClipboardFlag"));
				}
				window = UIPermissionWindow.NoWindows;
				clipboard = clipboardFlag;
			}
	public UIPermission(UIPermissionWindow windowFlag,
					    UIPermissionClipboard clipboardFlag)
			{
				if(windowFlag < UIPermissionWindow.NoWindows ||
				   windowFlag > UIPermissionWindow.AllWindows)
				{
					throw new ArgumentException(_("Arg_WindowFlag"));
				}
				if(clipboardFlag < UIPermissionClipboard.NoClipboard ||
				   clipboardFlag > UIPermissionClipboard.AllClipboard)
				{
					throw new ArgumentException(_("Arg_ClipboardFlag"));
				}
				window = windowFlag;
				clipboard = clipboardFlag;
			}

	// Convert an XML value into a permissions value.
	public override void FromXml(SecurityElement esd)
			{
				String value;
				if(esd == null)
				{
					throw new ArgumentNullException("esd");
				}
				if(esd.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Arg_PermissionVersion"));
				}
				value = esd.Attribute("Unrestricted");
				if(value != null && Boolean.Parse(value))
				{
					window = UIPermissionWindow.NoWindows;
					clipboard = UIPermissionClipboard.NoClipboard;
				}
				else
				{
					value = esd.Attribute("Window");
					if(value != null)
					{
						window = (UIPermissionWindow)
							Enum.Parse(typeof(UIPermissionWindow), value);
					}
					else
					{
						window = UIPermissionWindow.NoWindows;
					}
					value = esd.Attribute("Clipboard");
					if(value != null)
					{
						clipboard = (UIPermissionClipboard)
							Enum.Parse(typeof(UIPermissionClipboard), value);
					}
					else
					{
						clipboard = UIPermissionClipboard.NoClipboard;
					}
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(UIPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(IsUnrestricted())
				{
					element.AddAttribute("Unrestricted", "true");
				}
				else
				{
					element.AddAttribute("Window", window.ToString());
					element.AddAttribute("Clipboard", clipboard.ToString());
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new UIPermission(window, clipboard);
			}
	public override IPermission Intersect(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return target;
				}
				else if(!(target is UIPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((UIPermission)target)
							.IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}

				// Get the minimum flag values.
				UIPermissionWindow w = ((UIPermission)target).window;
				if(((int)w) > ((int)window))
				{
					w = window;
				}
				UIPermissionClipboard c = ((UIPermission)target).clipboard;
				if(((int)c) > ((int)clipboard))
				{
					c = clipboard;
				}

				// Create a new object for the intersection.
				if(w == UIPermissionWindow.NoWindows &&
				   c == UIPermissionClipboard.NoClipboard)
				{
					return null;
				}
				else
				{
					return new UIPermission(w, c);
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (window == UIPermissionWindow.NoWindows &&
							clipboard == UIPermissionClipboard.NoClipboard);
				}
				else if(!(target is UIPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((UIPermission)target)
							.IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else if(((int)window) >
							((int)(((UIPermission)target).window)))
				{
					return false;
				}
				else if(((int)clipboard) >
							((int)(((UIPermission)target).clipboard)))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
	public override IPermission Union(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is UIPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((UIPermission)target).IsUnrestricted())
				{
					return new UIPermission(PermissionState.Unrestricted);
				}

				// Get the maximum flag values.
				UIPermissionWindow w = ((UIPermission)target).window;
				if(((int)w) < ((int)window))
				{
					w = window;
				}
				UIPermissionClipboard c = ((UIPermission)target).clipboard;
				if(((int)c) < ((int)clipboard))
				{
					c = clipboard;
				}

				// Create a new object for the union.
				if(w == UIPermissionWindow.NoWindows &&
				   c == UIPermissionClipboard.NoClipboard)
				{
					return null;
				}
				else
				{
					return new UIPermission(w, c);
				}
			}

	// Determine if this object has unrestricted permissions.
	public bool IsUnrestricted()
			{
				return (window == UIPermissionWindow.AllWindows &&
						clipboard == UIPermissionClipboard.AllClipboard);
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
					if(value < UIPermissionWindow.NoWindows ||
					   value > UIPermissionWindow.AllWindows)
					{
						throw new ArgumentException(_("Arg_WindowFlag"));
					}
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
					if(value < UIPermissionClipboard.NoClipboard ||
					   value > UIPermissionClipboard.AllClipboard)
					{
						throw new ArgumentException(_("Arg_ClipboardFlag"));
					}
					clipboard = value;
				}
			}

}; // class UIPermission

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
