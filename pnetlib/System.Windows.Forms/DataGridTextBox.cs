/*
 * DataGridTextBox.cs - Implementation of "System.Windows.Forms.DataGridTextBox" 
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
	public class DataGridTextBox : TextBox
	{
		[TODO]
		public DataGridTextBox()
		{
			
		}

		[TODO]
		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			throw new NotImplementedException("OnKeyPress");
		}

		[TODO]
		protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			throw new NotImplementedException("OnMouseWheel");
		}

		[TODO]
		protected internal override bool ProcessKeyMessage(ref Message m)
		{
			throw new NotImplementedException("ProcessKeyMessage");
		}

		[TODO]
		public void SetDataGrid(System.Windows.Forms.DataGrid parentGrid)
		{
			throw new NotImplementedException("SetDataGrid");
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			// as I read docs ... is this really used at all in this impl...
			base.WndProc (ref m);
		}

		[TODO]
		public bool IsInEditOrNavigateMode 
		{
 			get
			{
				throw new NotImplementedException("IsInEditOrNavigateMode");
			}

 			set
			{
				throw new NotImplementedException("IsInEditOrNavigateMode");
			}

 		}

	}
}//namespace
