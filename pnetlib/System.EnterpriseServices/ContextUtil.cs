/*
 * ContextUtil.cs - Implementation of the
 *			"System.EnterpriseServices.ContextUtil" class.
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

// Don't use this class.  It is specific to Windows 2000 systems.
// Where possible, we try to pretend that Windows 2000 doesn't
// exists so that all platforms work the same way.

public sealed class ContextUtil
{
	// Internal state.
	[ThreadStatic] private static bool deactivateOnReturn;
	[ThreadStatic] private static bool consistent;

	// Cannot instantiate this class.
	private ContextUtil() {}

	// Get or set the context properties.
#if !ECMA_COMPAT
	public static Guid ActivityId
			{
				get
				{
					throw new PlatformNotSupportedException();
				}
			}
	public static Guid ApplicationId
			{
				get
				{
					throw new PlatformNotSupportedException();
				}
			}
	public static Guid ApplicationInstanceId
			{
				get
				{
					throw new PlatformNotSupportedException();
				}
			}
	public static Guid ContextId
			{
				get
				{
					throw new PlatformNotSupportedException();
				}
			}
	public static Guid PartitionId
			{
				get
				{
					throw new PlatformNotSupportedException();
				}
			}
	public static Guid TransactionId
			{
				get
				{
					throw new PlatformNotSupportedException();
				}
			}
#endif
	public static bool DeactivateOnReturn
			{
				get
				{
					return deactivateOnReturn;
				}
				set
				{
					deactivateOnReturn = value;
				}
			}
	public static bool IsInTransaction
			{
				get
				{
					return false;
				}
			}
	public static bool IsSecurityEnabled
			{
				get
				{
					return false;
				}
			}
	public static TransactionVote MyTransactionVote
			{
				get
				{
					if(consistent)
					{
						return TransactionVote.Commit;
					}
					else
					{
						return TransactionVote.Abort;
					}
				}
				set
				{
					consistent = (value == TransactionVote.Commit);
				}
			}
	public static Object Transaction
			{
				get
				{
				#if !ECMA_COMPAT
					throw new PlatformNotSupportedException();
				#else
					throw new NotSupportedException();
				#endif
				}
			}

	// Enable or disable commits.
	public static void EnableCommit()
			{
				consistent = true;
				deactivateOnReturn = false;
			}
	public static void DisableCommit()
			{
				consistent = false;
				deactivateOnReturn = false;
			}

	// Get a named property from the context.
	public static Object GetNamedProperty(String name)
			{
			#if !ECMA_COMPAT
				throw new PlatformNotSupportedException();
			#else
				throw new NotSupportedException();
			#endif
			}

	// Determine if the caller is within a specified role.
	public static bool IsCallerInRole(String role)
			{
				return SecurityCallContext.CurrentCall.IsCallerInRole(role);
			}

	// Abort the current transaction.
	public static void SetAbort()
			{
				consistent = false;
				deactivateOnReturn = true;
			}

	// Mark the current transaction as complete.
	public static void SetComplete()
			{
				consistent = true;
				deactivateOnReturn = true;
			}

}; // class ContextUtil

}; // namespace System.EnterpriseServices
