/*
 * UCOMIMoniker.cs - Implementation of the
 *			"System.Runtime.InteropServices.UCOMIMoniker" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

[Guid("0000000f-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
#if CONFIG_FRAMEWORK_1_2
[Obsolete("Use the class in System.Runtime.InteropServices.ComTypes instead")]
#endif
public interface UCOMIMoniker
{
	void BindToObject(UCOMIBindCtx pbc, UCOMIMoniker pmkToLeft,
					  ref Guid riidResult, out Object ppvResult);
	void BindToStorage(UCOMIBindCtx pbc, UCOMIMoniker pmkToLeft,
					   ref Guid riidResult, out Object ppvResult);
	void CommonPrefixWith(UCOMIMoniker pmkOther, out UCOMIMoniker ppmkPrefix);
	void ComposeWith(UCOMIMoniker pmkRight, bool fOnlyIfNotGeneric,
					 out UCOMIMoniker ppmkComposite);
	void Enum(bool fForward, out UCOMIEnumMoniker ppenumMoniker);
	void GetClassID(out Guid pClassID);
	void GetDisplayName(UCOMIBindCtx pbc, UCOMIMoniker pmkToLeft,
						out String ppszDisplayName);
	void GetSizeMax(out long pcbSize);
	void GetTimeOfLastChange(UCOMIBindCtx pbc, UCOMIMoniker pmkToLeft,
							 out FILETIME pFileTime);
	void Hash(out int pdwHash);
	void Inverse(out UCOMIMoniker ppmk);
	int IsDirty();
	void IsEqual(UCOMIMoniker pmkOtherMoniker);
	void IsRunning(UCOMIBindCtx pbc, UCOMIMoniker pmkToLeft,
				   UCOMIMoniker pmkNewlyRunning);
	void IsSystemMoniker(out int pdwMksys);
	void Load(UCOMIStream pStm);
	void ParseDisplayName(UCOMIBindCtx pbc, UCOMIMoniker pmkToLeft,
						  String pszDisplayName, out int pcbEaten,
						  out UCOMIMoniker ppmkOut);
	void Reduce(UCOMIBindCtx pbc, int dwReduceHowFar,
				ref UCOMIMoniker ppmkToLeft, out UCOMIMoniker ppmkReduced);
	void RelativePathTo(UCOMIMoniker pmkOther, out UCOMIMoniker ppmkRelPath);
	void Save(UCOMIStream pStm, bool fClearDirty);

}; // class UCOMIMoniker

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
