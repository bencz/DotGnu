/*
 * SymDocument.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymDocument" class.
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

public class SymDocument : ISymbolDocument
{
	// Internal state.
	private unsafe ISymUnmanagedDocument *pDocument;
	private SymReader reader;
	private String language;
	private String url;

	// Constructors.
	public unsafe SymDocument(ISymUnmanagedDocument *pDocument)
			{
				this.pDocument = pDocument;
			}
	internal SymDocument(SymReader reader, String language, String url)
			{
				this.reader = reader;
				this.language = language;
				this.url = url;
			}

	// Destructor (C++ style).
	~SymDocument() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Get the unmanaged version of this document.
	public unsafe ISymUnmanagedDocument *GetUnmanaged()
			{
				return pDocument;
			}

	// Implement the ISymbolDocument interface.
	public virtual int FindClosestLine(int line)
			{
				// Not used in this implementation.
				return line;
			}
	public virtual byte[] GetCheckSum()
			{
				throw new NotSupportedException();
			}
	public virtual byte[] GetSourceRange
				(int startLine, int startColumn, int endLine, int endColumn)
			{
				throw new NotSupportedException();
			}
	public virtual Guid CheckSumAlgorithmId 
			{
 				get
				{
					throw new NotSupportedException();
				}
 			}
	public virtual Guid DocumentType 
			{
 				get
				{
					// We assume that all source code is text-based.
					return SymDocumentType.Text;
				}
 			}
	public virtual bool HasEmbeddedSource 
			{
 				get
				{
					// We never store source documents in the symbol data.
					return false;
				}
 			}
	public virtual Guid Language 
			{
 				get
				{
					String ext = language;
					if(ext == null)
					{
						// If we don't have an explicit language specification,
						// then use the extension on the URL.
						String url = URL;
						if(url != null)
						{
							ext = Path.GetExtension(url);
							if(ext.Length > 0 && ext[0] == '.')
							{
								ext = ext.Substring(1);
							}
						}
					}
					if(ext == null)
					{
						return SymLanguageType.CSharp;
					}
					ext = ext.ToLower();
					switch(ext)
					{
						case "bas": case "basic": case "vb": case "vb.net":
						case "visualbasic": case "visualbasic.net":
							return SymLanguageType.Basic;
						case "c": case "h":
							return SymLanguageType.C;
						case "cob": case "cobol":
							return SymLanguageType.Cobol;
						case "cpp": case "cxx": case "cc": case "cplusplus":
						case "hpp": case "hxx": case "hh":
							return SymLanguageType.CPlusPlus;
						case "cs": case "csharp":
							return SymLanguageType.CSharp;
						case "il": case "iltmp": case "ilasm":
						case "ilassembly":
							return SymLanguageType.ILAssembly;
						case "java": case "jav":
							return SymLanguageType.Java;
						case "js": case "jscript":
							return SymLanguageType.JScript;
						case "mcpp": case "mcxx": case "mcc":
						case "mcplusplus": case "mhpp": case "mhxx":
						case "mhh":
							return SymLanguageType.MCPlusPlus;
						case "pas": case "pascal":
							return SymLanguageType.Pascal;
						case "smc":
							return SymLanguageType.SMC;
						default: break;
					}
					return SymLanguageType.CSharp;
				}
 			}
	public virtual Guid LanguageVendor 
			{
 				get
				{
					// For backwards-compatibility only.
					return SymLanguageVendor.Microsoft;
				}
 			}
	public virtual int SourceLength 
			{
 				get
				{
					// We never store source documents in the symbol data.
					return 0;
				}
 			}
	public virtual String URL 
			{
 				get
				{
					return url;
				}
 			}

}; // class SymDocument

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
