/*
 * ColumnHeader.cs - Implementation of the
 *			"System.Windows.Forms.ColumnHeader" class.
 *
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

namespace System.Windows.Forms
{

using System;
using System.Reflection;
using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL
	[DefaultProperty("Text")]
	[DesignTimeVisible(false)]
	[ToolboxItem(false)]
	public class ColumnHeader : Component, ICloneable
#else
	public class ColumnHeader : ICloneable, IDisposable
#endif
	{
		internal ListView listView;
		internal int width;
		internal HorizontalAlignment textAlign;
		internal string text;
		internal int index;

		public ColumnHeader()
		{
			width = 60;
			textAlign = HorizontalAlignment.Left;
			index = -1;
		}

#if !CONFIG_COMPONENT_MODEL
		// Destuctor.
		~ColumnHeader()
		{
			Dispose(false);
		}
#endif

#if CONFIG_COMPONENT_MODEL
		[Browsable(false)]
#endif
		public ListView ListView
		{
			get
			{
				return listView;
			}
		}

#if CONFIG_COMPONENT_MODEL
		[Browsable(false)]
#endif
		public int Index
		{
			get
			{
				return index;
			}
		}

#if CONFIG_COMPONENT_MODEL
		[Localizable(true)]
		[DefaultValue(HorizontalAlignment.Left)]
#endif
		[TODO]
		public HorizontalAlignment TextAlign
		{
			get
			{
				return textAlign;
			}

			set
			{
				if (value == textAlign)
				{
					return;
				}
				if (index == 0)
				{
					textAlign = HorizontalAlignment.Left;
				}
				else
				{
					textAlign = value;
				}

				if (listView != null)
				{
					// Fix: Update ListView
				}
			}
		}

#if CONFIG_COMPONENT_MODEL
		[Localizable(true)]
		[DefaultValue(60)]
#endif
		[TODO]
		public int Width
		{
			get
			{
				return width;
			}

			set
			{
				if (value == width)
				{
					return;
				}
				width = value;
				if (listView != null)
				{
					// Fix: Set Column width
					// Fix: Update ListView
				}
			}
		}

		public virtual object Clone()
		{
			ColumnHeader columnHeader = null;
			Type type = base.GetType();
			if (type == typeof(ColumnHeader))
			{
				columnHeader = new ColumnHeader();
			}
#if !ECMA_COMPAT
			else
			{
				columnHeader = Activator.CreateInstance(type) as ColumnHeader;
			}
#else
			else
				columnHeader = type.InvokeMember
					(String.Empty, BindingFlags.CreateInstance |
								   BindingFlags.Public |
								   BindingFlags.Instance,
					 null, null, null, null, null, null) as ColumnHeader;
#endif
			columnHeader.textAlign = TextAlign;
			columnHeader.text = text;
			columnHeader.Width = width;
			return columnHeader;
		}

#if CONFIG_COMPONENT_MODEL
		[Localizable(true)]
#endif
		[TODO]
		public string Text
		{
			get
			{
				if (text == null)
				{
					return "ColumnHeader";
				}
				else
				{
					return text;
				}
			}

			set
			{
				if (value == text)
				{
					return;
				}
				if (value == null)
				{
					text = string.Empty;
				}
				else
				{
					text = value;
				}
				if (listView != null)
				{
					// Fix: Update ListView
				}
			}
		}


		public override string ToString()
		{
			return "ColumnHeader: Text: " + Text;
		}

#if !CONFIG_COMPONENT_MODEL
		// Implement the IDisposable interface.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
#endif

		protected 
#if CONFIG_COMPONENT_MODEL
		override 
#else
		virtual
#endif
		void Dispose(bool disposing)
		{
			if (disposing && listView != null)
			{
				if (index != -1)
				{
					listView.Columns.RemoveAt(index);
				}
				listView = null;
			}
#if CONFIG_COMPONENT_MODEL
			base.Dispose(disposing);
#endif
		}

	}
}
