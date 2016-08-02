/*
 * IVsaScriptScope.cs - Access information within a scripting scope.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using Microsoft.Vsa;
using Microsoft.JScript.Vsa;

public interface IVsaScriptScope : IVsaItem
{

	IVsaItem AddItem(String itemName, VsaItemType type);
	IVsaItem AddItem(String itemName);
	IVsaItem GetDynamicItem(String itemName, VsaItemType type);
	IVsaItem GetItemAtIndex(int index);
	int GetItemCount();
	IVsaScriptScope Parent { get; }
	void RemoveItem(String itemName);
	void RemoveItem(IVsaItem item);
	void RemoveItemAtIndex(int index);

}; // interface IVsaScriptScope

}; // namespace Microsoft.JScript
