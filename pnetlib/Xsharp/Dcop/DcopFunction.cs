/*
 * DcopFunction.cs - DCOP function name parser.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace Xsharp.Dcop
{

using System;

public class DcopFunction
{

	private string name;
	private string[] paramList;
	private string returnValue;
	private string function;

	// `<QType> name([<QType>[,<QType>[,<QType>]...]])'
	public DcopFunction(string input)
	{
		int spaceOffset; // Space divides return val and the rest
		int firstBracketOffset; // First bracket offset
		string withoutReturn;
		string types;

		if(input == null)
		{
			throw new ArgumentNullException("input", "Argument cannot be null");
		}

		spaceOffset = input.IndexOf(' ');
		if(spaceOffset < 1)
		{
			throw new DcopNamingException("DCOP function should have return type and name");
		}
		returnValue = input.Substring(0, spaceOffset);
		withoutReturn = input.Substring(spaceOffset + 1); // Without return value type

		firstBracketOffset = withoutReturn.IndexOf('(');
		if(firstBracketOffset < 1)
		{
			throw new DcopNamingException("DCOP function should have name and argument list in brackets");
		}
		name = withoutReturn.Substring(0, firstBracketOffset); // Function name

		if(withoutReturn[withoutReturn.Length - 1] != ')')
		{
			throw new DcopNamingException("Argument list after function name should end with bracket");
		}

		// Get parameters and check for validity
		types = withoutReturn.Substring(firstBracketOffset + 1, withoutReturn.Length - (firstBracketOffset + 2)); // Types list
		if(types.Length == 0)
		{
			paramList = new string[0];
		}
		else
		{
			paramList = types.Split(','); 
			for (int i = 0; i < paramList.Length; i++)
			{
				if(paramList[i].Length == 0)
				{
					throw new DcopNamingException("Type names should be at least 1 character long");
				}
				spaceOffset = paramList[i].IndexOf(' ');
				if(spaceOffset >= 1)
				{
					paramList[i] = paramList[i].Substring(0, spaceOffset).Trim(); //Just to be sure. CAVEAT: This may return garbage.
				}
			}
		}

		function = name + '(' + String.Join(",", paramList) + ')'; // All is well		
	}

	public string Function
	{
		get
		{
			return function;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
	}

	public string ReturnValue
	{
		get
		{
			return returnValue;
		}
	}

	public string this[int key]
	{
		get
		{
			return paramList[key];
		}
	}

	public int Length
	{
		get
		{
			return paramList.Length;
		}
	}

	public override string ToString()
	{
		return String.Format("Dcop function `{0}', returning `{1}', with parameters `{2}'", name, returnValue, String.Join(", ", paramList));
	}

}; // Class DcopFunction

} // Namespace Xsharp.Dcop

