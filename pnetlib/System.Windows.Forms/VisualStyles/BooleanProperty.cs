/*
 * BooleanProperty.cs - Implementation of the
 *			"System.Windows.Forms.VisualStyles.BooleanProperty" class.
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

#if CONFIG_FRAMEWORK_2_0
namespace System.Windows.Forms.VisualStyles
{

public enum BooleanProperty
{
	Transparent			= 2201,
	AutoSize			= 2202,
	BorderOnly			= 2203,
	Composited			= 2204,
	BackgroundFill		= 2205,
	GlyphTransparent	= 2206,
	GlyphOnly			= 2207,
	AlwaysShowSizingBar	= 2208,
	MirrorImage			= 2209,
	UniformSizing		= 2210,
	IntegralSizing		= 2211,
	SourceGrow			= 2212,
	SourceShrink		= 2213
}; // enum BooleanProperty

}; // namespace System.Windows.Forms.VisualStyles
#endif // CONFIG_FRAMEWORK_2_0

