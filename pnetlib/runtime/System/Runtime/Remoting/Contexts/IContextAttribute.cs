/*
 * IContextAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Contexts.IContextAttribute" class.
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

namespace System.Runtime.Remoting.Contexts
{

#if CONFIG_REMOTING

using System.Runtime.Remoting.Activation;

public interface IContextAttribute
{
	// Get the properties for a new construction context.
	void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg);

	// Determine if a context is OK with respect to this attribute.
	bool IsContextOK(Context ctx, IConstructionCallMessage msg);

}; // interface IContextAttribute

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Contexts
