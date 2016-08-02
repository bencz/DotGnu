/*
 * CodeCodeMemberProperty.cs - Implementation of the
 *		System.CodeDom.CodeCodeMemberProperty class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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
public class CodeMemberProperty : CodeTypeMember
{

	// Internal state.
	private CodeStatementCollection _GetStatements;
	private bool _HasGet;
	private bool _HasSet;
	private CodeTypeReferenceCollection _ImplementationTypes;
	private CodeParameterDeclarationExpressionCollection _Parameters;
	private CodeTypeReference _PrivateImplementationType;
	private CodeStatementCollection _SetStatements;
	private CodeTypeReference _Type;

	// Constructors.
	public CodeMemberProperty()
	{
	}

	// Properties.
	public CodeStatementCollection GetStatements
	{
		get
		{
			if(_GetStatements == null)
			{
				_GetStatements = new CodeStatementCollection();
			}
			return _GetStatements;
		}
	}
	public bool HasGet
	{
		get
		{
			return _HasGet || (GetStatements.Count != 0);
		}
		set
		{
			_HasGet = value;
		}
	}
	public bool HasSet
	{
		get
		{
			return _HasSet || (SetStatements.Count != 0);
		}
		set
		{
			_HasSet = value;
		}
	}
	public CodeTypeReferenceCollection ImplementationTypes
	{
		get
		{
			if(_ImplementationTypes == null)
			{
				_ImplementationTypes = new CodeTypeReferenceCollection();
			}
			return _ImplementationTypes;
		}
	}
	public CodeParameterDeclarationExpressionCollection Parameters
	{
		get
		{
			if(_Parameters == null)
			{
				_Parameters = new CodeParameterDeclarationExpressionCollection();
			}
			return _Parameters;
		}
	}
	public CodeTypeReference PrivateImplementationType
	{
		get
		{
			return _PrivateImplementationType;
		}
		set
		{
			_PrivateImplementationType = value;
		}
	}
	public CodeStatementCollection SetStatements
	{
		get
		{
			if(_SetStatements == null)
			{
				_SetStatements = new CodeStatementCollection();
			}
			return _SetStatements;
		}
	}
	public CodeTypeReference Type
	{
		get
		{
			return _Type;
		}
		set
		{
			_Type = value;
		}
	}

}; // class CodeMemberProperty

#endif

}; // namespace System.CodeDom
