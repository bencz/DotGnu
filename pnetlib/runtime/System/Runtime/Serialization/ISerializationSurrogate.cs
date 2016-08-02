/*
 * ISerializationSurrogate.cs - Implementation of the
 *			"System.Runtime.Serialization.ISerializationSurrogate" interface.
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

namespace System.Runtime.Serialization
{

#if CONFIG_SERIALIZATION

public interface ISerializationSurrogate
{

	// Populate a SerializationInfo instance with serialization data.
	void GetObjectData(Object obj, SerializationInfo info,
					   StreamingContext context);

	// Populate an object using information in a SerializationInfo instance.
	Object SetObjectData(Object obj, SerializationInfo info,
						 StreamingContext context,
						 ISurrogateSelector selector);

}; // interface ISerializationSurrogate

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
