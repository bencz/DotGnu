/*
 * ServicedComponentException.cs - Implementation of the
 *			"System.EnterpriseServices.ServicedComponentException" class.
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

namespace System.EnterpriseServices
{

using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#if !ECMA_COMPAT
[ComVisible(false)]
[Serializable]
#endif
public sealed class ServicedComponentException : SystemException
{
	// Internal state.
	private RegistrationErrorInfo[] errorInfo;

	// Constructors.
	public ServicedComponentException() : base() {}
	public ServicedComponentException(String msg)
		: base(msg) {}
	public ServicedComponentException(String msg, Exception inner)
		: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	internal ServicedComponentException
				(SerializationInfo info, StreamingContext context)
			: base(info, context) {}
#endif

}; // class ServicedComponentException

}; // namespace System.EnterpriseServices
