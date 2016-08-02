/*
 * StackFrame.cs - Implementation of the
 *			"System.Diagnostics.StackFrame" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Diagnostics
{

using System.Reflection;
using System.Runtime.CompilerServices;

#if CONFIG_EXTENDED_DIAGNOSTICS
public
#else
internal
#endif
class StackFrame
{
	// Internal state.
	private String 		filename;
	private int    		line, column;
	private int    		offset;
	private int    		nativeOffset;
#if CONFIG_REFLECTION
	private MethodBase	method;
#endif

	// Value that is returned for unknown offsets.
	public const int OFFSET_UNKNOWN = -1;

	// Constructors.
	public StackFrame() : this(0, false)
			{
				Initialize(0, false);
			}
	public StackFrame(bool needFileInfo)
			{
				Initialize(0, needFileInfo);
			}
	public StackFrame(int skipFrames)
			{
				Initialize(skipFrames, false);
			}
	public StackFrame(int skipFrames, bool needFileInfo)
			{
				Initialize(skipFrames, needFileInfo);
			}
	public StackFrame(String fileName, int lineNumber)
			{
				Initialize(0, false);
				filename = fileName;
				line = lineNumber;
				column = 0;
			}
	public StackFrame(String fileName, int lineNumber, int colNumber)
			{
				Initialize(0, false);
				filename = fileName;
				line = lineNumber;
				column = colNumber;
			}

	// Construct a stack frame from packed exception or thread frame data.
	internal StackFrame(PackedStackFrame frame, bool needFileInfo)
			{
				offset = frame.offset;
				nativeOffset = frame.nativeOffset;
			#if CONFIG_REFLECTION
				method = MethodBase.GetMethodFromHandle(frame.method);
				if(needFileInfo && method != null &&
				   offset != OFFSET_UNKNOWN)
				{
					filename = InternalGetDebugInfo
						(frame.method, offset, out line, out column);
				}
			#endif
			}

	// Initialize this object with the stack frame information.
	private void Initialize(int skipFrames, bool needFileInfo)
			{
				if(skipFrames >= 0 && skipFrames < (Int32.MaxValue - 2))
				{
					// Account for the extra frames added by this class
					// in the process of getting here.
					skipFrames += 2;

					// Get the method and offset information.
				#if CONFIG_REFLECTION
					method = MethodBase.GetMethodFromHandle
								(InternalGetMethod(skipFrames));
				#endif
					offset = InternalGetILOffset(skipFrames);
					nativeOffset = InternalGetNativeOffset(skipFrames);
				}
				else
				{
					offset = OFFSET_UNKNOWN;
					nativeOffset = OFFSET_UNKNOWN;
				}
			#if CONFIG_REFLECTION
				if(needFileInfo && method != null &&
				   offset != OFFSET_UNKNOWN)
				{
					filename = InternalGetDebugInfo
						(method.MethodHandle, offset, out line, out column);
				}
			#endif
			}

	// Get the total number of stack frames in use, excluding
	// the one that will be set up to call this method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int InternalGetTotalFrames();

#if CONFIG_RUNTIME_INFRA
	// Get the method that is executing "skipFrames" frames up the stack,
	// where 0 indicates the method that called "InternalGetMethod".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static RuntimeMethodHandle
				InternalGetMethod(int skipFrames);

	// Get the filename, line number, and column number associated
	// with a particular method and IL offset.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String InternalGetDebugInfo
				(RuntimeMethodHandle method, int offset,
				 out int line, out int column);
#endif

	// Get the IL offset of the method that is executing "skipFrames"
	// frames up the stack.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int InternalGetILOffset(int skipFrames);

	// Get the native offset of the method that is executing "skipFrames"
	// frames up the stack.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int InternalGetNativeOffset(int skipFrames);

	// Get the column number where the stack frame was created.
	public virtual int GetFileColumnNumber()
			{
				return column;
			}

	// Get the line number where the stack frame was created.
	public virtual int GetFileLineNumber()
			{
				return line;
			}

	// Get the filename where the stack frame was created.
	public virtual String GetFileName()
			{
				return filename;
			}

	// Get the IL offset within the code of where the stack frame was created.
	public virtual int GetILOffset()
			{
				return offset;
			}

	// Get the native offset within the code of where
	// the stack frame was created.
	public virtual int GetNativeOffset()
			{
				return nativeOffset;
			}

#if CONFIG_REFLECTION

	// Get the method that was executing where the stack frame was created.
	public virtual MethodBase GetMethod()
			{
				return method;
			}

#endif // CONFIG_REFLECTION

	// Convert this stack frame into a string.
	public override String ToString()
			{
				String result;
			#if CONFIG_REFLECTION
				if(method != null)
				{
					result = method.Name;
					if(offset != OFFSET_UNKNOWN)
					{
						result = result + " at offset " + offset;
					}
				}
				else
			#endif
				if(offset != OFFSET_UNKNOWN)
				{
					result = "at offset " + offset;
				}
				else
				{
					result = String.Empty;
				}
				if(filename != null)
				{
					result = result + " in " + filename;
				}
				else
				{
					result = result + " in <unknown file>";
				}
				if(line != 0)
				{
					result = result + ":" + line;
					if(column != 0)
					{
						result = result + ":" + column;
					}
				}
				else if(column != 0)
				{
					result = result + ":0:" + column;
				}
				return result + Environment.NewLine;
			}

	// Get an exception stack trace for the currently executing context.
	// The runtime engine must skip over any frames that correspond to
	// exception constructors to find the actual frame that threw the
	// exception.  This assumes that exception constructors do not throw
	// exceptions of their own, which is usually a safe assumption.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static PackedStackFrame[] GetExceptionStackTrace();

}; // class StackFrame

}; // namespace System.Diagnostics
