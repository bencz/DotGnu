/*
 * CodeTypeMember.cs - Implementation of the
 *		System.CodeDom.CodeTypeMember class.
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

// Generated using "gencdom", and then manually fixed up to add
// the "set" accessor for "CustomAttributes".

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeTypeMember : CodeObject
{

	// Internal state.
	private MemberAttributes _Attributes;
	private CodeCommentStatementCollection _Comments;
	private CodeAttributeDeclarationCollection _CustomAttributes;
	private CodeLinePragma _LinePragma;
	private String _Name;

	// Constructors.
	public CodeTypeMember()
	{
	}

	// Properties.
	public MemberAttributes Attributes
	{
		get
		{
			return _Attributes;
		}
		set
		{
			_Attributes = value;
		}
	}
	public CodeCommentStatementCollection Comments
	{
		get
		{
			if(_Comments == null)
			{
				_Comments = new CodeCommentStatementCollection();
			}
			return _Comments;
		}
	}
	public CodeAttributeDeclarationCollection CustomAttributes
	{
		get
		{
			if(_CustomAttributes == null)
			{
				_CustomAttributes = new CodeAttributeDeclarationCollection();
			}
			return _CustomAttributes;
		}
		set
		{
			_CustomAttributes = value;
		}
	}
	public CodeLinePragma LinePragma
	{
		get
		{
			return _LinePragma;
		}
		set
		{
			_LinePragma = value;
		}
	}
	public String Name
	{
		get
		{
			return _Name;
		}
		set
		{
			_Name = value;
		}
	}

}; // class CodeTypeMember

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
