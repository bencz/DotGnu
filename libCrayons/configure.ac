dnl Process this file with autoconf to produce a configure script.

dnl
dnl
dnl ============================================================================
dnl  Versioning
dnl ============================================================================
dnl
dnl

dnl
dnl The package version is not the same as the shared library version and
dnl will be maintained according to the following rules:
dnl
dnl   major.minor.micro
dnl
dnl     major:
dnl       The major version number. This will be incremented when
dnl       the maintainer deems it appropriate.
dnl
dnl     minor:
dnl       The minor version number. This will be incremented when
dnl       the maintainer deems it appropriate.
dnl
dnl     micro:
dnl       The micro version number. This will be incremented every
dnl       release to an even number, and then again to an odd number
dnl       immediately following every release.
dnl
m4_define([CrayonsMajor], [0])
m4_define([CrayonsMinor], [0])
m4_define([CrayonsMicro], [1])

dnl
dnl The shared library version is not the same as the package version and
dnl must be maintained according to the following rules:
dnl
dnl   current:revision:age
dnl
dnl     current:
dnl       The current interface version. This must be incremented
dnl       whenever the public library interface is changed.
dnl
dnl     revision:
dnl       The revision number of the current interface. This must
dnl       be incremented whenever changes are made which do not
dnl       affect the public library interface, and reset to zero
dnl       whenever changes are made which do affect the public
dnl       library interface (i.e. whenever the current interface
dnl       version is incremented).
dnl
dnl     age:
dnl       The number of prior interface versions still supported.
dnl       This must be incremented whenever changes are made to the
dnl       public library interface which do not break compatibility
dnl       with the current interface version (i.e. addition of new
dnl       public features), and reset to zero whenever changes are
dnl       made which do break such compatibility (e.g. changing the
dnl       signature of a public function or method). This is in
dnl       addition to the incrementation of the current interface
dnl       version, which is required for either case.
dnl
m4_define([CrayonsCur], [1])
m4_define([CrayonsRev], [0])
m4_define([CrayonsAge], [0])

dnl
dnl
dnl ============================================================================
dnl  Initialization
dnl ============================================================================
dnl
dnl

# Initialize autoconf.
AC_PREREQ([2.57])
AC_INIT([libCrayons], [CrayonsMajor.CrayonsMinor.CrayonsMicro])
AC_CONFIG_SRCDIR([src/CGraphics.c])

# Initialize automake.
AM_INIT_AUTOMAKE
AM_CONFIG_HEADER(include/CrayonsConfig.h)

dnl
dnl
dnl ============================================================================
dnl  User Requests
dnl ============================================================================
dnl
dnl

# Enable testing, if requested.
CRAYONS_ARG_ENABLE(
	[testing],
	[make internal methods public],
	[no],
	[AC_DEFINE(
		[CTESTING],
		[1],
		[Define to 1 to make internal methods public.])],
	[])

# Disable debugging, if requested.
CRAYONS_ARG_ENABLE(
	[debugging],
	[disable debugging],
	[yes],
	[AC_DEFINE(
		[CDEBUG],
		[1],
		[Define to 1 to enable debugging.])],
	[])

dnl
dnl
dnl ============================================================================
dnl  Program Checks
dnl ============================================================================
dnl
dnl

# Check for programs.
AC_PROG_CC
AC_PROG_INSTALL
AC_PROG_LIBTOOL

dnl
dnl
dnl ============================================================================
dnl  Basic Header Checks
dnl ============================================================================
dnl
dnl

# Check for headers.
AC_HEADER_STDC
AC_HEADER_TIME
AC_CHECK_HEADERS([assert.h memory.h stdlib.h string.h strings.h unistd.h])
AC_CHECK_HEADERS([sys/types.h sys/stat.h errno.h fcntl.h])

dnl
dnl
dnl ============================================================================
dnl  Library/Header Checks
dnl ============================================================================
dnl
dnl

# Check for freetype configuration.
CRAYONS_CHECK_FREETYPE([7.0.1])
CRAYONS_CFLAGS="$FREETYPE_CFLAGS"
CRAYONS_LIBS="$FREETYPE_LIBS"

# Check for fontconfig configuration.
PKG_CHECK_MODULES([FONTCONFIG], [fontconfig >= 2.2.2])
CRAYONS_CFLAGS="$CRAYONS_CFLAGS $FONTCONFIG_CFLAGS"
CRAYONS_LIBS="$CRAYONS_LIBS $FONTCONFIG_LIBS"

# Check the availability of the math library.
CRAYONS_IF_MATH(
	[CRAYONS_REQUIRE_HEADER([math.h])],
	[AC_MSG_ERROR([libm not found but the math library is required.])])

# Check the availability of the pixman library.
CRAYONS_IF_PIXMAN(
	[CRAYONS_REQUIRE_HEADER([pixman.h])],
	[AC_MSG_ERROR([libpixman not found but the pixman library is required.])])

# Check the availability of the pthread library.
CRAYONS_IF_PTHREADS([CRAYONS_REQUIRE_HEADER([pthread.h])], [])

# Check the availability of Xlib.
CRAYONS_IF_XLIB(
	[crayons_x11_surface_define="@%:@define CRAYONS_HAVE_X11_SURFACE 1"
	 CRAYONS_REQUIRE_HEADER([X11/Xlib.h])
	 CRAYONS_CFLAGS="$CRAYONS_CFLAGS $X_CFLAGS"
	 CRAYONS_LDFLAGS="$X_PRE_LIBS $X_LIBS $X_EXTRA_LIBS -lX11"],
	[crayons_x11_surface_define="/* @%:@undef CRAYONS_HAVE_X11_SURFACE */"])

dnl
dnl
dnl ============================================================================
dnl  Compiler Feature Checks
dnl ============================================================================
dnl
dnl

# Check the availability of the const keyword in C.
CRAYONS_IF_C_CONST(
	[crayons_const_define="const"],
	[crayons_const_define=""])

dnl
dnl
dnl ============================================================================
dnl  Library Function Checks
dnl ============================================================================
dnl
dnl

# Check for library functions.
AC_FUNC_MALLOC
AC_FUNC_MEMCMP
AC_FUNC_REALLOC
AC_FUNC_STAT
AC_CHECK_FUNCS([bcmp bcopy memcmp memcpy memmove memset])
AC_CHECK_FUNCS([cos sin sqrt erf])
AC_CHECK_FUNCS([remove unlink stat access mkstemp open write])
AC_CHECK_FUNCS([gettimeofday getpid])

dnl
dnl
dnl ============================================================================
dnl  Type Checks
dnl ============================================================================
dnl
dnl

# Determine the integer types for various sizes on the target machine.
CRAYONS_CHECK_INTEGER_FOR_SIZE([1])
CRAYONS_CHECK_INTEGER_FOR_SIZE([2])
CRAYONS_CHECK_INTEGER_FOR_SIZE([4])
CRAYONS_CHECK_INTEGER_FOR_SIZE([8])

dnl
dnl
dnl ============================================================================
dnl  GCC Warnings
dnl ============================================================================
dnl
dnl

# Turn on warnings if compiling with gcc.
if test "x$GCC" = "xyes"; then
	CRAYONS_CFLAGS="$CRAYONS_CFLAGS \
		-Wall \
		-Wpointer-arith \
		-Wstrict-prototypes \
		-Wmissing-prototypes \
		-Wmissing-declarations \
		-Wnested-externs"
fi

dnl
dnl
dnl ============================================================================
dnl  Substitutions
dnl ============================================================================
dnl
dnl

# Shared library version.
AC_SUBST([CRAYONS_LIBRARY_VERSION], [CrayonsCur:CrayonsRev:CrayonsAge])dnl

# Package version.
AC_SUBST([crayons_version_major], [CrayonsMajor])dnl
AC_SUBST([crayons_version_minor], [CrayonsMinor])dnl
AC_SUBST([crayons_version_micro], [CrayonsMicro])dnl

# Compile and link flags.
AC_SUBST([CRAYONS_CFLAGS])
AC_SUBST([CRAYONS_LIBS])
AC_SUBST([CRAYONS_LDFLAGS])

# Integer types.
AC_SUBST([crayons_type_i1])
AC_SUBST([crayons_type_i2])
AC_SUBST([crayons_type_i4])
AC_SUBST([crayons_type_i8])

# Define const, as needed.
AC_SUBST([crayons_const_define])

# Features defines.
AC_SUBST([crayons_x11_surface_define])

dnl
dnl
dnl ============================================================================
dnl  Automake Conditionals
dnl ============================================================================
dnl
dnl

dnl
dnl NOTE: AM_CONDITIONAL is buggy, so be careful with the
dnl       syntax here (e.g. no quoting the first argument)
dnl

# Define pthread availability test.
AM_CONDITIONAL(CRAYONS_PTHREADS_ENABLED, [test "$CRAYONS_HAVE_PTHREADS" = "yes"])

# Define xlib availability test.
AM_CONDITIONAL(CRAYONS_X11_ENABLED, [test "$CRAYONS_HAVE_XLIB" = "yes"])

dnl
dnl
dnl ============================================================================
dnl  Finalization
dnl ============================================================================
dnl
dnl

dnl
dnl NOTE: AC_CONFIG_FILES is buggy, so be careful with the
dnl       syntax here (e.g. no quoting the list)
dnl

# Set output files.
AC_CONFIG_FILES(
	Makefile
	include/Makefile
	src/Makefile
	samples/Makefile
	include/CrayonsFeatures.h)

AC_OUTPUT
