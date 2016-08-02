/*
 * IMoniker.cs - Implementation of the
 *			"System.Runtime.InteropServices.ComTypes.IMoniker" class.
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

namespace System.Runtime.InteropServices.ComTypes
{

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

[Guid("0000000f-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
public interface IMoniker
{
	void BindToObject(IBindCtx pbc, IMoniker pmkToLeft,
					  ref Guid riidResult, out Object ppvResult);
	void BindToStorage(IBindCtx pbc, IMoniker pmkToLeft,
					   ref Guid riidResult, out Object ppvResult);
	void CommonPrefixWith(IMoniker pmkOther, out IMoniker ppmkPrefix);
	void ComposeWith(IMoniker pmkRight, bool fOnlyIfNotGeneric,
					 out IMoniker ppmkComposite);
	void Enum(bool fForward, out IEnumMoniker ppenumMoniker);
	void GetClassID(out Guid pClassID);
	void GetDisplayName(IBindCtx pbc, IMoniker pmkToLeft,
						out String ppszDisplayName);
	void GetSizeMax(out long pcbSize);
	void GetTimeOfLastChange(IBindCtx pbc, IMoniker pmkToLeft,
							 out FILETIME pFileTime);
	void Hash(out int pdwHash);
	void Inverse(out IMoniker ppmk);
	int IsDirty();
	void IsEqual(IMoniker pmkOtherMoniker);
	void IsRunning(IBindCtx pbc, IMoniker pmkToLeft,
				   IMoniker pmkNewlyRunning);
	void IsSystemMoniker(out int pdwMksys);
	void Load(IStream pStm);
	void ParseDisplayName(IBindCtx pbc, IMoniker pmkToLeft,
						  String pszDisplayName, out int pcbEaten,
						  out IMoniker ppmkOut);
	void Reduce(IBindCtx pbc, int dwReduceHowFar,
				ref IMoniker ppmkToLeft, out IMoniker ppmkReduced);
	void RelativePathTo(IMoniker pmkOther, out IMoniker ppmkRelPath);
	void Save(IStream pStm, bool fClearDirty);

}; // class IMoniker

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices.ComTypes
