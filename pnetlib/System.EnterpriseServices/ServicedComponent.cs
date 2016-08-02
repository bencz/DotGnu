/*
 * ServicedComponent.cs - Implementation of the
 *			"System.EnterpriseServices.ServicedComponent" class.
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

namespace System.EnterpriseServices
{

#if !ECMA_COMPAT
[Serializable]
public abstract class ServicedComponent : ContextBoundObject
#else
public abstract class ServicedComponent : MarshalByRefObject
#endif
	, IDisposable , IServicedComponentInfo, IRemoteDispatch
{
	// Constructor.
	public ServicedComponent() {}

	// Dispose of this object.
	public void Dispose()
			{
				DisposeObject(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				// Nothing to do here in the base class.
			}

	// Dispose a serviced component object.
	public static void DisposeObject(ServicedComponent sc)
			{
				sc.Dispose(true);
			}

	// Method that is called when the serviced component is activated.
	protected internal virtual void Activate() {}

	// Determine if this serviced component can be pooled.
	protected internal virtual bool CanBePooled()
			{
				return false;
			}

	// Method that is called when the serviced component is constructed.
	protected internal virtual void Construct(String s) {}

	// Method that is called just before the component is deactivated.
	protected internal virtual void Deactivate() {}

	// Implement the IServicedComponentInfo interface.
	void IServicedComponentInfo.GetComponentInfo
				(ref int infoMask, out String[] infoArray)
			{
				infoArray = new String [0];
			}

	// Implement the IRemoteDispatch interface.
	[AutoComplete(true)]
	String IRemoteDispatch.RemoteDispatchAutoDone(String s)
			{
				return null;
			}
	[AutoComplete(true)]
	String IRemoteDispatch.RemoteDispatchNotAutoDone(String s)
			{
				return null;
			}

}; // class ServicedComponent

}; // namespace System.EnterpriseServices
