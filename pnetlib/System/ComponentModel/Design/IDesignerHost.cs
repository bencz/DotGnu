/*
 * IDesignerHost.cs - Implementation of the
 *		"System.ComponentModel.Design.IDesignerHost" class.
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

using System.Runtime.InteropServices;

[ComVisible(true)]
public interface IDesignerHost : IServiceContainer, IServiceProvider
{
	// Get the container for this host.
	IContainer Container { get; }

	// Determine if the designer host is currently in a transaction.
	bool InTransaction { get; }

	// Determine if the designer host is current loading a document.
	bool Loading { get; }

	// Get the root component for the design.
	IComponent RootComponent { get; }

	// Get the class name for the root component.
	String RootComponentClassName { get; }

	// Get a description of the current transaction.
	String TransactionDescription { get; }

	// Activate the designer.
	void Activate();

	// Create a new component and add it to the design document.
	IComponent CreateComponent(Type componentClass);
	IComponent CreateComponent(Type componentClass, String name);

	// Create a new transaction.
	DesignerTransaction CreateTransaction();
	DesignerTransaction CreateTransaction(String description);

	// Destroy a component and remove it from the designer container.
	void DestroyComponent(IComponent component);

	// Get the designer associated with a specified component.
	IDesigner GetDesigner(IComponent component);

	// Get a type.
	Type GetType(String typeName);

	// Event that is emitted when the designer is activated.
	event EventHandler Activated;

	// Event that is emitted when the designer is deactivated.
	event EventHandler Deactivated;

	// Event that is emitted when the document load completes.
	event EventHandler LoadComplete;

	// Event that is emitted when a transaction is closed.
	event DesignerTransactionCloseEventHandler TransactionClosed;

	// Event that is emitted when a transaction is closing.
	event DesignerTransactionCloseEventHandler TransactionClosing;

	// Event that is emitted when a new transaction is opened.
	event EventHandler TransactionOpened;

	// Event that is emitted when a new transaction is opening.
	event EventHandler TransactionOpening;

}; // interface IDesignerHost

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
