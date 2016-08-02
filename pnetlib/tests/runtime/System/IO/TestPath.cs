/*
 * TestPath.cs - Test the "Path" class.
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

using CSUnit;
using System;
using System.IO;

public class TestPath : TestCase
{
	// Constructor.
	public TestPath(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				// Nothing to do here.
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	// Test the path character properties.
	public void TestPathCharacters()
			{
				char sep = Path.DirectorySeparatorChar;
				Assert("DirSep", (sep == '/' || sep == '\\'));
				sep = Path.AltDirectorySeparatorChar;
				Assert("AltDirSep", (sep == '/' || sep == '\\' || sep == '\0'));
				sep = Path.PathSeparator;
				Assert("PathSep", (sep == ':' || sep == ';'));
			#if !ECMA_COMPAT
				char vsep = Path.VolumeSeparatorChar;
				Assert("VolumeSep", (vsep == ':' || vsep == '\0'));
				if(vsep == ':')
				{
					Assert("PathSepNotColon", (sep != ':'));
				}
			#endif
			}

	// Check two paths for equal, taking separator differences into account.
	private void AssertPathEquals(String name, String expected, String actual)
			{
				// Check the null cases.
				if(expected == null)
				{
					AssertNull(name + " [1]", actual);
					return;
				}
				AssertNotNull(name + " [2]", actual);

				// Check that the path is correctly normalized.
				if(Path.DirectorySeparatorChar == '\\')
				{
					if(actual.IndexOf('/') != -1)
					{
						Fail(name + " [3]");
					}
					actual = actual.Replace('\\', '/');
				}
				else
				{
					if(actual.IndexOf('\\') != -1)
					{
						Fail(name + " [4]");
					}
				}

				// Check the paths for equality.
				if(String.Compare(expected, actual, true) != 0)
				{
					AssertEquals(name + " [5]", expected, actual);
				}
			}

	// Test the Combine method.
	public void TestPathCombine()
			{
				try
				{
					Path.Combine(null, "foo");
					Fail("Combine (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Path.Combine("foo", null);
					Fail("Combine (2)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				AssertPathEquals("Combine (3)", "foo/bar",
								 Path.Combine("", "foo/bar"));
				AssertPathEquals("Combine (4)", "foo/bar",
								 Path.Combine("foo\\bar", ""));
				AssertPathEquals("Combine (5)", "c:",
								 Path.Combine("/foo", "c:"));
				AssertPathEquals("Combine (6)", "/",
								 Path.Combine("foo", "/"));
				AssertPathEquals("Combine (7)", "c:a",
								 Path.Combine("c:", "a"));
				AssertPathEquals("Combine (8)", "c:/a",
								 Path.Combine("c:\\", "a"));
				AssertPathEquals("Combine (9)", "a/b",
								 Path.Combine("a", "b"));
				AssertPathEquals("Combine (10)", "a/b",
								 Path.Combine("a\\", "b"));
				AssertPathEquals("Combine (10)", "a/b/../c",
								 Path.Combine("a\\", "b/../c"));
			}

	// Test the GetDirectoryName method.
	public void TestPathGetDirectoryName()
			{
				AssertPathEquals("GetDirectoryName (1)", null,
						   		 Path.GetDirectoryName(null));
				try
				{
					Path.GetDirectoryName("");
					Fail("GetDirectoryName (2)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				AssertPathEquals("GetDirectoryName (3)", null,
							     Path.GetDirectoryName("/"));
				AssertPathEquals("GetDirectoryName (4)", "/",
							     Path.GetDirectoryName("\\foo"));
				AssertPathEquals("GetDirectoryName (5)", "",
							     Path.GetDirectoryName("a"));
				AssertPathEquals("GetDirectoryName (6)", "a",
							     Path.GetDirectoryName("a/b"));
				AssertPathEquals("GetDirectoryName (7)", "a",
							     Path.GetDirectoryName("a/"));
				AssertPathEquals("GetDirectoryName (8)", null,
							     Path.GetDirectoryName("c:"));
				AssertPathEquals("GetDirectoryName (9)", null,
							     Path.GetDirectoryName("c:\\"));
				AssertPathEquals("GetDirectoryName (10)", "c:/",
							     Path.GetDirectoryName("c:\\a"));
				AssertPathEquals("GetDirectoryName (10)", "a/b/c",
							     Path.GetDirectoryName("a/b/c/d"));
				try
				{
					Path.GetDirectoryName("\\\\foo");
					Fail("GetDirectoryName (11)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					Path.GetDirectoryName("\\\\foo\\");
					Fail("GetDirectoryName (12)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				AssertPathEquals("GetDirectoryName (13)", null,
							     Path.GetDirectoryName("\\\\foo\\bar"));
				AssertPathEquals("GetDirectoryName (14)", "//foo/bar",
							     Path.GetDirectoryName("\\\\foo\\bar\\baz"));
			}

	// Test the GetFileName method.
	public void TestPathGetFileName()
			{
				AssertNull("GetFileName (1)", Path.GetFileName(null));
				AssertEquals("GetFileName (2)", "",
							 Path.GetFileName(""));
				AssertEquals("GetFileName (3)", "",
							 Path.GetFileName("/"));
				AssertEquals("GetFileName (4)", "a",
							 Path.GetFileName("a"));
				AssertEquals("GetFileName (5)", "b",
							 Path.GetFileName("a\\b"));
				AssertEquals("GetFileName (6)", "",
							 Path.GetFileName("c:"));
				AssertEquals("GetFileName (7)", "",
							 Path.GetFileName("c:\\"));
				AssertEquals("GetFileName (8)", "foo",
							 Path.GetFileName("\\\\foo"));
				AssertEquals("GetFileName (9)", "bar",
							 Path.GetFileName("\\\\foo\\bar"));
			}

	// Test the GetFileNameWithoutExtension method.
	public void TestPathGetFileNameWithoutExtension()
			{
				AssertNull("GetFileNameWithoutExtension (1)",
						   Path.GetFileName(null));
				AssertEquals("GetFileNameWithoutExtension (2)", "",
							 Path.GetFileNameWithoutExtension(""));
				AssertEquals("GetFileNameWithoutExtension (3)", "",
							 Path.GetFileNameWithoutExtension("/"));
				AssertEquals("GetFileNameWithoutExtension (4)", "a",
							 Path.GetFileNameWithoutExtension("a"));
				AssertEquals("GetFileNameWithoutExtension (5)", "b",
							 Path.GetFileNameWithoutExtension("a\\b"));
				AssertEquals("GetFileNameWithoutExtension (6)", "",
							 Path.GetFileNameWithoutExtension("c:"));
				AssertEquals("GetFileNameWithoutExtension (7)", "",
							 Path.GetFileNameWithoutExtension("c:\\"));
				AssertEquals("GetFileNameWithoutExtension (8)", "foo",
							 Path.GetFileNameWithoutExtension("\\\\foo"));
				AssertEquals("GetFileNameWithoutExtension (9)", "bar",
							 Path.GetFileNameWithoutExtension("\\\\foo\\bar"));
				AssertEquals("GetFileNameWithoutExtension (10)", "a",
							 Path.GetFileNameWithoutExtension("a."));
				AssertEquals("GetFileNameWithoutExtension (11)", "a",
							 Path.GetFileNameWithoutExtension("a.exe"));
				AssertEquals("GetFileNameWithoutExtension (12)", "a",
							 Path.GetFileNameWithoutExtension("/foo/a.exe"));
				AssertEquals("GetFileNameWithoutExtension (13)", "a.b",
							 Path.GetFileNameWithoutExtension("a.b.c"));
			}

	// Test the GetExtension method.
	public void TestPathGetExtension()
			{
				AssertNull("GetExtension (1)",
						   Path.GetExtension(null));
				AssertEquals("GetExtension (2)", "",
							 Path.GetExtension(""));
				AssertEquals("GetExtension (3)", "",
							 Path.GetExtension("/"));
				AssertEquals("GetExtension (4)", "",
							 Path.GetExtension("a"));
				AssertEquals("GetExtension (5)", "",
							 Path.GetExtension("a\\b"));
				AssertEquals("GetExtension (6)", "",
							 Path.GetExtension("c:"));
				AssertEquals("GetExtension (7)", "",
							 Path.GetExtension("c:\\"));
				AssertEquals("GetExtension (8)", "",
							 Path.GetExtension("\\\\foo"));
				AssertEquals("GetExtension (9)", "",
							 Path.GetExtension("\\\\foo\\bar"));
				AssertEquals("GetExtension (10)", "",
							 Path.GetExtension("a."));
				AssertEquals("GetExtension (11)", ".exe",
							 Path.GetExtension("a.exe"));
				AssertEquals("GetExtension (12)", ".exe",
							 Path.GetExtension("/foo/a.exe"));
				AssertEquals("GetExtension (13)", ".c",
							 Path.GetExtension("a.b.c"));
				AssertEquals("GetExtension (14)", "",
							 Path.GetExtension("a.b."));
			}

	// Test the GetFullPath method.
	public void TestPathGetFullPath()
			{
				// Make sure that we aren't on drive X, if one exists
				// on this system, to prevent test mismatches below.
				String dir = Directory.GetCurrentDirectory();
				if(dir.Length >= 2)
				{
					if((dir[0] == 'x' || dir[0] == 'X') && dir[1] == ':')
					{
						Directory.SetCurrentDirectory
							("c:" + Path.DirectorySeparatorChar.ToString());
					}
				}

				// Run the tests.
				try
				{
					Path.GetFullPath(null);
					Fail("GetFullPath (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Path.GetFullPath("");
					Fail("GetFullPath (2)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					// Network share with host, but no root path.
					Path.GetFullPath("\\\\foo");
					Fail("GetFullPath (3)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				AssertPathEquals("GetFullPath (4)", "x:/abc",
								 Path.GetFullPath("x:\\abc"));
				AssertPathEquals("GetFullPath (5)", "x:/abc",
								 Path.GetFullPath("x:abc"));
				AssertPathEquals("GetFullPath (6)", "x:/def",
								 Path.GetFullPath("x:/abc/../def"));
				AssertPathEquals("GetFullPath (7)", "x:/",
								 Path.GetFullPath("x:"));
				AssertPathEquals("GetFullPath (8)", "x:/",
								 Path.GetFullPath("x:/"));
				AssertPathEquals("GetFullPath (9)", "/def",
								 Path.GetFullPath("/../def"));
				AssertPathEquals("GetFullPath (10)", "//foo/bar",
								 Path.GetFullPath("\\\\foo\\bar"));
				AssertPathEquals("GetFullPath (11)", "//foo/bar",
								 Path.GetFullPath("\\\\foo\\bar\\.."));
				AssertPathEquals("GetFullPath (12)", "/a/c/e",
								 Path.GetFullPath("/a/b/.././c/d/../e"));
			}

	// Test the GetPathRoot method.
	public void TestPathGetPathRoot()
			{
				try
				{
					Path.GetPathRoot(null);
					Fail("GetPathRoot (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Path.GetPathRoot("");
					Fail("GetPathRoot (2)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					// Network share with host, but no root path.
					Path.GetPathRoot("\\\\foo");
					Fail("GetPathRoot (3)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				AssertPathEquals("GetPathRoot (4)", "x:/",
								 Path.GetPathRoot("x:\\abc"));
				AssertPathEquals("GetPathRoot (5)", "x:/",
								 Path.GetPathRoot("x:abc"));
				AssertPathEquals("GetPathRoot (6)", "x:/",
								 Path.GetPathRoot("x:/abc/../def"));
				AssertPathEquals("GetPathRoot (7)", "x:/",
								 Path.GetPathRoot("x:"));
				AssertPathEquals("GetPathRoot (8)", "x:/",
								 Path.GetPathRoot("x:/"));
				AssertPathEquals("GetPathRoot (9)", "/",
								 Path.GetPathRoot("/../def"));
				AssertPathEquals("GetPathRoot (10)", "//foo/bar",
								 Path.GetPathRoot("\\\\foo\\bar"));
				AssertPathEquals("GetPathRoot (11)", "//foo/bar",
								 Path.GetPathRoot("\\\\foo\\bar\\.."));
				AssertPathEquals("GetPathRoot (12)", "//foo/bar",
								 Path.GetPathRoot("\\\\foo\\bar\\baz"));
				AssertPathEquals("GetPathRoot (13)", "/",
								 Path.GetPathRoot("/a/b/.././c/d/../e"));
			}

	// Test the GetTempPath method.
	public void TestPathGetTempPath()
			{
				String temp = Path.GetTempPath();
				AssertNotNull("GetTempPath (1)", temp);
				Assert("GetTempPath (2)", temp.Length > 0);
			}

	// Test the HasExtension method.
	public void TestPathHasExtension()
			{
				Assert("HasExtension (1)", !Path.HasExtension(null));
				Assert("HasExtension (2)", !Path.HasExtension(""));
				Assert("HasExtension (3)", !Path.HasExtension("/"));
				Assert("HasExtension (4)", !Path.HasExtension("a"));
				Assert("HasExtension (5)", !Path.HasExtension("a\\b"));
				Assert("HasExtension (6)", !Path.HasExtension("c:"));
				Assert("HasExtension (7)", !Path.HasExtension("c:\\"));
				Assert("HasExtension (8)", !Path.HasExtension("\\\\foo"));
				Assert("HasExtension (9)", !Path.HasExtension("\\\\foo\\bar"));
				Assert("HasExtension (10)", !Path.HasExtension("a."));
				Assert("HasExtension (11)", Path.HasExtension("a.exe"));
				Assert("HasExtension (12)", Path.HasExtension("/foo/a.exe"));
				Assert("HasExtension (13)", Path.HasExtension("a.b.c"));
				Assert("HasExtension (14)", !Path.HasExtension("a.b."));
			}

	// Test the IsPathRooted method.
	public void TestPathIsPathRooted()
			{
				Assert("IsPathRooted (1)", !Path.IsPathRooted(null));
				Assert("IsPathRooted (2)", Path.IsPathRooted("/"));
				Assert("IsPathRooted (3)", !Path.IsPathRooted("a"));
				Assert("IsPathRooted (4)", !Path.IsPathRooted(""));
				Assert("IsPathRooted (5)", Path.IsPathRooted("c:\\"));
				Assert("IsPathRooted (6)", Path.IsPathRooted("c:"));
				Assert("IsPathRooted (7)", Path.IsPathRooted("\\\\foo"));
			}

}; // class TestPath
