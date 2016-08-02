/*
 * PortMethods.cs - Implementation of the "Platform.PortMethods" class.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd
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

namespace Platform
{

using System;
using System.Threading;
using System.Runtime.CompilerServices;

internal class PortMethods
{
	// Serial port parameters.
	public class Parameters
	{
		public int baudRate;
		public int parity;
		public int dataBits;
		public int stopBits;
		public int handshake;
		public byte parityReplace;
		public bool discardNull;
		public int readBufferSize;
		public int writeBufferSize;
		public int receivedBytesThreshold;
		public int readTimeout;
		public int writeTimeout;

	}; // class Parameters

	// Serial port types.
	public const int SERIAL_REGULAR		= 0;
	public const int SERIAL_INFRARED	= 1;
	public const int SERIAL_USB			= 2;
	public const int SERIAL_RFCOMM		= 3;

	// Bits for various serial pins.
	public const int PIN_BREAK			= (1<<0);
	public const int PIN_CD				= (1<<1);
	public const int PIN_CTS			= (1<<2);
	public const int PIN_DSR			= (1<<3);
	public const int PIN_DTR			= (1<<4);
	public const int PIN_RTS			= (1<<5);
	public const int PIN_RING			= (1<<6);

	// Determine if a serial port type and name is valid.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool IsValid(int type, int portNumber);

	// Determine if a serial port type and name is accessible.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool IsAccessible(int type, int portNumber);

	// Open a serial port with certain initial parameters.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr Open
			(int type, int portNumber, Parameters parameters);

	// Close a serial port.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Close(IntPtr handle);

	// Modify the serial port parameters.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Modify(IntPtr handle, Parameters parameters);

	// Get the number of bytes that are ready to read from the port.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int GetBytesToRead(IntPtr handle);

	// Get the number of bytes that remain to the written to the port.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int GetBytesToWrite(IntPtr handle);

	// Read the values of the serial port pins.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int ReadPins(IntPtr handle);

	// Write the values of particular serial port pins.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void WritePins(IntPtr handle, int mask, int value);

	// Get the recommended default buffer sizes.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void GetRecommendedBufferSizes
				(out int readBufferSize, out int writeBufferSize,
				 out int receivedBytesThreshold);

	// Discard the contents of the input buffer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void DiscardInBuffer(IntPtr handle);

	// Discard the contents of the output buffer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void DiscardOutBuffer(IntPtr handle);

	// Drain the contents of the output buffer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void DrainOutBuffer(IntPtr handle);

	// Read data from a serial port.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int Read
			(IntPtr handle, byte[] buffer, int offset, int count);

	// Write data to a serial port.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int Write
			(IntPtr handle, byte[] buffer, int offset, int count);

	// Wait for a change in pin status.  Returns non-zero when a pin
	// change occurs, zero if the thread was interrupted, and -1
	// if pin status changes cannot be monitored.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int WaitForPinChange(IntPtr handle);

	// Wait for input to become available on a port.  Returns non-zero
	// when input is available, zero if the thread was interrupted,
	// and -1 if input cannot be monitored.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int WaitForInput(IntPtr handle, int timeout);

	// Interrupt a thread that is waiting for pin changes or input.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Interrupt(Thread thread);

}; // class PortMethods

}; // namespace Platform
