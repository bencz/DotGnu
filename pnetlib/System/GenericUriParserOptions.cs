/*
 * GenericUriParserOptions.cs - Implementation of the
 *								"System.GenericUriParserOptions" enumeration.
 *
 * Copyright (C) 2009  Free Software Foundation Inc.
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

namespace System
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

[Flags]
public enum GenericUriParserOptions
{

	Default							= 0x0000,
	GenericAuthority				= 0x0001,
	AllowEmptyAuthority				= 0x0002,
	NoUserInfo						= 0x0004,
	NoPort							= 0x0008,
	NoQuery							= 0x0010,
	NoFragment						= 0x0020,
	DontConvertPathBackslashes		= 0x0040,
	DontCompressPath				= 0x0080,
	DontUnescapePathDotsAndSlashes	= 0x0100,
	Idn								= 0x0200,
	IriParsing						= 0x0400

}; // enum GenericUriParserOptions

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System
