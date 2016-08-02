/*
 * OwnerDrawPropertyBag.cs - Implementation of the
 *			"System.Windows.Forms.OwnerDrawPropertyBag" class.
 *
 * Copyright (C) 2003 Neil Cawse
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
	using System.Drawing;
	using System.Runtime;
	using System.Runtime.Serialization;
	using System.Threading;

#if !ECMA_COMPAT
	[Serializable]
#endif
	public class OwnerDrawPropertyBag : MarshalByRefObject
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
	{
		private Font font;
		private Color foreColor = Color.Empty;
		private Color backColor = Color.Empty;

		public Font Font
		{
			get
			{
				return font;
			}

			set
			{
				font = value;
			}
		}

		public Color ForeColor
		{
			get
			{
				return foreColor;
			}

			set
			{
				foreColor = value;
			}
		}

		public Color BackColor
		{
			get
			{
				return backColor;
			}

			set
			{
				backColor = value;
			}
		}

#if CONFIG_SERIALIZATION

		internal OwnerDrawPropertyBag(SerializationInfo info, StreamingContext context)
		{
			foreach (SerializationEntry entry in info)
			{
				if (entry.Name == "Font")
				{
					font = entry.Value as Font;
				}
				else if (entry.Name == "BackColor")
				{
					backColor = (Color)entry.Value;
				}
				else if (entry.Name == "ForeColor")
				{
					foreColor = (Color)entry.Value;
				}
			}
		}

#endif // CONFIG_SERIALIZATION

		internal OwnerDrawPropertyBag()
		{
		}

		public virtual bool IsEmpty()
		{
			if (font == null && foreColor.IsEmpty)
			{
				return backColor.IsEmpty;
			}
			else
			{
				return false;
			}
		}

		public static OwnerDrawPropertyBag Copy(OwnerDrawPropertyBag value)
		{
			lock(typeof(OwnerDrawPropertyBag))
			{
				OwnerDrawPropertyBag newPropertyBag = new OwnerDrawPropertyBag();
				if (value != null)
				{
					newPropertyBag.backColor = value.backColor;
					newPropertyBag.foreColor = value.foreColor;
					newPropertyBag.Font = value.font;
				}
				return newPropertyBag;
			}
		}

#if CONFIG_SERIALIZATION

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("BackColor", BackColor);
			si.AddValue("ForeColor", ForeColor);
			si.AddValue("Font", Font);
		}

#endif // CONFIG_SERIALIZATION

	}

}
