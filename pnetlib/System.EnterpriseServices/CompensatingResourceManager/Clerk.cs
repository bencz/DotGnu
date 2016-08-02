/*
 * Clerk.cs - Implementation of the
 *			"System.EnterpriseServices.CompensatingResourceManager."
 *			"Clerk" class.
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

namespace System.EnterpriseServices.CompensatingResourceManager
{

public sealed class Clerk
{
	// Internal state.
	internal String activityId;
	internal String compensator;
	internal String description;
	internal String instanceId;
	internal CompensatorOptions flags;
	private String transactionUOW;
	private int count;

	// Constructors.
	public Clerk(String compensator, String description,
				 CompensatorOptions flags)
			{
				this.compensator = compensator;
				this.description = description;
				this.flags = flags;
			}
	public Clerk(Type compensator, String description,
				 CompensatorOptions flags)
			{
				this.compensator = compensator.AssemblyQualifiedName;
				this.description = description;
				this.flags = flags;
			}

	// Destructor.
	~Clerk()
			{
				// Nothing to do here in this implementation.
			}

	// Get the number of log records.
	public int LogRecordCount
			{
				get
				{
					return count;
				}
			}

	// Get the GUI that represents the transaction unit of work.
	public String TransactionUOW
			{
				get
				{
					return transactionUOW;
				}
			}

	// Force all log records to disk.
	public void ForceLog()
			{
				// Nothing to do in this implemenation.
			}

	// Force the transaction to immediately abort.
	public void ForceTransactionToAbort()
			{
				// Nothing to do in this implemenation.
			}

	// Forget the last log record.
	public void ForgetLogRecord()
			{
				// Nothing to do in this implemenation.
			}

	// Write a log record.
	public void WriteLogRecord(Object record)
			{
				++count;
			}

}; // class Clerk

}; // namespace System.EnterpriseServices.CompensatingResourceManager
