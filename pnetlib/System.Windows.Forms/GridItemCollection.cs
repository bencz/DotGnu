/*
 * GridItemCollection.cs - Implementation of the
 *			"System.Windows.Forms.GridItemCollection" class.
 *
 * Copyright (C) 2003  Neil Cawse, Pty Ltd.
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

namespace System.Windows.Forms
{
	using System;
	using System.Collections;

	public class GridItemCollection: ICollection
	{
		private static GridItemCollection empty;
		internal GridItem[] entries;

		internal GridItemCollection(GridItem[] entries)
		{
			if (entries == null)
				this.entries = new GridItem[0];
			else
				this.entries = entries;
		}

		public static GridItemCollection Empty
		{
			get
			{
				if (empty == null)
					empty = new GridItemCollection(new GridItem[0]);
				return empty;
			}
		}

		public virtual int Count
		{
			get
			{
				return entries.Length;
			}
		}

		public GridItem this[int index]
		{
			get
			{
				return entries[index];
			}
		}

		public GridItem this[string label]
		{
			get
			{
				for (int i = 0; i < entries.Length; i++)
					if (entries[i].Label == label)
						return entries[i];
				return null;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			Array.Copy(entries, 0, dest, index, entries.Length);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return entries.GetEnumerator();
		}
	}

}
