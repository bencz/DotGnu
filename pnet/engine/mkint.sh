#!/bin/sh
#
# Script that is used to generate "int_proto.h" and "int_table.c"
# from the compiled pnetlib assemblies.
#

# Try to locate the pnetlib sources.
if test -n "$1" ; then
	if test ! -f "$1/runtime/mscorlib.dll" ; then
		echo "$0: could not find mscorlib.dll in $1/runtime" 1>&2
		exit 1
	fi
	PNETLIB="$1"
else
	if test -f "../../pnetlib/runtime/mscorlib.dll" ; then
		PNETLIB="../../pnetlib"
	else
		echo "Usage: $0 DIR" 1>&2
		echo "where DIR is the pnetlib source directory" 1>&2
		exit 1
	fi
fi

# Convert the assemblies into the necessary internalcall tables.
# Make sure that the same locale is used while creating the tables because
# if that's not done to much noise is gererated by reordering the entries.
LANG=en_US
LC_ALL=en_US
export LANG LC_ALL

DLLS="$PNETLIB/runtime/mscorlib.dll $PNETLIB/System/System.dll $PNETLIB/I18N/I18N.CJK.dll $PNETLIB/DotGNU.Misc/DotGNU.Misc.dll"
../ilnative/ilinternal -p $DLLS >int_proto.h
../ilnative/ilinternal -t $DLLS >int_table.c
exit 0
