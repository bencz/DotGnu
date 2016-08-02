/*
 * il_serial.h - Serial port I/O routines.
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

#ifndef	_IL_SERIAL_H
#define	_IL_SERIAL_H

#include "il_values.h"
#include "il_thread.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Serial port types.
 */
#define	IL_SERIAL_REGULAR		0
#define	IL_SERIAL_INFRARED		1
#define	IL_SERIAL_USB			2
#define	IL_SERIAL_RFCOMM		3

/*
 * Bits for various serial pins.
 */
#define IL_PIN_BREAK			(1<<0)
#define IL_PIN_CD				(1<<1)
#define IL_PIN_CTS				(1<<2)
#define IL_PIN_DSR				(1<<3)
#define IL_PIN_DTR				(1<<4)
#define IL_PIN_RTS				(1<<5)
#define IL_PIN_RING				(1<<6)

/*
 * Parity values.
 */
#define	IL_PARITY_NONE			0
#define	IL_PARITY_ODD			1
#define	IL_PARITY_EVEN			2
#define	IL_PARITY_MARK			3
#define	IL_PARITY_SPACE			4

/*
 * Serial port handshake modes.
 */
#define	IL_HANDSHAKE_NONE		0
#define	IL_HANDSHAKE_XONOFF		1
#define	IL_HANDSHAKE_RTS		2
#define	IL_HANDSHAKE_RTS_XONOFF	3

/*
 * Serial port parameters.  Must match "PortMethods.Parameters".
 */
typedef struct
{
	ILInt32 baudRate;
	ILInt32 parity;
	ILInt32 dataBits;
	ILInt32 stopBits;
	ILInt32 handshake;
	ILUInt8 parityReplace;
	ILBool discardNull;
	ILInt32 readBufferSize;
	ILInt32 writeBufferSize;
	ILInt32 receivedBytesThreshold;
	ILInt32 readTimeout;
	ILInt32 writeTimeout;

} ILSerialParameters;

/*
 * Opaque serial port type.
 */
typedef struct _tagILSerial ILSerial;

/*
 * Determine if a serial port number is valid.
 */
int ILSerialIsValid(ILInt32 type, ILInt32 portNumber);

/*
 * Determine if a serial port is accessible.
 */
int ILSerialIsAccessible(ILInt32 type, ILInt32 portNumber);

/*
 * Open a serial port.
 */
ILSerial *ILSerialOpen(ILInt32 type, ILInt32 portNumber,
					   ILSerialParameters *parameters);

/*
 * Close a serial port.
 */
void ILSerialClose(ILSerial *handle);

/*
 * Modify the settings on a serial port.
 */
void ILSerialModify(ILSerial *handle, ILSerialParameters *parameters);

/*
 * Get the number of bytes that are available to be read.
 */
ILInt32 ILSerialGetBytesToRead(ILSerial *handle);

/*
 * Get the number of bytes that are buffered to be written.
 */
ILInt32 ILSerialGetBytesToWrite(ILSerial *handle);

/*
 * Read the state of the serial port pins.
 */
ILInt32 ILSerialReadPins(ILSerial *handle);

/*
 * Write the state of the serial port pins.
 */
void ILSerialWritePins(ILSerial *handle, ILInt32 mask, ILInt32 value);

/*
 * Get the recommended buffer sizes.
 */
void ILSerialGetRecommendedBufferSizes
			(ILInt32 *readBufferSize, ILInt32 *writeBufferSize,
			 ILInt32 *receivedBytesThreshold);

/*
 * Discard the contents of the input buffer.
 */
void ILSerialDiscardInBuffer(ILSerial *handle);

/*
 * Discard the contents of the output buffer.
 */
void ILSerialDiscardOutBuffer(ILSerial *handle);

/*
 * Drain the contents of the output buffer.
 */
void ILSerialDrainOutBuffer(ILSerial *handle);

/*
 * Read data from a serial port.
 */
ILInt32 ILSerialRead(ILSerial *handle, void *buffer, ILInt32 count);

/*
 * Write data to a serial port. Returns positive if success, negative if error,
 * 0 if timeout.
 */
int ILSerialWrite(ILSerial *handle, const void *buffer, ILInt32 count);

/*
 * Wait for a change in state on the incoming pins.  Returns non-zero
 * if a pin change was detected, zero if the thread was interrupted,
 * or -1 if pin changes cannot be detected.
 */
int ILSerialWaitForPinChange(ILSerial *handle);

/*
 * Wait for input on a serial port.  Returns non-zero if there was
 * input, or zero if the thread was interrupted, and -1 if input
 * detection is not possible.
 */
int ILSerialWaitForInput(ILSerial *handle, ILInt32 timeout);

/*
 * Send an interrupt signal to a serial port pin-change/input wait thread.
 */
void ILSerialInterrupt(ILThread *thread);

#ifdef	__cplusplus 
};
#endif

#endif	/* _IL_SERIAL_H */
