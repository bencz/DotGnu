/*
 * WindowsFormsComponentEditor.cs - Implementation of the
 *			"System.Windows.Forms.Design.WindowsFormsComponentEditor" class.
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

namespace System.Windows.Forms.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN

using System;
using System.ComponentModel;
using System.Windows.Forms;

	public abstract class WindowsFormsComponentEditor : ComponentEditor
	{

		protected virtual Type[] GetComponentEditorPages()
		{
			return null;
		}

		protected virtual int GetInitialComponentEditorPageIndex()
		{
			return 0;
		}

		public override bool EditComponent(ITypeDescriptorContext context, object component)
		{
			return EditComponent(context, component, null);
		}

		public bool EditComponent(object component, IWin32Window owner)
		{
			return EditComponent(null, component, owner);
		}

		public virtual bool EditComponent(ITypeDescriptorContext context, object component, IWin32Window owner)
		{
			Type[] types = GetComponentEditorPages();
			if (types != null && types.Length > 0 && new ComponentEditorForm(component, types).ShowForm(owner, GetInitialComponentEditorPageIndex()) == DialogResult.OK)
				return true;
			else
				return false;
		}
	}
#endif
}
