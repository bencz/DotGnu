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
 * $Revision: 1.2 $  $Date: 2007/05/23 20:16:42 $
 * 
 * --------------------------------------------------------------------------
 */
using System;
//using System.Reflection;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;

namespace DotGNU.XmlRpc
{
  public abstract class XmlRpcObject : ArrayList
  {
    private DateTimeFormatInfo dateFormat;
    private Stack containerStack;
    private object value;
    private string text;
    protected string methodName;
    private string nodeName;
    private XmlTextReader reader;
    
    public XmlRpcObject()
    {
      this.dateFormat = new DateTimeFormatInfo();
    }
    
    public void Read( XmlTextReader reader ) 
    {
      this.reader = reader;
      Read();
    }
    
    private void Read() { 
      containerStack = new Stack();
      
      // Itereate through the tree and construct an n-ary tree with
      // XmlRpcNodes so we can detect protocol errors and the current
      // scope/context of a value parameter.
      while( reader.Read() ) {
	switch( reader.NodeType ) {
	  //
	  // ELEMENT OPEN
	  //
	case XmlNodeType.Element:
	  nodeName = reader.Name;	  
	  switch( nodeName ) {
	  case "value":
	    value = null;
	    text  = null;
	    break;
	  case "struct":
	    PushScope( new XmlRpcStruct() );
	    break;
	  case "array":
	    PushScope( new XmlRpcArray() );
	    break;
	  }
	  break;

	  //
	  // ELEMENT TEXT
	  //
	case XmlNodeType.Text:
	  text = reader.Value;
	  switch( nodeName ) {
	  case "methodName":
	    methodName = text;
	    break;
	  default:
		AddValue();
	    break;
	  }
	  break;
	  
	  //
	  // ELEMENT CLOSE
	  //
	case XmlNodeType.EndElement:
	  switch( reader.Name ) {
	  case "struct":
	  case "array":
	    object closedScope = containerStack.Pop();

	    // now determine whether the bugger aint nested...
	    if( containerStack.Count > 0 ) {
	      // aaah yes. nested.  add him to the current stack object
	      object stackObject = containerStack.Peek();
	      if( stackObject is XmlRpcStruct ) {
		((XmlRpcStruct)stackObject).value = closedScope;
	      }
	      else if( stackObject is XmlRpcArray ) {
		((XmlRpcArray)stackObject).Add( closedScope );
	      }
	    }
	    else {
	      Add( closedScope );
	    }
	    break;
	  case "member":
	    object stackObject;
	    if( containerStack.Count > 0 ) {
	      stackObject = containerStack.Peek();
	      if( stackObject is XmlRpcStruct ) {
		((XmlRpcStruct)stackObject).Commit();
	      }
	    }
	    break;
	  } //switch
	  break; // switch XmlNodeType.EndElement
	}
      } // reader.Read()
    }
    
    private void AddValue()
    {
      object o;      
      switch( nodeName ) {
      case "i4":
      case "int":
	o = Int32.Parse( text );
	break;
      case "boolean":
	if( text == "0" ) {
	  text = "false";
	}
	else {
	  text = "true";
	}
	o = Convert.ToBoolean( text );
	break;
      case "double":
	o = Double.Parse( text );
	break;
      case "dateTime.iso8601":
	try {
	  o = DateTime.ParseExact( text, "s", dateFormat );
	}
	catch( FormatException e ){
	  string str = 
	    String.Format
	    ( "Cannot parse DateTime value: {0}, expected format is: {1}",
	      text, dateFormat.SortableDateTimePattern );	      
	  throw new XmlRpcBadFormatException( 200, str );
	}
	break;
      case "base64":
	o = Convert.FromBase64String( text );
	break;
      case "string":
      case "name":
	o = text;
	break;
      }

      object stackObject;
      if( containerStack.Count > 0 ) {
	stackObject = containerStack.Peek();
	if( stackObject is XmlRpcStruct ) {
	  switch( nodeName ) {
	  case "name":
	    ((XmlRpcStruct)stackObject).key = text;
	    break;
	  default:
	    ((XmlRpcStruct)stackObject).value = o;
	    break;
	  }
	}
	else if( stackObject is XmlRpcArray ) {
	  ((XmlRpcArray)stackObject).Add( o );
	}
      }
      else {
	if( o != null ) Add( o );
      }
    }
    
    private void PushScope( Object o )
    {
      containerStack.Push( o );
    }
  }
}

