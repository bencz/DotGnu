/*
 * XmlNameTable.cs - Implementation of the "System.Xml.XmlNameTable" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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
 
namespace System.Xml
{

public abstract class XmlNameTable
{
	// Constructor.
	protected XmlNameTable() {}
	
	// Add a string to the table if it doesn't already exist.
	public abstract String Add(String array);
	
	// Add a string to the table from an array.
	public abstract String Add(char[] array, int offset, int length);
	
	// Get a string from the table by name.
	public abstract String Get(String array);
	
	// Get a string from the table by array name.
	public abstract String Get(char[] array, int offset, int length);

}; // class XmlNameTable

}; // namespace System.Xml
