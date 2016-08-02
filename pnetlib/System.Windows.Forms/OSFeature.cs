/*
 * OSFeature.cs - Implementation of the
 *			"System.Windows.Forms.OSFeature" class.
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

public class OSFeature : FeatureSupport
{
	// Internal state.
	private static OSFeature feature = new OSFeature();

	// Standard feature names.
	public static readonly Object LayeredWindows = new Object();
	public static readonly Object Themes = new Object();

	// Constructor.
	private OSFeature () {}

	// Get the version of a specific feature which is present.
	public override Version GetVersionPresent(Object feature)
			{
				// We don't support any special features yet.
				return null;
			}

	// Get the main OS feature object.
	public static OSFeature Feature
			{
				get
				{
					return feature;
				}
			}

}; // class OSFeature

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
