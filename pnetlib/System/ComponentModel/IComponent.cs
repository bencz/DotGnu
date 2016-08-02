/*
 * IComponent.cs - Implementation of the
 *		"System.ComponentModel.IComponent" interface.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;

[ComVisible(true)]
[TypeConverter(typeof(ComponentConverter))]
#if CONFIG_COMPONENT_MODEL_DESIGN
[Designer
	("System.Windows.Forms.Design.ComponentDocumentDesigner, System.Design",
	 typeof(IRootDesigner))]
[Designer
	("System.ComponentModel.Design.ComponentDesigner, System.Design",
	 typeof(IDesigner))]
[RootDesignerSerializer
	("System.ComponentModel.Design.Serialization.RootCodeDomSerializer, System.Design",
	 "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design", true)]
#endif
public interface IComponent : IDisposable
{

	// Get or set the site associated with this component.
	ISite Site { get; set; }

	// Event that is raised when a component is disposed.
	event EventHandler Disposed;

}; // interface IComponent

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
