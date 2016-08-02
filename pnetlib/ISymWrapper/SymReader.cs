/*
 * SymReader.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymReader" class.
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

namespace System.Diagnostics.SymbolStore
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.IO;
using System.Text;
using System.Collections;

public class SymReader : ISymbolReader
{
	// Internal state.
	private unsafe ISymUnmanagedReader *pReader;
	private String linkDirectory;
	private String filename;
	private Encoding utf8;
	internal byte[] data;
	internal int indexOffset;
	internal int numIndexEntries;
	private int stringOffset;
	private int stringLength;
	private ISymbolDocument[] documents;
	private Hashtable documentCache;
	private Hashtable methodCache;

	// Types of data blocks within debug symbol data.
	internal const int DataType_LineColumn				= 1;
	internal const int DataType_LineOffsets				= 2;
	internal const int DataType_LineColumnOffsets		= 3;
	internal const int DataType_LocalVariables			= 4;
	internal const int DataType_LocalVariablesOffsets	= 5;

	// Constructors.
	public unsafe SymReader(ISymUnmanagedReader *pReader)
			{
				this.pReader = pReader;
			}
	internal SymReader(String filename, byte[] data)
			{
				// Store the parameters for later.
				this.filename = filename;
				this.data = data;

				// We need the UTF8 encoding object to decode strings.
				utf8 = Encoding.UTF8;

				// Read and validate the header.
				if(data.Length < 24)
				{
					throw new ArgumentException();
				}
				if(data[0] != (byte)'I' ||
				   data[1] != (byte)'L' ||
				   data[2] != (byte)'D' ||
				   data[3] != (byte)'B' ||
				   data[4] != (byte)0x01 ||
				   data[5] != (byte)0x00 ||
				   data[6] != (byte)0x00 ||
				   data[7] != (byte)0x00)
				{
					throw new ArgumentException();
				}
				indexOffset = Utils.ReadInt32(data, 8);
				numIndexEntries = Utils.ReadInt32(data, 12);
				stringOffset = Utils.ReadInt32(data, 16);
				stringLength = Utils.ReadInt32(data, 20);
				if(indexOffset < 24 || indexOffset >= data.Length)
				{
					throw new ArgumentException();
				}
				if(numIndexEntries < 0 ||
				   ((data.Length - indexOffset) / 8) < numIndexEntries)
				{
					throw new ArgumentException();
				}
				if(stringOffset < 24 || stringOffset >= data.Length)
				{
					throw new ArgumentException();
				}
				if(stringLength <= 0 ||
				   stringLength > (data.Length - stringOffset) ||
				   data[stringOffset + stringLength - 1] != 0)
				{
					throw new ArgumentException();
				}

				// Get the link directory, if specified.
				SymInfoEnumerator e = new SymInfoEnumerator(this, "LDIR");
				if(e.MoveNext())
				{
					linkDirectory = ReadString(e.GetNextInt());
				}
			}

	// Destructor (C++ style).
	~SymReader() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Implement the ISymbolReader interface.
	public virtual ISymbolDocument GetDocument
				(String url, Guid language, Guid languageVendor,
				 Guid documentType)
			{
				String lang;

				// Validate the url parameter.
				if(url == null)
				{
					throw new ArgumentNullException("url");
				}

				// Convert the language GUID into a language name.
				// We ignore the vendor and document type, because
				// they are not important.
				if(language == SymLanguageType.Basic)
				{
					lang = "basic";
				}
				else if(language == SymLanguageType.C)
				{
					lang = "c";
				}
				else if(language == SymLanguageType.Cobol)
				{
					lang = "cobol";
				}
				else if(language == SymLanguageType.CPlusPlus)
				{
					lang = "cplusplus";
				}
				else if(language == SymLanguageType.CSharp)
				{
					lang = "csharp";
				}
				else if(language == SymLanguageType.ILAssembly)
				{
					lang = "ilassembly";
				}
				else if(language == SymLanguageType.Java)
				{
					lang = "java";
				}
				else if(language == SymLanguageType.JScript)
				{
					lang = "jscript";
				}
				else if(language == SymLanguageType.MCPlusPlus)
				{
					lang = "mcplusplus";
				}
				else if(language == SymLanguageType.Pascal)
				{
					lang = "pascal";
				}
				else if(language == SymLanguageType.SMC)
				{
					lang = "smc";
				}
				else
				{
					lang = null;
				}

				// Create a new document object for the URL and return it.
				return new SymDocument(this, lang, url);
			}
	public virtual ISymbolDocument[] GetDocuments()
			{
				// Bail out early if we already loaded the document list.
				if(documents != null)
				{
					return documents;
				}

				// Read the document information list from the symbol data.
				ArrayList list = new ArrayList();
				SymInfoEnumerator e = new SymInfoEnumerator(this);
				String filename;
				ISymbolDocument doc;
				while(e.MoveNext())
				{
					if(e.Type == DataType_LineColumn ||
					   e.Type == DataType_LineOffsets ||
					   e.Type == DataType_LineColumnOffsets)
					{
						filename = ReadString(e.GetNextInt());
						doc = GetDocument(filename);
						if(doc != null && !list.Contains(doc))
						{
							list.Add(doc);
						}
					}
				}

				// Return the final document list.
				documents = new ISymbolDocument [list.Count];
				int index = 0;
				foreach(ISymbolDocument d in list)
				{
					documents[index++] = d;
				}
				return documents;
			}
	public virtual ISymbolVariable[] GetGlobalVariables()
			{
				throw new NotSupportedException();
			}
	public virtual ISymbolMethod GetMethod(SymbolToken method)
			{
				return GetMethod(method, 0);
			}
	public virtual ISymbolMethod GetMethod(SymbolToken method, int version)
			{
				ISymbolMethod meth;
				if(methodCache == null)
				{
					methodCache = new Hashtable();
				}
				else if((meth = (ISymbolMethod)methodCache
							[method.GetToken()]) != null)
				{
					return meth;
				}
				meth = new SymMethod(this, method.GetToken());
				methodCache[method.GetToken()] = meth;
				return meth;
			}
	public virtual ISymbolMethod GetMethodFromDocumentPosition
				(ISymbolDocument document, int line, int column)
			{
				if(document == null || document.URL == null)
				{
					return null;
				}
				SymInfoEnumerator e = new SymInfoEnumerator(this);
				String filename;
				ISymbolDocument doc;
				int tempLine, tempColumn, tempOffset;
				int closestBelow = 0;
				int closestBelowToken = 0;
				while(e.MoveNext())
				{
					// We only check on line because column values
					// are likely to be inaccurate.
					if(e.Type == DataType_LineColumn)
					{
						filename = ReadString(e.GetNextInt());
						doc = GetDocument(filename);
						if(doc != null && doc.URL == document.URL)
						{
							while((tempLine = e.GetNextInt()) != -1)
							{
								tempColumn = e.GetNextInt();
								if(tempLine == line)
								{
									return GetMethod(new SymbolToken(e.Token));
								}
								else if(tempLine < line &&
								        tempLine > closestBelow)
								{
									closestBelow = tempLine;
									closestBelowToken = e.Token;
								}
							}
						}
					}
					else if(e.Type == DataType_LineOffsets)
					{
						filename = ReadString(e.GetNextInt());
						doc = GetDocument(filename);
						if(doc != null && doc.URL == document.URL)
						{
							while((tempLine = e.GetNextInt()) != -1)
							{
								tempOffset = e.GetNextInt();
								if(tempLine == line)
								{
									return GetMethod(new SymbolToken(e.Token));
								}
								else if(tempLine < line &&
								        tempLine > closestBelow)
								{
									closestBelow = tempLine;
									closestBelowToken = e.Token;
								}
							}
						}
					}
					else if(e.Type == DataType_LineColumnOffsets)
					{
						filename = ReadString(e.GetNextInt());
						doc = GetDocument(filename);
						if(doc != null && doc.URL == document.URL)
						{
							while((tempLine = e.GetNextInt()) != -1)
							{
								tempColumn = e.GetNextInt();
								tempOffset = e.GetNextInt();
								if(tempLine == line)
								{
									return GetMethod(new SymbolToken(e.Token));
								}
								else if(tempLine < line &&
								        tempLine > closestBelow)
								{
									closestBelow = tempLine;
									closestBelowToken = e.Token;
								}
							}
						}
					}
				}
				if(closestBelowToken != 0)
				{
					// Return the closest match that we found in the
					// document that is below the specified line.
					return GetMethod(new SymbolToken(closestBelowToken));
				}
				return null;
			}
	public virtual ISymbolNamespace[] GetNamespaces()
			{
				throw new NotSupportedException();
			}
	public virtual byte[] GetSymAttribute(SymbolToken parent, String name)
			{
				throw new NotSupportedException();
			}
	public virtual ISymbolVariable[] GetVariables(SymbolToken parent)
			{
				throw new NotSupportedException();
			}
	public virtual SymbolToken UserEntryPoint 
			{
				get
				{
					// Not used in this implementation, because it
					// duplicates information available via metadata.
					return new SymbolToken(0);
				}
			}

	// Read a string value from the debug symbol information.
	internal String ReadString(int offset)
			{
				if(data == null)
				{
					return String.Empty;
				}
				if(offset < 0 || offset >= stringLength)
				{
					return String.Empty;
				}
				offset += stringOffset;
				int len = 0;
				while(data[offset + len] != 0)
				{
					++len;
				}
				return utf8.GetString(data, offset, len);
			}

	// Convert a filename into a URL.
	internal String FilenameToURL(String name)
			{
				String temp;
				bool checkOther;

				// Bail out if the name is empty.
				if(name == null || name.Length == 0)
				{
					return null;
				}
				
				// Get the full absolute pathname for the file.
				checkOther = true;
				if(!Path.IsPathRooted(name) && linkDirectory != null)
				{
					temp = Path.Combine(linkDirectory, name);
					if(File.Exists(temp))
					{
						name = temp;
						checkOther = false;
					}
				}
				if(checkOther && !Path.IsPathRooted(name) && filename != null)
				{
					temp = Path.Combine
						(Path.GetDirectoryName(filename), name);
					if(File.Exists(temp))
					{
						name = temp;
					}
				}
				name = Path.GetFullPath(name);

				// Normalize pathname separators to "/".
				name = name.Replace('\\', '/');

				// Add the "file:" prefix to the name to form the URL.
				if(name.Length >= 2 && name[1] == ':')
				{
					// The filename includes a Windows-style drive letter.
					return "file:/" + name;
				}
				else
				{
					// The filename is absolute from a Unix-style root.
					return "file:" + name;
				}
			}

	// Get a document block for a particular filename.
	internal ISymbolDocument GetDocument(String filename)
			{
				ISymbolDocument document;

				// Convert the filename into a full URL.
				filename = FilenameToURL(filename);
				if(filename == null)
				{
					return null;
				}

				// See if we already have a document for this file.
				if(documentCache == null)
				{
					documentCache = new Hashtable();
				}
				else if((document = (ISymbolDocument)documentCache[filename])
							!= null)
				{
					return document;
				}

				// Create a new document object and add it to the cache.
				document = new SymDocument(this, null, filename);
				documentCache[filename] = document;
				return document;
			}

}; // class SymReader

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
