/*
 * InheritanceService.cs - Implementation of "System.ComponentModel.Design.InheritanceService" class 
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


namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN
	
using System.Reflection;

public class InheritanceService : IInheritanceService, IDisposable
{
	public InheritanceService()
			{
			}

	public void AddInheritedComponents (IComponent component,
				IContainer container)
			{
				throw new NotImplementedException();
			}

	protected virtual void AddInheritedComponents (Type type,
				IComponent component,
				IContainer container)
			{
				throw new NotImplementedException();
			}

	public void Dispose()
			{
				throw new NotImplementedException();
			}

	public InheritanceAttribute GetInheritanceAttribute (IComponent component)
			{
				throw new NotImplementedException();
			}

	protected virtual bool IgnoreInheritedMember (MemberInfo member,
				IComponent component)
			{
				throw new NotImplementedException();
			}

	~InheritanceService()
			{
			}

} // class InheritanceService

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.ComponentModel.Design
