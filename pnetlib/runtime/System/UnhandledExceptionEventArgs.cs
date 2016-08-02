/*
 * UnhandledExceptionEventArgs.cs - Implementation of the
 *			"System.UnhandledExceptionEventArgs" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System
{

#if CONFIG_RUNTIME_INFRA

public class UnhandledExceptionEventArgs : EventArgs
{

	// Internal state.
	private Object exception;
	private bool   isTerminating;

	// Constructor.
	public UnhandledExceptionEventArgs(Object exception, bool isTerminating)
			{
				this.exception = exception;
				this.isTerminating = isTerminating;
			}

	// Get the exception object.
	public Object ExceptionObject
			{
				get
				{
					return exception;
				}
			}

	// Get the "isTerminating" flag.
	public bool IsTerminating
			{
				get
				{
					return isTerminating;
				}
			}

}; // class UnhandledExceptionEventArgs

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
