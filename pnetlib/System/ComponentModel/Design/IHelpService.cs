/*
 * IHelpService.cs - Implementation of the
 *		"System.ComponentModel.Design.IHelpService" class.
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

public interface IHelpService
{
	// Add a context attribute to the document.
	void AddContextAttribute
			(String name, String value, HelpKeywordType keywordType);

	// Clear all context attributes from the document.
	void ClearContextAttributes();

	// Create a local context to manage subcontexts.
	IHelpService CreateLocalContext(HelpContextType contextType);

	// Remove a context attribute
	void RemoveContextAttribute(String name, String value);

	// Remove a local context.
	void RemoveLocalContext(IHelpService localContext);

	// Show the help topic for a specific keyword.
	void ShowHelpFromKeyword(String helpKeyword);

	// Show the help topic that corresponds to a specified URL.
	void ShowHelpFromUrl(String helpUrl);

}; // interface IHelpService

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
