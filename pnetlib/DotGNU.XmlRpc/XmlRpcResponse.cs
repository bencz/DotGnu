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
 * $Revision: 1.3 $  $Date: 2007/05/23 20:16:42 $
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
  public class XmlRpcResponse : XmlRpcObject
  {
    public XmlRpcResponse()
    {
    }
    
    public override string ToString() 
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder("XmlRpcResponse contains:\n");
      foreach( object obj in this ) {
	    if( obj != null ) {
		  sb.Append(String.Format( "Type: {0}, Value: {1}", obj.GetType(), obj ));
        } else {
		  sb.Append(String.Format( "Type: null, Value: null" ));
		}
      }
      return sb.ToString();
    }
  }
}

