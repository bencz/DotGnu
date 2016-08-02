/*
 * partial4.cs - Test partial class declarations.
 *
 * Copyright (C) 2009  Southern Storm Software, Pty Ltd.
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

namespace Test
{
	public partial class Test1
	{
	}

	partial class Test2
	{
	}

	partial class Test3
	{
		partial class Test3Nested
		{
		}
	}

	public partial class Test4
	{
	}

	internal partial class Test5
	{
	}

	partial class Test6
	{
		internal partial class Test6Nested
		{
		}
	}

	partial class Test7
	{
	}

}

namespace Test
{
	public static partial class Test1
	{
	}

	internal partial class Test2
	{
	}

	internal partial class Test3
	{
		private partial class Test3Nested
		{
		}
	}

	partial class Test4
	{
	}

	partial class Test5
	{
	}

	partial class Test6
	{
		partial class Test6Nested
		{
		}
	}

	abstract partial class Test7
	{
		protected internal class Test7Nested
		{
		}
	}
}