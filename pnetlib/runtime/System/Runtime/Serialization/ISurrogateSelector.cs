/*
 * ISurrogateSelector.cs - Implementation of the
 *			"System.Runtime.Serialization.ISurrogateSelector" interface.
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

public interface ISurrogateSelector
{

	// Specify the next ISurrogateSelector in the chain to examine.
	void ChainSelector(ISurrogateSelector selector);

	// Get the next selector in the chain.
	ISurrogateSelector GetNextSelector();

	// Get the surrogate that represents a particular object type.
	ISerializationSurrogate GetSurrogate(Type type, StreamingContext context,
										 out ISurrogateSelector selector);

}; // interface ISurrogateSelector

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
