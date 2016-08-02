/*
 * HostServices.cs - Implementation of the
 *			"Microsoft.VisualBasic.HostServices" class.
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

#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class HostServices
{
	// Internal state.
	private static IVbHost host;

	// Get or set the global VB host.
	public static IVbHost VBHost
			{
				get
				{
					return host;
				}
				set
				{
					host = value;
				}
			}

}; // class HostServices

}; // namespace Microsoft.VisualBasic.CompilerServices
