#!/bin/sh
#
# make-zip.sh - Make the Windows .zip file package.
#
# Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

DISTDIR=/tmp/tc$$

# Create the directory structure.
mkdir $DISTDIR
mkdir $DISTDIR/bin
mkdir $DISTDIR/doc
mkdir $DISTDIR/doc/html

# Copy across the various files.
cp -p doc/binary_readme.txt $DISTDIR/Readme.txt
cp -p COPYING $DISTDIR
if test -f "treecc.exe" ; then
	cp -p treecc.exe $DISTDIR/bin
else
	cp -p treecc $DISTDIR/bin
fi
nroff -man doc/treecc.1 | sed -e '1,$s/.//g' >$DISTDIR/doc/treecc.1
cp -p doc/essay.html $DISTDIR/doc/intro.html
cd doc
sh ./mkhtml $DISTDIR/doc/html
sh ./mkpdf
cp -p treecc.pdf $DISTDIR/doc
cd ..

# Get the version number, to use when creating the zip file.
VERSION=`grep '^AM_INIT_AUTOMAKE' configure.in | \
	sed -e '1,$s/^.*treecc, //g' |
	sed -e '1,$s/)//g'`

# Zip up the result, being careful to use CRLF for end of lines.
ZIPFILE=`pwd`/../treecc-$VERSION.zip
cd $DISTDIR
rm -f $ZIPFILE
zip -l $ZIPFILE Readme.txt COPYING
zip -gr $ZIPFILE bin
zip -g $ZIPFILE doc doc/treecc.pdf
zip -glr $ZIPFILE doc/treecc.1 doc/intro.html doc/html
echo Result is in $ZIPFILE

# Clean up and exit.
cd /tmp
rm -rf $DISTDIR
exit 0
