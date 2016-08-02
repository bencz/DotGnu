/*
 * InputLanguageChangingEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.InputLanguageChangingEventArgs" class.
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

using System.Globalization;
using System.ComponentModel;

public class InputLanguageChangingEventArgs : CancelEventArgs
{
	// Internal state.
	private CultureInfo culture;
	private InputLanguage inputLanguage;
	private bool sysCharSet;

	// Constructors.
	public InputLanguageChangingEventArgs
				(CultureInfo culture, bool sysCharSet)
			{
				this.culture = culture;
				this.sysCharSet = sysCharSet;
			}
	public InputLanguageChangingEventArgs
				(InputLanguage inputLanguage, bool sysCharSet)
			{
				this.inputLanguage = inputLanguage;
				this.sysCharSet = sysCharSet;
			}

	// Get this object's properties.
	public CultureInfo Culture
			{
				get
				{
					return culture;
				}
			}
	public InputLanguage InputLanguage
			{
				get
				{
					return inputLanguage;
				}
			}
	public bool SysCharSet
			{
				get
				{
					return sysCharSet;
				}
			}

}; // class InputLanguageChangingEventArgs

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
