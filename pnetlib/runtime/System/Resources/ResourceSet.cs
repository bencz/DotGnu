/*
 * ResourceSet.cs - Implementation of the
 *		"System.Resources.ResourceSet" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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
using System.Runtime.InteropServices;

#if ECMA_COMPAT
internal
#else
public
#endif
class ResourceSet : IDisposable, IEnumerable
{
	// Internal state.
	protected IResourceReader Reader;
	protected Hashtable Table;
	private Hashtable ignoreCaseTable;

	// Constructors.
	protected ResourceSet()
			{
				Reader = null;
				Table = new Hashtable();
				ignoreCaseTable = null;
			}
	public ResourceSet(IResourceReader reader)
			{
				if(reader == null)
				{
					throw new ArgumentNullException("reader");
				}
				Reader = reader;
				Table = new Hashtable();
				ignoreCaseTable = null;
				ReadResources();
			}
	public ResourceSet(Stream stream)
			{
				Reader = new ResourceReader(stream);
				Table = new Hashtable();
				ignoreCaseTable = null;
				ReadResources();
			}
	public ResourceSet(String fileName)
			{
				Reader = new ResourceReader(fileName);
				Table = new Hashtable();
				ignoreCaseTable = null;
				ReadResources();
			}

	// Close this resource set.
	public virtual void Close()
			{
				Dispose(true);
			}

	// Dispose this resource set.
	protected virtual void Dispose(bool disposing)
			{
				if(Reader != null)
				{
					Reader.Close();
					Reader = null;
				}
				Table = null;
				ignoreCaseTable = null;
			}

	// Implement IDisposable.
	public void Dispose()
			{
				Dispose(true);
			}

	// Implement IEnumerable.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

	// Get the dictionary enumerator for this instance.
#if !ECMA_COMPAT
	[ComVisible(false)]
#endif
	public virtual IDictionaryEnumerator GetEnumerator()
			{
				return Table.GetEnumerator();
			}

	// Get the default reader type for this resource set.
	public virtual Type GetDefaultReader()
			{
				return typeof(ResourceReader);
			}

	// Get the default write type for this resource set.
	public virtual Type GetDefaultWriter()
			{
				return typeof(ResourceWriter);
			}

	// Create the "ignore case" table from the primary hash table.
	private void CreateIgnoreCaseTable()
			{
				TextInfo info = CultureInfo.InvariantCulture.TextInfo;
				ignoreCaseTable = new Hashtable();
				IDictionaryEnumerator e = Table.GetEnumerator();
				while(e.MoveNext())
				{
					ignoreCaseTable[info.ToLower((String)(e.Key))] = e.Value;
				}
			}

	// Get an object from this resource set.
	public virtual Object GetObject(String name)
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
				else
				{
					return Table[name];
				}
			}
	public virtual Object GetObject(String name, bool ignoreCase)
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

				// Try looking for the resource by exact name.
				Object value = Table[name];
				if(value != null || !ignoreCase)
				{
					return value;
				}

				// Create the "ignore case" table if necessary.
				if(ignoreCaseTable == null)
				{
					CreateIgnoreCaseTable();
				}

				// Look for the resource in the "ignore case" table.
				TextInfo info = CultureInfo.InvariantCulture.TextInfo;
				return ignoreCaseTable[info.ToLower(name)];
			}

	// Get a string from this resource set.
	public virtual String GetString(String name)
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
				try
				{
					return (String)(Table[name]);
				}
				catch(InvalidCastException)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceNotString"));
				}
			}
	public virtual String GetString(String name, bool ignoreCase)
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

				// Try looking for the resource by exact name.
				Object value = Table[name];
				if(value != null || !ignoreCase)
				{
					try
					{
						return (String)value;
					}
					catch(InvalidCastException)
					{
						throw new InvalidOperationException
							(_("Invalid_ResourceNotString"));
					}
				}

				// Create the "ignore case" table if necessary.
				if(ignoreCaseTable == null)
				{
					CreateIgnoreCaseTable();
				}

				// Look for the resource in the "ignore case" table.
				try
				{
					TextInfo info = CultureInfo.InvariantCulture.TextInfo;
					return (String)(ignoreCaseTable[info.ToLower(name)]);
				}
				catch(InvalidCastException)
				{
					throw new InvalidOperationException
						(_("Invalid_ResourceNotString"));
				}
			}

	// Read all resources into the hash table.
	protected virtual void ReadResources()
			{
				IDictionaryEnumerator e = Reader.GetEnumerator();
				while(e.MoveNext())
				{
					Table.Add(e.Key, e.Value);
				}
			}

}; // class ResourceSet

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Resources
