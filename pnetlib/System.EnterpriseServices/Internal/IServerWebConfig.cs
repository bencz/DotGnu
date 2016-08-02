/*
 * IServerWebConfig.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.IServerWebConfig" class.
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

namespace System.EnterpriseServices.Internal
{

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[Guid("6261e4b5-572a-4142-a2f9-1fe1a0c97097")]
#endif
public interface IServerWebConfig
{
#if !ECMA_COMPAT
	[DispId(1)]
#endif
	void AddElement(String FilePath, String AssemblyName, String TypeName,
					String ProgId, String Mode, out String Error);
#if !ECMA_COMPAT
	[DispId(2)]
#endif
	void Create(String FilePath, String FileRootName, out String Error);

}; // interface IServerWebConfig

}; // namespace System.EnterpriseServices.Internal
