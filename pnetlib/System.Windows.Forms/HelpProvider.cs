/*
 * HelpProvider.cs - Implementation of "ErrorProvider" class
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR   See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, , 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Drawing;

namespace System.Windows.Forms
{
#if CONFIG_COMPONENT_MODEL
	public class HelpProvider : System.ComponentModel.Component
	{
		private String helpNamespace;
		private System.Collections.Hashtable fields;

		public HelpProvider()
		{
			this.fields = new System.Collections.Hashtable();
		}

		public virtual String HelpNameSpace
		{
			get
			{
				return this.helpNamespace;
			}
			set
			{
				this.helpNamespace = value;
			}
		}

		public virtual String GetHelpKeyword(Control ctl)
		{
			Field field = (Field)this.fields[ctl];
			if(field == null)
			{
				return null;
			}
			return field.HelpKeyword;
		}

		public virtual void SetHelpKeyword(Control ctl, String helpKeyword)
		{
			Field field = this.getField(ctl);
			field.HelpKeyword = helpKeyword;
			field.ShowHelp = true;
		}

		public virtual HelpNavigator GetHelpHelpNavigator(Control ctl)
		{
			Field field = (Field)this.fields[ctl];
			if(field == null)
			{
				return HelpNavigator.AssociateIndex;
			}
			return field.Navigator;
		}

		public virtual void SetHelpHelpNavigator(Control ctl, HelpNavigator helpNavigator)
		{
			this.getField(ctl).Navigator = helpNavigator;
		}

		public virtual String GetHelpString(Control ctl)
		{
			Field field = (Field)this.fields[ctl];
			if(field == null)
			{
				return null;
			}
			return field.HelpString;
		}

		public virtual void SetHelpString(Control ctl, String helpString)
		{
			Field field = this.getField(ctl);
			field.HelpString = helpString;
			field.ShowHelp = true;
		}

		public virtual bool GetShowHelp(Control ctl)
		{
			Field field = (Field)this.fields[ctl];
			if(field == null)
			{
				return false;
			}
			return field.ShowHelp;
		}

		public virtual void SetShowHelp(Control ctl, bool showHelp)
		{
			this.getField(ctl).ShowHelp = showHelp;
		}

		private Field getField(Control ctl)
		{
			Field field = (Field)this.fields[ctl];
			if(field == null)
			{
				field = new Field(ctl);
				this.fields[ctl] = field;
			}
			return field;
		}

		private class Field
		{
			public readonly Control		Control;
			public String			HelpKeyword;
			public HelpNavigator		Navigator;
			public String			HelpString;
			public bool			ShowHelp;

			public Field(Control control)
			{
				this.Control = control;
				this.Navigator = HelpNavigator.AssociateIndex;
				this.ShowHelp = false;
			}
		}

		public override String ToString()
		{
			return base.ToString() + " HelpNamespace: " + helpNamespace;
		}
	}

#endif // CONFIG_COMPONENT_MODEL
}
