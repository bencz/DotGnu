/*
 * IConstructionCallMessage.cs - Implementation of the
 *		"System.Runtime.Remoting.Activation.IConstructionCallMessage" class.
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

namespace System.Runtime.Remoting.Activation
{

#if CONFIG_REMOTING

using System.Collections;
using System.Runtime.Remoting.Messaging;

public interface IConstructionCallMessage
	: IMethodCallMessage, IMethodMessage, IMessage
{

	// Get the type of activation.
	Type ActivationType { get; }

	// Get the name of the type of activation.
	String ActivationTypeName { get; }

	// Get or set the activator to use for this message.
	IActivator Activator { get; set; }

	// Get the call site attributes.
	Object[] CallSiteActivationAttributes { get; }

	// Get the context properties.
	IList ContextProperties { get; }

}; // interface IConstructionCallMessage

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Activation
