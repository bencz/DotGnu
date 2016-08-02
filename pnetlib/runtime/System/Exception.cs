/*
 * Exception.cs - Implementation of the "System.Exception" class.
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

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Diagnostics;

/*

Note on Exception messages:
--------------------------

This class library takes a slightly different approach for determining
what the default message text should be for internal exceptions.  Other
implementations override "Message" and "ToString()", and then back-patch
the "message" field in the base class if it is null.  This is a pain to
implement, especially for "ToString()" which must include the stack trace
in the result, amongst other things.

Instead, we provide two "internal" properties that only classes
in this library can access.  "MessageDefault" provides a default message
if "message" is null.  "MessageExtra" provides extra information to be
inserted into the "ToString()" result just after the message and before
the stack trace.

A similar approach is used to get the HResult values.

This design is cleaner to implement throughout the library.  Because the
extra properties are "internal", they will not pollute the name space of
applications that didn't expect them to be present.

*/

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_Exception))]
#endif
#endif
public class Exception
#if CONFIG_SERIALIZATION
	: ISerializable
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
	, _Exception
#endif
#elif CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
	: _Exception
#endif
{

	// Private members.
	private String message;
	private Exception innerException;
	private PackedStackFrame[] stackTrace;
#if !ECMA_COMPAT
	private String source;
	private String helpLink;
	private int hresult;
	private bool hresultSet;
	private String stackTraceString;
	private String remoteStackTraceString;
	private int remoteStackIndex;
	private String exceptionMethodString;
#endif

	// Constructors.
	public Exception() : this(null, null) {}
	public Exception(String msg) : this(msg, null) {}
	public Exception(String msg, Exception inner)
		{
			message = msg;
			innerException = inner;
		}
#if CONFIG_SERIALIZATION
	protected Exception(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
			{
				throw new ArgumentNullException("info");
			}
			message = info.GetString("Message");
			innerException = (Exception)(info.GetValue("InnerException",
													   typeof(Exception)));
			helpLink = info.GetString("HelpURL");
			source = info.GetString("Source");
			stackTraceString = info.GetString("StackTraceString");
			remoteStackTraceString = info.GetString("RemoteStackTraceString");
			remoteStackIndex = info.GetInt32("RemoteStackIndex");
			exceptionMethodString = info.GetString("ExceptionMethod");
			hresult = info.GetInt32("HResult");
			if(hresult != 0)
			{
				hresultSet = true;
			}
		}
#endif

	// Private constructor that is used for subclasses that
	// don't want stack traces.  e.g. OutOfMemoryException.
	internal Exception(String msg, Exception inner, bool wantTrace)
		{
			message = msg;
			innerException = inner;
			if(wantTrace)
			{
				try
				{
					stackTrace = StackFrame.GetExceptionStackTrace();
				}
				catch(NotImplementedException)
				{
					stackTrace = null;
				}
			}
		}

	// Get the base exception upon which this exception is based.
	public virtual Exception GetBaseException()
		{
			Exception result = this;
			Exception inner;
			while((inner = result.InnerException) != null)
			{
				result = inner;
			}
			return result;
		}

	// Convert the exception into a string.
	public override String ToString()
		{
			String className;
			String result;
			String temp;
			String message = Message;
			try
			{
				className = GetType().ToString();
			}
			catch(NotImplementedException)
			{
				// The runtime engine does not have reflection support.
				className = String.Empty;
			}
			if(message != null && message.Length > 0)
			{
				if(className != null && className.Length > 0)
				{
					result = className + ": " + message;
				}
				else
				{
					result = message;
				}
			}
			else if(className != null && className.Length > 0)
			{
				result = className;
			}
			else
			{
				// Default message if we cannot get a message from
				// the underlying resource sub-system.
				result = "Exception was thrown";
			}
			temp = MessageExtra;
			if(temp != null)
			{
				result = result + Environment.NewLine + temp;
			}
			if(innerException != null)
			{
				result = result + " ---> " + innerException.ToString();
			}
			temp = StackTrace;
			if(temp != null)
			{
				result = result + Environment.NewLine + temp;
			}
			return result;
		}

	// Properties.
#if !ECMA_COMPAT
	protected int HResult
		{
			get
			{
				if(!hresultSet)
				{
					hresult = (int)HResultDefault;
					hresultSet = true;
				}
				return hresult;
			}
			set
			{
				hresult = value;
				hresultSet = true;
			}
		}
	public virtual String HelpLink
		{
			get
			{
				return helpLink;
			}
			set
			{
				helpLink = value;
			}
		}
	public virtual String Source
		{
			get
			{
				if(source == null && stackTrace != null &&
				   stackTrace.Length > 0)
				{
					MethodBase method = MethodBase.GetMethodFromHandle
								(stackTrace[0].method);
					source = method.DeclaringType.Module.Assembly.FullName;
				}
				return source;
			}
			set
			{
				source = value;
			}
		}
	public MethodBase TargetSite
		{
			get
			{
				if(stackTrace != null && stackTrace.Length > 0)
				{
					return MethodBase.GetMethodFromHandle
								(stackTrace[0].method);
				}
				else
				{
					return null;
				}
			}
		}
#endif
	public Exception InnerException
		{
			get
			{
				return innerException;
			}
		}
	public virtual String Message
		{
			get
			{
				if(message != null)
				{
					return message;
				}
				else if((message = MessageDefault) != null)
				{
					return message;
				}
				else
				{
					try
					{
						return String.Format
							(_("Exception_WasThrown"), GetType().ToString());
					}
					catch(NotImplementedException)
					{
						return String.Empty;
					}
				}
			}
		}
	public virtual String StackTrace
		{
			get
			{
				if(stackTrace != null)
				{
					return (new StackTrace(this, true)).ToString();
				}
			#if !ECMA_COMPAT
				else if(stackTraceString != null)
				{
					return stackTraceString;
				}
			#endif
				else
				{
					return String.Empty;
				}
			}
		}

	// Get the packed stack trace information from this exception.
	internal PackedStackFrame[] GetPackedStackTrace()
		{
			return stackTrace;
		}

	// Get the extra data to be inserted into the "ToString" representation.
	internal virtual String MessageExtra
		{
			get
			{
				return null;
			}
		}

	// Get the default message to use if "message" was initialized to null.
	internal virtual String MessageDefault
		{
			get
			{
				return null;
			}
		}
	
	// Get the default HResult value for this type of exception.
	internal virtual uint HResultDefault
		{
			get
			{
				return 0x80131500;
			}
		}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this exception object.
	public virtual void GetObjectData(SerializationInfo info, 
									  StreamingContext context)
		{
			if(info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("ClassName", GetType().FullName, typeof(String));
			info.AddValue("Message", message, typeof(String));
			info.AddValue("InnerException", innerException, typeof(Exception));
			info.AddValue("HelpURL", helpLink, typeof(String));
			info.AddValue("Source", Source, typeof(String));
			info.AddValue("StackTraceString", StackTrace, typeof(String));
			info.AddValue("RemoteStackTraceString", remoteStackTraceString,
						  typeof(String));
			info.AddValue("RemoteStackIndex", remoteStackIndex);
			info.AddValue("ExceptionMethod", exceptionMethodString);
			info.AddValue("HResult", HResult);
		}

#endif // CONFIG_SERIALIZATION

}; // class Exception

}; // namespace System
