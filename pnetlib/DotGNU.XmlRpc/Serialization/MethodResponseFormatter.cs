/*
 * DotGNU XmlRpc implementation
 * 
 * Copyright (C) 2003  Free Software Foundation, Inc.
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
 * $Revision: 1.4 $  $Date: 2007/05/23 20:16:43 $
 * 
 * --------------------------------------------------------------------------
 */

using System;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using DotGNU.XmlRpc;

#if CONFIG_SERIALIZATION
namespace DotGNU.XmlRpc.Serialization.Formatters
{
  public sealed class MethodResponseFormatter : IFormatter
  {
    private SerializationBinder binder;
    private StreamingContext context;
    private ISurrogateSelector surrogateSelector;

    public MethodResponseFormatter() 
    {
      context = new StreamingContext( StreamingContextStates.All );
    }

    // Returns the object wrapped in the XmlRpc response
    public object Deserialize( Stream stream )
    {
      StreamReader reader = new StreamReader( stream );
      XmlTextReader tr = new XmlTextReader( stream );
      XmlRpcResponse response = new XmlRpcResponse();
      response.Read( tr );

      return response;
    }

    // Writes an object as an XmlRpc response
    public void Serialize( Stream stream, object o )
    {
      if(stream == null)
      {
        throw new ArgumentNullException("stream", "Argument must not be null");
      }

      if(o == null)
      {
        throw new ArgumentNullException("o", "Argument must not be null");
      }

      if( o is XmlRpcResponse ){
	StreamWriter sw = new StreamWriter( stream );
	XmlRpcWriter w = new XmlRpcWriter( sw );
	w.Write( (XmlRpcResponse)o );
	w.Flush();
      }
      else {
	  // TODO This should be an exception
	Console.Out.WriteLine
	  ( "iMethodResponseFormatter: On No! Wrong type given: {0}, expected and object of type {1}",
	    o.GetType(), "DotGNU.XmlRpc.XmlRpcResponse" );
      }
    }

    public SerializationBinder Binder 
    {
      get{
	return this.binder;
      }
      set{
	this.binder = value;
      }
    }

    public StreamingContext Context
    {
      get{
	return new StreamingContext();
      }
      set {
      }
    }
    
    public ISurrogateSelector SurrogateSelector
    {
      get {
	return this.surrogateSelector;
      }
      set {
	this.surrogateSelector = value;
      }
    }
  }
}
#endif // CONFIG_SERIALIZATION

