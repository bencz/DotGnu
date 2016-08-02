/*
 * sslfetch.cs - Sample program that uses "DotGNU.SSL" to fetch https data.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

/*

Usage: sslfetch url

Where "url" is a https URL.  This program connects to the specified
host and port and fetches the contents of the given URL.  The entire
HTTP response, including the header, is written to stdout.

*/

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using DotGNU.SSL;

public class SSLFetch
{
	// Main entry point.
	public static int Main(String[] args)
			{
				Uri uri;

				// Process the command-line options.
				if(args.Length != 1)
				{
				#if CONFIG_SMALL_CONSOLE
					Console.WriteLine("Usage: sslfetch url");
				#else
					Console.Error.WriteLine("Usage: sslfetch url");
				#endif
					return 1;
				}
				uri = new Uri(args[0]);
				if(uri.Scheme != "https")
				{
				#if CONFIG_SMALL_CONSOLE
					Console.WriteLine("{0} is not a https url", args[0]);
				#else
					Console.Error.WriteLine("{0} is not a https url", args[0]);
				#endif
					return 1;
				}

				// Resolve the hostname and build an end point.
				IPHostEntry entry = Dns.Resolve(uri.Host);
				IPEndPoint ep = new IPEndPoint(entry.AddressList[0], uri.Port);

				// Connect to the remote server.
			#if CONFIG_SMALL_CONSOLE
				Console.WriteLine("Connecting to {0} ...", ep.ToString());
			#else
				Console.Error.WriteLine("Connecting to {0} ...", ep.ToString());
			#endif
				Socket socket = new Socket(AddressFamily.InterNetwork,
										   SocketType.Stream,
										   ProtocolType.Unspecified);
				socket.Connect(ep);

				// Wrap the socket in an SSL client session.
				ISecureSessionProvider provider;
				ISecureSession session;
				provider = SessionProviderFactory.GetProvider();
				session = provider.CreateClientSession(Protocol.AutoDetect);

				// Perform the SSL handshake and get the stream.
			#if CONFIG_SMALL_CONSOLE
				Console.WriteLine("Performing the SSL handshake ...");
			#else
				Console.Error.WriteLine("Performing the SSL handshake ...");
			#endif
				Stream stream = session.PerformHandshake(socket);

				// Write the HTTP "GET" to the server.
			#if CONFIG_SMALL_CONSOLE
				Console.WriteLine("Sending HTTP request ...");
			#else
				Console.Error.WriteLine("Sending HTTP request ...");
			#endif
				String get = "GET " + uri.LocalPath + " HTTP/1.0\r\n";
				get += "\r\n";
				byte[] getData = Encoding.UTF8.GetBytes(get);
				stream.Write(getData, 0, getData.Length);
				stream.Flush();

				// Dump the response.
				byte[] buf = new byte [512];
				char[] cbuf = new char [2048];
				int size;
				while((size = stream.Read(buf, 0, buf.Length)) > 0)
				{
					size = Encoding.UTF8.GetChars(buf, 0, size, cbuf, 0);
					Console.Write(cbuf, 0, size);
				}

				// Close the secure stream.
			#if CONFIG_SMALL_CONSOLE
				Console.WriteLine("Closing the secure session ...");
			#else
				Console.Error.WriteLine("Closing the secure session ...");
			#endif
				stream.Close();
				session.Dispose();

				// Close the socket.
			#if CONFIG_SMALL_CONSOLE
				Console.WriteLine("Closing the underlying socket ...");
			#else
				Console.Error.WriteLine("Closing the underlying socket ...");
			#endif
				socket.Close();

				// Done.
				return 0;
			}

}; // class SSLFetch
