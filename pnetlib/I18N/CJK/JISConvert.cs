/*
 * JISConvert.cs - Implementation of the "System.Text.JISConvert" class.
 *
 * Copyright (c) 2002  Southern Storm Software, Pty Ltd
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

namespace I18N.CJK
{

using System;

// This class assists other encoding classes in converting back
// and forth between JIS character sets and Unicode.  It uses
// several large tables to do this, some of which are stored in
// the resource section of the assembly for efficient access.

internal unsafe sealed class JISConvert
{
	// Table identifiers.
	private const int JISX0208_To_Unicode = 1;
	private const int JISX0212_To_Unicode = 2;
	private const int CJK_To_JIS          = 3;
	private const int Greek_To_JIS        = 4;
	private const int Extra_To_JIS        = 5;

	// Public access to the conversion tables.
	public byte *jisx0208ToUnicode;
	public byte *jisx0212ToUnicode;
	public byte *cjkToJis;
	public byte *greekToJis;
	public byte *extraToJis;

	// Constructor.
	private JISConvert()
			{
				// Load the conversion tables.
				CodeTable table = new CodeTable("jis.table");
				jisx0208ToUnicode = table.GetSection(JISX0208_To_Unicode);
				jisx0212ToUnicode = table.GetSection(JISX0212_To_Unicode);
				cjkToJis = table.GetSection(CJK_To_JIS);
				greekToJis = table.GetSection(Greek_To_JIS);
				extraToJis = table.GetSection(Extra_To_JIS);
				table.Dispose();
			}

	// The one and only JIS conversion object in the system.
	private static JISConvert convert;

	// Get the primary JIS conversion object.
	public static JISConvert Convert
			{
				get
				{
					lock(typeof(JISConvert))
					{
						if(convert != null)
						{
							return convert;
						}
						else
						{
							convert = new JISConvert();
							return convert;
						}
					}
				}
			}

}; // class JISConvert

}; // namespace I18N.CJK
