/*
 * IToolboxService.cs - Implementation of the
 *		"System.Drawing.Design.IToolboxService" class.
 *
 * Copyright (C) 2005  Deryk Robosson  <deryk@0x0a.com>
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

namespace System.Drawing.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

public interface IToolboxService
{
	// Properties
	CategoryNameCollection CategoryNames {get;}
	string SelectedCategory {get; set;}

	// Methods
	void AddCreator(ToolboxItemCreatorCallback callback, string format);
	void AddCreator(ToolboxItemCreatorCallback callback, string format, IDesignerHost host);

	void AddLinkedToolboxItem(ToolboxItem item, IDesignerHost host);
	void AddLinkedToolboxItem(ToolboxItem item, string category, IDesignerHost host);

	void AddToolboxItem(ToolboxItem item);
	void AddToolboxItem(ToolboxItem item, string category);

	ToolboxItem DeserializeToolboxItem(object obj);
	ToolboxItem DeserializeToolboxItem(object obj, IDesignerHost host);

	ToolboxItem GetSelectedToolboxItem();
	ToolboxItem GetSelectedToolboxItem(IDesignerHost host);

	ToolboxItemCollection GetToolboxItems();
	ToolboxItemCollection GetToolboxItems(IDesignerHost host);
	ToolboxItemCollection GetToolboxItems(string category);
	ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host);

	bool IsSupported(object obj, ICollection collection);
	bool IsSupported(object obj, IDesignerHost host);

	bool IsToolboxItem(object obj);
	bool IsToolboxItem(object obj, IDesignerHost host);

	void Refresh();

	void RemoveCreator(string format);
	void RemoveCreator(string format, IDesignerHost host);

	void RemoveToolboxItem(ToolboxItem item);
	void RemoveToolboxItem(ToolboxItem item, string category);

	void SelectedToolboxItemUsed();

	object SerializeToolboxItem(ToolboxItem toolboxItem);

	bool SetCursor();

	void SetSelectedToolboxItem(ToolboxItem toolboxItem);

} // interface IToolboxService
#endif
}
