#!/bin/sh
#
# mkcvmdoc.sh - Make the CVM instruction set documentation.
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

# Usage: mkcvmdoc.sh top_srcdir top_builddir

# Validate the command-line.
if test "x$1" = "x" ; then
	exit 1
fi
if test "x$2" = "x" ; then
	exit 1
fi
top_srcdir="$1"
top_builddir="$2"

# Make the output directory.
if test ! -d "$top_builddir/doc/cvmdoc" ; then
	mkdir "$top_builddir/doc/cvmdoc"
fi

# Create the "cvm.xml" file from the interpreter sources.
if "$top_builddir/csdoc/src2xml" "$top_srcdir"/engine/cvm*.c >"$top_builddir/doc/cvmdoc/cvm.xml" ; then
	:
else
	rm -f "$top_builddir/doc/cvmdoc/cvm.xml"
	echo "$0: src2xml failed"
	exit 1
fi

# Run the "cvmdoc.py" script using Python.  This may fail if
# Python is not present or it doesn't have sufficient modules
# to process the XML input.  We continue in this case so that
# the main documentation build can complete.
if python "$top_srcdir/doc/cvmdoc.py" "$top_srcdir/doc/cvmdoc" "$top_srcdir/engine/cvm.h" <"$top_builddir/doc/cvmdoc/cvm.xml"; then
	:
else
	touch "$top_builddir/doc/cvmdoc/index.html"
	echo "$0: cvmdoc.py failed - dummy CVM documentation generated"
	echo "$0: this error is not serious and can be safely ignored"
	exit 0
fi

# Done.
exit 0
