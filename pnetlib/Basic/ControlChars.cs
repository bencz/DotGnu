/*
 * ControlChars.cs - Implementation of the
 *			"Microsoft.VisualBasic.ControlChars" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic
{

using System;

public sealed class ControlChars
{
	// Constructor.
	public ControlChars() {}

	// Special character values.
	public const char Back			= '\u0008';
	public const char Cr			= '\r';
	public const String CrLf		= "\r\n";
	public const char FormFeed		= '\u000C';
	public const char Lf			= '\n';
	public const String NewLine		= "\r\n";
	public const char NullChar		= '\0';
	public const char Quote			= '"';
	public const char Tab			= '\t';
	public const char VerticalTab	= '\u000B';

}; // class ControlChars

}; // namespace Microsoft.VisualBasic
