/*
 * _I18NCompareInfo.cs - Implementation of the
 *		"System.Globalization._I18NCompareInfo" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Globalization
{

#if CONFIG_REFLECTION

// This class exists to allow us to inherit from "CompareInfo" within
// the "I18N" code.  It must not be used in application programs.  There
// really isn't any other way of doing this because the specification
// says that "CompareInfo" does not have a public constructor.

[CLSCompliant(false)]
[NonStandardExtra]
public abstract class _I18NCompareInfo : CompareInfo
{
	// Constructors.
	public _I18NCompareInfo(int culture) : base(culture) {}

	// Compare two strings.  Note: "string1" or "string2" may be NULL
	// if "offsetX" and "lengthX" are both zero for that string.  Null
	// strings are treated like the empty string during comparisons.
	public abstract int CompareImpl(String string1, int offset1, int length1,
								    String string2, int offset2, int length2,
									CompareOptions options);

	// Get the sort key data for a string.
	public abstract byte[] GetSortKeyImpl(String source,
										  CompareOptions options);

}; // class _I18NCompareInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Globalization
