#! /bin/sh
#
# csccflags.sh - Determine the flags to supply to Portable.NET's "cscc".
#
# Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

# Find the compiler.
if test -n "$1" ; then
	CSCC="$1"
else
	CSCC="cscc"
fi
CSCC_DIR=`dirname "$CSCC"`

# Find the location of Portable.NET's source tree.
if test -n "$2" ; then
	PNET_DIR="$2"
else
	PNET_DIR=""
fi

# Add common options to the flags.
FLAGS=""

# Locate "ilasm" and add its path to the command-line options.
if test -x "$CSCC_DIR/ilasm" ; then
	FLAGS="$FLAGS -filasm-path=\"$CSCC_DIR/ilasm\""
else
	if test -n "$PNET_DIR" -a -x "$PNET_DIR/ilasm/ilasm" ; then
		FLAGS="$FLAGS -filasm-path=\"$PNET_DIR/ilasm/ilasm\""
	fi
fi

# Locate "cscc-cs" and add its path to the command-line options.
if test -x "$CSCC_DIR/cscc-cs" ; then
	FLAGS="$FLAGS -fplugin-cs-path=\"$CSCC_DIR/cscc-cs\""
fi

# Locate "cscc-java" and add its path to the command-line options.
if test -x "$CSCC_DIR/cscc-java" ; then
	FLAGS="$FLAGS -fplugin-java-path=\"$CSCC_DIR/cscc-java\""
fi

# Locate "cscc-c-s" and add its path to the command-line options.
if test -x "$CSCC_DIR/cscc-c-s" ; then
	FLAGS="$FLAGS -fplugin-c-path=\"$CSCC_DIR/cscc-c-s\""
fi

# Output the flags to stdout and exit.
echo "$FLAGS"
exit 0
