/*
 * ComponentEditorForm.cs - Implementation of the
 *			"System.Windows.Forms.Design.ComponentEditorForm" class.
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

	public class ComponentEditorForm : Form
	{
		[TODO]
		public ComponentEditorForm(object component, Type[] pageTypes)
		{
		}

		[TODO]
		protected override void OnActivated(EventArgs e)
		{
		}

		[TODO]
		protected override void OnHelpRequested(HelpEventArgs e)
		{
		}

		[TODO]
		protected virtual void OnSelChangeSelector(object source, TreeViewEventArgs e)
		{
		}

		public override bool PreProcessMessage(ref Message msg)
		{
			return false;
		}

		public virtual DialogResult ShowForm()
		{
			return ShowForm(null, 0);
		}

		public virtual DialogResult ShowForm(int page)
		{
			return ShowForm(null, page);
		}

		public virtual DialogResult ShowForm(IWin32Window owner)
		{
			return ShowForm(owner, 0);
		}

		[TODO]
		public virtual DialogResult ShowForm(IWin32Window owner, int page)
		{
			return 0;
		}
	}
#endif
}
