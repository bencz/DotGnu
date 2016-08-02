/*
 * SoapDateTime.cs - Implementation of the
 *		"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapDateTime" class.
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

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{

#if CONFIG_SERIALIZATION

using System.Globalization;

public sealed class SoapDateTime
{
	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "dateTime";
				}
			}

	// Format values for "Parse".
	private static String[] formats = {
				"yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.ffff",
				"yyyy-MM-dd'T'HH:mm:ss.ffffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.fff",
				"yyyy-MM-dd'T'HH:mm:ss.fffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.ff",
				"yyyy-MM-dd'T'HH:mm:ss.ffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.f",
				"yyyy-MM-dd'T'HH:mm:ss.fzzz",
				"yyyy-MM-dd'T'HH:mm:ss",
				"yyyy-MM-dd'T'HH:mm:sszzz",
				"yyyy-MM-dd'T'HH:mm:ss.fffff",
				"yyyy-MM-dd'T'HH:mm:ss.fffffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.ffffff",
				"yyyy-MM-dd'T'HH:mm:ss.ffffffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.fffffff",
				"yyyy-MM-dd'T'HH:mm:ss.ffffffff",
				"yyyy-MM-dd'T'HH:mm:ss.ffffffffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.fffffffff",
				"yyyy-MM-dd'T'HH:mm:ss.fffffffffzzz",
				"yyyy-MM-dd'T'HH:mm:ss.ffffffffff",
				"yyyy-MM-dd'T'HH:mm:ss.ffffffffffzzz"
			};

	// Parse a value into an instance of this class.
	public static DateTime Parse(String value)
			{
				if(value == null)
				{
					return DateTime.MinValue;
				}
				else if(value.EndsWith("Z"))
				{
					value = value.Substring(0, value.Length - 1) + "-00:00";
				}
				try
				{
					return DateTime.ParseExact
						(value, formats, CultureInfo.InvariantCulture,
					 	 DateTimeStyles.None);
				}
				catch(Exception)
				{
					throw new RemotingException(_("Arg_InvalidSoapValue"));
				}
			}

	// Convert this object into a string.
	public static String ToString(DateTime value)
			{
				return value.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
									  CultureInfo.InvariantCulture);
			}

}; // class SoapDateTime

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
