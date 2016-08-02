/*
 * IUIService.cs - Implementation of the
 *		"System.Windows.Forms.IUIService" class.
 *
 * Copyright (C) 2003  Neil Cawse, Pty Ltd.
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
using System.Collections;
using System.Windows.Forms;

	public interface IUIService
	{

		IDictionary Styles
		{
			get;
		}

		bool CanShowComponentEditor(object component);

		IWin32Window GetDialogOwnerWindow();

		void SetUIDirty();

		bool ShowComponentEditor(object component, IWin32Window parent);

		DialogResult ShowDialog(Form form);

		void ShowMessage(string message);

		void ShowMessage(string message, string caption);

		DialogResult ShowMessage(string message, string caption, MessageBoxButtons buttons);

		bool ShowToolWindow(Guid toolWindow);

		void ShowError(string message);

		void ShowError(Exception ex);

		void ShowError(Exception ex, string message);
	}
#endif
}
