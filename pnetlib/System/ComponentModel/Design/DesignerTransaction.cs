/*
 * DesignerTransaction.cs - Implementation of the
 *		"System.ComponentModel.Design.DesignerTransaction" class.
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

public abstract class DesignerTransaction : IDisposable
{
	// Internal state.
	private String description;
	private bool canceled;
	private bool committed;

	// Constructors.
	public DesignerTransaction() {}
	public DesignerTransaction(String description)
			{
				this.description = description;
			}

	// Destructor.
	~DesignerTransaction()
			{
				Dispose(false);
			}

	// Determine if this transaction has been canceled.
	public bool Canceled
			{
				get
				{
					return canceled;
				}
			}

	// Determine if this transaction has been committed.
	public bool Committed
			{
				get
				{
					return committed;
				}
			}

	// Get the description of this transaction.
	public String Description
			{
				get
				{
					return description;
				}
			}

	// Cancel the transaction.
	public void Cancel()
			{
				if(!canceled && !committed)
				{
					canceled = true;
					GC.SuppressFinalize(this);
					OnCancel();
				}
			}

	// Commit the transaction.
	public void Commit()
			{
				if(!canceled && !committed)
				{
					committed = true;
					GC.SuppressFinalize(this);
					OnCommit();
				}
			}

	// Dispose of this object.
	void IDisposable.Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				Cancel();
			}

	// Raise the cancel event.
	protected abstract void OnCancel();

	// Raise the commit event.
	protected abstract void OnCommit();

}; // class DesignerTransaction

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
