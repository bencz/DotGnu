/*
 * Brush.cs - Implementation of the "System.Drawing.Brush" class.
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

namespace System.Drawing
{

using System.Drawing.Toolkit;

public abstract class Brush : MarshalByRefObject, ICloneable, IDisposable
{
	// Internal state.
	internal IToolkit toolkit;
	internal IToolkitBrush toolkitBrush;

	// Constructor.
	internal Brush()
			{
				toolkit = null;
				toolkitBrush = null;
			}

	// Destructor.
	~Brush()
			{
				Dispose(false);
			}

	// Clone this brush.
	public abstract Object Clone();

	// Dispose of this brush.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				lock(this)
				{
					if(toolkitBrush != null)
					{
						toolkitBrush.Dispose();
						toolkitBrush = null;
					}
				}
			}

	// Mark this brush as modified, and flush all previous brush information.
	// Used when a subclass modifies a brush's properties.
	internal void Modified()
			{
				lock(this)
				{
					if(toolkitBrush != null)
					{
						toolkitBrush.Dispose();
						toolkitBrush = null;
					}
					toolkit = null;
				}
			}

	// Get the toolkit version of this brush for a specific toolkit.
	internal IToolkitBrush GetBrush(IToolkit toolkit)
			{
				lock(this)
				{
					if(this.toolkitBrush == null)
					{
						// We don't yet have a toolkit brush yet.
						this.toolkitBrush = CreateBrush(toolkit);
						this.toolkit = toolkit;
						return this.toolkitBrush;
					}
					else if(this.toolkit == toolkit)
					{
						// Same toolkit - return the cached brush information.
						return this.toolkitBrush;
					}
					else
					{
						// We have a brush for another toolkit,
						// so dispose it and create for this toolkit.
						// We null out "toolkitBrush" before calling
						// "CreateBrush()" just in case an exception
						// is thrown while creating the toolkit brush.
						this.toolkitBrush.Dispose();
						this.toolkitBrush = null;
						this.toolkitBrush = CreateBrush(toolkit);
						this.toolkit = toolkit;
						return this.toolkitBrush;
					}
				}
			}

	// Create this brush for a specific toolkit.  Inner part of "GetBrush()".
	internal virtual IToolkitBrush CreateBrush(IToolkit toolkit)
			{
				// Normally overridden in subclasses.
				throw new InvalidOperationException();
			}

}; // class Brush

}; // namespace System.Drawing
