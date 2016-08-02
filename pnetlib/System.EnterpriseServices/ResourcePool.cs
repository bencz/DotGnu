/*
 * ResourcePool.cs - Implementation of the
 *			"System.EnterpriseServices.ResourcePool" class.
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

public sealed class ResourcePool
{
	// Internal state.
	private TransactionEndDelegate cb;
	private Object resource;

	// Constructor.
	public ResourcePool(TransactionEndDelegate cb)
			{
				this.cb = cb;
			}

	// Get a resource from the current transaction.
	public Object GetResource()
			{
				return resource;
			}

	// Add a resource to the current transaction.
	public bool PutResource(Object resource)
			{
				this.resource = resource;
				return true;
			}

	// A delegate that handles returning resources at the end
	// of a transaction.
	public delegate void TransactionEndDelegate(Object resource);

}; // class ResourcePool

}; // namespace System.EnterpriseServices
