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
 * $Revision: 1.4 $  $Date: 2007/05/23 20:16:42 $
 * 
 * --------------------------------------------------------------------------
 */
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace DotGNU.XmlRpc
{  
  // For now, indentation is the default since it makes it easier for
  // a human to read the output.  later the output should not be
  // formatted at all.  ideally it should be an unindented block.
  // Ideally XmlTextWriter should write the tags, but it's indentation
  // is too broken so that its more human readable and debuggable.
  // Grrr Grrr Grrrrr
  public class XmlRpcWriter : XmlTextWriter
  {

    // TODO: build node tree in memory, then flush this tree at the
    // end which avoids us to do the WriteEndElements and allows us
    // for consistency checking

    private StringWriter sw;
    private XmlRpcMethod m;

    public XmlRpcWriter( TextWriter w ) : base( w )
    {
    }

    public XmlRpcWriter( Stream stream, Encoding encoding ) : base( stream, encoding )
    { 
    }

    public XmlRpcWriter( String filename, Encoding encoding ) : base( filename, encoding )
    {
    }

    public void Write( XmlRpcMethod method )
    {
      if(method == null)
      {
        throw new ArgumentNullException("method", "Argument cannot be null");
      }

      WriteStartDocument();
      WriteStartElement( "methodCall" );
      WriteElementString( "methodName", method.Name );
      WriteParams();
      foreach( object o in method ) {
	WriteParam( o );
      }
      WriteEndDocument();
    }

    private void WriteMethodResponse()
    {
      WriteStartElement( "methodResponse" );   
    }

    public void Write( XmlRpcResponse response )
    {
      if(response == null)
      {
        throw new ArgumentNullException("response", "Argument cannot be null");
      }

      WriteStartDocument();
      WriteMethodResponse();
      WriteParams();
      foreach( object o in response ) {
	WriteParam( o );
      }
      WriteEndDocument();
    }
    
    public void Write( XmlRpcException e )
    {
      if(e == null)
      {
        throw new ArgumentNullException("e", "Argument cannot be null");
      }

      WriteStartDocument();
      WriteMethodResponse();
      WriteStartElement( "fault" );
      XmlRpcStruct s = new XmlRpcStruct();
      s.Add( "faultCode", e.FaultCode );
      s.Add( "faultString", e.Message );
      WriteValue( s );
      WriteEndElement();
      WriteEndDocument();
    }

    public void WriteMethodResponse( Exception e )
    {
      XmlRpcException ex = new XmlRpcException( e );
      WriteMethodResponse( ex );
    }
    
    private void WriteParams()
    {
      WriteStartElement( "params" );
    }
    
    private void WriteParam( object o )
    {
      WriteStartElement( "param" );
      WriteValue( o );
      WriteEndElement();
    }
    
    private void WriteValue()
    {
      WriteStartElement( "value" );
    }

    private void WriteInt( int v )
    {
      WriteElementString( "i4", XmlConvert.ToString( v ) );
    }
    
    private void WriteStringValue( string v)
    {
      WriteElementString( "string", v );
    }
    
    private void WriteDouble( double v )
    {
      // XmlConvert is HORRID doing this!!!!  Try this with a value
      // of 2.0 and see what i mean
      WriteElementString( "double",  v.ToString() );
    }
    
    private void WriteBoolean( bool v )
    {
      WriteElementString( "boolean", XmlConvert.ToString( v ) );
    }

    private void WriteDateTime( DateTime v )
    {
      // ensure this is iso compliant

      // Why has MS got no clue about ISO 8601???  Horrid.  All
      // these newbies working on specs.  this is as close as it
      // gets but not exactly since the 'T' can be left out
      WriteElementString( "dateTime.iso8601", v.ToString( "s", null ) );
    }
    
    private void WriteBase64( byte[] v )
    {
      WriteStartElement( "base64" );
      WriteBase64( v, 0, v.Length);
      WriteEndElement();
    }

    private void WriteStruct( XmlRpcStruct v )
    {
      WriteStartElement( "struct" );
      
      foreach( DictionaryEntry entry in v ) {
	WriteStartElement( "member" );
	WriteElementString( "name", (string)entry.Key );
	WriteValue( entry.Value );
	WriteEndElement(); // member
      }
      WriteEndElement(); // struct
    }

    private void WriteArray( XmlRpcArray v )
    {
      WriteStartElement( "array" );
      WriteStartElement( "data" );
      foreach( object entry in v ) {
	WriteValue( entry );
      }
      WriteEndElement(); // data
      WriteEndElement(); // array
    }

    private void WriteValue( object o )
    {
      WriteStartElement( "value" );
      if( o is int ) {
	WriteInt( (int)o );
      }
      else if( o is double ) {
	WriteDouble( (double)o );
      }
      else if( o is string ) {
	WriteStringValue( (string)o );
      }
      else if( o is bool ) {
	WriteBoolean( (bool)o );
      }
      else if( o is DateTime ) {
	WriteDateTime( (DateTime)o );
      }
      else if( o is byte[] ) {
	WriteBase64( (byte[])o );
      }
      else if( o is XmlRpcStruct ) {
	WriteStruct( (XmlRpcStruct)o );
      }
      else if( o is XmlRpcArray ) {
	WriteArray( (XmlRpcArray)o );
      }
      WriteEndElement();
    }
  }
}
