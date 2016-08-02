/*
 * StandardToolWindows.cs - Implementation of the
 *		"System.ComponentModel.Design.StandardToolWindows" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

public class StandardToolWindows
{
	// Standard tool window identifier.
	public static readonly Guid ObjectBrowser
			= new Guid("{970d9861-ee83-11d0-a778-00a0c91110c3}");
	public static readonly Guid OutputWindow
			= new Guid("{34e76e81-ee4a-11d0-ae2e-00a0c90fffc3}");
	public static readonly Guid ProjectExplorer
			= new Guid("{3ae79031-e1bc-11d0-8f78-00a0c9110057}");
	public static readonly Guid PropertyBrowser
			= new Guid("{eefa5220-e298-11d0-8f78-00a0c9110057}");
	public static readonly Guid RelatedLinks
			= new Guid("{66dba47c-61df-11d2-aa79-00c04f990343}");
	public static readonly Guid ServerExplorer
			= new Guid("{74946827-37a0-11d2-a273-00c04f8ef4ff}");
	public static readonly Guid TaskList
			= new Guid("{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}");
	public static readonly Guid Toolbox
			= new Guid("{b1e99781-ab81-11d0-b683-00aa00a3ee26}");

}; // class StandardToolWindows

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
