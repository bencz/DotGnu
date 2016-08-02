/*
 * DotGNU XmlRpc implementation
 * 
 * Copyright (C) 2002 netFluid Technology Ltd
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
namespace DotGNU.XmlRpc
{
  using System;
  using System.Reflection;

  public class XmlRpcRequest
  {
    private String     method   = null; // Name of method being called
    private MethodInfo methinfo = null; // Method info
    private Object[]   args     = null; // Multi-typed Args to pass to method

    public String Method 
    {
      get {
	return this.method;
      }
      set {
	this.method = value;
      }
    }
    
    public MethodInfo MethodInformation 
    {
      get {
	return this.methinfo;
      }
      set {
	this.methinfo = value;
      }
    }
    
    public Object[] Arguments 
    {
      get {
	return this.args;
      }
      set {
	this.args = value;
      }
    }
    
    public XmlRpcRequest()
    {
    }
    
    public XmlRpcRequest( String methodName )
    {
      method = methodName;
    }

    public XmlRpcRequest( String methodName, MethodInfo minfo ) 
      : this( methodName )
    {
      methinfo = minfo;
    }

    public XmlRpcRequest( String methodName, MethodInfo minfo, Object[] al )
    {
      method   = methodName;
      methinfo = minfo;
      args     = al;
    }
  }
}

// end
