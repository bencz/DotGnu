/*
 * HitTestCode.cs - Implementation of the
 *			"System.Windows.Forms.VisualStyles.HitTestCode" class.
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

public enum HitTestCode
{
	Nowhere		= 0,
	Client		= 1,
	Left		= 10,
	Right		= 11,
	Top			= 12,
	TopLeft		= 13,
	TopRight	= 14,
	Bottom		= 15,
	BottomLeft	= 16,
	BottomRight	= 17
}; // enum HitTestCode

}; // namespace System.Windows.Forms.VisualStyles
#endif // CONFIG_FRAMEWORK_2_0

