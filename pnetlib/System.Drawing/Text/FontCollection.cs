/*
 * FontCollection.cs - Implementation of the
 *			"System.Drawing.Text.FontCollection" class.
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

namespace System.Drawing.Text
{

using System.Collections;

public abstract class FontCollection : IDisposable
{
	// Internal state.
	private ArrayList families;

	// Constructor.
	internal FontCollection()
			{
				families = new ArrayList();
			}

	// Destructor.
	~FontCollection()
			{
				Dispose(false);
			}

	// Add an element to this collection.
	internal void Add(FontFamily family)
			{
				families.Add(family);
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				// Nothing to do here.
			}

	// Get the font families.
	public FontFamily[] Families
			{
				get
				{
					return (FontFamily[])
						(families.ToArray(typeof(FontFamily)));
				}
			}

}; // class FontCollection

}; // namespace System.Drawing.Text
