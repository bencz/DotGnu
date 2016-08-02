/*
 * GridTableStylesCollection.cs - Implementation of "System.Windows.Forms.GridTableStylesCollection" 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004  Free Software Foundation, Inc.
 * Copyright (C) 2005  Boris Manojlovic.
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
	using System.ComponentModel;
	
	public class GridTableStylesCollection : BaseCollection
	{
		private ArrayList tableStylesCollection = new ArrayList();
		
		[TODO]
		public virtual int Add(DataGridTableStyle table)
		{
			return this.tableStylesCollection.Add(table);
		}

		[TODO]
		public virtual void AddRange(DataGridTableStyle[] tables)
		{
			throw new NotImplementedException("AddRange");
		}

		[TODO]
		public void Clear()
		{
			throw new NotImplementedException("Clear");
		}

		[TODO]
		public bool Contains(System.String name)
		{
			throw new NotImplementedException("Contains");
		}

		[TODO]
		public bool Contains(DataGridTableStyle table)
		{
			throw new NotImplementedException("Contains");
		}

		[TODO]
		protected void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			throw new NotImplementedException("OnCollectionChanged");
		}

		[TODO]
		public void Remove(DataGridTableStyle table)
		{
			throw new NotImplementedException("Remove");
		}

		[TODO]
		public void RemoveAt(int index)
		{
			throw new NotImplementedException("RemoveAt");
		}

		[TODO]
		public void add_CollectionChanged(System.ComponentModel.CollectionChangeEventHandler value)
		{
			throw new NotImplementedException("add_CollectionChanged");
		}

		[TODO]
		public void remove_CollectionChanged(System.ComponentModel.CollectionChangeEventHandler value)
		{
			throw new NotImplementedException("remove_CollectionChanged");
		}

		[TODO]
		public DataGridTableStyle this[int index] 
		{
 			get
			{
 				return (DataGridTableStyle)  this.tableStylesCollection[index];
			}

 		}

		[TODO]
		public DataGridTableStyle this[System.String tableName] 
		{
 			get
			{
				throw new NotImplementedException("Item");
			}

 		}

		[TODO]
		protected override ArrayList List 
		{
 			get
			{
 				return this.tableStylesCollection;
 			}

 		}

		public System.ComponentModel.CollectionChangeEventHandler CollectionChanged;

	}
}//namespace
