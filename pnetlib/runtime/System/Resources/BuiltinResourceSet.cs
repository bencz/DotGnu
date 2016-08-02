/*
 * BuiltinResourceSet.cs - Implementation of the
 *		"System.Resources.BuiltinResourceSet" class.
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

namespace System.Resources
{

#if CONFIG_RUNTIME_INFRA

using System;
using System.IO;
using System.Collections;
using System.Globalization;

internal sealed class BuiltinResourceSet : ResourceSet
{
	// Internal state.
	private bool readResourcesCalled;

	// Constructors.
	public BuiltinResourceSet(Stream stream)
			: base()
			{
				Reader = new ResourceReader(stream);
				readResourcesCalled = false;
			}
	public BuiltinResourceSet(String fileName)
			: base()
			{
				Reader = new ResourceReader(fileName);
				readResourcesCalled = false;
			}
	public BuiltinResourceSet(IResourceReader reader)
			: base()
			{
				if(reader == null)
				{
					throw new ArgumentNullException("reader");
				}
				Reader = reader;
				readResourcesCalled = false;
			}

	// Get an object from this resource set.
	public override Object GetObject(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(Reader == null)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceReaderClosed"));
				}
				else if(!readResourcesCalled)
				{
					Object o = base.GetObject(name);
					if( null != o ) 
					{
						return o;
					}
					// We can take a short-cut because we know that
					// the underlying reader is "ResourceReader".
					o = ((ResourceReader)Reader).GetObject(name);
					Table[name] = o;
					return o;
				}
				else
				{
					return Table[name];
				}
			}
	public override Object GetObject(String name, bool ignoreCase)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(Reader == null)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceReaderClosed"));
				}

				Object o = base.GetObject(name,ignoreCase);
				if( null != o ) 
				{
					return o;
				}
				
				// If we haven't read the resources yet, then attempt
				// to use a short-cut by way of "ResourceReader".
				if(!readResourcesCalled)
				{
					Object value = ((ResourceReader)Reader).GetObject(name);
					if(value != null || !ignoreCase)
					{
						Table[name] = value;
						return value;
					}
				}

				// Use the default implementation.
				return base.GetObject(name, ignoreCase);
			}

	// Get a string from this resource set.
	public override String GetString(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(Reader == null)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceReaderClosed"));
				}
				
				String o = base.GetString(name);
				if( null != o ) 
				{
					return o;
				}
				
				try
				{
					if(!readResourcesCalled)
					{
						// We can take a short-cut because we know that
						// the underlying reader is "ResourceReader".
						o = (String)(((ResourceReader)Reader).GetObject(name));
						Table[name] = o;
						return o;
					}
					else
					{
						return (String)(Table[name]);
					}
				}
				catch(InvalidCastException)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceNotString"));
				}
			}
	public override String GetString(String name, bool ignoreCase)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(Reader == null)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceReaderClosed"));
				}

				String o = base.GetString(name,ignoreCase);
				if( null != o ) 
				{
					return o;
				}
				
				// If we haven't read the resources yet, then attempt
				// to use a short-cut by way of "ResourceReader".
				if(!readResourcesCalled)
				{
					Object value = ((ResourceReader)Reader).GetObject(name);
					if(value != null || !ignoreCase)
					{
						try
						{
							Table[name] = value;
							return (String)value;
						}
						catch(InvalidCastException)
						{
							throw new InvalidOperationException
								(_("Invalid_ResourceNotString"));
						}
					}
				}

				// Use the default implementation.
				return base.GetString(name, ignoreCase);
			}

	// Read all resources into the hash table.
	protected override void ReadResources()
			{
				if(!readResourcesCalled)
				{
					IDictionaryEnumerator e = Reader.GetEnumerator();
					while(e.MoveNext())
					{
						Table.Add(e.Key, e.Value);
					}
					readResourcesCalled = true;
				}
			}

	// Get the dictionary enumerator for this instance.
	public override IDictionaryEnumerator GetEnumerator()
			{
				ReadResources();
				return base.GetEnumerator();
			}

}; // class BuiltinResourceSet

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Resources
