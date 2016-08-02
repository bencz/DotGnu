/*
 * ISynchronizeInvoke.cs - Implementation of the
 *			"System.ComponentModel.ISynchronizeInvoke" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

public interface ISynchronizeInvoke
{
	// Determine if a caller should use "Invoke" on this object.
	bool InvokeRequired { get; }

	// Invoke a delegate on the thread that this object belongs to.
	IAsyncResult BeginInvoke(Delegate method, Object[] args);

	// End a previous invocation and return the results.
	Object EndInvoke(IAsyncResult result);

	// Invoke a delegate on the thread that this object belongs to
	// and wait for the method to complete.
	Object Invoke(Delegate method, Object[] args);

}; // interface ISynchronizeInvoke

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
