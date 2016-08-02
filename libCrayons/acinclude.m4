dnl
dnl Determine the size of types on the build target.
dnl
dnl   $1 - type (e.g. unsigned int)
dnl
dnl  NOTE: this function defines CRAYONS_SIZEOF_{TYPE} (where
dnl        {TYPE} is equal to $1 with the type name converted
dnl        to upper-case, spaces converted to underscores, and
dnl        asterisks converted to 'P') to the size of the type
dnl        in bytes; in addition, the cache variable
dnl        crayons_cv_sizeof_{type} (where {type} is equal to
dnl        {TYPE}, with all characters in lower-case) is also
dnl        set to the size of the type in bytes
dnl
AC_DEFUN(
	[CRAYONS_COMPILE_CHECK_SIZEOF],
	[m4_define(
		[CRAYONS_TYPE_NAME],
		[m4_translit([crayons_sizeof_$1], [[a-z *]], [[A-Z_P]])])dnl
	 m4_define(
		[CRAYONS_CV_NAME],
		[m4_translit([crayons_cv_sizeof_$1], [[ *]], [[_p]])])dnl
	 AC_CACHE_CHECK(
		[the size of $1],
		[CRAYONS_CV_NAME],
		[for crayons_size in 4 8 1 2 16; do
			AC_COMPILE_IFELSE(
				[AC_LANG_PROGRAM(
					[#include "confdefs.h"
					 #include <sys/types.h>],
					[switch(0) case 0: case (sizeof($1) == $crayons_size):;])],
				[CRAYONS_CV_NAME=$crayons_size])
			if test x$CRAYONS_CV_NAME != x; then break; fi
		done])
	 if test x$CRAYONS_CV_NAME = x; then
		AC_MSG_ERROR([cannot determine the size of $1])
	 fi
	 AC_DEFINE_UNQUOTED(
		[CRAYONS_TYPE_NAME],
		[$CRAYONS_CV_NAME],
		[The size in bytes of $1.])
	 m4_undefine([CRAYONS_TYPE_NAME])dnl
	 m4_undefine([CRAYONS_CV_NAME])])

dnl
dnl Determine the type for a given size in bytes.
dnl
dnl   $1 - class of type (e.g. "integer", "floating point")
dnl   $2 - prefix (e.g. "i" for integers)
dnl   $3 - size in bytes
dnl   $4 - prologue for use in AC_LANG_PROGRAM
dnl   $5 - types (e.g. "int" "long" "long long")
dnl
dnl  NOTE: this function sets crayons_type_{prefix}{size} (where
dnl        {prefix} is equal to $2 and {size} is equal to $3) to
dnl        the type (among those in $5) which is exactly {size}
dnl        bytes in size, using the class of type (i.e. $1) for
dnl        printing checking and error messages
dnl
AC_DEFUN(
	[CRAYONS_CHECK_TYPE_FOR_SIZE],
	[m4_define(
		[CRAYONS_TYPE_NAME],
		[crayons_type_$2$3])dnl
	 m4_define(
		[CRAYONS_CV_NAME],
		[crayons_cv_type_$2$3])dnl
	 m4_define(
		[CRAYONS_CLASS_NAME],
		[crayons_class_type_$2$3])dnl
	 m4_ifvaln(
			[$1],
			[CRAYONS_CLASS_NAME="$1 type"],
			[CRAYONS_CLASS_NAME="type"])dnl
	 AC_CACHE_CHECK(
		[the $CRAYONS_CLASS_NAME which is exactly $3 bytes in size],
		[CRAYONS_CV_NAME],
		[for crayons_type in $5; do
			AC_COMPILE_IFELSE(
				[AC_LANG_PROGRAM(
					[$4],
					[switch(0) case 0: case (sizeof($crayons_type) == $3):;])],
				[CRAYONS_CV_NAME=$crayons_type])
			if test "x$CRAYONS_CV_NAME" != "x"; then break; fi
		done])
	 if test "x$CRAYONS_CV_NAME" = "x"; then
		AC_MSG_ERROR([cannot determine the $CRAYONS_CLASS_NAME which is exactly $3 bytes in size])
	 fi
	 CRAYONS_TYPE_NAME=$CRAYONS_CV_NAME
	 m4_undefine([CRAYONS_TYPE_NAME])dnl
	 m4_undefine([CRAYONS_CV_NAME])])

dnl
dnl Determine the integer type for a given size in bytes.
dnl
dnl   $1 - size in bytes
dnl
dnl  NOTE: this function sets crayons_type_i{size} (where
dnl        {size} is equal to $1) to the integer type which
dnl        is exactly {size} bytes in size
dnl
AC_DEFUN(
	[CRAYONS_CHECK_INTEGER_FOR_SIZE],
	[CRAYONS_CHECK_TYPE_FOR_SIZE(
		[integer],
		[i],
		[$1],
		[],
		["char" "short" "int" "long" "long long" "__int8" "__int16" "__int32" "__int64"])])

dnl
dnl Make enable/disable checks easier to write.
dnl
dnl   $1 - feature (e.g. foo for --enable-foo)
dnl   $2 - description (for use in help string)
dnl   $3 - default ('yes' or 'no')
dnl   $4 - action if enabled (optional)
dnl   $5 - action if disabled (optional)
dnl
dnl  NOTE: this function sets crayons_enable_{feature} (where
dnl        {feature} is equal to $1 with all '-' characters
dnl        converted to '_') to 'yes' or 'no' based on user
dnl        input and the provided default, and handles
dnl        erroneous user input by printing an error message
dnl        and exiting with a non-zero value
dnl
AC_DEFUN(
	[CRAYONS_ARG_ENABLE],
	[AC_ARG_ENABLE(
		[$1],
		[m4_if(
			[$3],
			[no],
			[AS_HELP_STRING(
				[--enable-$1],
				[$2 @<:@default=no@:>@])],
			[AS_HELP_STRING(
				[--disable-$1],
				[$2 @<:@default=no@:>@])])],
		[if test "x${enableval}" = "xyes" || test "x${enableval}" = "xno"; then
			[crayons_enable_]m4_bpatsubst([$1], -, _)=${enableval}
		 else
			AC_MSG_ERROR([invalid value (${enableval}) for --enable-$1])
		 fi],
		[[crayons_enable_]m4_bpatsubst([$1], -, _)=$3])
	 m4_ifvaln(
		[$4],
		[if test "[$crayons_enable_]m4_bpatsubst([$1], -, _)" == "yes"; then
			$4
		 fi])dnl
	 m4_ifvaln(
	 	[$5],
		[if test "[$crayons_enable_]m4_bpatsubst([$1], -, _)" == "no"; then
			$5
		 fi])])

dnl
dnl Perform actions based on the availability of the const keyword in C.
dnl
dnl   $1 - action if available (optional)
dnl   $2 - action if not available (optional)
dnl
dnl  NOTE: this function performs the default AC_C_CONST
dnl        behavior in addition to any provided actions,
dnl        and this function sets the environment variable
dnl        CRAYONS_HAVE_C_CONST to yes or no based on the
dnl        availability of the const keyword in C
dnl
AC_DEFUN(
	[CRAYONS_IF_C_CONST],
	[AC_REQUIRE([AC_C_CONST])dnl
	 AC_COMPILE_IFELSE(
		[AC_LANG_PROGRAM(
			[#include "confdefs.h"],
			[#ifdef const
				#error "const is missing"
			 #endif])],
		[m4_ifvaln([$1], [$1])dnl
		 CRAYONS_HAVE_C_CONST="yes"],
		[m4_ifvaln([$1], [$1])dnl
		 CRAYONS_HAVE_C_CONST="no"])])

dnl
dnl Perform actions based on the availability of Xlib.
dnl
dnl   $1 - action if available (optional)
dnl   $2 - action if not available (optional)
dnl
dnl  NOTE: this function performs the default AC_PATH_XTRA
dnl        behavior in addition to any provided actions,
dnl        and this function sets the environment variable
dnl        CRAYONS_HAVE_XLIB to yes or no based on the
dnl        availability of Xlib
dnl
AC_DEFUN(
	[CRAYONS_IF_XLIB],
	[AC_REQUIRE([AC_PATH_XTRA])dnl
	 AC_COMPILE_IFELSE(
		[AC_LANG_PROGRAM(
			[#include "confdefs.h"],
			[#ifdef X_DISPLAY_MISSING
				#error "X display is missing"
			 #endif])],
		[m4_ifvaln([$1], [$1])dnl
		 CRAYONS_HAVE_XLIB="yes"],
		[m4_ifvaln([$2], [$2])dnl
		 CRAYONS_HAVE_XLIB="no"])])

dnl
dnl Perform actions based on the availability of pthreads.
dnl
dnl   $1 - action if available (optional)
dnl   $2 - action if not available (optional)
dnl
dnl  NOTE: this function performs the default AC_CHECK_LIB
dnl        behavior in addition to any provided actions,
dnl        and this function sets the environment variable
dnl        CRAYONS_HAVE_PTHREADS to yes or no based on the
dnl        availability of libpthread
dnl
AC_DEFUN(
	[CRAYONS_IF_PTHREADS],
	[AC_CHECK_LIB([pthread], [pthread_mutex_init])
	 AC_COMPILE_IFELSE(
		[AC_LANG_PROGRAM(
			[#include "confdefs.h"],
			[#ifndef HAVE_LIBPTHREAD
				#error "pthread library is not available"
			 #endif])],
		[m4_ifvaln([$1], [$1])dnl
		 CRAYONS_HAVE_PTHREADS="yes"],
		[m4_ifvaln([$2], [$2])dnl
		 CRAYONS_HAVE_PTHREADS="no"])])

dnl
dnl Perform actions based on the availability of the math library.
dnl
dnl   $1 - action if available (optional)
dnl   $2 - action if not available (optional)
dnl
dnl  NOTE: this function performs the default AC_CHECK_LIB
dnl        behavior in addition to any provided actions,
dnl        and this function sets the environment variable
dnl        CRAYONS_HAVE_MATH to yes or no based on the
dnl        availability of libm
dnl
AC_DEFUN(
	[CRAYONS_IF_MATH],
	[AC_CHECK_LIB([m], [cos])
	 AC_COMPILE_IFELSE(
		[AC_LANG_PROGRAM(
			[#include "confdefs.h"],
			[#ifndef HAVE_LIBM
				#error "math library is not available"
			 #endif])],
		[m4_ifvaln([$1], [$1])dnl
		 CRAYONS_HAVE_MATH="yes"],
		[m4_ifvaln([$2], [$2])dnl
		 CRAYONS_HAVE_MATH="no"])])

dnl
dnl Perform actions based on the availability of the pixman library.
dnl
dnl   $1 - action if available (optional)
dnl   $2 - action if not available (optional)
dnl
dnl  NOTE: this function performs the default AC_CHECK_LIB
dnl        behavior in addition to any provided actions,
dnl        and this function sets the environment variable
dnl        CRAYONS_HAVE_PIXMAN to yes or no based on the
dnl        availability of libpixman
dnl
AC_DEFUN(
	[CRAYONS_IF_PIXMAN],
	[AC_CHECK_LIB([pixman], [pixman_image_create])
	 AC_COMPILE_IFELSE(
		[AC_LANG_PROGRAM(
			[#include "confdefs.h"],
			[#ifndef HAVE_LIBPIXMAN
				#error "pixman library is not available"
			 #endif])],
		[m4_ifvaln([$1], [$1])dnl
		 CRAYONS_HAVE_PIXMAN="yes"],
		[m4_ifvaln([$2], [$2])dnl
		 CRAYONS_HAVE_PIXMAN="no"])])

dnl
dnl Require a header.
dnl
dnl   $1 - required header
dnl
dnl  NOTE: this function calls AC_CHECK_HEADERS on the
dnl        given header before doing anything
dnl
AC_DEFUN(
	[CRAYONS_REQUIRE_HEADER],
	[AC_CHECK_HEADERS([$1])
	 crayons_require_header_xxx_def=[HAVE_]m4_translit([$1], [[a-z./\]], [[A-Z___]])
	 AC_COMPILE_IFELSE(
		[AC_LANG_PROGRAM(
			[#include "confdefs.h"],
			[#ifndef $crayons_require_header_xxx_def
				#error "header not available"
			 #endif])],
		[],
		[AC_MSG_ERROR([required header ($1) not found.])])])

dnl
dnl Check a version.
dnl
dnl   $1 - version (must be in major.minor.micro form)
dnl   $2 - minimum (must be in major.minor.micro form)
dnl   $3 - action if version is at least minimum (optional)
dnl   $4 - action if version is less than minimum (optional)
dnl
AC_DEFUN(
	[CRAYONS_CHECK_VERSION],
	[crayons_ckver_ok="yes"
	 crayons_ckver_ver="`echo $1 | sed 's/\([[0-9]]*\).\([[0-9]]*\).\([[0-9]]*\)/\1/'`"
	 crayons_ckver_min="`echo $2 | sed 's/\([[0-9]]*\).\([[0-9]]*\).\([[0-9]]*\)/\1/'`"
	 if test $crayons_ckver_ver -lt $crayons_ckver_min; then
		crayons_ckver_ok="no"
	 else
		if test $crayons_ckver_ver -eq $crayons_ckver_min; then
			crayons_ckver_ver="`echo $1 | sed 's/\([[0-9]]*\).\([[0-9]]*\).\([[0-9]]*\)/\2/'`"
			crayons_ckver_min="`echo $2 | sed 's/\([[0-9]]*\).\([[0-9]]*\).\([[0-9]]*\)/\2/'`"
			if test $crayons_ckver_ver -lt $crayons_ckver_min; then
				crayons_ckver_ok="no"
			else
				if test $crayons_ckver_ver -eq $crayons_ckver_min; then
					crayons_ckver_ver="`echo $1 | sed 's/\([[0-9]]*\).\([[0-9]]*\).\([[0-9]]*\)/\3/'`"
					crayons_ckver_min="`echo $2 | sed 's/\([[0-9]]*\).\([[0-9]]*\).\([[0-9]]*\)/\3/'`"
					if test $crayons_ckver_ver -lt $crayons_ckver_min; then
						crayons_ckver_ok="no"
					fi
				fi
			fi
		fi
	 fi
	 m4_ifvaln([$3], [if test $crayons_ckver_ok = yes; then $3; fi])dnl
	 m4_ifvaln([$4], [if test $crayons_ckver_ok =  no; then $4; fi])])

dnl
dnl Check freetype configuration.
dnl
dnl   $1 - minimum version
dnl
dnl  NOTE: this function sets FREETYPE_CFLAGS and
dnl        FREETYPE_LIBS as appopriate
dnl
AC_DEFUN(
	[CRAYONS_CHECK_FREETYPE],
	[AC_ARG_VAR([FREETYPE_CFLAGS], [C compiler flags for FREETYPE, overriding freetype-config])dnl
	 AC_ARG_VAR([FREETYPE_LIBS], [linker flags for FREETYPE, overriding freetype-config])dnl
	 AC_ARG_VAR([FREETYPE_CONFIG], [path to freetype-config, overriding the default search path])dnl
	 AC_PATH_PROG([FREETYPE_CONFIG], [freetype-config])
	 if test "x$FREETYPE_CONFIG" = "x"; then
		AC_MSG_ERROR([freetype-config not found but freetype is required.])
	 fi
	 crayons_ckft_ver=`$FREETYPE_CONFIG --version`
	 if test "x$crayons_ckft_ver" = "x"; then
		AC_MSG_ERROR([freetype-config is broken or missing.])
	 fi
	 CRAYONS_CHECK_VERSION(
		[$crayons_ckft_ver],
		[$1],
		[FREETYPE_CFLAGS=`$FREETYPE_CONFIG --cflags`
		 FREETYPE_LIBS=`$FREETYPE_CONFIG --libs`],
		[AC_MSG_ERROR([installed freetype is too old.])])dnl
	 AC_SUBST([FREETYPE_CFLAGS])
	 AC_SUBST([FREETYPE_LIBS])])
