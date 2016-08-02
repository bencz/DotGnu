#!/bin/sh
#
# auto_gen.sh - Generate the build files.
#
# Copyright (C) 2006  Free Software Foundation, Inc.
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
# Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

# Set the automake and aclocal binaries to the default.
AUTOMAKE_BIN=
ACLOCAL_BIN=

# Find a suitable version of automake.
AUTOMAKE_VERSION=`automake --version`
case "$AUTOMAKE_VERSION" in
	automake*1.7*) AUTOMAKE_BIN="automake"; ACLOCAL_BIN="aclocal" ;;
	automake*1.8*) AUTOMAKE_BIN="automake"; ACLOCAL_BIN="aclocal" ;;
	automake*1.9*) AUTOMAKE_BIN="automake"; ACLOCAL_BIN="aclocal" ;;
	automake*2.0*) AUTOMAKE_BIN="automake"; ACLOCAL_BIN="aclocal" ;;
	*)
		AUTOMAKE_BIN=`which automake-1.7`
		if test "x$AUTOMAKE_BIN" = "x"; then
			AUTOMAKE_BIN=`which automake-1.8`
			if test "x$AUTOMAKE_BIN" = "x"; then
				AUTOMAKE_BIN=`which automake-1.9`
				if test "x$AUTOMAKE_BIN" = "x"; then
					AUTOMAKE_BIN=`which automake-2.0`
					if test "x$AUTOMAKE_BIN" = "x"; then
						echo "error: unable to find a suitable version automake."
						exit 1
					else
						ACLOCAL_BIN=`which aclocal-2.0`
					fi
				else
					ACLOCAL_BIN=`which aclocal-1.9`
				fi
			else
				ACLOCAL_BIN=`which aclocal-1.8`
			fi
		else
			ACLOCAL_BIN=`which aclocal-1.7`
		fi
	;;
esac

# Ensure we have aclocal.
if test "x$ACLOCAL_BIN" = "x"; then
	echo "error: unable to find a suitable version of aclocal."
	exit 1
fi

# Print using message.
echo "Using automake: $AUTOMAKE_BIN"
echo "Using aclocal:  $ACLOCAL_BIN"

# Run aclocal to update the autoconf macros.
$ACLOCAL_BIN

# Run libtoolize, as needed.
if test ! -f "ltconfig" ; then
	libtoolize --copy 2>/dev/null
fi

# Run automake.
$AUTOMAKE_BIN --add-missing --copy --ignore-deps

# Run autoconf.
autoconf

# Exit successfully.
exit 0
