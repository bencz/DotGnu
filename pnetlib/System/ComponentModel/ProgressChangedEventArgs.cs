/*
 * ProgressChangedEventArgs.cs - Implementation of the
 *			"System.ComponentModel.ProgressChangedEventArgs" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL && CONFIG_FRAMEWORK_2_0

public class ProgressChangedEventArgs : EventArgs
{
	// Internal state.
	private int progressPercentage;
	private Object userState;

	// Constructors.
	public ProgressChangedEventArgs(int progressPercentage, Object userState)
			{
				this.progressPercentage = progressPercentage;
				this.userState = userState;
			}
	
	// Get this object's properties.
	public int ProgressPercentage 
			{
				get
				{
					return progressPercentage;
				}
			}
	public Object UserState 
			{
				get
				{
					return userState;
				}
			}
	public Object UserToken 
			{
				get
				{
					return userState;
				}
			}

}; // class ProgressChangedEventArgs

#endif // CONFIG_COMPONENT_MODEL && CONFIG_FRAMEWORK_2_0

}; // namespace System.ComponentModel
