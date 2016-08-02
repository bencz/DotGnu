/*
 * XmlParserContext.cs - Implementation of the
 *		"System.Xml.XmlParserContext" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004  Free Software Foundation, Inc.
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
using System.Text;
using System.Collections.Specialized;

public class XmlParserContext
{
	// Internal state.
	private String doctypename;
	private String internalsubset;
	private String publicid;
	private String systemid;
	private Encoding encoding;
	private XmlNameTable nametable;
	private XmlNamespaceManager namespacemanager;
	private StackManager stackmanager;


	// Constructors.
	public XmlParserContext
				(XmlNameTable nt,
				 XmlNamespaceManager nsMgr,
				 String xmlLang,
				 XmlSpace xmlSpace)
			: this(nt, nsMgr, null, null, null, null, "", xmlLang, xmlSpace, null)
			{
			}
	public XmlParserContext
				(XmlNameTable nt,
				 XmlNamespaceManager nsMgr,
				 String xmlLang,
				 XmlSpace xmlSpace,
				 Encoding enc)
			: this(nt, nsMgr, null, null, null, null, "", xmlLang, xmlSpace, enc)
			{
			}
	public XmlParserContext
				(XmlNameTable nt,
				 XmlNamespaceManager nsMgr,
				 String docTypeName,
				 String pubId,
				 String sysId,
				 String internalSubset,
				 String baseURI,
				 String xmlLang,
				 XmlSpace xmlSpace)
			: this(nt, nsMgr, docTypeName, pubId, sysId, internalSubset,
			       baseURI, xmlLang, xmlSpace, null)
			{
			}
	public XmlParserContext
				(XmlNameTable nt,
				 XmlNamespaceManager nsMgr,
				 String docTypeName,
				 String pubId,
				 String sysId,
				 String internalSubset,
				 String baseURI,
				 String xmlLang,
				 XmlSpace xmlSpace,
				 Encoding enc)
			{
				// set up the name table and namespace manager
				if(nsMgr != null && nt != null && nsMgr.NameTable != nt)
				{
					throw new XmlException(S._("Xml_WrongNameTable"));
				}
				else if(nt == null)
				{
					if(nsMgr == null)
					{
						nsMgr = new XmlNamespaceManager(new NameTable());
					}
					nametable = nsMgr.NameTable;
				}
				else
				{
					nametable = nt;
				}
				namespacemanager = nsMgr;

				// set various properties
				doctypename = (docTypeName == null) ? "" : docTypeName;
				publicid = (pubId == null) ? "" : pubId;
				systemid = (sysId == null) ? "" : sysId;
				internalsubset = (internalSubset == null) ? "" : internalSubset;
				encoding = enc;

				// set up scoped values
				baseURI = (baseURI == null) ? "" : baseURI;
				xmlLang = (xmlLang == null) ? "" : xmlLang;
				stackmanager = new StackManager(baseURI, xmlLang, xmlSpace);
			}

	// Properties.
	public String BaseURI
			{
				get { return (String)stackmanager[StackInfoType.BaseURI]; }
				set
				{
					if(value == null) { value = ""; }
					stackmanager[StackInfoType.BaseURI] = value;
				}
			}
	public String DocTypeName
			{
				get { return doctypename; }
				set
				{
					if(value == null) { value = ""; }
					doctypename = value;
				}
			}
	public Encoding Encoding
			{
				get { return encoding; }
				set { encoding = value; }
			}
	public String InternalSubset
			{
				get { return internalsubset; }
				set
				{
					if(value == null) { value = ""; }
					internalsubset = value;
				}
			}
	public XmlNameTable NameTable
			{
				get { return nametable; }
				set { nametable = value; }
			}
	public XmlNamespaceManager NamespaceManager
			{
				get { return namespacemanager; }
				set { namespacemanager = value; }
			}
	public String PublicId
			{
				get { return publicid; }
				set
				{
					if(value == null) { value = ""; }
					publicid = value;
				}
			}
	public String SystemId
			{
				get { return systemid; }
				set
				{
					if(value == null) { value = ""; }
					systemid = value;
				}
			}
	public String XmlLang
			{
				get { return (String)stackmanager[StackInfoType.XmlLang]; }
				set
				{
					if(value == null) { value = ""; }
					stackmanager[StackInfoType.XmlLang] = value;
				}
			}
	public XmlSpace XmlSpace
			{
				get { return (XmlSpace)stackmanager[StackInfoType.XmlSpace]; }
				set { stackmanager[StackInfoType.XmlSpace] = value; }
			}

#if !ECMA_COMPAT
//	internal SomeDTDRuleHandlingObjectGoesHere Foo
//			{
//				get { return foo; }
//				set { foo = value; }
//			}
#endif


	// Pop the current scope.
	internal bool PopScope()
			{
				return stackmanager.PopScope();
			}

	// Push a new scope.
	internal void PushScope()
			{
				stackmanager.PushScope();
			}

	// Reset all values to defaults.
	internal void Reset()
			{
				while(stackmanager.PopScope()) {}
				while(namespacemanager.PopScope()) {}
				BaseURI = String.Empty;
				XmlLang = String.Empty;
				XmlSpace = XmlSpace.None;
				SystemId = String.Empty;
				PublicId = String.Empty;
				DocTypeName = String.Empty;
				InternalSubset = String.Empty;
			}








	private enum StackInfoType
	{
		None     = 0,
		BaseURI  = 1,
		XmlLang  = 2,
		XmlSpace = 3

	}; // enum StackInfoType

	private sealed class StackManager
	{
		// Internal state.
		private StackInfo stack;

		// Constructor.
		public StackManager(Object baseURI, Object xmlLang, Object xmlSpace)
				{
					// Set the top-level scope values.
					stack = new StackInfo(StackInfoType.BaseURI, baseURI, null);
					stack = new StackInfo(StackInfoType.XmlLang, xmlLang, stack);
					stack = new StackInfo(StackInfoType.XmlSpace, xmlSpace, stack);
				}

		// Get or set the value for a given type.
		public Object this[StackInfoType type]
				{
					get
					{
						// search for the given type
						StackInfo info = stack;
						while(info != null)
						{
							if(info.type == type)
							{
								return info.value;
							}
							info = info.next;
						}
						return null;
					}
					set
					{
						// check for the given type in the current scope
						StackInfo info = stack;
						while(info != null && info.type != StackInfoType.None)
						{
							if(info.type == type)
							{
								// update the value and return
								info.value = value;
								return;
							}
							info = info.next;
						}

						// add a new information block to the current scope
						stack = new StackInfo(type, value, stack);
					}
				}

		// Push the current scope.
		public void PushScope()
				{
					stack = new StackInfo(stack);
				}

		// Pop the current scope.
		public bool PopScope()
				{
					// exit if there's no stack information to be popped
					if(stack == null) { return false; }

					// find the bottom of the stack, or the next scope boundary
					while(stack != null && stack.type != StackInfoType.None)
					{
						stack = stack.next;
					}

					// pop the scope boundary block and return
					if(stack != null) { stack = stack.next; }
					return true;
				}

		// Storage for information about BaseUri, XmlLang, or XmlSpace.
		private sealed class StackInfo
		{
			// Publicly accessible state.
			public StackInfoType type;
			public Object value;
			public StackInfo next;

			// Construct a new stack information block.
			public StackInfo(StackInfoType type, Object value, StackInfo next)
					{
						this.type = type;
						this.value = value;
						this.next = next;
					}

			// Construct a new scope boundary block.
			public StackInfo(StackInfo next)
					{
						this.type = StackInfoType.None;
						this.value = null;
						this.next = next;
					}

		}; // class StackInfo

	}; // class StackManager

}; // class XmlParserContext

}; // namespace System.Xml
