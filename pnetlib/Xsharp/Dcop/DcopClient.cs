/*
 * DcopClient.cs - ICE client for DCOP requests.
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

namespace Xsharp.Dcop
{

using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using Xsharp.Ice;

/// <summary>
/// <para>The <see cref="T:Xsharp.Dcop.DcopClient"/> class manages the client
/// side of a DCOP connection.</para>
/// </summary>
public sealed class DcopClient : IceClient
{
	// Internal state.
	private String appId;
	private byte[] key;
	private int replyId;

	private static DcopClient mainClient = null;

	/// <summary>
	/// <para>Construct a DCOP client handler to process DCOP messages.</para>
	/// </summary>
	///
	/// <param name="dpy">
	/// <para>The display to attach to the DCOP connection's message
	/// processor.</para>
	/// </param>
	///
	/// <param name="registerName">
	/// <para>The name of the application to register with DCOP, or
	/// <see langword="null"/> to register anonymously.</para>
	/// </param>
	///
	/// <param name="addPID">
	/// <para>Set to <see langword="true"/> to add the process ID to
	/// the registered name.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.Dcop.DcopException">
	/// <para>Raised if the connection to the DCOP server could not be
	/// established.</para>
	/// </exception>

	public DcopClient(Display dpy, String registerName, bool addPID)
			: base(dpy, "DCOP", "KDE", "2.0", 2, 0, GetDcopServer(dpy))
			{
				// Construct the full registration name for the DCOP client.
				if(registerName == null)
				{
					registerName = "anonymous";
				}
			#if CONFIG_EXTENDED_DIAGNOSTICS
				if(addPID)
				{
					int pid = Process.GetCurrentProcess().Id;
					if(pid != -1 && pid != 0)
					{
						registerName += "-" + pid.ToString();
					}
				}
			#endif

				replyId = 0;
				key = new byte[4];

				// Register the name with the DCOP server.
				appId = registerAs(registerName);

				mainClient = this;

			}

	/// <summary>
	/// <para>Construct a DCOP client handler to process DCOP messages.</para>
	/// </summary>
	///
	/// <param name="dpy">
	/// <para>The display to attach to the DCOP connection's message
	/// processor.</para>
	/// </param>
	///
	/// <param name="registerAs">
	/// <para>The name of the application to register with DCOP, or
	/// <see langword="null"/> to register anonymously.  The process ID
	/// will be automatically added to the name.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.DcopException">
	/// <para>Raised if the connection to the DCOP server could not be
	/// established.</para>
	/// </exception>
	public DcopClient(Display dpy, String registerAs)
			: this(dpy, registerAs, true) {}

	// Get the address of the DCOP server.  Returns null if not found.
	private static String GetDcopServer(Display dpy)
			{
				String value;

				// Bail out if the display is null.
				if(dpy == null)
				{
					throw new ArgumentNullException("dpy");
				}

				// Try the DCOPSERVER environment variable first.
				value = Environment.GetEnvironmentVariable("DCOPSERVER");
				if(value != null && value.Length > 0)
				{
					return value;
				}

				// Locate the DCOP authority file.
				value = Environment.GetEnvironmentVariable("DCOPAUTHORITY");
				if(value == null || value.Length == 0)
				{
					// We need the "HOME" environment variable.
					String home = Environment.GetEnvironmentVariable("HOME");
					if(home == null || home.Length == 0)
					{
						throw new DcopConnectionException("Home not found");
					}

					// Get the name of the display, without the screen number.
					String displayName = dpy.displayName;
					int index1 = displayName.LastIndexOf('.');
					int index2 = displayName.LastIndexOf(':');
					if(index1 >= 0 && index1 > index2)
					{
						displayName = displayName.Substring(0, index1);
					}
					displayName = displayName.Replace(":", "_");

					// Get the name of the local host.
					String hostname = Application.Hostname;
					if(hostname == null)
					{
						hostname = "localhost";
					}

					// Build the full name of the authority file.
					value = home + "/.DCOPserver_" + hostname + "_" +
							displayName;
				}

				// Bail out if the authority file does not exist.
				if(!File.Exists(value))
				{
					throw new DcopConnectionException("Authority file does not exist");
				}

				// Read the first line from the authority file.
				try
				{
					StreamReader reader = new StreamReader(value);
					value = reader.ReadLine();
					reader.Close();
					if(value != null && value.Length != 0)
					{
						return value;
					}
					else
					{
						throw new DcopConnectionException();
					}
				}
				catch(Exception)
				{
					throw new DcopConnectionException("Failed to read from authority file");
				}
			}

	/// <summary>
	/// <para>Get the application identifier for this DCOP client.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns the application identifier.</para>
	/// </value>
	public String AppId
			{
				get
				{
					return appId;
				}
			}

	// Process reply message
	protected override bool ProcessMessage
				(int opcode, Object oReply)
			{
				byte[] data;
				MemoryStream mem;
				QDataStream ds;
				DcopReply reply = oReply as DcopReply;
				key = ReceiveHeader(4); // Read 4-byte DCOP key

				data = Receive(); // Read all data
				mem = new MemoryStream(data);
				ds = new QDataStream(mem);

				switch((DcopMinorOpcode)opcode)
				{
					case DcopMinorOpcode.DcopReply:
						reply.status = DcopReply.ReplyStatus.Ok;
						reply.transactionId = 0;
						reply.calledApp = ds.ReadString();
						ds.ReadString(); // That's our app.
						reply.replyType = ds.ReadString();
						ds.ReadInt32(); // Reply length, throw it!
						reply.replyObject = ds.ReadObject(reply.replyType);
						return true;
					case DcopMinorOpcode.DcopReplyFailed:
						reply.status = DcopReply.ReplyStatus.Failed;
						reply.transactionId = 0;
						return true;
					// Following are just temporally solution, but for now it's good.
					case DcopMinorOpcode.DcopReplyWait:
						reply.status = DcopReply.ReplyStatus.Pending;
						reply.calledApp = ds.ReadString();
						ds.ReadString();
						reply.transactionId = ds.ReadInt32();
						return true;
					case DcopMinorOpcode.DcopReplyDelayed:
						string ca = ds.ReadString();
						ds.ReadString();
						int ti = ds.ReadInt32();
						if( ca != reply.calledApp || ti != reply.transactionId)
						{
							return false; // Got not that reply what was looking for
						}
						reply.status = DcopReply.ReplyStatus.Ok;
						reply.transactionId = 0;
						reply.replyType = ds.ReadString();
						ds.ReadInt32(); // Reply length, throw it!
						reply.replyObject = ds.ReadObject(reply.replyType);
						return true;
					default:
						return false; // TODO
				}
			}

	// Process new message, i.e. external call
	protected override bool ProcessMessage
				(int opcode)
			{
				// TODO
				Console.WriteLine("Got opcode {0}", (DcopMinorOpcode)opcode);
				return true;
			}

	/// <summary>
	/// <para>Calls function via this DCOP connection.</para>
	/// </summary>
	///
	/// <param name="remoteApp">
	/// <para>Remote application that will be called.</para>
	/// </param>
	///
	/// <param name="remoteObject">
	/// <para>Remote object in application, whose method will be called.</para>
	/// </param>
	///
	/// <param name="remoteFunction">
	/// <para>Remote function to call. See DcopFunction class for syntax.</para>
	/// </param>
	///
	/// <param name="parameters">
	/// <para>Parameters to pass to function. Keep them in sync with function definition.</para>
	/// </param>
	///
	/// <value>
	/// <para>Object, received from remote app. Its type depends on remote app not function name.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.DcopException">
	/// <para>Exception raised if there are problems either with connection or naming.</para>
	/// </exception>
	public Object Call(String remoteApp, String remoteObject,
					      String remoteFunction, params Object[] parameters)
			{
				byte[] header;
				DcopReply reply;
				QDataStream ds;

				DcopFunction fun = new DcopFunction(remoteFunction);
				MemoryStream mem = new MemoryStream();
				try
				{
					ds = QDataStream.Marshal(mem, fun, parameters);
				}
				catch (Exception)
				{
					throw new DcopNamingException("Failed to marshal parameters");
				}

				byte[] data = mem.ToArray();
												
				mem = new MemoryStream();
				ds = new QDataStream(mem);

				ds.Write(appId);
				ds.Write(remoteApp);
				ds.Write(remoteObject);
				ds.Write(fun.Function);
				ds.Write(data.Length);

				header = mem.ToArray();

				// A *LOT* nicely, isn't?
				try {
					BeginMessage((int)DcopMinorOpcode.DcopCall);
					SendHeader(key, header.Length + data.Length);
					SendData(header);
					SendData(data);
					FinishMessage();

					reply = new DcopReply(fun, replyId++);
					ProcessResponces(reply);
					// TODO: This is a hack. Do we need message queues?
					while(reply.status == DcopReply.ReplyStatus.Pending)
					{
						ProcessResponces(reply);
					}
				}
				catch (XException xe)
				{
					throw new DcopConnectionException("Failed to send message", xe);
				}

				if(reply.status == DcopReply.ReplyStatus.Failed)
				{
					throw new DcopNamingException("Dcop reply failed, likely that called function does not exists");
				}

				return reply.replyObject;
			}

	[DcopCall("QCString registerAs(QCString)")]
	private string registerAs(string name)
	{
		return (string)Call("DCOPServer", "", "QCString registerAs(QCString)", name);
	}

	/// <summary>
	/// <para>Returns list of applications currently registered on DCOP server.</para>
	/// </summary>
	[DcopCall("QCStringList registeredApplications()")]
	public string[] registeredApplications()
	{
		return (string[])Call("DCOPServer", "", "QCStringList registeredApplications()");
	}

	/// <summary>
	/// <para>Returns or sets current main (i.e. default) client. DcopRefs depend on this.</para>
	/// </summary>
	public static DcopClient MainClient
			{
				get
				{
					return mainClient;
				}
				set
				{
					mainClient = value;
				}
			}

}; // class DcopClient

}; // namespace Xsharp.Dcop

