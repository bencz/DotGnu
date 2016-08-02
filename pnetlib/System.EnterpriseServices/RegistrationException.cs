/*
 * RegistrationException.cs - Implementation of the
 *			"System.EnterpriseServices.RegistrationException" class.
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

using System.Runtime.Serialization;

#if !ECMA_COMPAT
[Serializable]
#endif
public sealed class RegistrationException : SystemException
{
	// Internal state.
	private RegistrationErrorInfo[] errorInfo;

	// Constructors.
	public RegistrationException(String msg) : base(msg) {}
#if CONFIG_SERIALIZATION
	internal RegistrationException
				(SerializationInfo info, StreamingContext context)
			: base(info, context)
			{
				errorInfo = (RegistrationErrorInfo[])info.GetValue
					("ErrorInfo", typeof(RegistrationErrorInfo[]));
			}

	// Get the serialization information for this object.
	public override void GetObjectData
				(SerializationInfo info, StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("ErrorInfo", errorInfo,
							  typeof(RegistrationErrorInfo[]));
			}
#endif

	// Get the error object's within this exception.
	public RegistrationErrorInfo[] ErrorInfo
			{
				get
				{
					return errorInfo;
				}
			}

}; // class RegistrationException

}; // namespace System.EnterpriseServices
