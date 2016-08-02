dnl Process this file with autoconf to produce a configure script.
AC_INIT([GNU Treecc], [0.3.11], [dotgnu-pnet@gnu.org])
AC_CONFIG_SRCDIR([input.h])
AC_CANONICAL_TARGET
AM_INIT_AUTOMAKE
AC_CONFIG_HEADERS([config.h])
AM_MAINTAINER_MODE

dnl Turn off the cygwin library if building for Win32.
AC_MSG_CHECKING([if building for some Win32 platform])
case "$host" in
	*-*-mingw*|*-*-cygwin*)
		platform_win32=yes
		CC="gcc -mno-cygwin"
		;;
	*-*-mingw*)
		platform_win32=yes
		;;
	*)
		platform_win32=no
		;;
esac
AC_MSG_RESULT($platform_win32)

AC_MSG_CHECKING([if building for BeOS platform])
case "$host" in
	*-*-beos*)
		platform_beos=yes
		LDADD=""
		;;
	*)
		platform_beos=no
		LDADD="-lm"
		;;
esac
AC_MSG_RESULT($platform_beos)
AC_SUBST(LDADD)

dnl Checks for programs.
AC_PROG_AWK
AC_PROG_CC
AC_PROG_INSTALL
AC_PROG_LN_S
AC_PROG_MAKE_SET
AC_PROG_RANLIB
AC_PROG_YACC
AM_PROG_LEX

dnl Check for file extensions.
AC_EXEEXT
AC_OBJEXT

dnl Checks for header files.
AC_HEADER_STDC
AC_CHECK_HEADERS(string.h strings.h stdlib.h stdarg.h varargs.h)
AC_CHECK_HEADERS(unistd.h sys/types.h sys/stat.h)

dnl Checks for typedefs, structures, and compiler characteristics.
AC_C_CONST

dnl Check to see if we are using gcc or not.
if test x$GCC = xyes ; then
	CFLAGS="$CFLAGS -Wall"
fi

dnl Checks for library functions.
AC_CHECK_FUNCS(vfprintf strchr qsort vsnprintf _vsnprintf vsprintf)
AC_CHECK_FUNCS(memcmp memcpy access stat)

dnl Determine if we can compile the C++ example, which
dnl uses bison-specific yacc features.
AC_SUBST(EXPR_CPP)
if test "x$YACC" = "xbison -y" ; then
	EXPR_CPP=expr_cpp
fi

dnl Check for building on a multi os system
if test x$GCC = xyes ; then
	multi_os_directory=`$CC -print-multi-os-directory`
	case $multi_os_directory in
	  .) ;; # Avoid trailing /.
	  *) libdir=$libdir/$multi_os_directory ;;
	esac
fi
AC_SUBST(libdir)

AC_CONFIG_FILES([
Makefile
etc/Makefile
doc/Makefile
tests/Makefile
examples/Makefile
treecc.spec])
AC_OUTPUT
