/*
 * _Exception.cs - Implementation of the
 *			"System.Runtime.InteropServices._Exception" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.InteropServices
{

using System.Reflection;
using System.Runtime.Serialization;

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

[CLSCompliant(false)]
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
[Guid("b36b5c63-42ef-38bc-a07e-0b34c98f164a")]
public interface _Exception
{
	bool Equals(Object obj);
	Exception GetBaseException();
	int GetHashCode();
#if CONFIG_SERIALIZATION
	void GetObjectData(SerializationInfo info, StreamingContext context);
#endif
	Type GetType();
	String ToString();
	String HelpLink { get; set; }
	Exception InnerException { get; }
	String Message { get; }
	String Source { get; set; }
	String StackTrace { get; }
	MethodBase TargetSite { get; }

}; // interface _Exception

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION

}; // namespace System.Runtime.InteropServices
