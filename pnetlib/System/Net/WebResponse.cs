/*
 * WebResponse.cs - Implementation of the "System.Net.WebResponse" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * With contributions from Jason Lee <jason.lee@mac.com>
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
 
namespace System.Net
{

using System;
using System.IO;

public abstract class WebResponse : MarshalByRefObject, IDisposable
{
	// Not used for anything. To create a new instance use Create method.
	protected WebResponse() {
    }
	
	public virtual void Close() {
		throw new NotSupportedException("Close");
    }
	
	public virtual Stream GetResponseStream() {
		throw new NotSupportedException("GetResponseStream");
    }
	
	void IDisposable.Dispose() {
        Close();
    }
	
	public virtual long ContentLength {
        get
        {
            throw new NotSupportedException("ContentLength");
        }
        set
        {
            throw new NotSupportedException("ContentLength");
        }
	}

	public virtual String ContentType {
        get
        {
            throw new NotSupportedException("ContentType");
        }
        set
        {
            throw new NotSupportedException("ContentType");
        }
        
    }
        
	public virtual WebHeaderCollection Headers {
        get
        {
            throw new NotSupportedException("Headers");
        }
        set
        {
            throw new NotSupportedException("Headers");
        }
        
    }
	
	public virtual Uri ResponseUri {
        get
        {
            throw new NotSupportedException("ResponseUri");
        }
        set
        {
            throw new NotSupportedException("ResponseUri");
        }
        
    }
	
}; //class WebResponse

}; //namespace System.Net
