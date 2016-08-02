/*
 * ErrorProvider.cs - Implementation of "ErrorProvider" class
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 * 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR   See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, , 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms
{
#if CONFIG_COMPONENT_MODEL
	public class ErrorProvider : Component, IExtenderProvider 
	{
		[TODO]
		public ErrorProvider()
		{
			throw new NotImplementedException("ctor");
		}

		[TODO]
		public ErrorProvider(ContainerControl parentControl)
		{
			throw new NotImplementedException("ctor");
		}

		[TODO]
		public void BindToDataAndErrors(Object newDataSource, String newDataMember)
		{
			throw new NotImplementedException("BindToDataAndErrors");
		}

		[TODO]
		public virtual bool CanExtend(Object extendee)
		{
			throw new NotImplementedException("CanExtend");
		}

		[TODO]
		protected override void Dispose(bool disposing)
		{
			throw new NotImplementedException("Dispose");
		}

		[TODO]
		public String GetError(Control control)
		{
			throw new NotImplementedException("GetError");
		}

		[TODO]
		public ErrorIconAlignment GetIconAlignment(Control control)
		{
			throw new NotImplementedException("GetIconAlignment");
		}

		[TODO]
		public int GetIconPadding(Control control)
		{
			throw new NotImplementedException("GetIconPadding");
		}

		[TODO]
		public void SetError(Control control, String value)
		{
			throw new NotImplementedException("SetError");
		}

		[TODO]
		public void SetIconAlignment(Control control, ErrorIconAlignment value)
		{
			throw new NotImplementedException("SetIconAlignment");
		}

		[TODO]
		public void SetIconPadding(Control control, int padding)
		{
			throw new NotImplementedException("SetIconPadding");
		}

		[TODO]
		public void UpdateBinding()
		{
			throw new NotImplementedException("UpdateBinding");
		}

		[TODO]
		public int BlinkRate 
		{
 			get
			{
				throw new NotImplementedException("BlinkRate");
			}

 			set
			{
				throw new NotImplementedException("BlinkRate");
			}

 		}

		[TODO]
		public ErrorBlinkStyle BlinkStyle 
		{
 			get
			{
				throw new NotImplementedException("BlinkStyle");
			}

 			set
			{
				throw new NotImplementedException("BlinkStyle");
			}

 		}

		[TODO]
		public ContainerControl ContainerControl 
		{
 			get
			{
				throw new NotImplementedException("ContainerControl");
			}

 			set
			{
				throw new NotImplementedException("ContainerControl");
			}

 		}

		[TODO]
		public String DataMember 
		{
 			get
			{
				throw new NotImplementedException("DataMember");
			}

 			set
			{
				throw new NotImplementedException("DataMember");
			}

 		}

		[TODO]
		public Object DataSource 
		{
 			get
			{
				throw new NotImplementedException("DataSource");
			}

 			set
			{
				throw new NotImplementedException("DataSource");
			}

 		}

		[TODO]
		public Icon Icon 
		{
 			get
			{
				throw new NotImplementedException("Icon");
			}

 			set
			{
				throw new NotImplementedException("Icon");
			}

 		}

		[TODO]
		public override ISite Site 
		{
 			set
			{
				throw new NotImplementedException("Site");
			}

 		}

	}
#endif // CONFIG_COMPONENT_MODEL
}//namespace
