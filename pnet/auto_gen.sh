#!/bin/sh
#
# auto_gen.sh - Make the Makefile.in and configure files.
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

banner() {
        echo
        TG=`echo $1 | sed -e "s,/.*/,,g"`
        LINE=`echo $TG |sed -e "s/./-/g"`
        echo $LINE
        echo $TG
        echo $LINE
        echo
}

banner "running libtool"
libtoolize --copy --force || exit

banner "running aclocal"
aclocal --version
aclocal || exit

banner "running autoheader"
autoheader || exit

banner "running automake"
automake --add-missing --copy --ignore-deps || exit

banner "running autoconf"
autoconf

for dir in libffi libgc; do
		TOPSRCDIR=$PWD
		cd $TOPSRCDIR/$dir && sh auto_gen.sh;
		cd $TOPSRCDIR
done

