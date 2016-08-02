/*
 * CodeDomSerializer.cs - Implementation of "System.ComponentModel.Design.Serialization.CodeDomSerializer" class 
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * 
 * Contributions by Adam Ballai <Adam@TheFrontNetworks.net>
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

namespace System.ComponentModel.Design.Serialization
{

#if CONFIG_COMPONENT_MODEL_DESIGN	
	
using System.CodeDom;

public abstract class CodeDomSerializer 
{
	protected CodeDomSerializer () 
			{

			}
	
	public virtual Object Deserialize ( 
			IDesignerSerializationManager manager, 
			Object codeObject )
			{
				return null;
			}

	protected Object DeserializeExpression (
			IDesignerSerializationManager manager,
			String name,
			CodeExpression expression )
			{
				return null;
			}

	protected void DeserializePropertiesFromResources (
			IDesignerSerializationManager manager,
			Object value,
			Attribute[] filter )
			{

			}

	protected void DeserializeStatement (
			IDesignerSerializationManager manager,
			CodeStatement statement )
			{

			}

	public virtual Object Serialize (
			IDesignerSerializationManager manager,
			Object value )
			{
				return null;
			}

	protected void SerializeEvents (
			IDesignerSerializationManager manager,
			CodeStatementCollection statements,
			Object value,
			Attribute[] filter )
			{

			}	

	protected void SerializeProperties (
			IDesignerSerializationManager manager,
			CodeStatementCollection statements,
			Object value,
			Attribute[] filter )
			{

			}

	protected void SerializePropertiesToResources (
			IDesignerSerializationManager manager,
			CodeStatementCollection statements,
			Object value,
			Attribute[] filter )
			{


			}

	protected void SerializeResource (
			IDesignerSerializationManager manager,
			String resourceName,
			Object value )
			{

			}
	
	protected void SerializeResourceInvariant (
			IDesignerSerializationManager manager,
			String resourceName,
			Object value )
			{

			}


	protected CodeExpression SerializeToExpression (
			IDesignerSerializationManager manager,
			Object value )
			{
				return null;
			}

	protected CodeExpression SerializeToReferenceExpression (
			IDesignerSerializationManager manager,
			Object value )
			{
				return null;
			}

} // class CodeDomSerializer 

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}//namespace
