/*
 * IMenuCommandService.cs - Implementation of the
 *		"System.ComponentModel.Design.IMenuCommandService" class.
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

using System.Runtime.InteropServices;

[ComVisible(true)]
public interface IMenuCommandService
{
	// Get an array of designer verbs that are currently available.
	DesignerVerbCollection Verbs { get; }

	// Add a command to the menu.
	void AddCommand(MenuCommand command);

	// Add a verb to the set of all designer verbs.
	void AddVerb(DesignerVerb verb);

	// Find a specific menu command by identifier.
	MenuCommand FindCommand(CommandID commandID);

	// Invoke a menu or verb command indicated by a specific command ID.
	bool GlobalInvoke(CommandID commandID);

	// Remove a command from the menu.
	void RemoveCommand(MenuCommand command);

	// Remove a verb from the set of all designer verbs.
	void RemoveVerb(DesignerVerb verb);

	// Show the context menu at a specific location.
	void ShowContextMenu(CommandID menuID, int x, int y);

}; // interface IMenuCommandService

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
