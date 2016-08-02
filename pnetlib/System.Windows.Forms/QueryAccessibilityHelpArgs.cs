/*
 * QueryAccessibilityHelpEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.QueryAccessibilityHelpEventArgs" class.
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

namespace System.Windows.Forms
{

#if !CONFIG_COMPACT_FORMS

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(true)]
#endif
public class QueryAccessibilityHelpEventArgs : EventArgs
{
	// Internal state.
	private String helpNamespace;
	private String helpString;
	private String helpKeyword;

	// Constructors.
	public QueryAccessibilityHelpEventArgs()
			{
				// Nothing to do here.
			}
	public QueryAccessibilityHelpEventArgs
				(String helpNamespace, String helpString, String helpKeyword)
			{
				this.helpNamespace = helpNamespace;
				this.helpString = helpString;
				this.helpKeyword = helpKeyword;
			}

	// Get or set this object's properties.
	public String HelpNamespace
			{
				get
				{
					return helpNamespace;
				}
				set
				{
					helpNamespace = value;
				}
			}
	public String HelpString
			{
				get
				{
					return helpString;
				}
				set
				{
					helpString = value;
				}
			}
	public String HelpKeyword
			{
				get
				{
					return helpKeyword;
				}
				set
				{
					helpKeyword = value;
				}
			}

}; // class QueryAccessibilityHelpEventArgs

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
