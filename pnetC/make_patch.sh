#!/bin/sh
#
# make_patch.sh - Make a patch file from the changes in a checked-out
#                 anonymous CVS source tree.
#
# Usage: make_patch.sh [-o patchfile] [file ...]
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

# Determine which temporary files to use, and arrange for them
# to be clean up on exit.
CHANGEDFILES="/tmp/mp$$"
CHANGEDFILES_COPY="/tmp/cvs_update_$$.log"
COMMENTS="/tmp/mpc$$"
trap 'rm -f ${CHANGEDFILES} ${COMMENTS}' 0 1 2 15

# Make sure that this is a CVS version of the tree, and not a .tar.gz version.
PROGNAME="$0"
if test ! -f "CVS/Root" ; then
	echo "${PROGNAME}: this script must be used with the CVS version of the source" 1>&2
	exit 1
fi

# Make sure that we are in the correct place to run this script.
if test ! -f "make_patch.sh" ; then
	echo "${PROGNAME}: you must be in the base directory to run this script" 1>&2
	exit 1
fi

# Get the name of the patch file.
if test "x$1" = "x-o" ; then
	PATCHFILE="$2"
	shift
	shift
else
	if test -z "$USER" ; then
		PATCHFILE="`date +%y%m%d`.patch"
	else
		PATCHFILE="$USER-`date +%y%m%d`.patch"
	fi
fi

# Define a function that silently searches for a given pattern.
# $? will be 0 if the pattern was found
grep_qs()
{
	grep -q -s "$1" $2
}

# Determine the list of files to be patched.
if test -z "$1" ; then
	# Contact the CVS server to collect up a list of all changed files.
	echo 'Contacting the CVS server to determine which files have changed' 1>&2
	echo '(this will update your source tree).' 1>&2
	if cvs -z3 update -d >${CHANGEDFILES} 2>/dev/null ; then
		:
	else
		echo "${PROGNAME}: 'cvs -z3 update -d' command failed"

		# We can get here when there are conflicts in the update,
		# so let the user see the log file.
		cp ${CHANGEDFILES} ${CHANGEDFILES_COPY}
		echo "(See ${CHANGEDFILES_COPY} for details)." 1>&2
		exit 1
	fi

	grep_qs "^[UP] " ${CHANGEDFILES}
	if test $? -eq 0 ; then
		echo 'Your source tree was not up-to-date. Please check that you still' 1>&2
		echo 'have a clean build, and then rerun this script.' 1>&2
		cp ${CHANGEDFILES} ${CHANGEDFILES_COPY}
		echo "(See ${CHANGEDFILES_COPY} for details)." 1>&2
		exit 1
	fi

	grep_qs "^C " ${CHANGEDFILES}
	if test $? -eq 0 ; then
		echo 'There were conflicts during the update of your source tree.' 1>&2
		echo 'Please fix them before rerunning this script.' 1>&2
		cp ${CHANGEDFILES} ${CHANGEDFILES_COPY}
		echo "(See ${CHANGEDFILES_COPY} for details)." 1>&2
		exit 1
	fi

	grep_qs "^Merging differences" ${CHANGEDFILES}
	if test $? -eq 0 ; then
		echo 'Your source tree was not up-to-date (merges were made).' 1>&2
		echo 'Please check that you still have a clean build, and then' 1>&2
		echo 'rerun this script.' 1>&1
		cp ${CHANGEDFILES} ${CHANGEDFILES_COPY}
		echo "(See ${CHANGEDFILES_COPY} for details)." 1>&2
		exit 1
	fi

else
	# Use the file list on the command-line.
	>${CHANGEDFILES}
	for file in $* ; do
		if test -f "${file}" ; then
			# Determine if the file is under CVS control or not.
			DIR=`dirname ${file}`
			BASE=`basename ${file}`
			IS_CVS=0
			if test -f "${DIR}/CVS/Entries" ; then
				LINE=`grep "^/${BASE}/" "${DIR}/CVS/Entries"`
				if test -n "$LINE" ; then
					IS_CVS=1
				fi
			fi
			if test "${IS_CVS}" = "1" ; then
				# This is an existing file that has been modified.
				echo "M ${file}" >>${CHANGEDFILES}
			else
				# This is a new file.
				echo "? ${file}" >>${CHANGEDFILES}
			fi
		else
			# One of the file arguments is incorrect.
			if test -e "${file}" ; then
				echo "${file}: Not a regular file" 1>&2
			else
				echo "${file}: No such file or directory" 1>&2
			fi
			exit 1
		fi
	done
fi

# Define a function for reporting comments for existing files.
comment_existing()
{
	case "$1" in
		\?) ;;
		 *)	echo "MP:    $2" >>${COMMENTS} ;;
	esac
	return 0
}

# Define a function for reporting comments for changed files.
comment_changed()
{
	case "$1" in
		\?) case "$2" in
			  *.c|*.cc|*.h|*.cs|*.tc|*.y|*.yy|*.l|*.ll|*.sh|*.am|*.html|*.texi|*.in)
			  		echo "MP:    $2" >>${COMMENTS} ;;
			  *) ;;
		    esac ;;
		 *)	;;
	esac
	return 0
}

# Create a default comment file.
echo '' >${COMMENTS}
echo 'MP: ----------------------------------------------------------------------' >>${COMMENTS}
echo 'MP: Enter patch description.  Lines beginning with `MP:'"'"' are removed' >>${COMMENTS}
echo 'MP: automatically' >>${COMMENTS}
echo 'MP:' >>${COMMENTS}
echo 'MP: Modified Files:' >>${COMMENTS}
while read LINE ; do
	comment_existing $LINE
done <${CHANGEDFILES}
echo 'MP: New Files:' >>${COMMENTS}
while read LINE ; do
	comment_changed $LINE
done <${CHANGEDFILES}
echo 'MP: ----------------------------------------------------------------------' >>${COMMENTS}

# Pop up the user's editor to create comment information.
if test -z "$EDITOR" ; then
	EDITOR=vi
fi
$EDITOR ${COMMENTS}

# Ask the user if they want to abort or continue.
echo -n '(a)bort or (c)ontinue? '
read LINE
case "$LINE" in
	""|c|C) ;;
	*) exit 1 ;;
esac

# Create the patch file and write the header to it.
echo "Writing patch header ..." 1>&2
echo "# Patch created by $USER" >"${PATCHFILE}"
echo "# Date: `date`" >>"${PATCHFILE}"
PWDOUT=`pwd`
echo "# Repository: `basename ${PWDOUT}`" >>"${PATCHFILE}"
echo "# Comments:" >>"${PATCHFILE}"
grep -v '^MP:' ${COMMENTS} | sed -e '1,$s/^/# /g' - >>"${PATCHFILE}"
echo '#### End of Preamble ####' >>"${PATCHFILE}"
echo '' >>"${PATCHFILE}"
echo '#### Patch data follows ####' >>"${PATCHFILE}"

# Define a function for processing the existing files.
existing()
{
	case "$1" in
		\?) ;;
		 *)	cvs -z3 diff -c "$2" >>"${PATCHFILE}" 2>/dev/null ;;
	esac
	return 0
}

# Define a function for processing the changed files.
changed()
{
	case "$1" in
		\?) case "$2" in
			  *.c|*.cc|*.h|*.cs|*.tc|*.y|*.yy|*.l|*.ll|*.sh|*.am|*.html|*.texi|*.in)
					echo "Index: $2" >>"${PATCHFILE}"
					echo '===================================================================' >>"${PATCHFILE}"
					diff -c /dev/null "$2" >>"${PATCHFILE}" ;;
			  *) ;;
		    esac ;;
		 *)	;;
	esac
	return 0
}

# Process the existing files in the list.
echo 'Creating the patch data for existing files ...' 1>&2
while read LINE ; do
	existing $LINE
done <${CHANGEDFILES}

# Process the new files in the list.
echo 'Creating the patch data for new files ...' 1>&2
while read LINE ; do
	changed $LINE
done <${CHANGEDFILES}

# Terminate the patch data.
echo "Writing patch footer ..." 1>&2
echo '#### End of Patch data ####' >>"${PATCHFILE}"
echo "The patch data has been written to ${PATCHFILE}" 1>&2

# Finished.
exit 0
