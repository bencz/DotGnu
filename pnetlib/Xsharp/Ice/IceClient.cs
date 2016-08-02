/*
 * IceClient.cs - Client handler for ICE iceConns.
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
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.Ice.IceClient"/> class manages the client
/// side of an ICE iceConn for a specific protocol.</para>
/// </summary>
public abstract unsafe class IceClient
{
	// Internal state.
	private Display dpy;
	private int majorOpcode;
	private IceConn *iceConn;
	private byte[] buffer;
	
	private bool messageTransaction;
	private int minorOpcode;
	private IceReplyWaitInfo waitInfo;
	private int messageLength = 0;

	/// <summary>
	/// <para>Construct an ICE client handler to process messages for
	/// a particular ICE protocol.</para>
	/// </summary>
	///
	/// <param name="dpy">
	/// <para>The display to attach to the ICE iceConn's message
	/// processor.</para>
	/// </param>
	///
	/// <param name="protocolName">
	/// <para>The name of the protocol to register.</para>
	/// </param>
	///
	/// <param name="vendor">
	/// <para>The name of the vendor for the protocol being registered.</para>
	/// </param>
	///
	/// <param name="release">
	/// <para>The name of the release for the protocol being registered.</para>
	/// </param>
	///
	/// <param name="majorVersion">
	/// <para>The major vesion number for the protocol being registered.</para>
	/// </param>
	///
	/// <param name="minorVersion">
	/// <para>The minor vesion number for the protocol being registered.</para>
	/// </param>
	///
	/// <param name="serverAddress">
	/// <para>The address of the ICE server to connect to.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="protocolName"/>,
	/// <paramref name="vendor"/>, <paramref name="release"/>, or
	/// <paramref name="serverAddress"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XInvalidOperationException">
	/// <para>Raised if the protocol could not be registered or the
	/// iceConn to the ICE server could not be established.</para>
	/// </exception>
	public IceClient(Display dpy, String protocolName, String vendor,
					 String release, int majorVersion, int minorVersion,
					 String serverAddress)
			{
				// Validate the parameters.
				if(dpy == null)
				{
					throw new ArgumentNullException("dpy");
				}
				if(protocolName == null)
				{
					throw new ArgumentNullException("protocolName");
				}
				if(vendor == null)
				{
					throw new ArgumentNullException("vendor");
				}
				if(release == null)
				{
					throw new ArgumentNullException("release");
				}
				if(serverAddress == null)
				{
					throw new ArgumentNullException("serverAddress");
				}

				// Register the protocol with "libICE".
				IcePoVersionRec version = new IcePoVersionRec();
				version.major_version = majorVersion;
				version.minor_version = minorVersion;
				version.process_msg_proc =
					new IcePoProcessMsgProc(ProcessMessage);
				String[] authNames = new String[] {"MIT-MAGIC-COOKIE-1"};
				ICE.IcePoAuthProcIncapsulator authProc =
					new ICE.IcePoAuthProcIncapsulator(new IcePoAuthProc(ICE._IcePoMagicCookie1Proc));
				// FIXME: this is overhead, it should be done if (_IceLastMajorOpcode < 1 ) only
				// FIXME: This should be called, but this sevgvs. If someone will take care of segv - decomment and delete hack from this::ProcessResponces()
/*				ICE.IceRegisterForProtocolSetup
					("DUMMY", "DUMMY", "DUMMY",
					 (Xlib.Xint)1, ref version,
					 (Xlib.Xint)1, authNames, ref authProc, null);*/
				majorOpcode = (int)ICE.IceRegisterForProtocolSetup
					(protocolName, vendor, release,
					 (Xlib.Xint)1, ref version,
					 (Xlib.Xint)1, authNames, ref authProc, null);
				if(majorOpcode < 0)
				{
					throw new XInvalidOperationException();
				}

				// Open the ICE iceConn to the server.
				byte[] errorBuffer = new byte [1024];
				iceConn = ICE.IceOpenConnection
					(serverAddress, (IntPtr)this.GetHashCode() /* This is hash code is not it? */, XBool.False,
					 (Xlib.Xint)majorOpcode, (Xlib.Xint)1024, errorBuffer);
				if(iceConn == null)
				{
					throw new XInvalidOperationException();
				}

				// We don't want shutdown negotiation.
				ICE.IceSetShutdownNegotiation(iceConn, XBool.False);

				// Perform protocol setup on the iceConn.
				IceProtocolSetupStatus status;
				Xlib.Xint majorRet, minorRet;
				IntPtr vendorRet, releaseRet;
				status = (IceProtocolSetupStatus)ICE.IceProtocolSetup
					(iceConn, (Xlib.Xint)majorOpcode, IntPtr.Zero, // We use OO language so we do not need to pass any pointers to callback; he already have its object
					 XBool.False, out majorRet, out minorRet,
					 out vendorRet, out releaseRet,
					 (Xlib.Xint)1024, errorBuffer);
				if(status != IceProtocolSetupStatus.IceProtocolSetupSuccess)
				{
					ICE.IceCloseConnection(iceConn);
					iceConn = null;
					throw new XInvalidOperationException();
				}

				// Check the iceConn status.
				if(ICE.IceConnectionStatus(iceConn) !=
						(Xlib.Xint)(IceConnectStatus.IceConnectAccepted))
				{
					ICE.IceCloseConnection(iceConn);
					iceConn = null;
					throw new XInvalidOperationException();
				}

				// Initialize other state information.
				this.buffer = errorBuffer;
				this.dpy = dpy;
				this.messageTransaction = false;

				// TODO: register IceConnectionNumber(iceConn) with
				// the select loop.
			}

	/// <summary>
	/// <para>Close the ICE client if it is currently active.</para>
	/// </summary>
	~IceClient()
			{
				Close();
			}

	/// <summary>
	/// <para>Get the major opcode that is being used for ICE requests.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns the major opcode value.</para>
	/// </value>
	// FIXME: Should not it be protected or even private?
	public int MajorOpcode
			{
				get
				{
					return majorOpcode;
				}
			}

	/// <summary>
	/// <para>Close this ICE client iceConn.</para>
	/// </summary>
	public virtual void Close()
			{
				if(iceConn != null)
				{
					// TODO: deregister the fd from the select loop.
					ICE.IceProtocolShutdown
						(iceConn, (Xlib.Xint)majorOpcode);
					ICE.IceCloseConnection(iceConn);
					iceConn = null;
				}
			}

	/// <summary>
	/// <para>Process a message that was received by this ICE client.</para>
	/// </summary>
	///
	/// <param name="opcode">
	/// <para>The minor opcode for the message.</para>
	/// </param>
	///
	/// <param name="reply">
	/// <para>Object passed to ProcessResponces().</para>
	/// </param>
	protected abstract bool ProcessMessage
				(int opcode, Object reply);

	protected abstract bool ProcessMessage
				(int opcode);

	// Callback from "libICE" to process a message.
	private void ProcessMessage
				(IntPtr iceConn, IntPtr clientData, Xlib.Xint opcode,
				 Xlib.Xulong length, XBool swap,
				 ref IceReplyWaitInfo replyWait, ref XBool replyReadyRet)
			{
				bool haveReply;
				if(messageTransaction)
				{
					throw new XInvalidOperationException("Attempt to process message in the middle of sending another one"); // I always wondered, what will happen if we throw exception from callback? :)
				}

				this.messageTransaction = true;
				this.messageLength = (int)length;

				// Process the message.
				try
				{
					replyWait = replyWait;
					haveReply = true;
					replyReadyRet = ProcessMessage((int)opcode, replyWait.reply) ? XBool.True:XBool.False; // We omit `swap' here, can one need it? Even theoretrically?..
				}
				catch (NullReferenceException)
				{
					haveReply = false;
					replyReadyRet = ProcessMessage((int)opcode) ? XBool.True:XBool.False;
				}

				this.messageTransaction = false;
			}

	// Process incoming messages on the ICE iceConn.
	protected void ProcessResponces(Object replyStruct)
			{
				XBool readyRet = XBool.False;
				IceProcessMessagesStatus status;
				waitInfo.sequence_of_request = (uint)ICE.IceLastSentSequenceNumber(iceConn);
				waitInfo.major_opcode_of_request = 2; // FIXME: This is a terrible hack, see this::.ctor
				waitInfo.reply = replyStruct;
				do
				{
					status = (IceProcessMessagesStatus)ICE.IceProcessMessages(iceConn, ref waitInfo, ref readyRet);
					if(status != IceProcessMessagesStatus.IceProcessMessagesSuccess)
					{
						Close();
						throw new XInvalidOperationException("I/O Error or server gone away?");
					}
				}
				while(readyRet == XBool.False);
			}

	protected void BeginMessage(int minor)
		{
			if(messageTransaction)
			{
				throw new XInvalidOperationException("Attempt to initiate new message in the middle of other");
			}

			this.minorOpcode = minor;
			this.messageTransaction = true;
			waitInfo.major_opcode_of_request = majorOpcode;
			waitInfo.minor_opcode_of_request = minorOpcode;
		}

	// Form message header, which is ICE's own header + user generated header
	// Caller is guilty for user header bytesex
	protected void SendHeader(byte[] header, int length)
			{
				if(! messageTransaction)
				{
					throw new XInvalidOperationException("Can not send header until not started message");
				}

				int headerLength = header.Length;
				int size = 8 + headerLength;

				byte[] buffer = new byte[size];
				buffer[0] = (byte)majorOpcode;
				buffer[1] = (byte)minorOpcode;
				buffer[2] = (byte)0;
				buffer[3] = (byte)0;
				if(ICE.IsLittleEndian)
				{
					buffer[4] = (byte)length;//(byte)size;
					buffer[5] = (byte)(length >> 8);
					buffer[6] = (byte)(length >> 16);
					buffer[7] = (byte)(length >> 24);
				}
				else
				{
					buffer[4] = (byte)(length >> 24);
					buffer[5] = (byte)(length >> 16);
					buffer[6] = (byte)(length >> 8);
					buffer[7] = (byte)length;
				}
				Array.Copy(header, 0, buffer, 8, headerLength);
				ICE.IceFlush(iceConn);
				ICE._IceWrite(iceConn, (Xlib.Xulong)size, buffer);
			}

	protected void SendData(byte[] data)
			{
				if(! messageTransaction)
				{
					throw new XInvalidOperationException("Can not send data until not started message");
				}

				ICE.IceFlush(iceConn);
				ICE._IceWrite(iceConn, (Xlib.Xulong)data.Length, data); // If someone want to send part of buffer he should fsck with it himself
			}

	protected void FinishMessage()
			{
				if(! messageTransaction)
				{
					throw new XInvalidOperationException("Can not finish message until not started it");
				}

				ICE.IceFlush(iceConn);

				this.messageTransaction = false;
				if (ICE.IceConnectionStatus(iceConn) != (Xlib.Xint)IceConnectStatus.IceConnectAccepted)
				{
					throw new XInvalidOperationException("Connection not accepted");
				}
			}

	protected byte[] Receive(int length) // Cheap and Angry
			{
				if(! messageTransaction)
				{
					throw new XInvalidOperationException("Can not read data when not receiving message");
				}
			
				// Client should solve its bytesex himself
				byte[] outbuf = new byte[length];
				ICE._IceRead(iceConn, (Xlib.Xulong)length, outbuf);
				this.messageLength -= length;
				return outbuf;
			}

	protected byte[] ReceiveHeader(int length) // Cheap and Angry
			{
				if(! messageTransaction)
				{
					throw new XInvalidOperationException("Can not read data when not receiving message");
				}
			
				// Client should solve its bytesex himself
				byte[] outbuf = new byte[length];
				ICE._IceRead(iceConn, (Xlib.Xulong)length, outbuf);
				return outbuf;
			}

	protected byte[] Receive() // Cheap and Angry
			{
				return Receive(messageLength);
			}

} // class IceClient

} // namespace Xsharp.Ice

