/*
 * TransactionAttribute.cs - Implementation of the
 *			"System.EnterpriseServices.TransactionAttribute" class.
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

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
[AttributeUsage(AttributeTargets.Class, Inherited=true)]
public sealed class TransactionAttribute : Attribute
{
	// Internal state.
	private TransactionOption val;
	private TransactionIsolationLevel isolation;
	private int timeout;

	// Constructors.
	public TransactionAttribute()
			: this(TransactionOption.Required) {}
	public TransactionAttribute(TransactionOption val)
			{
				this.val = val;
				this.isolation = TransactionIsolationLevel.Serializable;
				this.timeout = -1;
			}

	// Get or set this attribute's values.
	public TransactionIsolationLevel Isolation
			{
				get
				{
					return isolation;
				}
				set
				{
					isolation = value;
				}
			}
	public int Timeout
			{
				get
				{
					return timeout;
				}
				set
				{
					timeout = value;
				}
			}
	public TransactionOption Value
			{
				get
				{
					return val;
				}
			}

}; // class TransactionAttribute

}; // namespace System.EnterpriseServices
