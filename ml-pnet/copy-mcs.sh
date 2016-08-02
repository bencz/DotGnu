#!/bin/sh
#
# copy-mcs.sh - Copy the mcs sources for use in a release tarball.
#
# Copyright (C) 2003  Southern Storm Software, Pty Ltd.
#
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

if test -z "$1" ; then
	echo "Usage: $0 mcs-location" 1>&2
	exit 1
fi
if test ! -f "$1/class/library.make" ; then
	if test ! -f "$1/build/library.make" ; then
		echo "Could not fine mcs in the specified directory" 1>&2
		exit 1
	fi
fi
MCS_SOURCES="$1"

mkdir mcs-sources
mkdir mcs-sources/build
mkdir mcs-sources/class
mkdir mcs-sources/class/corlib
mkdir mcs-sources/class/corlib/System
mkdir mcs-sources/class/System
mkdir mcs-sources/class/System.XML
mkdir mcs-sources/tools

if test -f "$MCS_SOURCES/class/library.make" ; then
	cp -p "$MCS_SOURCES/class/library.make" mcs-sources/class/library.make
else
	cp -p "$MCS_SOURCES/build/library.make" mcs-sources/build/library.make
fi
cp -p "$MCS_SOURCES/class/corlib/System/TODOAttribute.cs" mcs-sources/class/corlib/System/TODOAttribute.cs
cp -pr "$MCS_SOURCES/class/corlib/Test" mcs-sources/class/corlib
cp -pr "$MCS_SOURCES/class/System/Test" mcs-sources/class/System
cp -pr "$MCS_SOURCES/class/System.XML/Test" mcs-sources/class/System.XML
if test -d "$MCS_SOURCES/tools/sqlsharp" ; then
	cp -pr "$MCS_SOURCES/tools/sqlsharp" mcs-sources/tools
else
	mkdir mcs-sources/tools/sqlsharp
	cp -pr "$MCS_SOURCES"/tools/SqlSharp/* mcs-sources/tools/sqlsharp
fi
cp -pr "$MCS_SOURCES/tools/wsdl" mcs-sources/tools
cp -pr "$MCS_SOURCES/jay" mcs-sources/
cp -pr "$MCS_SOURCES/nunit20" mcs-sources/

for dir in Custommarshalers \
		  System.Configuration.Install \
		  System.Management \
		  System.Messaging \
		  System.ServiceProcess \
		  System.Runtime.Serialization.Formatters.Soap \
		  System.Runtime.Remoting \
		  PEAPI \
		  Mono.Data.Tds \
		  System.Data \
		  Mono.Data.SqliteClient \
		  Mono.Data.SybaseClient \
		  Mono.Data.TdsClient \
		  ByteFX.Data \
		  IBM.Data.DB2 \
		  Npgsql \
		  System.Data.OracleClient \
		  System.Web \
		  System.Web.Services \
		  Mono.GetOptions \
		  Mono.Posix \
		  Mono.Cairo \
		  Mono.Http \
		  Novell.Directory.Ldap \
		  System.DirectoryServices \
		  Mono.Security \
		  Mono.Security.Win32 \
		  System.Security ; do

	cp -pr "$MCS_SOURCES/class/$dir" "mcs-sources/class/$dir"

done

find mcs-sources -name CVS -print | xargs rm -rf

exit 0
