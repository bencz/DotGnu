/*
 * BaseVsaSite.cs - Base class that implements IVsaSite.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.Vsa
{

using System;

public class BaseVsaSite : IVsaSite
{
	// Fetch the compiled information from this site.
	public virtual byte[] Assembly
			{
				get
				{
					return null;
				}
			}
	public virtual byte[] DebugInfo
			{
				get
				{
					return null;
				}
			}

	// Implement the IVsaSite interface.
	public virtual void GetCompiledState(out byte[] pe, out byte[] debugInfo)
			{
				pe = Assembly;
				debugInfo = DebugInfo;
			}
	public virtual Object GetEventSourceInstance
				(String itemName, String eventSourceName)
			{
				throw new VsaException(VsaError.CallbackUnexpected);
			}
	public virtual Object GetGlobalInstance(String name)
			{
				throw new VsaException(VsaError.CallbackUnexpected);
			}
	public virtual void Notify(String notify, Object info)
			{
				throw new VsaException(VsaError.CallbackUnexpected);
			}
	public virtual bool OnCompilerError(IVsaError error)
			{
				return false;
			}

}; // class BaseVsaSite

internal class ThrowOnErrorVsaSite : BaseVsaSite
{

	public override bool OnCompilerError(IVsaError error)
			{
				throw (Exception)error;
			}

}; // class ThrowOnErrorVsaSite

}; // namespace Microsoft.Vsa
