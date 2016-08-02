/*
 * AspNetHostingPermissionLevel.cs - Implementation of the
 *		"System.Web.AspNetHostingPermissionLevel" class.
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

namespace System.Web
{

#if CONFIG_PERMISSIONS && !ECMA_COMPAT

[Serializable]
public enum AspNetHostingPermissionLevel
{
	None			= 100,
	Minimal			= 200,
	Low				= 300,
	Medium			= 400,
	High			= 500,
	Unrestricted	= 600

}; // enum AspNetHostingPermissionLevel

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Web
