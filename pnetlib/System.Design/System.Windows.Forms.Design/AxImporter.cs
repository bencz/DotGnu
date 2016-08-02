/*
 * AxImporter.cs - Implementation of "System.Windows.Forms.Design.AxImporter" class 
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


namespace System.Windows.Forms.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN	
	
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

public class AxImporter
{
	public 	AxImporter(AxImporter.Options options)
			{
				throw new NotImplementedException ();
			}
	public string[] GeneratedAsemblies
			{
				get 
				{
					throw new NotImplementedException ();
				}
			}
	
	public string[] GeneratedSources
			{
				get
			       	{
					throw new NotImplementedException ();
				}
			}
 	public TYPELIBATTR[] GeneratedTypeLibAttribute
			{
 				get 
				{
 					throw new NotImplementedException ();
 				}
 			}
	
	public string GenerateFromFile (FileInfo file)
			{
				throw new NotImplementedException ();
			}

	public string GenerateFromTypeLibrary(UCOMITypeLib typeLib)
			{
				throw new NotImplementedException ();
			}
	
	public string GenerateFromTypeLibrary(UCOMITypeLib typeLib, Guid clsid)
			{
				throw new NotImplementedException ();
			}
	
	public static string GetFileOfTypeLib(ref TYPELIBATTR tlibattr)
			{
				throw new NotImplementedException ();
			}
	
	public interface IReferenceResolver
	{
		String ResolveActiveXReference ( UCOMITypeLib typeLib );

		String ResolveComReference ( AssemblyName name );
		
		String ResolveComReference ( UCOMITypeLib typeLib );

		String ResolveManagedReference ( String assemName );

	} // interface IReferenceResolver
	
	public sealed class Options
	{
		public Options ()
				{
				}

		public bool delaySign;
		public bool GenSources;
		public string keyContainer;
		public string keyfile;
		public StrongNameKeyPair keyPair;

		public bool noLogo;
		public string outputDirectory;
		public string outputName;
		public bool overwriteRCW;
		public byte[] publicKey;
		
		public AxImporter.IReferenceResolver references;
		public bool silentMode;
		public bool verboseMode;
	} // class Options

} // class AxImporter

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
