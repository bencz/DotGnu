/*
 * This file is generated from rules.txt using gencdom - do not edit.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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
public class CodeArgumentReferenceExpression : CodeExpression
{

	// Internal state.
	private String _ParameterName;

	// Constructors.
	public CodeArgumentReferenceExpression()
	{
	}
	public CodeArgumentReferenceExpression(String _ParameterName)
	{
		this._ParameterName = _ParameterName;
	}

	// Properties.
	public String ParameterName
	{
		get
		{
			return _ParameterName;
		}
		set
		{
			_ParameterName = value;
		}
	}

}; // class CodeArgumentReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeArrayIndexerExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _TargetObject;
	private CodeExpressionCollection _Indices;

	// Constructors.
	public CodeArrayIndexerExpression()
	{
	}
	public CodeArrayIndexerExpression(CodeExpression _TargetObject, params CodeExpression[] _Indices)
	{
		this._TargetObject = _TargetObject;
		this.Indices.AddRange(_Indices);
	}

	// Properties.
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public CodeExpressionCollection Indices
	{
		get
		{
			if(_Indices == null)
			{
				_Indices = new CodeExpressionCollection();
			}
			return _Indices;
		}
	}

}; // class CodeArrayIndexerExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeAssignStatement : CodeStatement
{

	// Internal state.
	private CodeExpression _Left;
	private CodeExpression _Right;

	// Constructors.
	public CodeAssignStatement()
	{
	}
	public CodeAssignStatement(CodeExpression _Left, CodeExpression _Right)
	{
		this._Left = _Left;
		this._Right = _Right;
	}

	// Properties.
	public CodeExpression Left
	{
		get
		{
			return _Left;
		}
		set
		{
			_Left = value;
		}
	}
	public CodeExpression Right
	{
		get
		{
			return _Right;
		}
		set
		{
			_Right = value;
		}
	}

}; // class CodeAssignStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeAttributeArgumentCollection : CollectionBase
{

	// Constructors.
	public CodeAttributeArgumentCollection()
	{
	}
	public CodeAttributeArgumentCollection(CodeAttributeArgument[] value)
	{
		AddRange(value);
	}
	public CodeAttributeArgumentCollection(CodeAttributeArgumentCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeAttributeArgument this[int index]
	{
		get
		{
			return (CodeAttributeArgument)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeAttributeArgument value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeAttributeArgument[] value)
	{
		foreach(CodeAttributeArgument e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeAttributeArgumentCollection value)
	{
		foreach(CodeAttributeArgument e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeAttributeArgument value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeAttributeArgument[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeAttributeArgument value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeAttributeArgument value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeAttributeArgument value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeAttributeArgumentCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeAttributeArgument : Object
{

	// Internal state.
	private String _Name;
	private CodeExpression _Value;

	// Constructors.
	public CodeAttributeArgument()
	{
	}
	public CodeAttributeArgument(CodeExpression _Value)
	{
		this._Value = _Value;
	}
	public CodeAttributeArgument(String _Name, CodeExpression _Value)
	{
		this._Name = _Name;
		this._Value = _Value;
	}

	// Properties.
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
	public CodeExpression Value
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
		}
	}

}; // class CodeAttributeArgument

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeAttributeDeclarationCollection : CollectionBase
{

	// Constructors.
	public CodeAttributeDeclarationCollection()
	{
	}
	public CodeAttributeDeclarationCollection(CodeAttributeDeclaration[] value)
	{
		AddRange(value);
	}
	public CodeAttributeDeclarationCollection(CodeAttributeDeclarationCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeAttributeDeclaration this[int index]
	{
		get
		{
			return (CodeAttributeDeclaration)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeAttributeDeclaration value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeAttributeDeclaration[] value)
	{
		foreach(CodeAttributeDeclaration e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeAttributeDeclarationCollection value)
	{
		foreach(CodeAttributeDeclaration e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeAttributeDeclaration value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeAttributeDeclaration[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeAttributeDeclaration value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeAttributeDeclaration value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeAttributeDeclaration value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeAttributeDeclarationCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeAttributeDeclaration : Object
{

	// Internal state.
	private String _Name;
	private CodeAttributeArgumentCollection _Arguments;

	// Constructors.
	public CodeAttributeDeclaration()
	{
	}
	public CodeAttributeDeclaration(String _Name)
	{
		this._Name = _Name;
	}
	public CodeAttributeDeclaration(String _Name, CodeAttributeArgument[] _Arguments)
	{
		this._Name = _Name;
		this.Arguments.AddRange(_Arguments);
	}

	// Properties.
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
	public CodeAttributeArgumentCollection Arguments
	{
		get
		{
			if(_Arguments == null)
			{
				_Arguments = new CodeAttributeArgumentCollection();
			}
			return _Arguments;
		}
	}

}; // class CodeAttributeDeclaration

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeBaseReferenceExpression : CodeExpression
{

	// Constructors.
	public CodeBaseReferenceExpression()
	{
	}

}; // class CodeBaseReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeBinaryOperatorExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _Left;
	private CodeBinaryOperatorType _Operator;
	private CodeExpression _Right;

	// Constructors.
	public CodeBinaryOperatorExpression()
	{
	}
	public CodeBinaryOperatorExpression(CodeExpression _Left, CodeBinaryOperatorType _Operator, CodeExpression _Right)
	{
		this._Left = _Left;
		this._Operator = _Operator;
		this._Right = _Right;
	}

	// Properties.
	public CodeExpression Left
	{
		get
		{
			return _Left;
		}
		set
		{
			_Left = value;
		}
	}
	public CodeBinaryOperatorType Operator
	{
		get
		{
			return _Operator;
		}
		set
		{
			_Operator = value;
		}
	}
	public CodeExpression Right
	{
		get
		{
			return _Right;
		}
		set
		{
			_Right = value;
		}
	}

}; // class CodeBinaryOperatorExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeCatchClauseCollection : CollectionBase
{

	// Constructors.
	public CodeCatchClauseCollection()
	{
	}
	public CodeCatchClauseCollection(CodeCatchClause[] value)
	{
		AddRange(value);
	}
	public CodeCatchClauseCollection(CodeCatchClauseCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeCatchClause this[int index]
	{
		get
		{
			return (CodeCatchClause)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeCatchClause value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeCatchClause[] value)
	{
		foreach(CodeCatchClause e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeCatchClauseCollection value)
	{
		foreach(CodeCatchClause e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeCatchClause value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeCatchClause[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeCatchClause value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeCatchClause value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeCatchClause value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeCatchClauseCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeComment : CodeObject
{

	// Internal state.
	private String _Text;
	private bool _DocComment;

	// Constructors.
	public CodeComment()
	{
	}
	public CodeComment(String _Text)
	{
		this._Text = _Text;
	}
	public CodeComment(String _Text, bool _DocComment)
	{
		this._Text = _Text;
		this._DocComment = _DocComment;
	}

	// Properties.
	public String Text
	{
		get
		{
			return _Text;
		}
		set
		{
			_Text = value;
		}
	}
	public bool DocComment
	{
		get
		{
			return _DocComment;
		}
		set
		{
			_DocComment = value;
		}
	}

}; // class CodeComment

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeCommentStatementCollection : CollectionBase
{

	// Constructors.
	public CodeCommentStatementCollection()
	{
	}
	public CodeCommentStatementCollection(CodeCommentStatement[] value)
	{
		AddRange(value);
	}
	public CodeCommentStatementCollection(CodeCommentStatementCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeCommentStatement this[int index]
	{
		get
		{
			return (CodeCommentStatement)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeCommentStatement value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeCommentStatement[] value)
	{
		foreach(CodeCommentStatement e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeCommentStatementCollection value)
	{
		foreach(CodeCommentStatement e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeCommentStatement value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeCommentStatement[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeCommentStatement value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeCommentStatement value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeCommentStatement value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeCommentStatementCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeCompileUnit : CodeObject
{

	// Internal state.
	private CodeAttributeDeclarationCollection _AssemblyCustomAttributes;
	private CodeNamespaceCollection _Namespaces;
	private StringCollection _ReferencedAssemblies;

	// Constructors.
	public CodeCompileUnit()
	{
	}

	// Properties.
	public CodeAttributeDeclarationCollection AssemblyCustomAttributes
	{
		get
		{
			if(_AssemblyCustomAttributes == null)
			{
				_AssemblyCustomAttributes = new CodeAttributeDeclarationCollection();
			}
			return _AssemblyCustomAttributes;
		}
	}
	public CodeNamespaceCollection Namespaces
	{
		get
		{
			if(_Namespaces == null)
			{
				_Namespaces = new CodeNamespaceCollection();
			}
			return _Namespaces;
		}
	}
	public StringCollection ReferencedAssemblies
	{
		get
		{
			if(_ReferencedAssemblies == null)
			{
				_ReferencedAssemblies = new StringCollection();
			}
			return _ReferencedAssemblies;
		}
	}

}; // class CodeCompileUnit

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeConstructor : CodeMemberMethod
{

	// Internal state.
	private CodeExpressionCollection _BaseConstructorArgs;
	private CodeExpressionCollection _ChainedConstructorArgs;

	// Constructors.
	public CodeConstructor()
	{
	}

	// Properties.
	public CodeExpressionCollection BaseConstructorArgs
	{
		get
		{
			if(_BaseConstructorArgs == null)
			{
				_BaseConstructorArgs = new CodeExpressionCollection();
			}
			return _BaseConstructorArgs;
		}
	}
	public CodeExpressionCollection ChainedConstructorArgs
	{
		get
		{
			if(_ChainedConstructorArgs == null)
			{
				_ChainedConstructorArgs = new CodeExpressionCollection();
			}
			return _ChainedConstructorArgs;
		}
	}

}; // class CodeConstructor

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeDelegateCreateExpression : CodeExpression
{

	// Internal state.
	private CodeTypeReference _DelegateType;
	private CodeExpression _TargetObject;
	private String _MethodName;

	// Constructors.
	public CodeDelegateCreateExpression()
	{
	}
	public CodeDelegateCreateExpression(CodeTypeReference _DelegateType, CodeExpression _TargetObject, String _MethodName)
	{
		this._DelegateType = _DelegateType;
		this._TargetObject = _TargetObject;
		this._MethodName = _MethodName;
	}

	// Properties.
	public CodeTypeReference DelegateType
	{
		get
		{
			return _DelegateType;
		}
		set
		{
			_DelegateType = value;
		}
	}
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public String MethodName
	{
		get
		{
			return _MethodName;
		}
		set
		{
			_MethodName = value;
		}
	}

}; // class CodeDelegateCreateExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeDelegateInvokeExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _TargetObject;
	private CodeExpressionCollection _Parameters;

	// Constructors.
	public CodeDelegateInvokeExpression()
	{
	}
	public CodeDelegateInvokeExpression(CodeExpression _TargetObject)
	{
		this._TargetObject = _TargetObject;
	}
	public CodeDelegateInvokeExpression(CodeExpression _TargetObject, CodeExpression[] _Parameters)
	{
		this._TargetObject = _TargetObject;
		this.Parameters.AddRange(_Parameters);
	}

	// Properties.
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public CodeExpressionCollection Parameters
	{
		get
		{
			if(_Parameters == null)
			{
				_Parameters = new CodeExpressionCollection();
			}
			return _Parameters;
		}
	}

}; // class CodeDelegateInvokeExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeDirectionExpression : CodeExpression
{

	// Internal state.
	private FieldDirection _Direction;
	private CodeExpression _Expression;

	// Constructors.
	public CodeDirectionExpression()
	{
	}
	public CodeDirectionExpression(FieldDirection _Direction, CodeExpression _Expression)
	{
		this._Direction = _Direction;
		this._Expression = _Expression;
	}

	// Properties.
	public FieldDirection Direction
	{
		get
		{
			return _Direction;
		}
		set
		{
			_Direction = value;
		}
	}
	public CodeExpression Expression
	{
		get
		{
			return _Expression;
		}
		set
		{
			_Expression = value;
		}
	}

}; // class CodeDirectionExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeEntryPointMethod : CodeMemberMethod
{

	// Constructors.
	public CodeEntryPointMethod()
	{
	}

}; // class CodeEntryPointMethod

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeEventReferenceExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _TargetObject;
	private String _EventName;

	// Constructors.
	public CodeEventReferenceExpression()
	{
	}
	public CodeEventReferenceExpression(CodeExpression _TargetObject, String _EventName)
	{
		this._TargetObject = _TargetObject;
		this._EventName = _EventName;
	}

	// Properties.
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public String EventName
	{
		get
		{
			return _EventName;
		}
		set
		{
			_EventName = value;
		}
	}

}; // class CodeEventReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeExpressionCollection : CollectionBase
{

	// Constructors.
	public CodeExpressionCollection()
	{
	}
	public CodeExpressionCollection(CodeExpression[] value)
	{
		AddRange(value);
	}
	public CodeExpressionCollection(CodeExpressionCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeExpression this[int index]
	{
		get
		{
			return (CodeExpression)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeExpression value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeExpression[] value)
	{
		foreach(CodeExpression e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeExpressionCollection value)
	{
		foreach(CodeExpression e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeExpression value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeExpression[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeExpression value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeExpression value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeExpression value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeExpressionCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeExpression : CodeObject
{

	// Constructors.
	public CodeExpression()
	{
	}

}; // class CodeExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeExpressionStatement : CodeStatement
{

	// Internal state.
	private CodeExpression _Expression;

	// Constructors.
	public CodeExpressionStatement()
	{
	}
	public CodeExpressionStatement(CodeExpression _Expression)
	{
		this._Expression = _Expression;
	}

	// Properties.
	public CodeExpression Expression
	{
		get
		{
			return _Expression;
		}
		set
		{
			_Expression = value;
		}
	}

}; // class CodeExpressionStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeFieldReferenceExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _TargetObject;
	private String _FieldName;

	// Constructors.
	public CodeFieldReferenceExpression()
	{
	}
	public CodeFieldReferenceExpression(CodeExpression _TargetObject, String _FieldName)
	{
		this._TargetObject = _TargetObject;
		this._FieldName = _FieldName;
	}

	// Properties.
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public String FieldName
	{
		get
		{
			return _FieldName;
		}
		set
		{
			_FieldName = value;
		}
	}

}; // class CodeFieldReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeIndexerExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _TargetObject;
	private CodeExpressionCollection _Indices;

	// Constructors.
	public CodeIndexerExpression()
	{
	}
	public CodeIndexerExpression(CodeExpression _TargetObject, params CodeExpression[] _Indices)
	{
		this._TargetObject = _TargetObject;
		this.Indices.AddRange(_Indices);
	}

	// Properties.
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public CodeExpressionCollection Indices
	{
		get
		{
			if(_Indices == null)
			{
				_Indices = new CodeExpressionCollection();
			}
			return _Indices;
		}
	}

}; // class CodeIndexerExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeIterationStatement : CodeStatement
{

	// Internal state.
	private CodeStatement _InitStatement;
	private CodeExpression _TestExpression;
	private CodeStatement _IncrementStatement;
	private CodeStatementCollection _Statements;

	// Constructors.
	public CodeIterationStatement()
	{
	}
	public CodeIterationStatement(CodeStatement _InitStatement, CodeExpression _TestExpression, CodeStatement _IncrementStatement, params CodeStatement[] _Statements)
	{
		this._InitStatement = _InitStatement;
		this._TestExpression = _TestExpression;
		this._IncrementStatement = _IncrementStatement;
		this.Statements.AddRange(_Statements);
	}

	// Properties.
	public CodeStatement InitStatement
	{
		get
		{
			return _InitStatement;
		}
		set
		{
			_InitStatement = value;
		}
	}
	public CodeExpression TestExpression
	{
		get
		{
			return _TestExpression;
		}
		set
		{
			_TestExpression = value;
		}
	}
	public CodeStatement IncrementStatement
	{
		get
		{
			return _IncrementStatement;
		}
		set
		{
			_IncrementStatement = value;
		}
	}
	public CodeStatementCollection Statements
	{
		get
		{
			if(_Statements == null)
			{
				_Statements = new CodeStatementCollection();
			}
			return _Statements;
		}
	}

}; // class CodeIterationStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeLabeledStatement : CodeStatement
{

	// Internal state.
	private String _Label;
	private CodeStatement _Statement;

	// Constructors.
	public CodeLabeledStatement()
	{
	}
	public CodeLabeledStatement(String _Label)
	{
		this._Label = _Label;
	}
	public CodeLabeledStatement(String _Label, CodeStatement _Statement)
	{
		this._Label = _Label;
		this._Statement = _Statement;
	}

	// Properties.
	public String Label
	{
		get
		{
			return _Label;
		}
		set
		{
			_Label = value;
		}
	}
	public CodeStatement Statement
	{
		get
		{
			return _Statement;
		}
		set
		{
			_Statement = value;
		}
	}

}; // class CodeLabeledStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeMethodReferenceExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _TargetObject;
	private String _MethodName;

	// Constructors.
	public CodeMethodReferenceExpression()
	{
	}
	public CodeMethodReferenceExpression(CodeExpression _TargetObject, String _MethodName)
	{
		this._TargetObject = _TargetObject;
		this._MethodName = _MethodName;
	}

	// Properties.
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public String MethodName
	{
		get
		{
			return _MethodName;
		}
		set
		{
			_MethodName = value;
		}
	}

}; // class CodeMethodReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeMethodReturnStatement : CodeStatement
{

	// Internal state.
	private CodeExpression _Expression;

	// Constructors.
	public CodeMethodReturnStatement()
	{
	}
	public CodeMethodReturnStatement(CodeExpression _Expression)
	{
		this._Expression = _Expression;
	}

	// Properties.
	public CodeExpression Expression
	{
		get
		{
			return _Expression;
		}
		set
		{
			_Expression = value;
		}
	}

}; // class CodeMethodReturnStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeNamespaceCollection : CollectionBase
{

	// Constructors.
	public CodeNamespaceCollection()
	{
	}
	public CodeNamespaceCollection(CodeNamespace[] value)
	{
		AddRange(value);
	}
	public CodeNamespaceCollection(CodeNamespaceCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeNamespace this[int index]
	{
		get
		{
			return (CodeNamespace)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeNamespace value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeNamespace[] value)
	{
		foreach(CodeNamespace e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeNamespaceCollection value)
	{
		foreach(CodeNamespace e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeNamespace value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeNamespace[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeNamespace value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeNamespace value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeNamespace value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeNamespaceCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeParameterDeclarationExpressionCollection : CollectionBase
{

	// Constructors.
	public CodeParameterDeclarationExpressionCollection()
	{
	}
	public CodeParameterDeclarationExpressionCollection(CodeParameterDeclarationExpression[] value)
	{
		AddRange(value);
	}
	public CodeParameterDeclarationExpressionCollection(CodeParameterDeclarationExpressionCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeParameterDeclarationExpression this[int index]
	{
		get
		{
			return (CodeParameterDeclarationExpression)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeParameterDeclarationExpression value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeParameterDeclarationExpression[] value)
	{
		foreach(CodeParameterDeclarationExpression e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeParameterDeclarationExpressionCollection value)
	{
		foreach(CodeParameterDeclarationExpression e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeParameterDeclarationExpression value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeParameterDeclarationExpression[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeParameterDeclarationExpression value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeParameterDeclarationExpression value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeParameterDeclarationExpression value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeParameterDeclarationExpressionCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodePrimitiveExpression : CodeExpression
{

	// Internal state.
	private Object _Value;

	// Constructors.
	public CodePrimitiveExpression()
	{
	}
	public CodePrimitiveExpression(Object _Value)
	{
		this._Value = _Value;
	}

	// Properties.
	public Object Value
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
		}
	}

}; // class CodePrimitiveExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodePropertyReferenceExpression : CodeExpression
{

	// Internal state.
	private CodeExpression _TargetObject;
	private String _PropertyName;

	// Constructors.
	public CodePropertyReferenceExpression()
	{
	}
	public CodePropertyReferenceExpression(CodeExpression _TargetObject, String _PropertyName)
	{
		this._TargetObject = _TargetObject;
		this._PropertyName = _PropertyName;
	}

	// Properties.
	public CodeExpression TargetObject
	{
		get
		{
			return _TargetObject;
		}
		set
		{
			_TargetObject = value;
		}
	}
	public String PropertyName
	{
		get
		{
			return _PropertyName;
		}
		set
		{
			_PropertyName = value;
		}
	}

}; // class CodePropertyReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodePropertySetValueReferenceExpression : CodeExpression
{

	// Constructors.
	public CodePropertySetValueReferenceExpression()
	{
	}

}; // class CodePropertySetValueReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeSnippetExpression : CodeExpression
{

	// Internal state.
	private String _Value;

	// Constructors.
	public CodeSnippetExpression()
	{
	}
	public CodeSnippetExpression(String _Value)
	{
		this._Value = _Value;
	}

	// Properties.
	public String Value
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
		}
	}

}; // class CodeSnippetExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeSnippetStatement : CodeStatement
{

	// Internal state.
	private String _Value;

	// Constructors.
	public CodeSnippetStatement()
	{
	}
	public CodeSnippetStatement(String _Value)
	{
		this._Value = _Value;
	}

	// Properties.
	public String Value
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
		}
	}

}; // class CodeSnippetStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeSnippetTypeMember : CodeTypeMember
{

	// Internal state.
	private String _Text;

	// Constructors.
	public CodeSnippetTypeMember()
	{
	}
	public CodeSnippetTypeMember(String _Text)
	{
		this._Text = _Text;
	}

	// Properties.
	public String Text
	{
		get
		{
			return _Text;
		}
		set
		{
			_Text = value;
		}
	}

}; // class CodeSnippetTypeMember

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeStatement : CodeObject
{

	// Internal state.
	private CodeLinePragma _LinePragma;

	// Constructors.
	public CodeStatement()
	{
	}

	// Properties.
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

}; // class CodeStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeThisReferenceExpression : CodeExpression
{

	// Constructors.
	public CodeThisReferenceExpression()
	{
	}

}; // class CodeThisReferenceExpression

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeThrowExceptionStatement : CodeStatement
{

	// Internal state.
	private CodeExpression _ToThrow;

	// Constructors.
	public CodeThrowExceptionStatement()
	{
	}
	public CodeThrowExceptionStatement(CodeExpression _ToThrow)
	{
		this._ToThrow = _ToThrow;
	}

	// Properties.
	public CodeExpression ToThrow
	{
		get
		{
			return _ToThrow;
		}
		set
		{
			_ToThrow = value;
		}
	}

}; // class CodeThrowExceptionStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeTryCatchFinallyStatement : CodeStatement
{

	// Internal state.
	private CodeStatementCollection _TryStatements;
	private CodeCatchClauseCollection _CatchClauses;
	private CodeStatementCollection _FinallyStatements;

	// Constructors.
	public CodeTryCatchFinallyStatement()
	{
	}
	public CodeTryCatchFinallyStatement(CodeStatement[] _TryStatements, CodeCatchClause[] _CatchClauses)
	{
		this.TryStatements.AddRange(_TryStatements);
		this.CatchClauses.AddRange(_CatchClauses);
	}
	public CodeTryCatchFinallyStatement(CodeStatement[] _TryStatements, CodeCatchClause[] _CatchClauses, CodeStatement[] _FinallyStatements)
	{
		this.TryStatements.AddRange(_TryStatements);
		this.CatchClauses.AddRange(_CatchClauses);
		this.FinallyStatements.AddRange(_FinallyStatements);
	}

	// Properties.
	public CodeStatementCollection TryStatements
	{
		get
		{
			if(_TryStatements == null)
			{
				_TryStatements = new CodeStatementCollection();
			}
			return _TryStatements;
		}
	}
	public CodeCatchClauseCollection CatchClauses
	{
		get
		{
			if(_CatchClauses == null)
			{
				_CatchClauses = new CodeCatchClauseCollection();
			}
			return _CatchClauses;
		}
	}
	public CodeStatementCollection FinallyStatements
	{
		get
		{
			if(_FinallyStatements == null)
			{
				_FinallyStatements = new CodeStatementCollection();
			}
			return _FinallyStatements;
		}
	}

}; // class CodeTryCatchFinallyStatement

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeTypeConstructor : CodeMemberMethod
{

	// Constructors.
	public CodeTypeConstructor()
	{
	}

}; // class CodeTypeConstructor

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeTypeDeclarationCollection : CollectionBase
{

	// Constructors.
	public CodeTypeDeclarationCollection()
	{
	}
	public CodeTypeDeclarationCollection(CodeTypeDeclaration[] value)
	{
		AddRange(value);
	}
	public CodeTypeDeclarationCollection(CodeTypeDeclarationCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeTypeDeclaration this[int index]
	{
		get
		{
			return (CodeTypeDeclaration)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeTypeDeclaration value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeTypeDeclaration[] value)
	{
		foreach(CodeTypeDeclaration e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeTypeDeclarationCollection value)
	{
		foreach(CodeTypeDeclaration e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeTypeDeclaration value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeTypeDeclaration[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeTypeDeclaration value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeTypeDeclaration value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeTypeDeclaration value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeTypeDeclarationCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeTypeMemberCollection : CollectionBase
{

	// Constructors.
	public CodeTypeMemberCollection()
	{
	}
	public CodeTypeMemberCollection(CodeTypeMember[] value)
	{
		AddRange(value);
	}
	public CodeTypeMemberCollection(CodeTypeMemberCollection value)
	{
		AddRange(value);
	}

	// Properties.
	public CodeTypeMember this[int index]
	{
		get
		{
			return (CodeTypeMember)(List[index]);
		}
		set
		{
			List[index] = value;
		}
	}

	// Methods.
	public int Add(CodeTypeMember value)
	{
		return List.Add(value);
	}
	public void AddRange(CodeTypeMember[] value)
	{
		foreach(CodeTypeMember e in value)
		{
			List.Add(e);
		}
	}
	public void AddRange(CodeTypeMemberCollection value)
	{
		foreach(CodeTypeMember e in value)
		{
			List.Add(e);
		}
	}
	public bool Contains(CodeTypeMember value)
	{
		return List.Contains(value);
	}
	public void CopyTo(CodeTypeMember[] array, int index)
	{
		List.CopyTo(array, index);
	}
	public int IndexOf(CodeTypeMember value)
	{
		return List.IndexOf(value);
	}
	public void Insert(int index, CodeTypeMember value)
	{
		List.Insert(index, value);
	}
	public void Remove(CodeTypeMember value)
	{
		int index = List.IndexOf(value);
		if(index < 0)
		{
			throw new ArgumentException(S._("Arg_NotCollMember"), "value");
		}
		List.RemoveAt(index);
	}

}; // class CodeTypeMemberCollection

[Serializable]
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
#endif
public class CodeVariableReferenceExpression : CodeExpression
{

	// Internal state.
	private String _VariableName;

	// Constructors.
	public CodeVariableReferenceExpression()
	{
	}
	public CodeVariableReferenceExpression(String _VariableName)
	{
		this._VariableName = _VariableName;
	}

	// Properties.
	public String VariableName
	{
		get
		{
			return _VariableName;
		}
		set
		{
			_VariableName = value;
		}
	}

}; // class CodeVariableReferenceExpression

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
