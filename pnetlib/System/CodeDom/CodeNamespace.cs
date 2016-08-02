/*
 * CodeNamespace.cs - Implementation of the
 *		System.CodeDom.CodeNamespace class.
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

namespace System.CodeDom
{

#if CONFIG_CODEDOM

using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Specialized;

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeNamespace : CodeObject
{

	// Internal state.
	private CodeCommentStatementCollection comments;
	private CodeNamespaceImportCollection imports;
	private String name;
	private CodeTypeDeclarationCollection types;

	// Constructors.
	public CodeNamespace()
			{
			}
	public CodeNamespace(String name)
			{
				this.name = name;
			}

	// Properties.
	public CodeCommentStatementCollection Comments
			{
				get
				{
					if(comments == null)
					{
						comments = new CodeCommentStatementCollection();
						if(PopulateComments != null)
						{
							PopulateComments(this, EventArgs.Empty);
						}
					}
					return comments;
				}
			}
	public CodeNamespaceImportCollection Imports
			{
				get
				{
					if(imports == null)
					{
						imports = new CodeNamespaceImportCollection();
						if(PopulateImports != null)
						{
							PopulateImports(this, EventArgs.Empty);
						}
					}
					return imports;
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}
	public CodeTypeDeclarationCollection Types
			{
				get
				{
					if(types == null)
					{
						types = new CodeTypeDeclarationCollection();
						if(PopulateTypes != null)
						{
							PopulateTypes(this, EventArgs.Empty);
						}
					}
					return types;
				}
			}

	// Events.
	public event EventHandler PopulateComments;
	public event EventHandler PopulateImports;
	public event EventHandler PopulateTypes;

}; // class CodeNamespace

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
