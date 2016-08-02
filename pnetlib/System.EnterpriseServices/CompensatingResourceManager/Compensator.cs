/*
 * Compensator.cs - Implementation of the
 *			"System.EnterpriseServices.CompensatingResourceManager."
 *			"Compensator" class.
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

public class Compensator : ServicedComponent
{
	// Internal state.
	private Clerk clerk;

	// Constructor.
	public Compensator() {}

	// Get the clerk associated with this compensator.
	public Clerk Clerk
			{
				get
				{
					return clerk;
				}
			}

	// Send a log record during the abort phase.
	public virtual bool AbortRecord(LogRecord rec)
			{
				// Nothing to do in the base class.
				return false;
			}

	// Begin the abort phase.
	public virtual void BeginAbort(bool fRecovery)
			{
				// Nothing to do in the base class.
			}

	// Begin the commit phase.
	public virtual void BeginCommit(bool fRecovery)
			{
				// Nothing to do in the base class.
			}

	// Begin the prepare phase.
	public virtual void BeginPrepare()
			{
				// Nothing to do in the base class.
			}

	// Send a log record during the commit phase.
	public virtual bool CommitRecord(LogRecord rec)
			{
				// Nothing to do in the base class.
				return false;
			}

	// End the abort phase.
	public virtual void EndAbort()
			{
				// Nothing to do in the base class.
			}

	// End the commit phase.
	public virtual void EndCommit()
			{
				// Nothing to do in the base class.
			}

	// End the prepare phase.
	public virtual bool EndPrepare()
			{
				// Nothing to do in the base class.
				return true;
			}

	// Send a log record during the prepare phase.
	public virtual bool PrepareRecord(LogRecord rec)
			{
				// Nothing to do in the base class.
				return false;
			}

}; // class Compensator

}; // namespace System.EnterpriseServices.CompensatingResourceManager
