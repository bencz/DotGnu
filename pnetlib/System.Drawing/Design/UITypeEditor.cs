/*
 * UITypeEditor.cs - Implementation of the
 *		"System.Drawing.Design.UITypeEditor" class.
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

namespace System.Drawing.Design
{
#if CONFIG_COMPONENT_MODEL_DESIGN

using System;
using System.ComponentModel;
using System.Collections;
using System.IO;

public class UITypeEditor
{
	static UITypeEditor()
	{
		Hashtable hashtable = new Hashtable();
		hashtable[typeof(Stream)] = "System.ComponentModel.Design.BinaryEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		hashtable[typeof(DateTime)] = "System.ComponentModel.Design.DateTimeEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		hashtable[typeof(Array)] = "System.ComponentModel.Design.ArrayEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		hashtable[typeof(ICollection)] = "System.ComponentModel.Design.CollectionEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		hashtable[typeof(byte[])] = "System.ComponentModel.Design.BinaryEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		hashtable[typeof(string[])] = "System.Windows.Forms.Design.StringArrayEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		TypeDescriptor.AddEditorTable(typeof(UITypeEditor), hashtable);
		hashtable[typeof(IList)] = "System.ComponentModel.Design.CollectionEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
	}

	public object EditValue(IServiceProvider provider, object value)
	{
		return EditValue(null, provider, value);
	}

	public virtual object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
	{
		return value;
	}

	public UITypeEditorEditStyle GetEditStyle()
	{
		return GetEditStyle(null);
	}

	public bool GetPaintValueSupported()
	{
		return GetPaintValueSupported(null);
	}

	public virtual bool GetPaintValueSupported(ITypeDescriptorContext context)
	{
		return false;
	}

	public virtual UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
	{
		return UITypeEditorEditStyle.None;
	}

	public void PaintValue(object value, Graphics canvas, Rectangle rectangle)
	{
		PaintValue(new PaintValueEventArgs(null, value, canvas, rectangle));
	}

	public virtual void PaintValue(PaintValueEventArgs e)
	{
	}

}; // class UITypeEditor
#endif
}; // namespace System.Drawing.Design
