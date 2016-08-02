/*
 * Panel.cs - Implementation of "System.Windows.Forms.Panel" class
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Neil Cawse.
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
using System.Drawing;

namespace System.Windows.Forms
{
	public class Panel: ScrollableControl
	{
		public Panel() : base()
		{
			TabStop = false;
			SetStyle(ControlStyles.AllPaintingInWmPaint /*| ControlStyles.Selectable*/, false);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.Selectable, false);
		}

		public BorderStyle BorderStyle 
		{
 			get
			{
				return BorderStyleInternal;
			}
 			set
			{
				BorderStyleInternal = value;
			}
 		}

		protected override Size DefaultSize 
		{
 			get
			{
				return new Size(200,100);
			}

 		}

		public override string ToString()
		{
			return base.ToString() + ", BorderStyle: "+ BorderStyleInternal.ToString();
		}

	}
}//namespace
