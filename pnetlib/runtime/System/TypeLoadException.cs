/*
 * TypeLoadException.cs - Implementation of the
 *			"System.TypeLoadException" class.
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

namespace System
{

#if CONFIG_RUNTIME_INFRA

using System.Runtime.Serialization;

public class TypeLoadException : SystemException
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	// Internal state.
	private String typeName;
	private String assemblyName;
	private String messageArg;
	private int resourceId;

	// Constructors.
	public TypeLoadException()
		: base(_("Exception_TypeLoad")) {}
	public TypeLoadException(String msg)
		: base(msg) {}
	public TypeLoadException(String msg, Exception inner)
		: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	protected TypeLoadException(SerializationInfo info,
								StreamingContext context)
		: base(info, context)
		{
			typeName = info.GetString("TypeLoadClassName");
			assemblyName = info.GetString("TypeLoadAssemblyName");
			messageArg = info.GetString("TypeLoadMessageArg");
			resourceId = info.GetInt32("TypeLoadResourceID");
		}
#endif

	// Internal constructor that is used by the runtime engine.
	private TypeLoadException(String typeName, String assembly)
		: base(null)
		{
			typeName = typeName;
			assemblyName = assembly;
		}

	// Properties.
	public String TypeName
			{
				get
				{
					return typeName;
				}
			}
	public override String Message
			{
				get
				{
					String result = String.Empty;
					if(assemblyName != null)
					{
						result = result + "[" + assemblyName + "]";
					}
					if(typeName != null)
					{
						result = result + typeName + ":";
					}
					return result + base.Message;
				}
			}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_TypeLoad");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131522;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("TypeLoadClassName", typeName);
				info.AddValue("TypeLoadAssemblyName", assemblyName);
				info.AddValue("TypeLoadMessageArg", messageArg);
				info.AddValue("TypeLoadResourceID", resourceId);
			}
#endif

}; // class TypeLoadException

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
