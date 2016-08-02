/*
 * InstallUtil.cs - Implementation of the
 *	    "System.Configuration.InstallUtil" class.
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

namespace System.Configuration
{

using System.Configuration.Install;

// This class implements the .NET assembly installation utility,
// which is typically called "InstallUtil.exe".  We call it
// "ilinstall" to prevent naming conflicts with other systems.

public class InstallUtil
{
	// Main entry point for the program.
	public static int Main(String[] args)
			{
			#if !ECMA_COMPAT
				int exitCode = 0;
				try
				{
					ManagedInstallerClass.InstallHelper(args);
				}
				catch(SystemException e)
				{
					Console.WriteLine(e.Message);
					exitCode = 1;
				}
				return exitCode;
			#else
				Console.WriteLine("This program is not available in ECMA mode");
				return 1;
			#endif
			}

}; // class InstallUtil

}; // namespace System.Configuration
