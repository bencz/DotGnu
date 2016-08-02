/*
 * IsolatedStorageFile.cs - Implementation of the
 *		"System.IO.IsolatedStorage.IsolatedStorageFile" class.
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

namespace System.IO.IsolatedStorage
{

#if CONFIG_ISOLATED_STORAGE

using System.Collections;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using Platform;

// Note: see the general comments in "IsolatedStorage.cs".

public sealed class IsolatedStorageFile : IsolatedStorage, IDisposable
{
	// Internal state.
	private String baseDirectory;
	private int refCount;
	private bool closed;
	private static Hashtable stores = new Hashtable();

	// Constructor.
	internal IsolatedStorageFile(IsolatedStorageScope scope,
								 String baseDirectory)
			{
				InitStore(scope, null, null);
				this.baseDirectory = baseDirectory;
				this.refCount = 1;
				this.closed = false;
			}

	// Destructor.
	~IsolatedStorageFile()
			{
				Close();
			}

	// Get the current amount of space that has been used.
	[CLSCompliant(false)]
	public override ulong CurrentSize
			{
				get
				{
					return base.CurrentSize;
				}
			}

	// Get the maximum amount of space that can be used.
	[CLSCompliant(false)]
	public override ulong MaximumSize
			{
				get
				{
					return base.MaximumSize;
				}
			}

	// Close this storage area.
	public void Close()
			{
				lock(typeof(IsolatedStorageFile))
				{
					if(!closed)
					{
						if(--refCount == 0)
						{
							// Remove the store from the global list because
							// nothing is referencing it at the moment.
							stores.Remove(Scope);
						}
						closed = true;
					}
				}
			}

	// Get the full path of an item in this storage area.
	private String GetFullPath(String name, String path)
			{
				// Perform simple validation on the pathname.
				if(path == null)
				{
					throw new ArgumentNullException(name);
				}

				// Validate the pathname components and build
				// the full pathname.
				StringBuilder builder = new StringBuilder();
				int posn = 0;
				int posn2;
				builder.Append(baseDirectory);
				builder.Append(Path.DirectorySeparatorChar);
				while(posn < path.Length)
				{
					posn2 = posn;
					while(posn2 < path.Length &&
						  path[posn2] != '\\' && path[posn2] != '/' &&
						  path[posn2] != Path.DirectorySeparatorChar &&
						  path[posn2] != Path.AltDirectorySeparatorChar)
					{
						++posn2;
					}
					if(posn2 == posn)
					{
						// Empty pathname component.
						throw new IsolatedStorageException
							(_("IO_InvalidPathname"));
					}
					else if(posn2 == (posn + 1) && path[posn] == '.')
					{
						// Reference to the "current" directory.
						throw new IsolatedStorageException
							(_("IO_InvalidPathname"));
					}
					else if(posn2 == (posn + 2) && path[posn] == '.' &&
							path[posn + 1] == '.')
					{
						// Reference to the "parent" directory.
						throw new IsolatedStorageException
							(_("IO_InvalidPathname"));
					}
					builder.Append(path, posn, posn2 - posn);
					if(posn2 < path.Length)
					{
						posn = posn2 + 1;
						builder.Append(Path.DirectorySeparatorChar);
					}
					else
					{
						posn = posn2;
					}
				}

				// Return the final pathname to the caller.
				return builder.ToString();
			}

	// Create a directory within this storage area.
	public void CreateDirectory(String dir)
			{
				try
				{
					Directory.CreateDirectory(GetFullPath("dir", dir));
				}
				catch(IOException e)
				{
					throw new IsolatedStorageException
						(_("IO_IsolatedStorage"), e);
				}
			}

	// Delete a file from this storage area.
	public void DeleteFile(String file)
			{
				try
				{
					File.Delete(GetFullPath("file", file));
				}
				catch(IOException e)
				{
					throw new IsolatedStorageException
						(_("IO_IsolatedStorage"), e);
				}
			}

	// Delete a directory from this storage area.
	public void DeleteDirectory(String dir)
			{
				try
				{
					Directory.Delete(GetFullPath("dir", dir), false);
				}
				catch(IOException e)
				{
					throw new IsolatedStorageException
						(_("IO_IsolatedStorage"), e);
				}
			}

	// Dispose this storage area.
	public void Dispose()
			{
				Close();
				GC.SuppressFinalize(this);
			}

	// Get a list of directories from a storage area directory.
	public String[] GetDirectoryNames(String searchPattern)
			{
				// Split the pattern into directory and wildcards.
				String fullPath = GetFullPath("searchPattern", searchPattern);
				int index = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
				searchPattern = fullPath.Substring(index);
				fullPath = fullPath.Substring(0, index - 1);

				// Scan the directory and return the names.
				try
				{
					return Directory.GetDirectories(fullPath, searchPattern);
				}
				catch(IOException e)
				{
					throw new IsolatedStorageException
						(_("IO_IsolatedStorage"), e);
				}
			}

	// Get a list of files from a storage area directory.
	public String[] GetFileNames(String searchPattern)
			{
				// Split the pattern into directory and wildcards.
				String fullPath = GetFullPath("searchPattern", searchPattern);
				int index = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
				searchPattern = fullPath.Substring(index);
				fullPath = fullPath.Substring(0, index - 1);

				// Scan the directory and return the names.
				try
				{
					return Directory.GetFiles(fullPath, searchPattern);
				}
				catch(IOException e)
				{
					throw new IsolatedStorageException
						(_("IO_IsolatedStorage"), e);
				}
			}

#if CONFIG_PERMISSIONS

	// Get isolated storage permission information from a permission set.
	protected override IsolatedStoragePermission
				GetPermission(PermissionSet ps)
			{
				if(ps == null)
				{
					return null;
				}
				else if(ps.IsUnrestricted())
				{
					return new IsolatedStorageFilePermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return (IsolatedStoragePermission)
					  (ps.GetPermission(typeof(IsolatedStorageFilePermission)));
				}
			}

#endif

	// Get the base directory for an isolated storage scope.
	private static String GetBaseDirectory(IsolatedStorageScope scope)
			{
				// Find the base directory to start with.
				String baseDir;
				if(InfoMethods.GetPlatformID() == PlatformID.Unix)
				{
					// Use the user storage directory under Unix systems.
					baseDir = InfoMethods.GetUserStorageDir();
				}
				else if((scope & IsolatedStorageScope.Roaming) != 0)
				{
					// Use the roaming application data area under Win32.
					try
					{
						baseDir = Environment.GetFolderPath
							(Environment.SpecialFolder.ApplicationData);
					}
					catch(Exception)
					{
						return null;
					}
				}
				else
				{
					// Use the non-roaming application data area under Win32.
					try
					{
						baseDir = Environment.GetFolderPath
							(Environment.SpecialFolder.LocalApplicationData);
					}
					catch(Exception)
					{
						return null;
					}
				}
				if(baseDir == null || baseDir.Length == 0)
				{
					return null;
				}
				baseDir = Path.Combine(baseDir, "isolated-storage");

				// Add the assembly sub-directory name.
				if((scope & IsolatedStorageScope.Assembly) != 0)
				{
					String name = Assembly.GetEntryAssembly().FullName;
					int index = name.IndexOf(',');
					if(index != -1)
					{
						name = name.Substring(0, index);
					}
					baseDir = Path.Combine(baseDir, name);
				}
				else
				{
					baseDir = Path.Combine(baseDir, "default");
				}

				// Add the domain sub-directory name.
				if((scope & IsolatedStorageScope.Domain) != 0)
				{
					baseDir = Path.Combine
						(baseDir, AppDomain.CurrentDomain.ToString());
				}
				return baseDir;
			}

	// Get an isolated storage area.  We don't use evidence information
	// in this implementation because the underlying filesystem classes
	// take care of the security issues.
	private static IsolatedStorageFile GetStore(IsolatedStorageScope scope)
			{
				lock(typeof(IsolatedStorageFile))
				{
					// Search for an existing open reference to the scope.
					IsolatedStorageFile file;
					file = (IsolatedStorageFile)(stores[scope]);
					if(file != null)
					{
						++(file.refCount);
						return file;
					}

					// Get the base directory for the scope, which will also
					// check that the caller has sufficient permissions.
					String baseDirectory = GetBaseDirectory(scope);
					if(baseDirectory == null)
					{
						throw new IsolatedStorageException
							(_("IO_IsolatedStorage"));
					}

					// Make sure that the directory exists, because
					// it may have been removed previously.
					if(!Directory.Exists(baseDirectory))
					{
						try
						{
							Directory.CreateDirectory(baseDirectory);
						}
						catch(IOException e)
						{
							throw new IsolatedStorageException
								(_("IO_IsolatedStorage"), e);
						}
					}

					// Create a new isolated storage area.
					file = new IsolatedStorageFile(scope, baseDirectory);

					// Add the storage area to the global list.
					stores[scope] = file;

					// Return the store to the caller.
					return file;
				}
			}
	public static IsolatedStorageFile GetStore
				(IsolatedStorageScope scope, Object domainIdentity,
				 Object assemblyIdentity)
			{
				return GetStore(scope);
			}
	public static IsolatedStorageFile GetStore
				(IsolatedStorageScope scope, Type domainEvidenceType,
				 Type assemblyEvidenceType)
			{
				return GetStore(scope);
			}
	public static IsolatedStorageFile GetStore
				(IsolatedStorageScope scope, Evidence domainEvidence,
				 Type domainEvidenceType, Evidence assemblyEvidence,
				 Type assemblyEvidenceType)
			{
				return GetStore(scope);
			}

	// Get the user storage area for the current assembly.
	public static IsolatedStorageFile GetUserStoreForAssembly()
			{
				return GetStore(IsolatedStorageScope.User |
								IsolatedStorageScope.Assembly);
			}

	// Get the user storage area for the current domain.
	public static IsolatedStorageFile GetUserStoreForDomain()
			{
				return GetStore(IsolatedStorageScope.User |
								IsolatedStorageScope.Domain);
			}

	// Remove this isolated storage object.
	public override void Remove()
			{
				try
				{
					Directory.Delete(baseDirectory, true);
				}
				catch(IOException e)
				{
					throw new IsolatedStorageException
						(_("IO_IsolatedStorage"), e);
				}
			}

	// Remove all stores of a particular type.
	public static void Remove(IsolatedStorageScope scope)
			{
				IsolatedStorageFile file = GetStore(scope);
				try
				{
					file.Remove();
				}
				finally
				{
					file.Close();
				}
			}

	// Enumerate all storage areas of a particular type.
	public static IEnumerator GetEnumerator(IsolatedStorageScope scope)
			{
				IsolatedStorageFile store;

				// We can only do this for particular storage scopes.
				if(scope == IsolatedStorageScope.User ||
				   scope == (IsolatedStorageScope.User |
						     IsolatedStorageScope.Roaming))
				{
					try
					{
						store = GetStore(scope);
					}
					catch(Exception)
					{
						store = null;
					}
				}
				else
				{
					store = null;
				}

				// Build and return an enumerator for the scope that we found.
				return new ScopeEnumerator(store);
			}

	// Get the base directory for this isolated storage area.
	internal String BaseDirectory
			{
				get
				{
					return baseDirectory;
				}
			}

	// Enumerator class for isolated storage areas.
	private sealed class ScopeEnumerator : IEnumerator
	{
		// Internal state.
		private IsolatedStorageFile store;
		private IsolatedStorageFile current;
		private bool done;
		private bool reset;

		// Constructor.
		public ScopeEnumerator(IsolatedStorageFile store)
				{
					this.store = store;
					this.current = null;
					this.done = (store == null);
					this.reset = false;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(done)
					{
						current = null;
						return false;
					}
					else if(reset)
					{
						current = store;
						reset = false;
						return true;
					}
					else
					{
						current = null;
						done = true;
						return false;
					}
				}
		public void Reset()
				{
					done = (store == null);
					current = null;
					reset = true;
				}
		public Object Current
				{
					get
					{
						if(current == null)
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
						return current;
					}
				}

	}; // class ScopeEnumerator

}; // class IsolatedStorageFile

#endif // CONFIG_ISOLATED_STORAGE

}; // namespace System.IO.IsolatedStorage
