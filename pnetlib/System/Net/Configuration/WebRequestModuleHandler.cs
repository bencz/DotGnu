/*
 * WebRequestModuleHandler.cs - Implementation of "WebRequestModuleHandler" class 
 *
 * Copyright (C) 2005  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V 
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
using System.IO;
using System.Collections;
using System.Configuration;

#if SECOND_PASS
using System.Xml;
#endif

#if !ECMA_COMPAT && SECOND_PASS 
namespace System.Net.Configuration 
{
	[TODO]
	internal class WebRequestModuleHandler : IConfigurationSectionHandler
	{
		public WebRequestModuleHandler () : base()
		{
			// Nothing here
		}
		[TODO]
		public virtual Object Create( Object parent, 
									  Object configContext, 
									  XmlNode section)
		{
			// TODO
			return null;
		}
	}
}
#endif
