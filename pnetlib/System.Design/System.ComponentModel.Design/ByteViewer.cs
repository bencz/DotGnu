/*
 * ByteViewer.cs - Implementation of "System.ComponentModel.Design.ByteViewer" class 
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * 
 * Contributions by Adam Ballai <Adam@TheFrontNetworks.net>
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


namespace System.ComponentModel.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN
	
using System.Windows.Forms;

public class ByteViewer : Control
{
	public ByteViewer()
			{
			}

	public override ISite Site 
			{
				get 
				{
				       throw new NotImplementedException(); 
				} 
				set 
				{ 
					throw new NotImplementedException(); 
				}
			}

	public virtual DisplayMode GetDisplayMode()
			{
				throw new NotImplementedException();
			}

	public virtual void SaveToFile (String path)
			{
				throw new NotImplementedException();
			}

	public virtual void SetBytes (byte[] bytes)
			{
				throw new NotImplementedException();
			}

	public virtual void SetDisplayMode (DisplayMode mode)
			{
				throw new NotImplementedException();
			}

	public virtual void SetFile (String path)
			{
				throw new NotImplementedException();
			}

	public virtual void SetStartLine (int line)
			{
				throw new NotImplementedException();
			}

	protected override void OnKeyDown (KeyEventArgs e)
			{
				throw new NotImplementedException();
			}

	protected override void OnPaint (PaintEventArgs e)
			{
				throw new NotImplementedException();
			}

	protected override void OnResize (EventArgs e)
			{
				throw new NotImplementedException();
			}

	~ByteViewer()
			{
			}
	
} // class ByteViewer

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.ComponentModel.Design
