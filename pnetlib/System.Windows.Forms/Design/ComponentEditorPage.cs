/*
 * ComponentEditorPage.cs - Implementation of the
 *			"System.Windows.Forms.Design.ComponentEditorPage" class.
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
using System.Drawing;

	public abstract class ComponentEditorPage : Panel
	{
		private Icon icon;
		private IComponentEditorPageSite pageSite;
		private IComponent component;
		private bool firstActivate;
		private bool loadRequired;
		private int loading = 0;
		private bool commitOnDeactivate;

		protected IComponentEditorPageSite PageSite
		{
			get
			{
				return pageSite;
			}

			set
			{
				pageSite = value;
			}
		}

		protected IComponent Component
		{
			get
			{
				return component;
			}

			set
			{
				component = value;
			}
		}

		protected bool FirstActivate
		{
			get
			{
				return firstActivate;
			}

			set
			{
				firstActivate = value;
			}
		}

		protected bool LoadRequired
		{
			get
			{
				return loadRequired;
			}

			set
			{
				loadRequired = value;
			}
		}

		protected int Loading
		{
			get
			{
				return loading;
			}

			set
			{
				loading = value;
			}
		}

		public bool CommitOnDeactivate
		{
			get
			{
				return commitOnDeactivate;
			}

			set
			{
				commitOnDeactivate = value;
			}
		}

		public Icon Icon
		{
			get
			{
				return icon;
			}

			set
			{
				icon = value;
			}
		}

		public virtual string Title
		{
			get
			{
				return base.Text;
			}
		}

		public ComponentEditorPage()
		{
			firstActivate = true;
			Visible = false;
		}

		public virtual void Activate()
		{
			if (loadRequired)
			{
				EnterLoadingMode();
				LoadComponent();
				ExitLoadingMode();
				loadRequired = false;
			}
			Visible = true;
			firstActivate = false;
		}

		public virtual void ApplyChanges()
		{
			SaveComponent();
		}

		public virtual void Deactivate()
		{
			base.Visible = false;
		}

		protected void EnterLoadingMode()
		{
			loading++;
		}

		protected void ExitLoadingMode()
		{
			loading--;
		}

		public virtual Control GetControl()
		{
			return this;
		}

		protected IComponent GetSelectedComponent()
		{
			return component;
		}

		public virtual bool IsPageMessage(ref Message msg)
		{
			return base.PreProcessMessage(ref msg);
		}

		protected bool IsFirstActivate()
		{
			return firstActivate;
		}

		protected bool IsLoading()
		{
			return loading != 0;
		}

		protected abstract void LoadComponent();

		public virtual void OnApplyComplete()
		{
			ReloadComponent();
		}

		protected virtual void ReloadComponent()
		{
			if (!Visible)
				loadRequired = true;
		}

		protected abstract void SaveComponent();

		protected virtual void SetDirty()
		{
			if (!IsLoading())
				pageSite.SetDirty();
		}

		public virtual void SetComponent(IComponent component)
		{
			loadRequired = true;
			this.component = component;
		}

		public virtual void SetSite(IComponentEditorPageSite site)
		{
			pageSite = site;
			pageSite.GetControl().Controls.Add(this);
		}

		public virtual void ShowHelp()
		{
		}

		public virtual bool SupportsHelp()
		{
			return false;
		}
	}
#endif
}
