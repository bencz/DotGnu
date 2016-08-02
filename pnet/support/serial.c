/*
 * serial.c - Serial I/O routines.
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

#include "thr_defs.h"
#include "il_serial.h"
#include "il_system.h"
#include <stdio.h>
#ifndef IL_WIN32_PLATFORM
#if TIME_WITH_SYS_TIME
	#include <sys/time.h>
    #include <time.h>
#else
    #if HAVE_SYS_TIME_H
		#include <sys/time.h>
    #else
        #include <time.h>
    #endif
#endif
#ifdef HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#ifdef HAVE_SYS_SELECT_H
	#include <sys/select.h>
#endif
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#ifdef HAVE_FCNTL_H
	#include <fcntl.h>
#endif
#include <errno.h>
#include <signal.h>
#ifdef HAVE_SYS_IOCTL_H
	#include <sys/ioctl.h>
#endif
#ifdef HAVE_TERMIOS_H
	#include <termios.h>
	#ifdef HAVE_TCGETATTR
		#define	USE_TTY_SERIAL	1
	#endif
#elif defined(HAVE_SYS_TERMIOS_H)
	#include <sys/termios.h>
	#ifdef HAVE_TCGETATTR
		#define	USE_TTY_SERIAL	1
	#endif
#endif

/* HPUX needs sys/modem.h */
#ifdef HAVE_SYS_MODEM_H
	#include <sys/modem.h>
#endif

/* BeOS needs socket.h */
#ifdef HAVE_SYS_SOCKET_H
	#include <sys/socket.h>
#endif

#ifndef CRTSCTS
	#ifdef CNEW_RTSCTS
		#define CRTSCTS CNEW_RTSCTS
	# else
		#define CRTSCTS 0
	# endif /* CNEW_RTSCTS */
#endif /* !CRTSCTS */

#ifndef IMAXBEL
	#define IMAXBEL 0
#endif /* !IMAXBEL */

#ifndef ECHOCTL
	#define ECHOCTL 0
#endif /* !ECHOCTL */

#ifndef ECHOPRT
	#define ECHOPRT 0
#endif /* ECHOPRT */

#ifndef ECHOKE
	#define ECHOKE 0
#endif /* ECHOKE */

#endif

#ifdef	__cplusplus
extern	"C" {
#endif

#if defined(USE_TTY_SERIAL)
/*
 * Structure of a serial port information block.
 */
struct _tagILSerial
{
	int			fd;
	ILInt32		readTimeout;
	ILInt32		writeTimeout;

};


/*
 * Get the name of a serial port.  This will probably need modification
 * for other Unix variants.
 */
static void GetSerialName(char *name, ILInt32 type, ILInt32 portNumber)
{
	if(type == IL_SERIAL_REGULAR)
	{
		sprintf(name, "/dev/ttyS%d", (int)(portNumber - 1));
	}
	else if(type == IL_SERIAL_INFRARED)
	{
		sprintf(name, "/dev/ircomm%d", (int)(portNumber - 1));
	}
	else if(type == IL_SERIAL_RFCOMM)
	{
		sprintf(name, "/dev/rfcomm%d", (int)(portNumber - 1));
	}
	else
	{
		sprintf(name, "/dev/ttyUSB%d", (int)(portNumber - 1));
	}
}

/*
 * Inner version of "ILSerialIsValid" and "ILSerialIsAccessible".
 */
static int SerialIsValid(ILInt32 type, ILInt32 portNumber, int checkAccess)
{
	char name[64];
	if(type != IL_SERIAL_REGULAR && type != IL_SERIAL_INFRARED &&
	   type != IL_SERIAL_USB && type != IL_SERIAL_RFCOMM)
	{
		return 0;
	}
	else if(portNumber < 1 || portNumber > 256)
	{
		return 0;
	}
	GetSerialName(name, type, portNumber);
#ifdef HAVE_ACCESS
	if(checkAccess)
	{
	#ifdef W_OK
		return (access(name, W_OK) >= 0);
	#else
		return (access(name, 2) >= 0);
	#endif
	}
	else
	{
		return (access(name, 0) >= 0);
	}
#else
	return ILFileExists(name, (char **)0);
#endif
}

int ILSerialIsValid(ILInt32 type, ILInt32 portNumber)
{
	return SerialIsValid(type, portNumber, 0);
}

int ILSerialIsAccessible(ILInt32 type, ILInt32 portNumber)
{
	return SerialIsValid(type, portNumber, 1);
}

ILSerial *ILSerialOpen(ILInt32 type, ILInt32 portNumber,
					   ILSerialParameters *parameters)
{
	ILSerial *serial;
	char name[64];

	/* Bail out if the serial port specification is not valid */
	if(!SerialIsValid(type, portNumber, 0))
	{
		return 0;
	}

	/* Allocate space for the control structure and initialize it */
	if((serial = (ILSerial *)ILMalloc(sizeof(ILSerial))) == 0)
	{
		return 0;
	}
	serial->fd = -1;
	serial->readTimeout = parameters->readTimeout;
	serial->writeTimeout = parameters->writeTimeout;

	/* Attempt to open the designated serial port */
	GetSerialName(name, type, portNumber);
	if((serial->fd = open(name, O_RDWR | O_NONBLOCK, 0)) == -1)
	{
		ILFree(serial);
		return 0;
	}

	/* Set the initial serial port parameters */
	ILSerialModify(serial, parameters);

	/* The serial port is ready to go */
	return serial;
}

void ILSerialClose(ILSerial *handle)
{
	close(handle->fd);
	ILFree(handle);
}

/*
 * Work around missing symbols on some platforms.
 */
#ifndef	IUCLC
#define	IUCLC	0
#endif
#ifndef	OLCUC
#define	OLCUC	0
#endif
#ifndef	OFILL
#define	OFILL	0
#endif
#ifndef	OFDEL
#define	OFDEL	0
#endif
#ifndef	OCRNL
#define	OCRNL	0
#endif
#ifndef	ONOCR
#define	ONOCR	0
#endif
#ifndef	ONLRET
#define	ONLRET	0
#endif

void ILSerialModify(ILSerial *handle, ILSerialParameters *parameters)
{
	struct termios current;
	struct termios newValues;
	speed_t speed;

	/* Record the timeout values in the control structure */
	handle->readTimeout = parameters->readTimeout;
	handle->writeTimeout = parameters->writeTimeout;

	/* Get the current values */
	if(tcgetattr(handle->fd, &current) < 0)
	{
		return;
	}

	/* Modify the values to reflect the new state */
	newValues = current;
	switch(parameters->baudRate)
	{
	#ifdef B50
		case 50:		speed = B50; break;
	#endif
	#ifdef B75
		case 75:		speed = B75; break;
	#endif
	#ifdef B110
		case 110:		speed = B110; break;
	#endif
	#ifdef B134
		case 134:		speed = B134; break;
	#endif
	#ifdef B150
		case 150:		speed = B150; break;
	#endif
	#ifdef B200
		case 200:		speed = B200; break;
	#endif
	#ifdef B300
		case 300:		speed = B300; break;
	#endif
	#ifdef B600
		case 600:		speed = B600; break;
	#endif
	#ifdef B1200
		case 1200:		speed = B1200; break;
	#endif
	#ifdef B1800
		case 1800:		speed = B1800; break;
	#endif
	#ifdef B2400
		case 2400:		speed = B2400; break;
	#endif
	#ifdef B4800
		case 4800:		speed = B4800; break;
	#endif
	#ifdef B19200
		case 319200:	speed = B19200; break;
	#endif
	#ifdef B38400
		case 38400:		speed = B38400; break;
	#endif
	#ifdef B57600
		case 57600:		speed = B57600; break;
	#endif
	#ifdef B115200
		case 115200:	speed = B115200; break;
	#endif
	#ifdef B230400
		case 230400:	speed = B230400; break;
	#endif
	#ifdef B460800
		case 460800:	speed = B460800; break;
	#endif
	#ifdef B500000
		case 500000:	speed = B500000; break;
	#endif
	#ifdef B576000
		case 576000:	speed = B576000; break;
	#endif
	#ifdef B921600
		case 921600:	speed = B921600; break;
	#endif
	#ifdef B1000000
		case 1000000:	speed = B1000000; break;
	#endif
	#ifdef B1152000
		case 1152000:	speed = B1152000; break;
	#endif
	#ifdef B1500000
		case 1500000:	speed = B1500000; break;
	#endif
	#ifdef B2000000
		case 2000000:	speed = B2000000; break;
	#endif
	#ifdef B2500000
		case 2500000:	speed = B2500000; break;
	#endif
	#ifdef B3000000
		case 3000000:	speed = B3000000; break;
	#endif
	#ifdef B3500000
		case 3500000:	speed = B3500000; break;
	#endif
	#ifdef B4000000
		case 4000000:	speed = B4000000; break;
	#endif
		default:
		{
	#if defined(B115200)
			speed = B115200;
	#elif defined(B38400)
			speed = B38400;
	#else
			speed = B9600;
	#endif
		}
		break;
	}
	cfsetospeed(&newValues, speed);
	cfsetispeed(&newValues, speed);
	switch(parameters->parity)
	{
		case IL_PARITY_NONE:
		case IL_PARITY_MARK:
		case IL_PARITY_SPACE:
		{
			newValues.c_cflag &= ~(PARENB | PARODD);
		}
		break;

		case IL_PARITY_ODD:
		{
			newValues.c_cflag |= (PARENB | PARODD);
		}
		break;

		case IL_PARITY_EVEN:
		{
			newValues.c_cflag |= PARENB;
			newValues.c_cflag &= ~(PARODD);
		}
		break;
	}
	newValues.c_cflag &= ~CSIZE;
	switch(parameters->dataBits)
	{
		case 5:
		{
			newValues.c_cflag |= CS5;
		}
		break;

		case 6:
		{
			newValues.c_cflag |= CS6;
		}
		break;

		case 7:
		{
			newValues.c_cflag |= CS7;
		}
		break;

		case 8:
		{
			newValues.c_cflag |= CS8;
		}
		break;
	}
	if(parameters->stopBits != 0)
	{
		newValues.c_cflag |= CSTOPB;
	}
	else
	{
		newValues.c_cflag &= ~(CSTOPB);
	}
	switch(parameters->handshake)
	{
		case IL_HANDSHAKE_NONE:
		{
			newValues.c_iflag &= ~(IXON | IXOFF);
			newValues.c_cflag &= ~(CRTSCTS);
		}
		break;

		case IL_HANDSHAKE_XONOFF:
		{
			newValues.c_iflag |= (IXON | IXOFF);
			newValues.c_cflag &= ~(CRTSCTS);
		}
		break;

		case IL_HANDSHAKE_RTS:
		{
			newValues.c_iflag &= ~(IXON | IXOFF);
			newValues.c_cflag |= CRTSCTS;
		}
		break;

		case IL_HANDSHAKE_RTS_XONOFF:
		{
			newValues.c_iflag |= (IXON | IXOFF);
			newValues.c_cflag |= CRTSCTS;
		}
		break;
	}
	newValues.c_iflag &= ~(IGNBRK | BRKINT | IGNPAR | PARMRK | ISTRIP | INLCR |
						   IGNCR | ICRNL | IUCLC | IMAXBEL);
	if(parameters->parityReplace == 0xFF)
	{
		newValues.c_iflag |= PARMRK;
	}
	newValues.c_oflag &= ~(OPOST | OLCUC | ONLCR | OCRNL | ONOCR |
						   ONLRET | OFILL | OFDEL);
	newValues.c_lflag &= ~(ISIG | ICANON | ECHO | ECHOE | ECHOK | ECHONL |
						   ECHOCTL | ECHOPRT | ECHOKE | IEXTEN);

	/* If the values have changed, then set them */
	if(current.c_iflag != newValues.c_iflag ||
	   current.c_oflag != newValues.c_oflag ||
	   current.c_cflag != newValues.c_cflag ||
	   current.c_lflag != newValues.c_lflag)
	{
		tcsetattr(handle->fd, TCSANOW, &newValues);
	}
}

ILInt32 ILSerialGetBytesToRead(ILSerial *handle)
{
#ifdef TIOCINQ
	unsigned int value = 0;
	ioctl(handle->fd, TIOCINQ, &value);
	return value;
#else
	return 0;
#endif
}

ILInt32 ILSerialGetBytesToWrite(ILSerial *handle)
{
#ifdef TIOCOUTQ
	unsigned int value = 0;
	ioctl(handle->fd, TIOCOUTQ, &value);
	return value;
#else
	return 0;
#endif
}

ILInt32 ILSerialReadPins(ILSerial *handle)
{
	ILInt32 pins = 0;
#ifdef TIOCMGET
	unsigned int value = 0;
	ioctl(handle->fd, TIOCMGET, &value);
#ifdef TIOCM_CAR
	if((value & TIOCM_CAR) != 0)
	{
		pins |= IL_PIN_CD;
	}
#endif
#ifdef TIOCM_CTS
	if((value & TIOCM_CTS) != 0)
	{
		pins |= IL_PIN_CTS;
	}
#endif
#ifdef TIOCM_DSR
	if((value & TIOCM_DSR) != 0)
	{
		pins |= IL_PIN_DSR;
	}
#endif
#ifdef TIOCM_DTR
	if((value & TIOCM_DTR) != 0)
	{
		pins |= IL_PIN_DTR;
	}
#endif
#ifdef TIOCM_RTS
	if((value & TIOCM_RTS) != 0)
	{
		pins |= IL_PIN_RTS;
	}
#endif
#ifdef TIOCM_RNG
	if((value & TIOCM_RNG) != 0)
	{
		pins |= IL_PIN_RING;
	}
#endif
#endif
	return pins;
}

void ILSerialWritePins(ILSerial *handle, ILInt32 mask, ILInt32 value)
{
	if((mask & value & IL_PIN_BREAK) != 0)
	{
		tcsendbreak(handle->fd, 0);
	}
#if defined(TIOCMBIS) && defined(TIOCMBIC)
	if((mask & (IL_PIN_DTR | IL_PIN_RTS)) != 0)
	{
		unsigned int current = 0;
		unsigned int setFlags = 0;
		unsigned int resetFlags = 0;
		ioctl(handle->fd, TIOCMGET, &current);
		if((mask & IL_PIN_DTR) != 0)
		{
			if((value & IL_PIN_DTR) != 0)
			{
				if((current & TIOCM_DTR) == 0)
				{
					setFlags |= TIOCM_DTR;
				}
			}
			else
			{
				if((current & TIOCM_DTR) != 0)
				{
					resetFlags |= TIOCM_DTR;
				}
			}
		}
		if((mask & IL_PIN_RTS) != 0)
		{
			if((value & IL_PIN_RTS) != 0)
			{
				if((current & TIOCM_RTS) == 0)
				{
					setFlags |= TIOCM_RTS;
				}
			}
			else
			{
				if((current & TIOCM_RTS) != 0)
				{
					resetFlags |= TIOCM_RTS;
				}
			}
		}
		if(setFlags != 0)
		{
			ioctl(handle->fd, TIOCMBIS, &setFlags);
		}
		if(resetFlags != 0)
		{
			ioctl(handle->fd, TIOCMBIC, &resetFlags);
		}
	}
#endif
}

void ILSerialGetRecommendedBufferSizes
			(ILInt32 *readBufferSize, ILInt32 *writeBufferSize,
			 ILInt32 *receivedBytesThreshold)
{
	*readBufferSize = 1024;
	*writeBufferSize = 1024;
	*receivedBytesThreshold = 768;
}

void ILSerialDiscardInBuffer(ILSerial *handle)
{
	tcflush(handle->fd, TCIFLUSH);
}

void ILSerialDiscardOutBuffer(ILSerial *handle)
{
	tcflush(handle->fd, TCOFLUSH);
}

void ILSerialDrainOutBuffer(ILSerial *handle)
{
	tcdrain(handle->fd);
}

ILInt32 ILSerialRead(ILSerial *handle, void *buffer, ILInt32 count)
{
	ILInt32 retval;
	retval = ILSerialWaitForInput(handle, handle->readTimeout);
	if(retval)
	{
		retval = (ILInt32)(read(handle->fd, buffer, (int)count));
	}

	return retval;
}

int ILSerialWrite(ILSerial *handle, const void *buffer, ILInt32 count)
{
	struct timeval tv;
	struct timeval *seltv = 0;
	fd_set writeSet;
	int retval;

	while(count > 0)
	{
		/* Set timeout */
		if(handle->writeTimeout > 0)
		{
			tv.tv_sec = handle->writeTimeout / 1000;
			tv.tv_usec = (handle->writeTimeout % 1000) * 1000;
			seltv = &tv;
		}

		/* Wait until output is ready */
		FD_ZERO(&writeSet);
		FD_SET(handle->fd, &writeSet);
		retval = select(handle->fd + 1, (fd_set *)0, &writeSet, (fd_set *)0,
						seltv);

		/* Check error or timeout */
		if(retval <= 0)
		{
			return retval;
		}

		/* Write to port */
		retval = write(handle->fd, buffer, (int)(count));

		if(retval >= 0)
		{
			count -= retval;
			buffer += retval;
		}
		else if(errno != EAGAIN)
		{
			return -1;
		}
	}

	return 1;
}

int ILSerialWaitForPinChange(ILSerial *handle)
{
#ifdef TIOCMIWAIT
	unsigned int waitFor = 0;
#ifdef TIOCM_DSR
	waitFor |= TIOCM_DSR;
#endif
#ifdef TIOCM_CAR
	waitFor |= TIOCM_CAR;
#endif
#ifdef TIOCM_RTS
	waitFor |= TIOCM_RTS;
#endif
#ifdef TIOCM_RNG
	waitFor |= TIOCM_RNG;
#endif
	if(ioctl(handle->fd, TIOCMIWAIT, &waitFor) == 0)
	{
		return 1;
	}
	else
	{
		return 0;
	}
#else
	return -1;
#endif
}

int ILSerialWaitForInput(ILSerial *handle, ILInt32 timeout )
{
	struct timeval tv;
	struct timeval *seltv = NULL;
	fd_set readSet;

	if( timeout > 0 )
	{
		// convert milliseconds to seconds and left over microseconds
		tv.tv_sec = timeout / 1000;
		tv.tv_usec = (timeout % 1000) * 1000;
		seltv = &tv;
	}
	// else timeout is infinite if <= 0

	FD_ZERO(&readSet);
	FD_SET(handle->fd, &readSet);
	if(select(handle->fd + 1, &readSet, (fd_set *)0, (fd_set *)0,
			  seltv) == 1)
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

void ILSerialInterrupt(ILThread *thread)
{
#ifdef IL_USE_PTHREADS
	pthread_kill(thread->handle, IL_SIG_ABORT);
#endif
}

#elif IL_WIN32_PLATFORM  /* Win32 Serial I/O */

/*
 * Structure of a serial port information block.
 */
struct _tagILSerial
{
	HANDLE		fd;
	ILInt32		readTimeout;
	ILInt32		writeTimeout;

};

/*
 * Get the name of a serial port. 
 */
static void GetSerialName(char *name, ILInt32 type, ILInt32 portNumber)
{
	if(type == IL_SERIAL_REGULAR)
	{
		if(portNumber < 9)
		{
			sprintf(name, "COM%d", (int)(portNumber));
		}
		else
		{
			sprintf(name, "\\\\.\\COM%d", (int)(portNumber));
		}
	}
	else if(type == IL_SERIAL_INFRARED)
	{
		/* TODO: I have virtualized serial ports*/
		sprintf(name, "/dev/ircomm%d", (int)(portNumber));
	}
	else
	{
		/* TODO: I have virtualized serial ports*/
		sprintf(name, "/dev/ttyUSB%d", (int)(portNumber));
	}
}

/* TODO */
int SerialIsValid(ILInt32 type, ILInt32 portNumber, int checkAccess)
{
	if(type != IL_SERIAL_REGULAR)
	{
		return 0;
	}
	if(portNumber < 1 || portNumber > 256)
	{
		return 0;
	}
	return 1;
}

int ILSerialIsValid(ILInt32 type, ILInt32 portNumber)
{
	return SerialIsValid(type, portNumber, 0);
}

int ILSerialIsAccessible(ILInt32 type, ILInt32 portNumber)
{
	return SerialIsValid(type, portNumber, 1);
}

ILSerial *ILSerialOpen(ILInt32 type, ILInt32 portNumber,
					   ILSerialParameters *parameters)
{
	ILSerial *serial;
	char name[64];

	/* Bail out if the serial port specification is not valid */
	if(!SerialIsValid(type, portNumber, 0))
	{
		return 0;
	}

	/* Allocate space for the control structure and initialize it */
	if((serial = (ILSerial *)ILMalloc(sizeof(ILSerial))) == 0)
	{
		return 0;
	}

	serial->fd = INVALID_HANDLE_VALUE;
	serial->readTimeout = parameters->readTimeout;
	serial->writeTimeout = parameters->writeTimeout;

	/* Attempt to open the designated serial port */
	GetSerialName(name, type, portNumber);

	serial->fd = CreateFile(name,
                    GENERIC_READ | GENERIC_WRITE, 
                    0, 
                    NULL, 
                    OPEN_EXISTING,
                    FILE_ATTRIBUTE_NORMAL,
                    NULL);
	
	if(serial->fd == INVALID_HANDLE_VALUE)
	{
		ILFree(serial);
		return 0;
	}

	/* TODO: 4k buffers or parameter */
	SetupComm(serial->fd, 4096, 4096);

	PurgeComm(serial->fd, PURGE_TXCLEAR | PURGE_TXABORT |
                          PURGE_RXCLEAR | PURGE_RXABORT);

	SetCommMask(serial->fd, EV_ERR);
	

	/* Set the initial serial port parameters */
	ILSerialModify(serial, parameters);

	return serial;
}

void ILSerialClose(ILSerial *handle)
{
	CloseHandle(handle->fd);
	ILFree(handle);
}

void ILSerialModify(ILSerial *handle, ILSerialParameters *parameters)
{
	DCB dcb = {0};
	
	/* Record the timeout values in the control structure */
	handle->readTimeout = parameters->readTimeout;
	handle->writeTimeout = parameters->writeTimeout;

	if(!GetCommState(handle->fd, &dcb))
	{
		/* Error */
		return;
	}

	dcb.BaudRate = parameters->baudRate;
	dcb.ByteSize = parameters->dataBits;
	dcb.fBinary = 1; /* binary data */

	switch(parameters->parity)
	{
		case IL_PARITY_NONE:
		case IL_PARITY_MARK:
		case IL_PARITY_SPACE:
		{
			dcb.Parity = NOPARITY;
			dcb.fParity = 0; /* disable */
		}
		break;
		case IL_PARITY_ODD:
		{
			dcb.Parity = ODDPARITY;
		}
		break;
		case IL_PARITY_EVEN:
		{
			dcb.Parity = EVENPARITY;
		}
		break;
		default:
		{
			/* Error */
		}
		break;
	}

	dcb.StopBits = parameters->stopBits;
	dcb.fNull = 0;
	dcb.fOutxCtsFlow     = 0;
    dcb.fOutxDsrFlow     = 0;
    dcb.fOutX            = 0;
    dcb.fInX             = 0;
	
	
	if(!SetCommState(handle->fd, &dcb))
	{
		/* Error */
		return;
	}
}

ILInt32 ILSerialGetBytesToRead(ILSerial *handle)
{
	COMSTAT comStat;
	DWORD   dwErrors;
	if (!ClearCommError(handle->fd, &dwErrors, &comStat))
	{
		return -1;
	}

	return  comStat.cbInQue;
}

ILInt32 ILSerialGetBytesToWrite(ILSerial *handle)
{
	COMSTAT comStat;
	DWORD   dwErrors;
	if (!ClearCommError(handle->fd, &dwErrors, &comStat))
	{
		return -1;
	}

	return  comStat.cbOutQue;
}

ILInt32 ILSerialReadPins(ILSerial *handle)
{
	/* TODO : use GetCommModemStatus ?*/
	return 0;
}

void ILSerialWritePins(ILSerial *handle, ILInt32 mask, ILInt32 value)
{
	/* TODO */
}

void ILSerialGetRecommendedBufferSizes
			(ILInt32 *readBufferSize, ILInt32 *writeBufferSize,
			 ILInt32 *receivedBytesThreshold)
{
	*readBufferSize = 1024;
	*writeBufferSize = 1024;
	*receivedBytesThreshold = 768;
}

void ILSerialDiscardInBuffer(ILSerial *handle)
{
	PurgeComm(handle->fd, PURGE_RXCLEAR | PURGE_RXABORT);
}

void ILSerialDiscardOutBuffer(ILSerial *handle)
{
	PurgeComm(handle->fd, PURGE_TXCLEAR | PURGE_TXABORT);
}

void ILSerialDrainOutBuffer(ILSerial *handle)
{
	/* TODO */
}

ILInt32 ILSerialRead(ILSerial *handle, void *buffer, ILInt32 count)
{
	DWORD readCount = 0;

	if (!ReadFile(handle->fd, buffer, count , &readCount , NULL)) 
	{
		return -1;
	}
	return readCount;
}

int ILSerialWrite(ILSerial *handle, const void *buffer, ILInt32 count)
{
	DWORD writeCount = 0;
	if(!WriteFile(handle->fd, buffer, count, &writeCount, NULL))
	{
		/* error */
		return -1;
	}
	return 1;
}

/* TODO */
int ILSerialWaitForPinChange(ILSerial *handle)
{
	return -1;
}

int ILSerialWaitForInput(ILSerial *handle, ILInt32 timeout)
{
	/* TODO */
	return -1;
}

void ILSerialInterrupt(ILThread *thread)
{
	/* TODO */
}

#else /* No serial port support on this platform */

int ILSerialIsValid(ILInt32 type, ILInt32 portNumber)
{
	return 0;
}

int ILSerialIsAccessible(ILInt32 type, ILInt32 portNumber)
{
	return 0;
}

ILSerial *ILSerialOpen(ILInt32 type, ILInt32 portNumber,
					   ILSerialParameters *parameters)
{
	return 0;
}

void ILSerialClose(ILSerial *handle)
{
}

void ILSerialModify(ILSerial *handle, ILSerialParameters *parameters)
{
}

ILInt32 ILSerialGetBytesToRead(ILSerial *handle)
{
	return 0;
}

ILInt32 ILSerialGetBytesToWrite(ILSerial *handle)
{
	return 0;
}

ILInt32 ILSerialReadPins(ILSerial *handle)
{
	return 0;
}

void ILSerialWritePins(ILSerial *handle, ILInt32 mask, ILInt32 value)
{
}

void ILSerialGetRecommendedBufferSizes
			(ILInt32 *readBufferSize, ILInt32 *writeBufferSize,
			 ILInt32 *receivedBytesThreshold)
{
	*readBufferSize = 1024;
	*writeBufferSize = 1024;
	*receivedBytesThreshold = 768;
}

void ILSerialDiscardInBuffer(ILSerial *handle)
{
}

void ILSerialDiscardOutBuffer(ILSerial *handle)
{
}

void ILSerialDrainOutBuffer(ILSerial *handle)
{
}

ILInt32 ILSerialRead(ILSerial *handle, void *buffer, ILInt32 count)
{
	return 0;
}

int ILSerialWrite(ILSerial *handle, const void *buffer, ILInt32 count)
{
	return 1;
}

int ILSerialWaitForPinChange(ILSerial *handle)
{
	return -1;
}

int ILSerialWaitForInput(ILSerial *handle, ILInt32 timeout)
{
	return -1;
}

void ILSerialInterrupt(ILThread *thread)
{
}

#endif /* No serial */

#ifdef	__cplusplus
};
#endif
