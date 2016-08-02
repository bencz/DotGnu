/*
 * LocalizationExtenderProvider.cs - Implementation of "System.ComponentModel.Design.LocalizationExtenderProvider" class 
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
	
using System.Globalization;

public class LocalizationExtenderProvider : IExtenderProvider,
	                                            IDisposable
{
	public LocalizationExtenderProvider (ISite serviceProvider,
				IComponent baseComponent)
			{
			}

	public bool CanExtend (object o)
			{
				throw new NotImplementedException();
			}

	public void Dispose()
			{
				throw new NotImplementedException();
			}

	public CultureInfo GetLanguage (object o)
			{
				throw new NotImplementedException();
			}

	public CultureInfo GetLoadLanguage (object o)
			{
				throw new NotImplementedException();
			}

	public bool GetLocalizable (object o)
			{
				throw new NotImplementedException();
			}

	public void ResetLanguage (object o)
			{
				throw new NotImplementedException();
			}

	public void SetLanguage (object o, CultureInfo language)
			{
				throw new NotImplementedException();
			}

	public void SetLocalizable (object o, bool localizable)
			{
				throw new NotImplementedException();
			}

	public bool ShouldSerializeLanguage (object o)
			{
				throw new NotImplementedException();
			}

	~LocalizationExtenderProvider()
			{
			}

} // class LocalizationExtenderProvider

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.ComponentModel.Design
