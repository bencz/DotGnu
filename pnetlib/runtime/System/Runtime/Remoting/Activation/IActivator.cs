/*
 * IActivator.cs - Implementation of the
 *			"System.Runtime.Remoting.Activation.IActivator" class.
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

public interface IActivator
{

	// Get the level of this activator.
	ActivatorLevel Level { get; }

	// Get or set the next activator in the chain.
	IActivator NextActivator { get; set; }

	// Activate a new instance of a type.
	IConstructionReturnMessage Activate(IConstructionCallMessage msg);

}; // interface IActivator

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Activation
