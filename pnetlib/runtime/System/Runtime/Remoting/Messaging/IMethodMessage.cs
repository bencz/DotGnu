/*
 * IMethodMessage.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.IMethodMessage" class.
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

#if CONFIG_SERIALIZATION

using System.Reflection;

public interface IMethodMessage : IMessage
{
	// Get the number of arguments.
	int ArgCount { get; }

	// Get the argument values.
	Object[] Args { get; }

	// Determine if the message has varargs.
	bool HasVarArgs { get; }

	// Get the logical calling context for this message.
	LogicalCallContext LogicalCallContext { get; }

	// Get the method base.
	MethodBase MethodBase { get; }

	// Get the name of the method.
	String MethodName { get; }

	// Get the method's signature.
	Object MethodSignature { get; }

	// Get the name of the called object's type.
	String TypeName { get; }

	// Get the called object's URI.
	String Uri { get; set; }

	// Get a specific argument.
	Object GetArg(int argNum);

	// Get the name of a specific argument.
	String GetArgName(int index);

}; // interface IMethodMessage

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Messaging
