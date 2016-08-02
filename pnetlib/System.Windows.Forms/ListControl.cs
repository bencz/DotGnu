/*
 * ListControl.cs - Implementation of the
 *			"System.Windows.Forms.ListControl" class.
 *
 * Copyright (C) 2003 Free Software Foundation
 *
 * Contributions from Cecilio Pardo <cpardo@imayhem.com>,
 *                    Brian Luft <brian@electroly.com>
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

using System;
using System.Reflection;
using System.Collections;
using System.Globalization;

namespace System.Windows.Forms
{

[TODO]
public abstract class ListControl : Control
{
	internal object dataSource;
	internal string displayMember;
	internal string valueMember;

	public BorderStyle BorderStyle
	{
		get
		{
			return BorderStyleInternal;
		}
		set
		{
			if(BorderStyleInternal != value)
			{
				BorderStyleInternal = value;
				OnBorderStyleChanged(EventArgs.Empty);
			}
		}
	}
	
	protected virtual void OnBorderStyleChanged(EventArgs e)
	{
		EventHandler handler;
		handler = (EventHandler)
			(GetHandler(EventId.BorderStyleChanged));
		if(handler != null)
		{
			handler(this, e);
		}
	}

	public virtual Object DataSource 
	{ 
		get 
		{ 
			return this.dataSource; 
		}
		set 
		{
			this.dataSource = value;
		}  
	}
	
	public virtual String DisplayMember 
	{ 
		get
		{
			return this.displayMember;
		}
		set
		{
			this.displayMember = value;
		}
	}
	
	public virtual String ValueMember
	{ 
		get
		{
			return this.valueMember;
		}
		set
		{
			this.valueMember = value;
		}
	}

	public abstract int SelectedIndex { get; set; }  

	[TODO]
	public Object SelectedValue
	{
		get
		{
			return null;
		}
		
		set 
		{
		}
	}


	public String GetItemText( Object i )
	{
		if ( i == null )
		{
			return null;
		}
		
		if ( displayMember == null || displayMember == "" )
		{
			return i.ToString();
		}
		
		PropertyInfo pi;
		try 
		{
			pi = i.GetType().GetProperty( displayMember );
		}
		catch ( AmbiguousMatchException ) 
		{
			return null;
		}
		
		if ( pi == null )
		{
			return null;
		}
		
		Object v = pi.GetValue( i, null );
		
		if ( v == null )
		{
			return null;
		}
		
		return v.ToString();
	}

	public event EventHandler DataSourceChanged;
	public event EventHandler DisplayMemberChanged;
	public event EventHandler SelectedValueChanged;
	public event EventHandler SelectedIndexChanged;
	public event EventHandler ValueMemberChanged;

	[TODO]
	protected ListControl()
	{
	}
	
	[TODO]
	protected CurrencyManager DataManager
	{
		get
			{
				return null;
			}
	}

	[TODO]
	protected override bool IsInputKey( Keys k )
	{
		return false;
	}

	protected virtual void OnDataSourceChanged( EventArgs ev )
	{
		if(this.DataSourceChanged != null)
			this.DataSourceChanged(this, ev);
	}

	protected virtual void OnDisplayMemberChanged( EventArgs ev )
	{
		if(this.DisplayMemberChanged != null)
			this.DisplayMemberChanged(this, ev);
	}

	protected virtual void OnSelectedIndexChanged( EventArgs ev )
	{
		if(this.SelectedIndexChanged != null)
			this.SelectedIndexChanged(this, ev);
	}

	protected virtual void OnSelectedValueChanged( EventArgs ev )
	{
		if(this.SelectedValueChanged != null)
			this.SelectedValueChanged(this, ev);
	}

	protected virtual void OnValueMemberChanged( EventArgs ev )
	{
		if(this.ValueMemberChanged != null)
			this.ValueMemberChanged(this, ev);
	}
	
	protected abstract void RefreshItem( int index );

	[TODO]
	protected virtual void SetItemCore(int index, object value)
	{
		throw new NotImplementedException("SetItemCore");
	}

	[TODO]
	protected virtual void SetItemsCore(IList value)
	{
		throw new NotImplementedException("SetItemsCore");
	}

	internal int FindStringInternal(string s, IList items, int start, bool exact)
	{
		if (s == null || items == null || start < -1 || start > items.Count - 2)
			return -1; 
		int pos = start;
		do
		{
			pos++;
			if (exact)
			{
				if (string.Compare(s, this.GetItemText(items[pos]), true) == 0)
					return pos;
			}
			else
			{
				if (string.Compare(s, 0, this.GetItemText(items[pos]), 0, s.Length, true) == 0)
					return pos;
			}
			if (pos == items.Count-1)
				pos = -1;
		}
		while (pos != start);
		return -1; 
	} 


}; // class ListControl
	
}; // namespace System.Windows.Forms
