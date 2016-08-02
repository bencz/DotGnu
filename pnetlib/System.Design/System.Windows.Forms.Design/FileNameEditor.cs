/*
 * FileNameEditor.cs - Implementation of "System.Windows.Forms.Design.FileNameEditor" class 
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;

public class FileNameEditor : UITypeEditor
{
	public FileNameEditor()
			{
				// TODO
			}

	public override Object EditValue(ITypeDescriptorContext context,
					IServiceProvider provider,
					Object Value)
			{
				// TODO
				return null;
			}

	public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			{
				// TODO
				return new UITypeEditorEditStyle();
			}

	protected virtual void InitializeDialog(OpenFileDialog openFileDialog)
			{
				// TODO
			}

} // class FileNameEditor

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
