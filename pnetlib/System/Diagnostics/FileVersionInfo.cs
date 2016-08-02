/*
 * FileVersionInfo.cs - Implementation of the
 *			"System.Diagnostics.FileVersionInfo" class.
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

namespace System.Diagnostics
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.IO;

public sealed class FileVersionInfo
{
	// Internal state.
	private String comments;
	private String companyName;
	private int fileBuildPart;
	private String fileDescription;
	private int fileMajorPart;
	private int fileMinorPart;
	private String fileName;
	private int filePrivatePart;
	private String internalName;
	private bool isDebug;
	private bool isPatched;
	private bool isPreRelease;
	private bool isPrivateBuild;
	private bool isSpecialBuild;
	private String language;
	private String legalCopyright;
	private String legalTrademarks;
	private String originalFilename;
	private String privateBuild;
	private int productBuildPart;
	private int productMajorPart;
	private int productMinorPart;
	private String productName;
	private int productPrivatePart;
	private String specialBuild;

	// Constructor.
	private FileVersionInfo(String fileName)
			{
				this.fileName = fileName;
			}

	// Properties.
	public String Comments
			{
				get
				{
					return comments;
				}
			}
	public String CompanyName
			{
				get
				{
					return companyName;
				}
			}
	public int FileBuildPart
			{
				get
				{
					return fileBuildPart;
				}
			}
	public String FileDescription
			{
				get
				{
					return fileDescription;
				}
			}
	public int FileMajorPart
			{
				get
				{
					return fileMajorPart;
				}
			}
	public int FileMinorPart
			{
				get
				{
					return fileMinorPart;
				}
			}
	public String FileName
			{
				get
				{
					return fileName;
				}
			}
	public int FilePrivatePart
			{
				get
				{
					return filePrivatePart;
				}
			}
	public String FileVersion
			{
				get
				{
					return (new Version(FileMajorPart, FileMinorPart,
									    FileBuildPart, FilePrivatePart))
								.ToString();
				}
			}
	public String InternalName
			{
				get
				{
					return internalName;
				}
			}
	public bool IsDebug
			{
				get
				{
					return isDebug;
				}
			}
	public bool IsPatched
			{
				get
				{
					return isPatched;
				}
			}
	public bool IsPreRelease
			{
				get
				{
					return isPreRelease;
				}
			}
	public bool IsPrivateBuild
			{
				get
				{
					return isPrivateBuild;
				}
			}
	public bool IsSpecialBuild
			{
				get
				{
					return isSpecialBuild;
				}
			}
	public String Language
			{
				get
				{
					return language;
				}
			}
	public String LegalCopyright
			{
				get
				{
					return legalCopyright;
				}
			}
	public String LegalTrademarks
			{
				get
				{
					return legalTrademarks;
				}
			}
	public String OriginalFilename
			{
				get
				{
					return originalFilename;
				}
			}
	public String PrivateBuild
			{
				get
				{
					return privateBuild;
				}
			}
	public int ProductBuildPart
			{
				get
				{
					return productBuildPart;
				}
			}
	public int ProductMajorPart
			{
				get
				{
					return productMajorPart;
				}
			}
	public int ProductMinorPart
			{
				get
				{
					return productMinorPart;
				}
			}
	public String ProductName
			{
				get
				{
					return productName;
				}
			}
	public int ProductPrivatePart
			{
				get
				{
					return productPrivatePart;
				}
			}
	public String ProductVersion
			{
				get
				{
					return (new Version(ProductMajorPart, ProductMinorPart,
									    ProductBuildPart, ProductPrivatePart))
								.ToString();
				}
			}
	public String SpecialBuild
			{
				get
				{
					return specialBuild;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				// We don't support getting Windows version information
				// in this implementation, so all we have to do is return
				// the filename information.
				return "File: " + fileName + Environment.NewLine;
			}

	// Get version information for a file.
	public static FileVersionInfo GetVersionInfo(String fileName)
			{
				// We don't support getting Windows version information
				// in this implementation, so all we have to do is return
				// an object that contains the filename.
				if(fileName == null)
				{
					throw new ArgumentNullException("fileName");
				}
				return new FileVersionInfo(Path.GetFullPath(fileName));
			}

}; // class FileVersionInfo

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
