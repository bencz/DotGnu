/*
 * XmlNamespaceManager.cs - Implementation of the
 *		"System.Xml.XmlNamespaceManager" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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
 
namespace System.Xml
{

using System;
using System.Collections;

public class XmlNamespaceManager : IEnumerable
{
	// We generally manipulate values as "Object" rather than "String"
	// in this class.  This allows us to do quicker string comparisons
	// using object "==" rather than string "==".  For this reason,
	// all strings to be compared must have been interned in the name
	// table using "nameTable.Add()" or "nameTable.Get()".

	// Internal state.
	private XmlNameTable nameTable;
	private NamespaceInfo namespaces;
	private Object xmlCompareQuick;
	private Object xmlNsCompareQuick;

	// Constructor.
	public XmlNamespaceManager(XmlNameTable nameTable)
			{
				// Validate the parameters.
				if(nameTable == null)
				{
					throw new ArgumentNullException("nameTable");
				}

				// Record the name table for later.
				this.nameTable = nameTable;

				// Add special namespaces for "xml" and "xmlns".
				xmlCompareQuick = nameTable.Add("xml");
				xmlNsCompareQuick = nameTable.Add("xmlns");
				namespaces = new NamespaceInfo
					(xmlCompareQuick,
					 nameTable.Add(XmlDocument.xmlnsXml),
					 null);
				namespaces = new NamespaceInfo
					(xmlNsCompareQuick,
					 nameTable.Add(XmlDocument.xmlns),
					 namespaces);

				// Mark the position of the outermost scope level.
				namespaces = new NamespaceInfo(null, String.Empty, namespaces);
			}

	// Add a namespace to this manager.
	public virtual void AddNamespace(String prefix, String uri)
			{
				// Validate the parameters.
				if(((Object)prefix) == null)
				{
					throw new ArgumentNullException("prefix");
				}
				else if(((Object)uri) == null)
				{
					throw new ArgumentNullException("uri");
				}
				prefix = nameTable.Add(prefix);
				uri = nameTable.Add(uri);
				if(((Object)prefix) == xmlCompareQuick ||
		 	       ((Object)prefix) == xmlNsCompareQuick)
				{
					throw new ArgumentException
						(S._("Xml_InvalidNamespace"), "prefix");
				}

				// See if we already have this prefix in the current scope.
				NamespaceInfo info = namespaces;
				while(info != null && info.prefix != null)
				{
					if(info.prefix == (Object)prefix)
					{
						// Update the URI information and return.
						info.uri = uri;
						return;
					}
					info = info.next;
				}

				// Add a new namespace information block to the current scope.
				namespaces = new NamespaceInfo(prefix, uri, namespaces);
			}

	// Implement the IEnumerable interface.
	public virtual IEnumerator GetEnumerator()
			{
				Hashtable list = new Hashtable();
				NamespaceInfo info = namespaces;
				while(info != null)
				{
					if(info.prefix != null &&
					   !list.Contains(info.prefix))
					{
						list.Add(info.prefix, null);
					}
					info = info.next;
				}
				if(!list.Contains(String.Empty))
				{
					list.Add(String.Empty, null);
				}
				return list.Keys.GetEnumerator();
			}

	// Determine if this namespace manager has a specific namespace.
	public virtual bool HasNamespace(String prefix)
			{
				if(((Object)prefix) == null)
				{
					// The null prefix name is never valid.
					return false;
				}
				else if(prefix.Length == 0)
				{
					// We always have the default namespace in the scope.
					return true;
				}
				else
				{
					// Search for the prefix name.
					NamespaceInfo info = namespaces;
					prefix = nameTable.Get(prefix);
					if(prefix == null)
					{
						// If the prefix is not in the name table, then
						// it cannot be in the namespace list either.
						return false;
					}
					while(info != null)
					{
						if(info.prefix == (Object)prefix)
						{
							return true;
						}
						info = info.next;
					}
					return false;
				}
			}

	// Get the URI associated with a specific namespace prefix.
	public virtual String LookupNamespace(String prefix)
			{
				if(((Object)prefix) == null)
				{
					return null;
				}
				else
				{
					// Search for the prefix name.
					NamespaceInfo info = namespaces;
					String newPrefix = nameTable.Get(prefix);
					if(((Object)newPrefix) == null)
					{
						// If the prefix is not in the name table, then
						// it cannot be in the namespace list unless it
						// is the default namespace specifier.
						if(prefix.Length == 0)
						{
							return String.Empty;
						}
						else
						{
							return null;
						}
					}
					while(info != null)
					{
						if(info.prefix == (Object)newPrefix)
						{
							return (String)(info.uri);
						}
						info = info.next;
					}
					if(newPrefix.Length == 0)
					{
						// The default namespace must alway have a value.
						return String.Empty;
					}
					return null;
				}
			}

	// Look up the prefix that corresponds to a specific URI.
	public virtual String LookupPrefix(String uri)
			{
				if(((Object)uri) == null)
				{
					return null;
				}
				else
				{
					// Search for the URI in the namespace list.
					NamespaceInfo info = namespaces;
					uri = nameTable.Get(uri);
					if(((Object)uri) == null)
					{
						// If the URI is not in the name table, then
						// it cannot be in the namespace list either.
						return String.Empty;
					}
					while(info != null)
					{
						if(info.uri == (Object)uri)
						{
							return (String)(info.prefix);
						}
						info = info.next;
					}
					return String.Empty;
				}
			}

	// Push the current namespace scope.
	public virtual void PushScope()
			{
				namespaces = new NamespaceInfo(namespaces);
			}

	// Pop the current namespace scope.
	public virtual bool PopScope()
			{
				// Bail out if there are no namespaces to be popped.
				if(namespaces == null ||
				   (namespaces.prefix == null && namespaces.uri != null))
				{
					return false;
				}

				// Find the end of the namespace list, or the next
				// scope boundary block.
				while(namespaces != null && namespaces.prefix != null)
				{
					namespaces = namespaces.next;
				}

				// Pop the scope boundary block also.
				if(namespaces != null && namespaces.uri == null)
				{
					namespaces = namespaces.next;
				}
				return true;
			}

	// Remove a namespace from the current scope.
	public virtual void RemoveNamespace(String prefix, String uri)
			{
				// Validate the parameters.
				if(((Object)prefix) == null)
				{
					throw new ArgumentNullException("prefix");
				}
				else if(((Object)uri) == null)
				{
					throw new ArgumentNullException("uri");
				}

				// Map the prefix and URI into the name table.
				// If they aren't present, then the namespace scope
				// list is guaranteed not to contain the values.
				prefix = nameTable.Get(prefix);
				uri = nameTable.Get(uri);
				if(((Object)prefix) == null || ((Object)uri) == null)
				{
					return;
				}

				// Find the prefix and URI in the current namespace scope.
				NamespaceInfo info = namespaces;
				NamespaceInfo prev = null;
				while(info != null && info.prefix != null)
				{
					if(info.prefix == (Object)prefix &&
					   info.uri == (Object)uri)
					{
						// Remove this entry and return.
						if(prev != null)
						{
							prev.next = info.next;
						}
						else
						{
							namespaces = info.next;
						}
						return;
					}
					else
					{
						info = info.next;
					}
				}
			}

	// Get the URI for the default namespace.
	public virtual String DefaultNamespace
			{
				get
				{
					String uri = LookupNamespace(String.Empty);
					if(((Object)uri) != null)
					{
						return uri;
					}
					else
					{
						return String.Empty;
					}
				}
			}

	// Get the name table that is used by this namespace manager.
	public XmlNameTable NameTable
			{
				get
				{
					return nameTable;
				}
			}

	// Storage for information about a specific namespace.
	private sealed class NamespaceInfo
	{
		// Publicly accessible state.
		public Object prefix;
		public Object uri;
		public NamespaceInfo next;

		// Construct a new namespace information block.
		public NamespaceInfo(Object prefix, Object uri, NamespaceInfo next)
				{
					this.prefix = prefix;
					this.uri = uri;
					this.next = next;
				}

		// Construct a new scope boundary block.
		public NamespaceInfo(NamespaceInfo next)
				{
					this.prefix = null;
					this.uri = null;
					this.next = next;
				}

	}; // class NamespaceInfo

}; // class XmlNamespaceManager

}; // namespace System.Xml
