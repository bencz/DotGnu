/*
 * MenuCommand.cs - Implementation of the
 *		"System.ComponentModel.Design.MenuCommand" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Text;
using System.Runtime.InteropServices;

[ComVisible(true)]
public class MenuCommand
{
	// Internal state.
	private EventHandler handler;
	private CommandID command;
	private int oleStatus;

	// Flag bits in "oleStatus".
	private const int OleStatus_Supported = 1;
	private const int OleStatus_Enabled   = 2;
	private const int OleStatus_Checked   = 4;
	private const int OleStatus_Invisible = 16;

	// Constructor.
	public MenuCommand(EventHandler handler, CommandID command)
			{
				this.handler = handler;
				this.command = command;
				this.oleStatus = OleStatus_Supported |
								 OleStatus_Enabled;
			}

	// Get or set this object's properties.
	public virtual bool Checked
			{
				get
				{
					return ((oleStatus & OleStatus_Checked) != 0);
				}
				set
				{
					int newStatus;
					if(value)
					{
						newStatus = oleStatus | OleStatus_Checked;
					}
					else
					{
						newStatus = oleStatus & ~OleStatus_Checked;
					}
					if(newStatus != oleStatus)
					{
						oleStatus = newStatus;
						OnCommandChanged(EventArgs.Empty);
					}
				}
			}
	public virtual CommandID CommandID
			{
				get
				{
					return command;
				}
			}
	public virtual bool Enabled
			{
				get
				{
					return ((oleStatus & OleStatus_Enabled) != 0);
				}
				set
				{
					int newStatus;
					if(value)
					{
						newStatus = oleStatus | OleStatus_Enabled;
					}
					else
					{
						newStatus = oleStatus & ~OleStatus_Enabled;
					}
					if(newStatus != oleStatus)
					{
						oleStatus = newStatus;
						OnCommandChanged(EventArgs.Empty);
					}
				}
			}
	public virtual int OleStatus
			{
				get
				{
					return oleStatus;
				}
			}
	public virtual bool Supported
			{
				get
				{
					return ((oleStatus & OleStatus_Supported) != 0);
				}
				set
				{
					int newStatus;
					if(value)
					{
						newStatus = oleStatus | OleStatus_Supported;
					}
					else
					{
						newStatus = oleStatus & ~OleStatus_Supported;
					}
					if(newStatus != oleStatus)
					{
						oleStatus = newStatus;
						OnCommandChanged(EventArgs.Empty);
					}
				}
			}
	public virtual bool Visible
			{
				get
				{
					return ((oleStatus & OleStatus_Invisible) == 0);
				}
				set
				{
					int newStatus;
					if(!value)
					{
						newStatus = oleStatus | OleStatus_Invisible;
					}
					else
					{
						newStatus = oleStatus & ~OleStatus_Invisible;
					}
					if(newStatus != oleStatus)
					{
						oleStatus = newStatus;
						OnCommandChanged(EventArgs.Empty);
					}
				}
			}

	// Invoke the menu command.
	public virtual void Invoke()
			{
				if(handler != null)
				{
					handler(this, EventArgs.Empty);
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(command.ToString());
				builder.Append(" : ");
				bool haveItem = false;
				if((oleStatus & OleStatus_Supported) != 0)
				{
					builder.Append("Supported");
					haveItem = true;
				}
				if((oleStatus & OleStatus_Enabled) != 0)
				{
					if(!haveItem)
					{
						builder.Append('|');
					}
					builder.Append("Enabled");
					haveItem = true;
				}
				if((oleStatus & OleStatus_Invisible) == 0)
				{
					if(!haveItem)
					{
						builder.Append('|');
					}
					builder.Append("Visible");
					haveItem = true;
				}
				if((oleStatus & OleStatus_Checked) != 0)
				{
					if(!haveItem)
					{
						builder.Append('|');
					}
					builder.Append("Checked");
					haveItem = true;
				}
				return builder.ToString();
			}

	// Event that is emitted when the command is changed.
	public event EventHandler CommandChanged;

	// Emit the "CommandChanged" event.
	protected virtual void OnCommandChanged(EventArgs e)
			{
				if(CommandChanged != null)
				{
					CommandChanged(this, e);
				}
			}

}; // class MenuCommand

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
