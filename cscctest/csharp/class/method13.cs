/*
 * method13.cs - Test "base" member access.
 *
 * Copyright (C) 2002 Free Software Foundation
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

/* Thanks to Jeff Post <j_post@pacbell.net> for this test case */

using System;

namespace INHERIT
{
   class A
   {
      public int i = 0;

      public void show()
      {
         //Console.WriteLine("i in base class: " + i);
      }
   }

   class B:A
   {
      new int i;

      public B(int a, int b)
      {
         base.i = a;
         i = b;
      }

      new public void show()
      {
         base.show();
         //Console.WriteLine("i in derived class: " + i);
      }
   }

   class UncoverName
   {
      public static void Main()
      {
         B ob = new B(1, 2);

         ob.show();
      }
   }
}


