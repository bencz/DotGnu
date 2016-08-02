/*
 * NotSupportedByProviderException.cs - Implementation of the
 *		"System.Configuration.Provider.NotSupportedByProviderException" class.
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

namespace System.Configuration.Provider
{

#if CONFIG_FRAMEWORK_1_2

using System.Runtime.Serialization;

[Serializable]
public class NotSupportedByProviderException : Exception
{
	// Constructors.
	public NotSupportedByProviderException() : base() {}
	public NotSupportedByProviderException(String message) : base(message) {}
	public NotSupportedByProviderException(String message, Exception inner)
			: base(message, inner) {}
#if CONFIG_SERIALIZATION
	protected NotSupportedByProviderException(SerializationInfo info,
									 		  StreamingContext context)
			: base(info, context) {}
#endif // CONFIG_SERIALIZATION

}; // class NotSupportedByProviderException

#endif // CONFIG_FRAMEWORK_1_2

}; // namespace System.Configuration.Provider
