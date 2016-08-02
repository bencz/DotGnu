/*
 * MethodReturnMessageWrapper.cs - Implementation of the
 *		"System.Runtime.Remoting.Messaging.MethodReturnMessageWrapper" class.
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

namespace System.Runtime.Remoting.Messaging
{

#if CONFIG_REMOTING

using System.Collections;
using System.Reflection;

public class MethodReturnMessageWrapper
	: InternalMessageWrapper, IMethodReturnMessage, IMethodMessage, IMessage
{
	// Internal state.
	private IMethodReturnMessage mrm;

	// Constructor.
	public MethodReturnMessageWrapper(IMethodReturnMessage msg)
			: base(msg)
			{
				this.mrm = msg;
			}

	// Implement the IMethodReturnMessage interface.
	public virtual IDictionary Properties
			{
				get
				{
					return mrm.Properties;
				}
			}
	public virtual int ArgCount
			{
				get
				{
					return mrm.ArgCount;
				}
			}
	public virtual Object[] Args
			{
				get
				{
					return mrm.Args;
				}
			}
	public virtual bool HasVarArgs
			{
				get
				{
					return mrm.HasVarArgs;
				}
			}
	public virtual LogicalCallContext LogicalCallContext
			{
				get
				{
					return mrm.LogicalCallContext;
				}
			}
	public virtual MethodBase MethodBase
			{
				get
				{
					return mrm.MethodBase;
				}
			}
	public virtual String MethodName
			{
				get
				{
					return mrm.MethodName;
				}
			}
	public virtual Object MethodSignature
			{
				get
				{
					return mrm.MethodSignature;
				}
			}
	public virtual String TypeName
			{
				get
				{
					return mrm.TypeName;
				}
			}
	public String Uri
			{
				get
				{
					return mrm.Uri;
				}
				set
				{
					mrm.Uri = value;
				}
			}
	public virtual Object GetArg(int argNum)
			{
				return mrm.GetArg(argNum);
			}
	public virtual String GetArgName(int index)
			{
				return mrm.GetArgName(index);
			}
	public virtual Exception Exception
			{
				get
				{
					return mrm.Exception;
				}
			}
	public virtual int OutArgCount
			{
				get
				{
					return mrm.OutArgCount;
				}
			}
	public virtual Object[] OutArgs
			{
				get
				{
					return mrm.OutArgs;
				}
			}
	public virtual Object ReturnValue
			{
				get
				{
					return mrm.ReturnValue;
				}
			}
	public virtual Object GetOutArg(int argNum)
			{
				return mrm.GetOutArg(argNum);
			}
	public virtual String GetOutArgName(int index)
			{
				return mrm.GetOutArgName(index);
			}

}; // class MethodReturnMessageWrapper

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Messaging
