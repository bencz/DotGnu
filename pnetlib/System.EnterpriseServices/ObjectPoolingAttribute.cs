/*
 * ObjectPoolingAttribute.cs - Implementation of the
 *			"System.EnterpriseServices.ObjectPoolingAttribute" class.
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

using System.Runtime.InteropServices;
using System.Collections;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
[AttributeUsage(AttributeTargets.Class, Inherited=true)]
public sealed class ObjectPoolingAttribute : Attribute, IConfigurationAttribute
{
	// Internal state.
	private bool enabled;
	private int minPoolSize;
	private int maxPoolSize;
	private int creationTimeout;

	// Constructors.
	public ObjectPoolingAttribute() : this(true) {}
	public ObjectPoolingAttribute(bool enable)
			{
				this.enabled = enable;
				this.minPoolSize = -1;
				this.maxPoolSize = -1;
				this.creationTimeout = -1;
			}
	public ObjectPoolingAttribute(int minPoolSize, int maxPoolSize)
			: this(true, minPoolSize, maxPoolSize) {}
	public ObjectPoolingAttribute(bool enable, int minPoolSize, int maxPoolSize)
			{
				this.enabled = enable;
				this.minPoolSize = minPoolSize;
				this.maxPoolSize = maxPoolSize;
				this.creationTimeout = -1;
			}

	// Get or set this attribute's values.
	public int CreationTimeout
			{
				get
				{
					return creationTimeout;
				}
				set
				{
					creationTimeout = value;
				}
			}
	public bool Enabled
			{
				get
				{
					return enabled;
				}
				set
				{
					enabled = value;
				}
			}
	public int MaxPoolSize
			{
				get
				{
					return maxPoolSize;
				}
				set
				{
					maxPoolSize = value;
				}
			}
	public int MinPoolSize
			{
				get
				{
					return minPoolSize;
				}
				set
				{
					minPoolSize = value;
				}
			}

	// Implement the IConfigurationAttribute interface.
	public bool AfterSaveChanges(Hashtable info)
			{
				// Not used in this implementation.
				return false;
			}
	public bool Apply(Hashtable info)
			{
				// Not used in this implementation.
				return true;
			}
	public bool IsValidTarget(String s)
			{
				// Not used in this implementation.
				return (s == "Component");
			}

}; // class ObjectPoolingAttribute

}; // namespace System.EnterpriseServices
