/*
 * ApplicationContext.cs - Implementation of the
 *			"System.Windows.Forms.ApplicationContext" class.
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
public
#else
internal
#endif
class ApplicationContext
{
	// Internal state.
	private Form mainForm;

	// Constructors.
	public ApplicationContext() {}
	public ApplicationContext(Form mainForm)
			{
				MainForm = mainForm;
			}

	// Destructor.
	~ApplicationContext()
			{
				Dispose(false);
			}

	// Event handler for the main form's handle destroy event.
	private void MainFormDestroyed(Object sender, EventArgs e)
			{
				Form form = (sender as Form);
				if(form != null && !form.RecreatingHandle)
				{
					form.HandleDestroyed -=
						new EventHandler(MainFormDestroyed);
					OnMainFormClosed(sender, e);
				}
			}

	// Get or set the main form for this context.
	public Form MainForm
			{
				get
				{
					return mainForm;
				}
				set
				{
					if(mainForm != value)
					{
						EventHandler handler =
							new EventHandler(MainFormDestroyed);
						if(mainForm != null)
						{
							mainForm.HandleDestroyed -= handler;
						}
						mainForm = value;
						if(mainForm != null)
						{
							mainForm.HandleDestroyed += handler;
						}
					}
				}
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				MainForm = null;
			}

	// Exit the current thread's message loop.
	public void ExitThread()
			{
				ExitThreadCore();
			}
	protected virtual void ExitThreadCore()
			{
				if(ThreadExit != null)
				{
					ThreadExit(this, EventArgs.Empty);
				}
			}

	// Event that is raised when the thread message loop should exit.
	public event EventHandler ThreadExit;

	// Method that is called when the main form is closed.
	protected virtual void OnMainFormClosed(Object sender, EventArgs e)
			{
				ExitThreadCore();
			}

}; // class ApplicationContext

}; // namespace System.Windows.Forms
