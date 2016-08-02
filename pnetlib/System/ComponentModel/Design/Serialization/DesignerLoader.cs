/*
 * DesignerLoader.cs - Implementation of the
 *		"System.ComponentModel.Design.Serialization.DesignerLoader" class.
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

namespace System.ComponentModel.Design.Serialization
{

#if CONFIG_COMPONENT_MODEL_DESIGN

public abstract class DesignerLoader
{
	// Constructor.
	protected DesignerLoader() {}

	// Determine if we are currently loading.
	public virtual bool Loading
			{
				get
				{
					// Nothing to do here in the base class.
					return false;
				}
			}

	// Begin loading a designer.
	public abstract void BeginLoad(IDesignerLoaderHost host);

	// Dispose of the loader.
	public abstract void Dispose();

	// Flush any changes that were made.
	public virtual void Flush()
			{
				// Nothing to do here in the base class.
			}

}; // class DesignerLoader

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design.Serialization
