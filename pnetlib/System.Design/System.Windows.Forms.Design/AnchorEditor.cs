/*
 * AnchorEditor.cs - Implementation of "System.Windows.Forms.Design.AnchorEditor" class 
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

namespace System.Windows.Forms.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN	
	
using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.Remoting;
public sealed class AnchorEditor : UITypeEditor 
{
	public AnchorEditor () 
			{
			}

	// Edits the value of the specified object using the specified service provider and context. 
	public override Object EditValue (ITypeDescriptorContext context, IServiceProvider provider, Object value)
			{
				throw new NotImplementedException ();
			}
	
	// Gets the editor style used by the EditValue method.
	public override UITypeEditorEditStyle GetEditStyle (ITypeDescriptorContext context)
			{
				throw new NotImplementedException ();
			}

} // class AnchorEditor

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
