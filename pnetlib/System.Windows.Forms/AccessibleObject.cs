/*
 * AccessibleObject.cs - Implementation of the
 *			"System.Windows.Forms.AccessibleObject" class.
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
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Reflection;
using System.Globalization;
using Accessibility;

#if !ECMA_COMPAT
[ComVisible(true)]
#endif
public class AccessibleObject : MarshalByRefObject
#if __CSCC__
	, IAccessible
#endif
#if !ECMA_COMPAT
	, IReflect
#endif
{
	// Internal state.
	private Control control;
	private String value;

	// Extra storage fields that are used by "Control".
	internal String defaultAction;
	internal String description;
	internal String name;
	internal AccessibleRole role;

	// Constructor.
	public AccessibleObject() : this(null) {}
	internal AccessibleObject(Control control)
			{
				this.control = control;
				this.role = AccessibleRole.Default;
			}

	// Get or set this object's properties.
	public virtual Rectangle Bounds
			{
				get
				{
					if(control != null)
					{
						Control parent = control.Parent;
						if(parent != null)
						{
							return parent.RectangleToScreen(control.Bounds);
						}
						else
						{
							return control.Bounds;
						}
					}
					return Rectangle.Empty;
				}
			}
	public virtual String DefaultAction
			{
				get
				{
					if(control != null)
					{
						return control.AccessibleDefaultActionDescription;
					}
					return null;
				}
			}
	public virtual String Description
			{
				get
				{
					if(control != null)
					{
						return control.AccessibleDescription;
					}
					return null;
				}
			}
	public virtual String Help
			{
				get
				{
					if(control != null)
					{
						return control.GetAccessibilityHelp();
					}
					return null;
				}
			}
	public virtual String KeyboardShortcut
			{
				get
				{
					if(control != null)
					{
						return control.GetKeyboardShortcut();
					}
					return null;
				}
			}
	public virtual String Name
			{
				get
				{
					if(control != null)
					{
						return control.AccessibleName;
					}
					else
					{
						return null;
					}
				}
				set
				{
					if(control != null)
					{
						control.AccessibleName = value;
					}
				}
			}
	public virtual AccessibleObject Parent
			{
				get
				{
					if(control != null)
					{
						Control parent = control.Parent;
						if(parent != null)
						{
							return parent.AccessibilityObject;
						}
					}
					return null;
				}
			}
	public virtual AccessibleRole Role
			{
				get
				{
					if(control != null)
					{
						return control.AccessibleRole;
					}
					return AccessibleRole.None;
				}
			}
	public virtual AccessibleStates State
			{
				get
				{
					return AccessibleStates.None;
				}
			}
	public virtual String Value
			{
				get
				{
					return value;
				}
				set
				{
					this.value = value;
				}
			}

	// Perform the default action associated with this object.
	public virtual void DoDefaultAction()
			{
				if(control != null)
				{
					control.DoDefaultAction();
				}
			}

	// Get the accessible object for a specific child.
	public virtual AccessibleObject GetChild(int index)
			{
				if(control != null)
				{
					Control child = control.GetChildByIndex(index);
					if(child != null)
					{
						return child.AccessibilityObject;
					}
				}
				return null;
			}

	// Get the number of children.
	public virtual int GetChildCount()
			{
				if(control != null)
				{
					return control.GetNumChildren();
				}
				return -1;
			}

	// Get the accessible object for the focused control.
	public virtual AccessibleObject GetFocused()
			{
				if(control != null)
				{
					Control child = control.GetFocusedChild();
					if(child != null)
					{
						return child.AccessibilityObject;
					}
				}
				return null;
			}

	// Get the help topic for this accessible object.
	public virtual int GetHelpTopic(out String fileName)
			{
				if(control != null)
				{
					return control.GetHelpTopic(out fileName);
				}
				fileName = null;
				return -1;
			}

	// Get the currently selected child.
	public virtual AccessibleObject GetSelected()
			{
				if(control != null)
				{
					Control child = control.GetSelectedChild();
					if(child != null)
					{
						return child.AccessibilityObject;
					}
				}
				return null;
			}

	// Perform a screen co-ordinate hit test to find a child.
	public virtual AccessibleObject HitTest(int x, int y)
			{
				return null;
			}

	// Navigate to another object from this one.
	public virtual AccessibleObject Navigate(AccessibleNavigation navdir)
			{
				return null;
			}

	// Select this accessible object.
	public virtual void Select(AccessibleSelection flags)
			{
				// Nothing to do here.
			}

#if __CSCC__

	// Stub out the IAccessible implementation, which we don't use.
	// It is peculiar to Microsoft's COM implementation of accessibility.
	void IAccessible.accDoDefaultAction(out Object varChild)
			{
				throw new NotImplementedException();
			}
	Object IAccessible.accHitTest(int xLeft, int yTop)
			{
				throw new NotImplementedException();
			}
	void IAccessible.accLocation
				(out int pxLeft, out int pyTop,
				 out int pcxWidth, out int pcyHeight,
			 	 Object varChild)
			{
				throw new NotImplementedException();
			}
	Object IAccessible.accNavigate(int navDir, Object varStart)
			{
				throw new NotImplementedException();
			}
	void IAccessible.accSelect(int flagsSelect, Object varChild)
			{
				throw new NotImplementedException();
			}
	[IndexerName("accChild")]
	Object IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	int IAccessible.accChildCount
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	Object IAccessible.accFocus
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	Object IAccessible.accParent
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	Object IAccessible.accSelection
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	[IndexerName("accDefaultAction")]
	String IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	[IndexerName("accDescription")]
	String IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	[IndexerName("accHelp")]
	String IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	int IAccessible.get_accHelpTopic(out String pszHelp, Object varChild)
			{
				throw new NotImplementedException();
			}
	[IndexerName("accKeyboardShortcut")]
	String IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
			}
	[IndexerName("accName")]
	String IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}
	[IndexerName("accRole")]
	Object IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}
	[IndexerName("accState")]
	Object IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}
	[IndexerName("accValue")]
	String IAccessible.this[Object varChild]
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}
#endif

#if !ECMA_COMPAT

	// Stub out the IReflect implementation, which we don't use.
	// It is peculiar to Microsoft's COM implementation of accessibility.
	FieldInfo IReflect.GetField(String name, BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	MemberInfo[] IReflect.GetMember(String name, BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	MethodInfo IReflect.GetMethod(String name, BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	MethodInfo IReflect.GetMethod(String name, BindingFlags bindingAttr,
						 Binder binder, Type[] types,
						 ParameterModifier[] modifiers)
			{
				throw new NotImplementedException();
			}
	MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	PropertyInfo IReflect.GetProperty(String name, BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	PropertyInfo IReflect.GetProperty(String name, BindingFlags bindingAttr,
							 Binder binder, Type returnType,
							 Type[] types, ParameterModifier[] modifiers)
			{
				throw new NotImplementedException();
			}
	PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
			{
				throw new NotImplementedException();
			}
	Object IReflect.InvokeMember(String name, BindingFlags invokeAttr,
						Binder binder, Object target, Object[] args,
						ParameterModifier[] modifiers,
						CultureInfo culture, String[] namedParameters)
			{
				throw new NotImplementedException();
			}
	Type IReflect.UnderlyingSystemType
			{
				get
				{
					throw new NotImplementedException();
				}
			}

#endif // !ECMA_COMPAT

}; // class AccessibleObject

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
