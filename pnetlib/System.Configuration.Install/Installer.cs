/*
 * Installer.cs - Implementation of the
 *	    "System.Configuration.Install.Installer" class.
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

namespace System.Configuration.Install
{

#if !ECMA_COMPAT

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

#if CONFIG_COMPONENT_MODEL_DESIGN
[Designer("Microsoft.VisualStudio.Configuration.InstallerDesigner, Microsoft.VisualStudio", typeof(IRootDesigner))]
#endif
#if CONFIG_COMPONENT_MODEL
[DefaultEvent("AfterInstall")]
public class Installer : Component
#else
public class Installer
#endif
{
    // Internal state.
    private InstallContext context;
    internal InstallerCollection installers;
    internal Installer parent;

    // Constructor.
    public Installer() {}

    // Get or set the installer's context.
#if CONFIG_COMPONENT_MODEL
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
#endif
    public InstallContext Context
			{
				get
				{
					return context;
				}
				set
				{
					context = value;
				}
			}

    // Get the help text for this installer.
    public virtual String HelpText
			{
				get
				{
					return null;
				}
			}

    // Get the collection of installers contained within this one.
#if CONFIG_COMPONENT_MODEL
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(false)]
#endif
    public InstallerCollection Installers
			{
				get
				{
					if(installers == null)
					{
						installers = new InstallerCollection(this);
					}
					return installers;
				}
			}

    // Get or set this installer's parent collection.
#if CONFIG_COMPONENT_MODEL
	[TypeConverter("System.Configuration.Design.InstallerParentConverter")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(true)]
#endif
    public Installer Parent
			{
				get
				{
					return parent;
				}
				set
				{
					if(parent != value)
					{
						if(parent != null)
						{
							parent.Installers.Remove(this);
						}
						if(value != null)
						{
							value.Installers.Add(this);
						}
					}
				}
			}

	// Check a state argument for null.
	private static void CheckNullState(IDictionary savedState)
			{
				if(savedState == null)
				{
					throw new ArgumentException
						(S._("Installer_InvalidSavedState"), "savedState");
				}
			}

	// Log an exception.
	internal void LogException(String request, Exception e)
			{
				if(Context.IsParameterTrue("ShowCallStack"))
				{
					Context.LogLine(request + ": " + e.ToString());
				}
				else
				{
					Context.LogLine(request + ": " + e.Message);
				}
			}

	// Get the nested state for a pariticular installer.
	private IDictionary GetNestedState(IDictionary state, int index)
			{
				IDictionary[] table = (state["reserved_nestedSavedStates"]
											as IDictionary[]);
				if(table == null)
				{
					table = new IDictionary [Installers.Count];
					state["reserved_nestedSavedStates"] = table;
				}
				if(table[index] == null)
				{
					table[index] = new Hashtable(index);
				}
				return table[index];
			}

    // Commit the installation transaction.
    public virtual void Commit(IDictionary savedState)
			{
				Exception e = null;
				int index;
				IDictionary nestedState;

				// Validate the parameter.
				CheckNullState(savedState);

				// Raise the "Committing" event.
				try
				{
					OnCommitting(savedState);
				}
				catch(SystemException e1)
				{
					LogException("OnCommitting", e1);
					e = e1;
				}

				// Copy the parent's context down to the children.
				for(index = 0; index < Installers.Count; ++index)
				{
					Installers[index].Context = Context;
				}

				// Commit each of the installers in turn.
				for(index = 0; index < Installers.Count; ++index)
				{
					nestedState = GetNestedState(savedState, index);
					try
					{
						Installers[index].Commit(nestedState);
					}
					catch(SystemException e2)
					{
						if(e2.InnerException == null)
						{
							LogException("Commit", e2);
						}
						e = e2;
					}
				}

				// Raise the "Committed" event.
				try
				{
					OnCommitted(savedState);
				}
				catch(SystemException e3)
				{
					LogException("OnCommitted", e3);
					e = e3;
				}

				// Re-throw the last-occurring exception.
				if(e != null)
				{
					if(e.InnerException == null)
					{
						e = new InstallException(e.Message, e);
					}
					throw e;
				}
			}

    // Perform the installation process, saving the previous
    // state in the "stateSaver" object.
    public virtual void Install(IDictionary stateSaver)
			{
				Exception e = null;
				int index;
				IDictionary nestedState;

				// Validate the parameter.
				if(stateSaver == null)
				{
					throw new ArgumentException
						(S._("Installer_InvalidSavedState"), "stateSaver");
				}

				// Raise the "BeforeInstall" event.
				try
				{
					OnBeforeInstall(stateSaver);
				}
				catch(SystemException e1)
				{
					LogException("OnBeforeInstall", e1);
					e = e1;
				}

				// Copy the parent's context down to the children.
				for(index = 0; index < Installers.Count; ++index)
				{
					Installers[index].Context = Context;
				}

				// Run each of the installers in turn.
				for(index = 0; index < Installers.Count; ++index)
				{
					nestedState = GetNestedState(stateSaver, index);
					try
					{
						Installers[index].Install(nestedState);
					}
					catch(SystemException e2)
					{
						if(e2.InnerException == null)
						{
							LogException("Install", e2);
						}
						e = e2;
					}
				}

				// Raise the "AfterInstall" event.
				try
				{
					OnAfterInstall(stateSaver);
				}
				catch(SystemException e3)
				{
					LogException("OnAfterInstall", e3);
					e = e3;
				}

				// Re-throw the last-occurring exception.
				if(e != null)
				{
					if(e.InnerException == null)
					{
						e = new InstallException(e.Message, e);
					}
					throw e;
				}
			}

    // Roll back the current installation to "savedState".
    public virtual void Rollback(IDictionary savedState)
			{
				Exception e = null;
				int index;
				IDictionary nestedState;

				// Validate the parameter.
				CheckNullState(savedState);

				// Raise the "BeforeRollback" event.
				try
				{
					OnBeforeRollback(savedState);
				}
				catch(SystemException e1)
				{
					LogException("OnBeforeRollback", e1);
					e = e1;
				}

				// Copy the parent's context down to the children.
				for(index = Installers.Count - 1; index >= 0; --index)
				{
					Installers[index].Context = Context;
				}

				// Rollback each of the installers in turn, in reverse order.
				for(index = Installers.Count - 1; index >= 0; --index)
				{
					nestedState = GetNestedState(savedState, index);
					try
					{
						Installers[index].Rollback(nestedState);
					}
					catch(SystemException e2)
					{
						if(e2.InnerException == null)
						{
							LogException("Rollback", e2);
						}
						e = e2;
					}
				}

				// Raise the "AfterRollback" event.
				try
				{
					OnAfterRollback(savedState);
				}
				catch(SystemException e3)
				{
					LogException("OnAfterRollback", e3);
					e = e3;
				}

				// Re-throw the last-occurring exception.
				if(e != null)
				{
					if(e.InnerException == null)
					{
						e = new InstallException(e.Message, e);
					}
					throw e;
				}
			}

    // Uninstall and return to a previously saved state.
    public virtual void Uninstall(IDictionary savedState)
			{
				Exception e = null;
				int index;
				IDictionary nestedState;

				// Validate the parameter.
				CheckNullState(savedState);

				// Raise the "BeforeUninstall" event.
				try
				{
					OnBeforeUninstall(savedState);
				}
				catch(SystemException e1)
				{
					LogException("OnBeforeUninstall", e1);
					e = e1;
				}

				// Copy the parent's context down to the children.
				for(index = Installers.Count - 1; index >= 0; --index)
				{
					Installers[index].Context = Context;
				}

				// Uninstall each of the installers in turn, in reverse order.
				for(index = Installers.Count - 1; index >= 0; --index)
				{
					nestedState = GetNestedState(savedState, index);
					try
					{
						Installers[index].Uninstall(nestedState);
					}
					catch(SystemException e2)
					{
						if(e2.InnerException == null)
						{
							LogException("Uninstall", e2);
						}
						e = e2;
					}
				}

				// Raise the "AfterUninstall" event.
				try
				{
					OnAfterUninstall(savedState);
				}
				catch(SystemException e3)
				{
					LogException("OnAfterUninstall", e3);
					e = e3;
				}

				// Re-throw the last-occurring exception.
				if(e != null)
				{
					if(e.InnerException == null)
					{
						e = new InstallException(e.Message, e);
					}
					throw e;
				}
			}

    // Public events.
    public event InstallEventHandler AfterInstall;
    public event InstallEventHandler AfterRollback;
    public event InstallEventHandler AfterUninstall;
    public event InstallEventHandler BeforeInstall;
    public event InstallEventHandler BeforeRollback;
    public event InstallEventHandler BeforeUninstall;
    public event InstallEventHandler Committed;
    public event InstallEventHandler Committing;

    // Raise the public events.
    protected virtual void OnAfterInstall(IDictionary savedState)
			{
				if(AfterInstall != null)
				{
					AfterInstall(this, new InstallEventArgs(savedState));
				}
			}
    protected virtual void OnAfterRollback(IDictionary savedState)
			{
				if(AfterRollback != null)
				{
					AfterRollback(this, new InstallEventArgs(savedState));
				}
			}
    protected virtual void OnAfterUninstall(IDictionary savedState)
			{
				if(AfterUninstall != null)
				{
					AfterUninstall(this, new InstallEventArgs(savedState));
				}
			}
    protected virtual void OnBeforeInstall(IDictionary savedState)
			{
				if(BeforeInstall != null)
				{
					BeforeInstall(this, new InstallEventArgs(savedState));
				}
			}
    protected virtual void OnBeforeRollback(IDictionary savedState)
			{
				if(BeforeRollback != null)
				{
					BeforeRollback(this, new InstallEventArgs(savedState));
				}
			}
    protected virtual void OnBeforeUninstall(IDictionary savedState)
			{
				if(BeforeUninstall != null)
				{
					BeforeUninstall(this, new InstallEventArgs(savedState));
				}
			}
    protected virtual void OnCommitted(IDictionary savedState)
			{
				if(Committed != null)
				{
					Committed(this, new InstallEventArgs(savedState));
				}
			}
    protected virtual void OnCommitting(IDictionary savedState)
			{
				if(Committing != null)
				{
					Committing(this, new InstallEventArgs(savedState));
				}
			}

}; // class Installer

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Install
