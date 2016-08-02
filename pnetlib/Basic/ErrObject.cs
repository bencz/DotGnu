/*
 * ErrObject.cs - Implementation of the
 *			"Microsoft.VisualBasic.ErrObject" class.
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

namespace Microsoft.VisualBasic
{

using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;

public sealed class ErrObject
{
	// Internal state.
	private Exception exception;
	private String description;
	private int erl;
	private int helpContext;
	private String helpFile;
	private int number;
	private String source;

	// Constructor.
	internal ErrObject()
			{
				Clear();
			}

	// Clear the error information.
	public void Clear()
			{
				exception = null;
				description = String.Empty;
				erl = 0;
				helpContext = 0;
				helpFile = String.Empty;
				number = -1;
				source = String.Empty;
			}

	// Get the exception within this error object.
	public Exception GetException()
			{
				return exception;
			}

	// Set the exception within this error object.
	internal void SetException(Exception e)
			{
				exception = e;
			}
	internal void SetException(Exception e, int erl)
			{
				exception = e;
				this.erl = erl;
			}

	// Raise a particular error.
	public void Raise(int Number,
					  [Optional] [DefaultValue(null)] Object Source,
					  [Optional] [DefaultValue(null)] Object Description,
					  [Optional] [DefaultValue(null)] Object HelpFile,
					  [Optional] [DefaultValue(null)] Object HelpContext)
			{
				if(Number == 0)
				{
					throw new ArgumentException
						(S._("VB_InvalidErrorNumber"), "Number");
				}
				this.Number = Number;
				this.Source = StringType.FromObject(Source);
				this.Description = StringType.FromObject(Description);
				this.HelpFile = StringType.FromObject(HelpFile);
				this.HelpContext = IntegerType.FromObject(HelpContext);
				this.exception = CreateExceptionFromNumber
						(Number, this.Description);
			#if !ECMA_COMPAT
				this.exception.Source = this.Source;
				this.exception.HelpLink = this.HelpFile;
			#endif
				throw this.exception;
			}

	// Get or set the error's description.
	public String Description
			{
				get
				{
					return description;
				}
				set
				{
					description = value;
				}
			}

	// Get the error's ERL value.
	public int Erl
			{
				get
				{
					return erl;
				}
			}

	// Get or set the error's help context.
	public int HelpContext
			{
				get
				{
					return helpContext;
				}
				set
				{
					helpContext = value;
				}
			}

	// Get or set the error's help file.
	public String HelpFile
			{
				get
				{
					return helpFile;
				}
				set
				{
					helpFile = value;
				#if !ECMA_COMPAT
					if(exception != null)
					{
						exception.HelpLink = helpFile;
					}
				#endif
				}
			}

	// Get the last-occurring Win32 error code.
	public int LastDllError
			{
				get
				{
					return Marshal.GetLastWin32Error();
				}
			}

	// Get a standard number for an exception.
	internal static int GetNumberForException(Exception exception)
			{
				if(exception is OverflowException)
				{
					return 6;
				}
				if(exception is OutOfMemoryException)
				{
					return 7;
				}
				if(exception is IndexOutOfRangeException ||
				   exception is RankException ||
				   exception is ArrayTypeMismatchException)
				{
					return 9;
				}
				if(exception is DivideByZeroException)
				{
					return 11;
				}
				if(exception is InvalidCastException ||
				   exception is NotSupportedException ||
				   exception is FormatException)
				{
					return 13;
				}
				if(exception is NotFiniteNumberException)
				{
					if(((NotFiniteNumberException)exception).OffendingNumber
							!= 0.0)
					{
						return 6;
					}
					else
					{
						return 11;
					}
				}
				if(exception is StackOverflowException)
				{
					return 28;
				}
			#if !ECMA_COMPAT
				if(exception is DllNotFoundException)
				{
					return 53;
				}
			#endif
				if(exception is FileNotFoundException)
				{
					return 53;
				}
				if(exception is IOException)
				{
					return 57;
				}
				if(exception is EndOfStreamException)
				{
					return 62;
				}
				if(exception is DirectoryNotFoundException)
				{
					return 76;
				}
				if(exception is NullReferenceException)
				{
					return 91;
				}
#if CONFIG_COM_INTEROP
				if(exception is COMException)
				{
					return ((COMException)exception).ErrorCode;
				}
				if(exception is SEHException)
				{
					return 99;
				}
				if(exception is InvalidOleVariantTypeException)
				{
					return 458;
				}
#endif
				if(exception is TypeLoadException)
				{
					return 429;
				}
				if(exception is MissingFieldException)
				{
					return 422;
				}
				if(exception is MissingMemberException)
				{
					return 438;
				}
				if(exception is EntryPointNotFoundException)
				{
					return 453;
				}
				return 5;
			}

	// Convert a number into an exception.
	internal static Exception CreateExceptionFromNumber
				(int number, String message)
			{
				switch(number)
				{
					case 3: case 20: case 94: case 100:
						return new InvalidOperationException(message);

					case 5: case 446: case 448: case 449:
						return new ArgumentException(message);

					case 6:
						return new OverflowException(message);

					case 7: case 14:
						return new OutOfMemoryException(message);

					case 9:
						return new IndexOutOfRangeException(message);

					case 11:
						return new DivideByZeroException(message);

					case 13:
						return new InvalidCastException(message);

					case 28:
						return new StackOverflowException(message);

					case 48: case 429:
						return new TypeLoadException(message);

					case 52: case 54: case 55: case 57: case 58:
					case 59: case 61: case 63: case 67: case 68:
					case 70: case 71: case 74: case 75:
						return new IOException(message);

					case 53: case 432:
						return new FileNotFoundException(message);

					case 62:
						return new EndOfStreamException(message);

					case 76:
						return new DirectoryNotFoundException(message);

					case 91:
						return new NullReferenceException(message);

#if CONFIG_COM_INTEROP
					case 99:
						return new SEHException(message);

					case 458:
						return new InvalidOleVariantTypeException(message);
#endif

					case 422:
						return new MissingFieldException(message);

					case 438:
						return new MissingMemberException(message);

					case 453:
						return new EntryPointNotFoundException(message);

					default:
						return new Exception(message);
				}
			}

	// Convert a HRESULT value into an error number.
	internal static int HResultToNumber(int hr)
			{
				if((((uint)hr) & 0xFFFF0000) == 0x800A0000)
				{
					return (hr & 0xFFFF);
				}
				switch((uint)hr)
				{
					case 0x80004001: return 445;
					case 0x80004002: return 430;
					case 0x80004004: return 287;
					case 0x80020001: return 438;
					case 0x80020003: return 438;
					case 0x80020004: return 448;
					case 0x80020005: return 13;
					case 0x80020006: return 438;
					case 0x80020007: return 446;
					case 0x80020008: return 458;
					case 0x8002000A: return 6;
					case 0x8002000B: return 9;
					case 0x8002000C: return 447;
					case 0x8002000D: return 10;
					case 0x8002000E: return 450;
					case 0x8002000F: return 449;
					case 0x80020011: return 451;
					case 0x80020012: return 11;
					case 0x80028016: return 32790;
					case 0x80028017: return 461;
					case 0x80028018: return 32792;
					case 0x80028019: return 32793;
					case 0x8002801C: return 32796;
					case 0x8002801D: return 32797;
					case 0x80028027: return 32807;
					case 0x80028028: return 32808;
					case 0x80028029: return 32809;
					case 0x8002802A: return 32810;
					case 0x8002802B: return 32811;
					case 0x8002802C: return 32812;
					case 0x8002802D: return 32813;
					case 0x8002802E: return 32814;
					case 0x8002802F: return 453;
					case 0x800288BD: return 35005;
					case 0x800288C5: return 35013;
					case 0x80028CA0: return 13;
					case 0x80028CA1: return 9;
					case 0x80028CA2: return 57;
					case 0x80028CA3: return 322;
					case 0x80029C4A: return 48;
					case 0x80029C83: return 40067;
					case 0x80029C84: return 40068;
					case 0x80030001: return 32774;
					case 0x80030002: return 53;
					case 0x80030003: return 76;
					case 0x80030004: return 67;
					case 0x80030005: return 70;
					case 0x80030006: return 32772;
					case 0x80030008: return 7;
					case 0x80030012: return 67;
					case 0x80030013: return 70;
					case 0x80030019: return 32771;
					case 0x8003001D: return 32773;
					case 0x8003001E: return 32772;
					case 0x80030020: return 75;
					case 0x80030021: return 70;
					case 0x80030050: return 58;
					case 0x80030070: return 61;
					case 0x800300FB: return 32792;
					case 0x800300FC: return 53;
					case 0x800300FD: return 32792;
					case 0x800300FE: return 32768;
					case 0x80030100: return 70;
					case 0x80030101: return 70;
					case 0x80030102: return 32773;
					case 0x80030103: return 57;
					case 0x80030104: return 32793;
					case 0x80030105: return 32793;
					case 0x80030106: return 32789;
					case 0x80030107: return 32793;
					case 0x80030108: return 32793;
					case 0x80040112: return 429;
					case 0x80040154: return 429;
					case 0x800401E3: return 429;
					case 0x800401E6: return 432;
					case 0x800401EA: return 432;
					case 0x800401F3: return 429;
					case 0x800401F5: return 429;
					case 0x800401FE: return 429;
					case 0x80070005: return 70;
					case 0x8007000E: return 7;
					case 0x80070057: return 5;
					case 0x800706BA: return 462;
					case 0x80080005: return 429;
				}
				return hr;
			}

	// Get or set the error's number.
	public int Number
			{
				get
				{
					if(number == -1)
					{
						if(exception != null)
						{
							number = GetNumberForException(exception);
						}
						else
						{
							return 0;
						}
					}
					return number;
				}
				set
				{
					number = value;
				}
			}

	// Get or set the error's source.
	public String Source
			{
				get
				{
					return source;
				}
				set
				{
					source = value;
				#if !ECMA_COMPAT
					if(exception != null)
					{
						exception.Source = value;
					}
				#endif
				}
			}

}; // class ErrObject

}; // namespace Microsoft.VisualBasic
