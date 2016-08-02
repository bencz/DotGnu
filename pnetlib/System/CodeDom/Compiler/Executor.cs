/*
 * Executor.cs - Implementation of the
 *		System.CodeDom.Compiler.Executor class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.CodeDom.Compiler
{

#if CONFIG_CODEDOM

public sealed class Executor
{
	// Cannot instantiate this class.
	private Executor() {}

	// Execute the compiler and wait for it to return.
	public static void ExecWait(String cmd, TempFileCollection tempFiles)
			{
				String outputName = null;
				String errorName = null;
				ExecWaitWithCapture(IntPtr.Zero, cmd, null, tempFiles,
									ref outputName, ref errorName);
			}

	// Execute the compiler and capture its output.
	public static int ExecWaitWithCapture
				(String cmd, TempFileCollection tempFiles,
				 ref String outputName, ref String errorName)
			{
				return ExecWaitWithCapture(IntPtr.Zero, cmd, null,
										   tempFiles, ref outputName,
										   ref errorName);
			}
	public static int ExecWaitWithCapture
				(IntPtr userToken, String cmd, TempFileCollection tempFiles,
				 ref String outputName, ref String errorName)
			{
				return ExecWaitWithCapture(userToken, cmd, null,
										   tempFiles, ref outputName,
										   ref errorName);
			}
	public static int ExecWaitWithCapture
				(String cmd, String currentDir, TempFileCollection tempFiles,
				 ref String outputName, ref String errorName)
			{
				return ExecWaitWithCapture(IntPtr.Zero, cmd, currentDir,
										   tempFiles, ref outputName,
										   ref errorName);
			}
	public static int ExecWaitWithCapture
				(IntPtr userToken, String cmd, String currentDir,
				 TempFileCollection tempFiles, ref String outputName,
				 ref String errorName)
			{
				// We do not allow the compiler to be launched this way
				// as it is a severe security threat to allow arbitrary
				// command-lines to be passed to the underlying system.
				// Besides, Unix-style systems don't like command lines
				// that consist of a single string.  Use ICodeCompiler
				// to compile source files instead of this class.

				throw new NotImplementedException();
			}

}; // class Executor

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
