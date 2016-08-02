/*
 * CompilerParameters.cs - Implementation of the
 *		System.CodeDom.Compiler.CompilerParameters class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.CodeDom.Compiler
{

#if CONFIG_CODEDOM

using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Policy;

[ComVisible(false)]
public class CompilerParameters
{
	// Internal state.
	private StringCollection referencedAssemblies;
	private String outputName;
	private bool includeDebugInformation;
	private String compilerOptions;
	private Evidence evidence;
	private bool generateExecutable;
	private bool generateInMemory;
	private String mainClass;
	private TempFileCollection tempFiles;
	private bool treatWarningsAsErrors;
	private IntPtr userToken;
	private int warningLevel;
	private String win32Resource;

	// Constructors.
	public CompilerParameters()
			: this(null, null, false)
			{
				// Nothing to do here
			}
	public CompilerParameters(String[] assemblyNames)
			: this(assemblyNames, null, false)
			{
				// Nothing to do here
			}
	public CompilerParameters(String[] assemblyNames, String outputName)
			: this(assemblyNames, outputName, false)
			{
				// Nothing to do here
			}
	public CompilerParameters(String[] assemblyNames, String outputName,
							  bool includeDebugInformation)
			{
				this.outputName = outputName;
				this.includeDebugInformation = includeDebugInformation;
				this.warningLevel = -1;
				if(assemblyNames != null)
				{
					ReferencedAssemblies.AddRange(assemblyNames);
				}
			}

	// Properties.
	public String CompilerOptions
			{
				get
				{
					return compilerOptions;
				}
				set
				{
					compilerOptions = value;
				}
			}
	public Evidence Evidence
			{
				get
				{
					return evidence;
				}
				set
				{
					evidence = value;
				}
			}
	public bool GenerateExecutable
			{
				get
				{
					return generateExecutable;
				}
				set
				{
					generateExecutable = value;
				}
			}
	public bool GenerateInMemory
			{
				get
				{
					return generateInMemory;
				}
				set
				{
					generateInMemory = value;
				}
			}
	public bool IncludeDebugInformation
			{
				get
				{
					return includeDebugInformation;
				}
				set
				{
					includeDebugInformation = value;
				}
			}
	public String MainClass
			{
				get
				{
					return mainClass;
				}
				set
				{
					mainClass = value;
				}
			}
	public String OutputAssembly
			{
				get
				{
					return outputName;
				}
				set
				{
					outputName = value;
				}
			}
	public StringCollection ReferencedAssemblies
			{
				get
				{
					if(referencedAssemblies == null)
					{
						referencedAssemblies = new StringCollection();
					}
					return referencedAssemblies;
				}
			}
	public TempFileCollection TempFiles
			{
				get
				{
					if(tempFiles == null)
					{
						tempFiles = new TempFileCollection();
					}
					return tempFiles;
				}
				set
				{
					tempFiles = value;
				}
			}
	public bool TreatWarningsAsErrors
			{
				get
				{
					return treatWarningsAsErrors;
				}
				set
				{
					treatWarningsAsErrors = value;
				}
			}
	public IntPtr UserToken
			{
				get
				{
					return userToken;
				}
				set
				{
					userToken = value;
				}
			}
	public int WarningLevel
			{
				get
				{
					return warningLevel;
				}
				set
				{
					warningLevel = value;
				}
			}
	public String Win32Resource
			{
				get
				{
					return win32Resource;
				}
				set
				{
					win32Resource = value;
				}
			}

}; // class CompilerParameters

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
