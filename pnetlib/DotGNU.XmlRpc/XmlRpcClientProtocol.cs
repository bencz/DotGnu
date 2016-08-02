/*
 * DotGNU XmlRpcClientProtocol implementation
 * 
 * Copyright (C) 2003 netFluid Technology Ltd
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
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA
 *
 * $Revision: 1.3 $  $Date: 2006/05/14 11:22:47 $
 * 
 * --------------------------------------------------------------------------
 */
namespace DotGNU.XmlRpc
{
#if CONFIG_SERIALIZATION
	using System;
	using System.IO;
	using System.Text;
	using System.Net;
	using System.Net.Sockets;
	using System.Web.Services.Protocols;
	using System.Text.RegularExpressions;
	using DotGNU.XmlRpc.Serialization.Formatters;

	public class XmlRpcClientProtocol
	{
		private String header;
		private Stream ns;
		private String dest;
		public String Url;
		bool	connected = false;
		protected ICredentials Credentials;

	    public XmlRpcClientProtocol()
		{
		}

		public Object[] Invoke(String method_name, Object[] args )
		{
			if(method_name == null)
			{
				throw new ArgumentNullException("method_name", "Argument must not be null");
			}

			if(args == null)
			{
				throw new ArgumentNullException("args", "Argument must not be null");
			}

			MemoryStream ms = new MemoryStream();
			MethodCallFormatter call = new MethodCallFormatter();
			XmlRpcMethod method = new XmlRpcMethod( method_name );

			foreach( object obj in args) { method.Add( obj ); }

			call.Serialize( ms, method );
			ms.Seek(0,0);

			WebRequest wr = WebRequest.Create(Url);
			wr.Method="POST";
			wr.ContentLength = ms.Length;
			wr.Credentials = Credentials;

			ns = wr.GetRequestStream();

			// Got stream to send (ms)
			int len = 1024; 
			int bread;
			byte[] buffer = new byte[len]; 
			while( (bread = ms.Read(buffer, 0, len)) > 0) {
//String s = Encoding.ASCII.GetString(buffer, 0, bread );
//Console.Write( "{0}", s );
				ns.Write(buffer, 0, bread);
			}
			ns.Close();
			ns = (wr.GetResponse()).GetResponseStream();

//while( (bread = ns.Read(buffer, 0, len)) > 0) {
//String s = Encoding.ASCII.GetString(buffer, 0, bread );
//Console.Write( "{0}", s );
//			}

            // Get Response
			MethodResponseFormatter response = new MethodResponseFormatter();
			XmlRpcResponse r = (XmlRpcResponse)response.Deserialize( ns );

			ns.Close();
			Object[] results = r.ToArray();

            // Handle Fault responses
			if( results.Length > 0 && 
			    results[0].GetType().ToString() == "DotGNU.XmlRpc.XmlRpcStruct") 
			{
			  XmlRpcStruct f = (XmlRpcStruct)results[0];

			  if( f.ContainsKey("faultCode") ) {
			    throw new XmlRpcException( (int)f["faultCode"], 
				                           (string)f["faultString"] );
			  }
		    }

			return results;
		}
	}
#endif // CONFIG_SERIALIZATION
}
