/*
 * StackTrace.cs - Implementation of the
 *			"System.Diagnostics.StackTrace" class.
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
using System.Threading;
using System.Runtime.CompilerServices;

#if CONFIG_EXTENDED_DIAGNOSTICS
public
#else
internal
#endif
class StackTrace
{
	// Internal state.
	private StackFrame[] frames;
	private int          numFrames;

	// Number of methods that should be omitted from the stack trace.
	public const int METHODS_TO_SKIP = 0;

	// Constructors.
	public StackTrace()
			{
				Initialize(null, 0, false);
			}
	public StackTrace(int skipFrames)
			{
				Initialize(null, skipFrames, false);
			}
	public StackTrace(bool needFileInfo)
			{
				Initialize(null, 0, needFileInfo);
			}
	public StackTrace(int skipFrames, bool needFileInfo)
			{
				Initialize(null, skipFrames, needFileInfo);
			}
	public StackTrace(Exception e)
			{
				Initialize(e, 0, false);
			}
	public StackTrace(Exception e, int skipFrames)
			{
				Initialize(e, skipFrames, false);
			}
	public StackTrace(Exception e, bool needFileInfo)
			{
				Initialize(e, 0, needFileInfo);
			}
	public StackTrace(Exception e, int skipFrames, bool needFileInfo)
			{
				Initialize(e, skipFrames, needFileInfo);
			}
	public StackTrace(StackFrame frame)
			{
				frames = new StackFrame[1];
				frames[0] = frame;
				numFrames = 1;
			}
	public StackTrace(Thread thread, bool needFileInfo)
			{
				if(thread == null || thread == Thread.CurrentThread)
				{
					Initialize(null, 0, needFileInfo);
				}
				else
				{
					UnpackStackTrace(thread.GetPackedStackTrace(),
									 0, needFileInfo);
				}
			}

	// Initialize this object with a complete stack trace.
	private void Initialize(Exception e, int skipFrames, bool needFileInfo)
			{
				if(e == null)
				{
					// Get the total number of frames on the stack at present.
					int totalFrames = StackFrame.InternalGetTotalFrames();

					// Validate "skipFrames", and then account for the
					// extra frames that we have pushed while getting here.
					if(skipFrames < 0 || skipFrames > (totalFrames - 2))
					{
						skipFrames = 0;
					}
					skipFrames += 2;

					// Create the frame array.
					totalFrames -= 2;
					numFrames = totalFrames;
					frames = new StackFrame [totalFrames];

					// Fill the frame array.
					int posn = 0;
					while(posn < totalFrames)
					{
						frames[posn] =
							new StackFrame(posn + skipFrames, needFileInfo);
						++posn;
					}
				}
				else
				{
					// Unpack an exception's "packed" stack trace
					// to determine where it was thrown from.
					UnpackStackTrace(e.GetPackedStackTrace(),
									 skipFrames, needFileInfo);
				}
			}

	// Get the number of frames in the stack trace.
	public virtual int FrameCount
			{
				get
				{
					return numFrames;
				}
			}

	// Get information about a specific stack frame.
	public virtual StackFrame GetFrame(int index)
			{
				if(index >= 0 && index < numFrames)
				{
					return frames[index];
				}
				else
				{
					return null;
				}
			}

	// Convert the stack trace into a string.
	public override String ToString()
			{
			#if CONFIG_REFLECTION
				String result = String.Empty;
				int posn;
				MethodBase method;
				Type type;
				String nmspace;
				ParameterInfo[] paramList;
				int param;
				String filename;
				int line, col;

				for(posn = 0; posn < numFrames; ++posn)
				{
					method = frames[posn].GetMethod();
					if(method != null)
					{
						result = result + "\tat ";
						type = method.DeclaringType;
						if(type != null)
						{
							nmspace = type.Namespace;
							if(nmspace != null)
							{
								result = result + nmspace + ".";
							}
							result = result + type.Name + ".";
						}
						result = result + method.Name + "(";
						paramList = method.GetParameters();
						for(param = 0; param < paramList.Length; ++param)
						{
							if(param != 0)
							{
								result = result + ", ";
							}
							result = result +
									 paramList[param].ParameterType.Name;
						}
						result = result + ")";
					}
					else if(posn < (numFrames - 1))
					{
						result = result + "\tat <unknown method>";
					}
					else
					{
						// This frame is probably the top-most native
						// function frame for the thread, which is not
						// normally interesting for a stack trace report.
						continue;
					}
					filename = frames[posn].GetFileName();
					if(filename != null)
					{
						result = result + " in " + filename;
						line = frames[posn].GetFileLineNumber();
						col = frames[posn].GetFileColumnNumber();
						if(line != 0)
						{
							result = result + ":" + line.ToString();
							if(col != 0)
							{
								result = result + ":" + col.ToString();
							}
						}
					}
					result = result + Environment.NewLine;
				}

				return result;
			#else
				return String.Empty;
			#endif
			}

	// Unpack a stack trace which was encoded into an exception or thread.
	private void UnpackStackTrace(PackedStackFrame[] trace,
								  int skipFrames, bool needFileInfo)
			{
				int traceSize, posn;

				// Get the size of the packed stack trace.
				if(trace != null)
				{
					traceSize = trace.Length;
				}
				else
				{
					traceSize = 0;
				}

				// Validate the "skipFrames" parameter.
				if(skipFrames < 0)
				{
					skipFrames = 0;
				}
				else if(skipFrames > traceSize)
				{
					skipFrames = traceSize;
				}

				// Convert the trace information into its expanded form.
				numFrames = traceSize - skipFrames;
				frames = new StackFrame [numFrames];
				for(posn = 0; posn < numFrames; ++posn)
				{
					frames[posn] = new StackFrame(trace[posn + skipFrames],
												  needFileInfo);
				}
			}

}; // class StackTrace

}; // namespace System.Diagnostics
