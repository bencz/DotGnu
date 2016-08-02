/*
 * Binding.cs - Implementation of the
 *			"System.Windows.Forms.Binding" class.
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

using System.ComponentModel;
using System.Reflection;
#if !CONFIG_COMPACT_FORMS || CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
[TypeConverter(typeof(ListBindingConverter))]
#endif // !CONFIG_COMPACT_FORMS || CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
public class Binding
{
	// Internal state.
	private String propertyName;
	private Object dataSource;
	internal BindingManagerBase bindingManagerBase;
	private BindingMemberInfo bindingMemberInfo;
	private Control control;
	private bool isBinding;
	private Object dataSourceData;
	
	// Constructor.
	public Binding(String propertyName, Object dataSource, String dataMember)
			{
				this.propertyName = propertyName;
				this.dataSource = dataSource;
				this.bindingMemberInfo = new BindingMemberInfo(dataMember);

			}

	// Get this object's properties.
	public BindingManagerBase BindingManagerBase
			{
				get
				{
					return bindingManagerBase;
				}
			}
	public BindingMemberInfo BindingMemberInfo
			{
				get
				{
					return bindingMemberInfo;
				}
			}
	public Control Control
			{
				get
				{
					return control;
				}
			}
	public Object DataSource
			{
				get
				{
					return dataSource;
				}
			}
	internal Object DataSourceData
			{
				get
				{	
					return dataSourceData;
				}
				set
				{
					dataSourceData = value; 
				}
			}
	public bool IsBinding
			{
				get
				{
					return isBinding;
				}
			}
	[DefaultValue("")]
	public String PropertyName
			{
				get
				{
					return propertyName;
				}
			}

	// Associate a control with this binding.
	internal void AssociateControl(Control control)
			{
				this.control = control;
			}

	// Event that is raised when a property's value is bound.
	public event ConvertEventHandler Format;

	// Event that is raised when a property is changed.
	public event ConvertEventHandler Parse;

	// Raise the "Format" event.
	protected virtual void OnFormat(ConvertEventArgs e)
			{
				if(Format != null)
				{
					Format(this, e);
				}
			}

	// Raise the "Parse" event.
	protected virtual void OnParse(ConvertEventArgs e)
			{
				if(Parse != null)
				{
					Parse(this, e);
				}
			}

	internal void UpdateSource(Object data)
			{
				Type sourceType = this.DataSource.GetType();
				PropertyInfo setInfo;
				/* Usually System.Data Objects */
				if( sourceType.GetInterface("IListSource") != null )
				{
					if(sourceType.ToString() == "System.Data.DataSet")
					{
						/* TODO: Handle DataSet walking so we can
						 * write to datasets =) */
						Object itemTableVal = GetTableData(this.BindingMemberInfo.BindingPath, this.DataSource );
						SetRowData(data, this.BindingMemberInfo.BindingField, itemTableVal, 0 );
					}
					else if(sourceType.ToString() == "System.Data.DataTable")
					{
						SetRowData(data, this.BindingMemberInfo.BindingField, this.DataSource, 0 );
					}
				}
				else /* Default Simple Object */
				{
					if( this.BindingMemberInfo.BindingFieldType != null )
						setInfo = sourceType.GetProperty( this.BindingMemberInfo.BindingField,
							this.BindingMemberInfo.BindingFieldType );
					else
						setInfo = sourceType.GetProperty( this.BindingMemberInfo.BindingField);

					setInfo.SetValue(this.DataSource, data, null);
				}
	
			}
	
	private void SetRowData(Object data, Object field, Object datatable, int rowid)
	{
		if( field == null )
			throw new ArgumentNullException("Field");

		if( datatable == null )
			throw new ArgumentNullException("DataTable");

		if( rowid < 0 )
			throw new ArgumentOutOfRangeException("RowID");

		Type t = datatable.GetType();
		PropertyInfo sInfo = t.GetProperty("Rows");
		Object rowVal = sInfo.GetValue(datatable, null);
		t = rowVal.GetType();
		/* TODO: FIX ME, get the first row? */
		sInfo = t.GetProperty("Item", new Type[] {
				Type.GetType("System.Int32") } );
		Object itemRowVal = sInfo.GetValue(rowVal, new object[] { rowid } );

		t = itemRowVal.GetType();
		sInfo = t.GetProperty("Item", new Type[] { Type.GetType("System.String") });
		sInfo.SetValue(itemRowVal, data, new object[] { field } );
		
	}
	
	private Object GetRowData(Object field, Object datatable, int rowid)
	{
		Type t;
		PropertyInfo sInfo;
		Object rowVal, itemRowVal, sVal;
		
		if( field == null )
			throw new ArgumentNullException(S._("Field"));

		if( datatable == null )
			throw new ArgumentNullException(S._("DataTable"));

		if( rowid < 0 )
			throw new ArgumentOutOfRangeException(S._("RowID"));

		t = datatable.GetType();
		sInfo = t.GetProperty("Rows");
		rowVal = sInfo.GetValue(datatable, null);
		t = rowVal.GetType();
		/* TODO: FIX ME, get the first row? */
		sInfo = t.GetProperty("Item", new Type[] {
				Type.GetType("System.Int32") } );
		itemRowVal = sInfo.GetValue(rowVal, new object[] { rowid } );

		t = itemRowVal.GetType();
		sInfo = t.GetProperty("Item", new Type[] { Type.GetType("System.String") });
		sVal = sInfo.GetValue(itemRowVal, new object[] { field } );
		
		return sVal;
				
	}
	
	private Object GetTableData(Object field, Object dataset)
	{
		Type t;
		PropertyInfo sInfo;
		Object tableVal, itemTableVal;
		
		if( field == null )
			throw new ArgumentNullException(S._("Field"));

		if( dataset == null )
			throw new ArgumentNullException(S._("DataSet"));
			
		t = dataset.GetType();
		sInfo = t.GetProperty("Tables");
		
		tableVal = sInfo.GetValue(this.DataSource, null);
		t = tableVal.GetType();
		sInfo = t.GetProperty("Item", new Type[] { Type.GetType("System.String") });
		itemTableVal = sInfo.GetValue(tableVal, new object[] { field } );

		return itemTableVal;
	}
	
	
	// Pull data from the data source (called from BindingManagerBase).
	internal void PullData()
			{
				Type sourceType = this.DataSource.GetType();
				/* Usually System.Data Objects */
				if( sourceType.GetInterface("IListSource") != null )
				{
					if(sourceType.ToString() == "System.Data.DataSet")
					{
						Object itemTableVal = GetTableData(this.BindingMemberInfo.BindingPath, this.DataSource );
						Object sVal = GetRowData(this.BindingMemberInfo.BindingField, itemTableVal, 0 );
						dataSourceData = sVal;
						
					}
					else if(sourceType.ToString() == "System.Data.DataTable")
					{
						Object sVal = GetRowData(this.BindingMemberInfo.BindingField, this.DataSource, 0 );
						dataSourceData = sVal;
					}
				}
				else  /* Default Simple Object */
				{

					/* Let's get the PropertyInfo for the DataField prescribed, then determine
					 * its type, get the PropertyInfo for the property we're binding to, then
					 * pull the data from the source and attach it to the property. */
					PropertyInfo sInfo = sourceType.GetProperty(this.BindingMemberInfo.BindingField);
					PropertyInfo controlDataInfo = sourceType.GetProperty(this.PropertyName);
					Object sVal = sInfo.GetValue(this.DataSource, null);
					this.bindingMemberInfo.BindingFieldType = sVal.GetType();
	
					dataSourceData = sVal;
				}
	
			}

	// Push data to the data bound control (called from BindingManagerBase).
	internal void PushData()
			{
				Type controlType = control.GetType();
				PropertyInfo controlInfo = controlType.GetProperty(propertyName);
				controlInfo.SetValue(control, dataSourceData, null);
			}

}; // class Binding

}; // namespace System.Windows.Forms
