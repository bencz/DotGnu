/*
 * ObjectHandle.cs - Implementation of the
 *			"System.Runtime.Remoting.ObjectHandle" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Remoting
{

#if CONFIG_REMOTING

using System.Runtime.InteropServices;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#endif
public class ObjectHandle : MarshalByRefObject, IObjectHandle
{
	// Internal state.
	private Object obj;

	// Constructor.
	public ObjectHandle(Object obj)
			{
				this.obj = obj;
			}

	// Unwrap the object handle.
	public Object Unwrap()
			{
				return obj;
			}

	// Initialize the lifetime service value for this object.
	public override Object InitializeLifetimeService()
			{
				return base.InitializeLifetimeService();
			}

}; // class ObjectHandle

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting
