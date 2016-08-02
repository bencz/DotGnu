/*
 * CodeRemoveEventStatement.cs - Implementation of the
 *		System.CodeDom.CodeRemoveEventStatement class.
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
public class CodeRemoveEventStatement : CodeStatement
{

	// Internal state.
	private CodeEventReferenceExpression eventRef;
	private CodeExpression listener;

	// Constructors.
	public CodeRemoveEventStatement()
			{
			}
	public CodeRemoveEventStatement(CodeEventReferenceExpression eventRef,
									CodeExpression listener)
			{
				this.eventRef = eventRef;
				this.listener = listener;
			}
	public CodeRemoveEventStatement(CodeExpression targetObject,
									String eventName,
									CodeExpression listener)
			{
				this.eventRef = new CodeEventReferenceExpression
						(targetObject, eventName);
				this.listener = listener;
			}

	// Properties.
	public CodeEventReferenceExpression Event
			{
				get
				{
					return eventRef;
				}
				set
				{
					eventRef = value;
				}
			}
	public CodeExpression Listener
			{
				get
				{
					return listener;
				}
				set
				{
					listener = value;
				}
			}

}; // class CodeRemoveEventStatement

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
