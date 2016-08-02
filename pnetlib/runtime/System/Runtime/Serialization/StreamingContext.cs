/*
 * StreamingContext.cs - Implementation of the
 *			"System.Runtime.Serialization.StreamingContext" structure.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

public struct StreamingContext
{
	// Internal state.
	private StreamingContextStates state;
	private Object additional;

	// Constructors.
	public StreamingContext(StreamingContextStates state)
			{
				this.state = state;
				this.additional = null;
			}
	public StreamingContext(StreamingContextStates state, Object additional)
			{
				this.state = state;
				this.additional = additional;
			}

	// Get the additional context object.
	public Object Context
			{
				get
				{
					return additional;
				}
			}

	// Get the streaming context state flags.
	public StreamingContextStates State
			{
				get
				{
					return state;
				}
			}

	// Determine if two StreamingContext values are equal.
	public override bool Equals(Object obj)
			{
				if(obj != null && obj is StreamingContext)
				{
					StreamingContext other = (StreamingContext)obj;
					return (state == other.state &&
							additional == other.additional);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this StreamingContext instance.
	public override int GetHashCode()
			{
				return (int)state;
			}

}; // struct StreamingContext

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
