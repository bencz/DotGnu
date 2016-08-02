/*
 * ICE.cs - Native method interface for ICE.
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

namespace Xsharp.Ice
{

using System;
using System.Runtime.InteropServices;
using OpenSystem.Platform;
using OpenSystem.Platform.X11;

internal sealed unsafe class ICE
{

	[StructLayout(LayoutKind.Explicit)]
	public struct IcePoAuthProcIncapsulator
	{
		[FieldOffset(0)]
		IcePoAuthProc cb;
		public IcePoAuthProcIncapsulator(IcePoAuthProc cb) { this.cb = cb; }
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct IcePoProcessMsgProcIncapsulator
	{
		[FieldOffset(0)]
		IcePoProcessMsgProc cb;
		public IcePoProcessMsgProcIncapsulator(IcePoProcessMsgProc cb) { this.cb = cb; }
	}


	[DllImport("ICE")]
	extern public static Xlib.Xint IceRegisterForProtocolSetup
			(String protocolName, String vendor, String release,
			 Xlib.Xint versionCount, ref IcePoVersionRec versionRecs,
			 Xlib.Xint authCount, String[] authNames,
			 ref IcePoAuthProcIncapsulator authProcs, IceIOErrorProc ioErrorProc);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceRegisterForProtocolReply
			(String protocolName, String vendor, String release,
			 Xlib.Xint versionCount, ref IcePaVersionRec versionRecs,
			 Xlib.Xint authCount, ref String[] authNames,
			 ref IcePaAuthProc authProcs,
			 IceHostBasedAuthProc hostBasedAuthProc,
			 IceProtocolSetupProc protocolSetupProc,
			 IceProtocolActivateProc protocolActivateProc,
			 IceIOErrorProc ioErrorProc);

	[DllImport("ICE")]
	extern public static IceConn *IceOpenConnection
			(String networkIdsList, IntPtr context,
			 XBool mustAuthenticate, Xlib.Xint majorOpcodeCheck,
			 Xlib.Xint errorLength, byte[] errorStringRet);

	[DllImport("ICE")]
	extern public static IntPtr IceGetConnectionContext(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static XStatus IceListenForConnections
			(ref Xlib.Xint countRet, ref IntPtr listenObjsRet,
			 Xlib.Xint errorLength, byte[] errorStringRet);

	[DllImport("ICE")]
	extern public static XStatus IceListenForWellKnownConnections
			(String port, ref Xlib.Xint countRet, ref IntPtr listenObjsRet,
			 Xlib.Xint errorLength, byte[] errorStringRet);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceGetListenConnectionNumber
			(IntPtr listenObj);

	[DllImport("ICE")]
	extern public static String IceGetListenConnectionString
			(IntPtr listenObj);

	[DllImport("ICE")]
	extern public static String IceComposeNetworkIdList
			(Xlib.Xint count, IntPtr listenObjs);

	[DllImport("ICE")]
	extern public static void IceFreeListenObjs
			(Xlib.Xint count, IntPtr listenObjs);

	[DllImport("ICE")]
	extern public static void IceSetHostBasedAuthProc
			(IntPtr listenObj, IceHostBasedAuthProc hostBasedAuthProc);

	[DllImport("ICE")]
	extern public static IceConn *IceAcceptConnection
			(IntPtr listenObj, ref Xlib.Xint statusRet);

	[DllImport("ICE")]
	extern public static void IceSetShutdownNegotiation
			(IceConn *iceConn, XBool negotiate);

	[DllImport("ICE")]
	extern public static XBool IceCheckShutdownNegotiation(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceCloseConnection(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static XStatus IceAddConnectionWatch
			(IceWatchProc watchProc, IntPtr clientData);

	[DllImport("ICE")]
	extern public static XStatus IceRemoveConnectionWatch
			(IceWatchProc watchProc, IntPtr clientData);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceProtocolSetup
			(IceConn *iceConn, Xlib.Xint myOpcode, IntPtr clientData,
			 XBool mustAuthenticate, out Xlib.Xint majorVersionRet,
			 out Xlib.Xint minorVersionRet, out IntPtr vendorRet,
			 out IntPtr releaseRet, Xlib.Xint errorLength,
			 byte[] errorStringRet);

	[DllImport("ICE")]
	extern public static XStatus IceProtocolShutdown
			(IceConn *iceConn, Xlib.Xint majorOpcode);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceProcessMessages
			(IceConn *iceConn, ref IceReplyWaitInfo replyWait, ref XBool replyReadyRet);

	[DllImport("ICE")]
	extern public static XStatus IcePing
			(IceConn *iceConn, IcePingReplyProc pingReplyProc,
			 IntPtr clientData);

	[DllImport("ICE")]
	extern public static IntPtr IceAllocScratch
			(IceConn *iceConn, Xlib.Xulong size);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceFlush(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceGetOutBufSize(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceGetInBufSize(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceConnectionStatus(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static String IceVendor(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static String IceRelease(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceProtocolVersion(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceProtocolRevision(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint IceConnectionNumber(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static String IceConnectionString(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xulong IceLastSentSequenceNumber(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xulong IceLastReceivedSequenceNumber
			(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static XBool IceSwapping(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static IntPtr IceSetErrorHandler(IceErrorHandler handler);

	[DllImport("ICE")]
	extern public static IntPtr IceSetErrorHandler(IntPtr handler);

	[DllImport("ICE")]
	extern public static IntPtr IceSetIOErrorHandler(IceIOErrorHandler handler);

	[DllImport("ICE")]
	extern public static IntPtr IceSetIOErrorHandler(IntPtr handler);

	[DllImport("ICE")]
	extern public static XStatus IceInitThreads();

	[DllImport("ICE")]
	extern public static void IceAppLockConn(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static void IceAppUnlockConn(IceConn *iceConn);

	[DllImport("ICE")]
	extern public static Xlib.Xint _IcePoMagicCookie1Proc
			(IntPtr iceConn, IntPtr authStatePtr, XBool cleanUp,
			 XBool swap, Xlib.Xint authDataLen, IntPtr authData,
			 ref Xlib.Xint replyDataLenRet, ref IntPtr replyDataRet,
			 ref IntPtr errorStringRet);

	[DllImport("ICE")]
	extern public static Xlib.Xint _IcePaMagicCookie1Proc
			(IntPtr iceConn, IntPtr authStatePtr,
			 XBool swap, Xlib.Xint authDataLen, IntPtr authData,
			 ref Xlib.Xint replyDataLenRet, ref IntPtr replyDataRet,
			 ref IntPtr errorStringRet);

	[DllImport("ICE")]
	extern public static XStatus _IceRead
			(IceConn *iceConn, Xlib.Xulong nbytes, byte[] ptr);

	[DllImport("ICE")]
	extern public static void _IceReadSkip
			(IceConn *iceConn, Xlib.Xulong nbytes);

	[DllImport("ICE")]
	extern public static void _IceWrite
			(IceConn *iceConn, Xlib.Xulong nbytes, byte[] ptr);

	[DllImport("ICE")]
	extern public static void _IceErrorBadMinor
			(IceConn *iceConn, Xlib.Xint majorOpcode,
			 Xlib.Xint offendingMinor, Xlib.Xint severity);

	[DllImport("ICE")]
	extern public static void _IceErrorBadState
			(IceConn *iceConn, Xlib.Xint majorOpcode,
			 Xlib.Xint offendingMinor, Xlib.Xint severity);

	[DllImport("ICE")]
	extern public static void _IceErrorBadLength
			(IceConn *iceConn, Xlib.Xint majorOpcode,
			 Xlib.Xint offendingMinor, Xlib.Xint severity);

	[DllImport("ICE")]
	extern public static void _IceErrorBadValue
			(IceConn *iceConn, Xlib.Xint majorOpcode,
			 Xlib.Xint offendingMinor, Xlib.Xint offset,
			 Xlib.Xint length, IntPtr value);

	// Determine if this platform is little-endian.
	private static int endian = -1;
	public static bool IsLittleEndian
			{
				get
				{
					lock(typeof(ICE))
					{
						if(endian == -1)
						{
							IntPtr buf = Marshal.AllocHGlobal(2);
							Marshal.WriteInt16(buf, 0, 0x0102);
							if(Marshal.ReadByte(buf, 0) == 0x01)
							{
								endian = 1;
							}
							else
							{
								endian = 0;
							}
							Marshal.FreeHGlobal(buf);
						}
						return (endian == 0);
					}
				}
			}

	// Send data over an ICE connection.
	public static void IceSendData(IceConn *iceConn, int nbytes, byte[] data)
			{
				IceFlush(iceConn);
				_IceWrite(iceConn, (Xlib.Xulong)(uint)nbytes, data);
			}

	// Read data from an ICE connection.
	public static bool IceReadData(IceConn *iceConn, int nbytes, byte[] data)
			{
				return (_IceRead(iceConn, (Xlib.Xulong)(uint)nbytes, data)
							!= XStatus.Zero);
			}

} // class ICE

} // namespace Xsharp.Ice
