/*
 * indexer1.cs - Test indexer declarations and access
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

using System;
public class TestIndexer
{
	public int this[ int x, params object[] args]
	{
		get 
		{ 
			return x; 
		}
		set
		{
		}
	}
	public void test()
	{
		this[1,2,3,4,5,6,7,8,9]=42;
		int x=this[12,22,34,45];
	}
}
