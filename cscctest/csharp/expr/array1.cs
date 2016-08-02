/*
 * array1.cs - Test the valid cases of array indexing.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

class Test
{
	// Test various l-value combinations on 1-D arrays.
	void m1_int(int[] x, int y)
	{
		int v = x[y];
		x[y] = v;
		x[y] += v;
		v = ++(x[y]);
		(x[y])--;
		v = (x[y])--;
	}
	void m1_byte(byte[] x, int y)
	{
		byte v = x[y];
		x[y] = v;
		//x[y] += v;
		v = ++(x[y]);
		(x[y])--;
		v = (x[y])--;
	}
	void m1_long(long[] x, int y)
	{
		long v = x[y];
		x[y] = v;
		x[y] += v;
		v = ++(x[y]);
		(x[y])--;
		v = (x[y])--;
	}

	// Test array accesses on 1-D arrays of other types.
	decimal m1_decimal(decimal[] x, int y)
	{
		x[y] = 2.0m;
		x[y] += 2.0m;
		return x[y];
	}
	string m1_string(string[] x, int y)
	{
		x[y] = "Hello World";
		return x[y];
	}

	// Test various l-value combinations on 2-D arrays.
	void m2_int(int[,] x, int y, int z)
	{
		int v = x[y, z];
		x[y, z] = v;
		x[y, z] += v;
		v = ++(x[y, z]);
		(x[y, z])--;
		v = (x[y, z])--;
	}
	void m2_byte(byte[,] x, int y, int z)
	{
		byte v = x[y, z];
		x[y, z] = v;
		//x[y, z] += v;
		v = ++(x[y, z]);
		(x[y, z])--;
		v = (x[y, z])--;
	}
	void m2_long(long[,] x, int y, int z)
	{
		long v = x[y, z];
		x[y, z] = v;
		x[y, z] += v;
		v = ++(x[y, z]);
		(x[y, z])--;
		v = (x[y, z])--;
	}

	// Test array accesses on 2-D arrays of other types.
	decimal m2_decimal(decimal[,] x, int y, int z)
	{
		x[y, z] = 2.0m;
		x[y, z] += 2.0m;
		return x[y, z];
	}
	string m2_string(string[,] x, int y, int z)
	{
		x[y, z] = "Hello World";
		return x[y, z];
	}

	// Test array accesses with index types other than int.
	int m3_uint(int[] x, uint y)
	{
		return x[y + 1U];
	}
	int m3_long(int[] x, long y)
	{
		return x[y + 1L];
	}
	int m3_ulong(int[] x, ulong y)
	{
		return x[y + 1UL];
	}
	int m3_uint(int[] x, ushort y)
	{
		return x[y];
	}
	int m3_c_uint(int[] x, uint y)
	{
		checked
		{
			return x[y + 1U];
		}
	}
	int m3_c_long(int[] x, long y)
	{
		checked
		{
			return x[y + 1L];
		}
	}
	int m3_c_ulong(int[] x, ulong y)
	{
		checked
		{
			return x[y + 1UL];
		}
	}
	int m3_c_uint(int[] x, ushort y)
	{
		checked
		{
			return x[y];
		}
	}
}
