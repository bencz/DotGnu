/*
 * SurrogateSelector.cs - Implementation of the
 *			"System.Runtime.Serialization.SurrogateSelector" interface.
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

namespace System.Runtime.Serialization
{

#if CONFIG_SERIALIZATION

using System.Collections;

public class SurrogateSelector : ISurrogateSelector
{
	// Internal state.
	private ISurrogateSelector nextSelector;
	private Hashtable table;

	// Key value for the surrogate table.
	private class KeyInfo
	{
		// Internal state.
		private Type type;
		private StreamingContext context;

		// Constructor.
		public KeyInfo(Type type, StreamingContext context)
				{
					this.type = type;
					this.context = context;
				}

		// Check two KeyInfo objects for equality.
		public override bool Equals(Object obj)
				{
					KeyInfo info = (obj as KeyInfo);
					if(info != null)
					{
						return (info.type == type &&
							    info.context.Equals(context));
					}
					return false;
				}

	}; // class KeyInfo

	// Constructor.
	public SurrogateSelector()
			{
				table = new Hashtable();
			}

	// Determine if a selector is in a list of selectors.
	private static bool IsInList(ISurrogateSelector selector,
								 ISurrogateSelector list)
			{
				while(list != null)
				{
					if(list == selector)
					{
						return true;
					}
					list = list.GetNextSelector();
				}
				return false;
			}

	// Implement the ISurrogateSelector interface.
	public virtual void ChainSelector(ISurrogateSelector selector)
			{
				// Validate the parameter.
				if(selector == null)
				{
					throw new ArgumentNullException("selector");
				}
				else if(IsInList(this, selector))
				{
					throw new ArgumentException
						(_("Security_SelectorCycle"));
				}
				ISurrogateSelector temp = selector;
				ISurrogateSelector last = null;
				while(temp != null)
				{
					if(IsInList(temp, this))
					{
						throw new ArgumentException
							(_("Security_SelectorCycle"));
					}
					last = temp;
					temp = temp.GetNextSelector();
				}

				// Add the selector just after this one.
				temp = nextSelector;
				nextSelector = selector;

				// Move the original "next selector" to the end
				// of the list for "selector".
				if(temp != null)
				{
					last.ChainSelector(temp);
				}
			}
	public virtual ISurrogateSelector GetNextSelector()
			{
				return nextSelector;
			}
	public virtual ISerializationSurrogate GetSurrogate
					(Type type, StreamingContext context,
					 out ISurrogateSelector selector)
			{
				// Validate the type parameter.
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}

				// Look for a surrogate in this selector.
				ISerializationSurrogate surrogate;
				surrogate = (table[new KeyInfo(type, context)]
								as ISerializationSurrogate);
				if(surrogate != null)
				{
					selector = this;
					return surrogate;
				}

				// Look for a surrogate in the next selector.
				if(nextSelector != null)
				{
					return nextSelector.GetSurrogate
						(type, context, out selector);
				}

				// We were unable to find a surrogate.
				selector = this;
				return null;
			}

	// Add a surrogate for a specific type.
	public virtual void AddSurrogate(Type type, StreamingContext context,
									 ISerializationSurrogate surrogate)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				if(surrogate == null)
				{
					throw new ArgumentNullException("surrogate");
				}
				table.Add(new KeyInfo(type, context), surrogate);
			}

	// Remove the surrogate for a specific type.
	public virtual void RemoveSurrogate(Type type, StreamingContext context)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				table.Remove(new KeyInfo(type, context));
			}

}; // class SurrogateSelector

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
