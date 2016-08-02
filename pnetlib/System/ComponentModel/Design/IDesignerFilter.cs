/*
 * IDesignerFilter.cs - Implementation of the
 *		"System.ComponentModel.Design.IDesignerFilter" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

using System.Collections;

public interface IDesignerFilter
{
	// Filter a set of attributes before they are used.
	void PreFilterAttributes(IDictionary attributes);

	// Filter a set of attributes after they are used.
	void PostFilterAttributes(IDictionary attributes);

	// Filter a set of events before they are used.
	void PreFilterEvents(IDictionary events);

	// Filter a set of events after they are used.
	void PostFilterEvents(IDictionary events);

	// Filter a set of properties before they are used.
	void PreFilterProperties(IDictionary properties);

	// Filter a set of properties after they are used.
	void PostFilterProperties(IDictionary properties);

}; // interface IDesignerFilter

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
