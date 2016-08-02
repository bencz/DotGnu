/*
 * ReturnMessage.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.ReturnMessage" class.
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

public class ReturnMessage : IMethodReturnMessage, IMethodMessage, IMessage
{
	// Internal state.
	private Exception exception;
	private IMethodCallMessage mcm;
	private Object ret;
	private Object[] outArgs;
	private int outArgsCount;
	private LogicalCallContext callCtx;

	// Constructor.
	public ReturnMessage(Exception e, IMethodCallMessage mcm)
			{
				this.exception = e;
				this.mcm = mcm;
			}
	public ReturnMessage(Object ret, Object[] outArgs, int outArgsCount,
						 LogicalCallContext callCtx, IMethodCallMessage mcm)
			{
				this.ret = ret;
				this.outArgs = outArgs;
				this.outArgsCount = outArgsCount;
				this.callCtx = callCtx;
				this.mcm = mcm;
			}

	// Implement the IMethodReturnMessage interface.
	public Exception Exception
			{
				get
				{
					return exception;
				}
			}
	public int OutArgCount
			{
				get
				{
					return outArgsCount;
				}
			}
	public Object[] OutArgs
			{
				get
				{
					return outArgs;
				}
			}
	public virtual Object ReturnValue
			{
				get
				{
					return ret;
				}
			}
	public Object GetOutArg(int argNum)
			{
				return outArgs[argNum];
			}
	public String GetOutArgName(int index)
			{
				// We don't have argument names available.
				return null;
			}

	// Implement the IMethodMessage interface.
	public int ArgCount
			{
				get
				{
					return mcm.ArgCount;
				}
			}
	public Object[] Args
			{
				get
				{
					return mcm.Args;
				}
			}
	public bool HasVarArgs
			{
				get
				{
					return mcm.HasVarArgs;
				}
			}
	public LogicalCallContext LogicalCallContext
			{
				get
				{
					if(callCtx != null)
					{
						return callCtx;
					}
					else
					{
						return mcm.LogicalCallContext;
					}
				}
			}
	public MethodBase MethodBase
			{
				get
				{
					return mcm.MethodBase;
				}
			}
	public String MethodName
			{
				get
				{
					return mcm.MethodName;
				}
			}
	public Object MethodSignature
			{
				get
				{
					return mcm.MethodSignature;
				}
			}
	public String TypeName
			{
				get
				{
					return mcm.TypeName;
				}
			}
	public String Uri
			{
				get
				{
					return mcm.Uri;
				}
				set
				{
					mcm.Uri = value;
				}
			}
	public Object GetArg(int argNum)
			{
				return mcm.GetArg(argNum);
			}
	public String GetArgName(int index)
			{
				return mcm.GetArgName(index);
			}

	// Implement the IMessage interface.
	public virtual IDictionary Properties
			{
				get
				{
					return mcm.Properties;
				}
			}

}; // interface IMethodReturnMessage

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Messaging
