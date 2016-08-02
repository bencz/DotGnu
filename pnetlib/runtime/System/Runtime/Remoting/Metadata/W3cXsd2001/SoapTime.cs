/*
 * SoapTime.cs - Implementation of the
 *		"System.Runtime.Remoting.Metadata.W3cXsd2001.SoapTime" class.
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

[Serializable]
public sealed class SoapTime : ISoapXsd
{
	// Internal state.
	private DateTime value;

	// Constructors.
	public SoapTime() {}
	public SoapTime(DateTime value)
			{
				this.value = value;
			}

	// Get or set this object's value.
	public DateTime Value
			{
				get
				{
					return value;
				}
				set
				{
					this.value = value;
				}
			}

	// Get the schema type for this class.
	public static String XsdType
			{
				get
				{
					return "time";
				}
			}

	// Implement the ISoapXsd interface.
	public String GetXsdType()
			{
				return XsdType;
			}

	// Format values for "Parse".
	private static String[] formats = {
				"HH:mm:ss.fffffffzzz",
				"HH:mm:ss.ffff",
				"HH:mm:ss.ffffzzz",
				"HH:mm:ss.fff",
				"HH:mm:ss.fffzzz",
				"HH:mm:ss.ff",
				"HH:mm:ss.ffzzz",
				"HH:mm:ss.f",
				"HH:mm:ss.fzzz",
				"HH:mm:ss", 
				"HH:mm:sszzz",
				"HH:mm:ss.fffff",
				"HH:mm:ss.fffffzzz",
				"HH:mm:ss.ffffff",
				"HH:mm:ss.ffffffzzz",
				"HH:mm:ss.fffffff",
				"HH:mm:ss.ffffffff",
				"HH:mm:ss.ffffffffzzz",
				"HH:mm:ss.fffffffff",
				"HH:mm:ss.fffffffffzzz",
				"HH:mm:ss.fffffffff",
				"HH:mm:ss.fffffffffzzz"
			};

	// Parse a value into an instance of this class.
	public static SoapTime Parse(String value)
			{
				if(value != null && value.EndsWith("Z"))
				{
					value = value.Substring(0, value.Length - 1) + "-00:00";
				}
				return new SoapTime(DateTime.ParseExact
					(value, formats, CultureInfo.InvariantCulture,
					 DateTimeStyles.None));
			}

	// Convert this object into a string.
	public override String ToString()
			{
				DateTime time = DateTime.Today + value.TimeOfDay;
				return time.ToString("HH:mm:ss.fffffffzzz",
									 CultureInfo.InvariantCulture);
			}

}; // class SoapTime

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Metadata.W3cXsd2001
