
noinst_LIBRARIES = libILCCommon.a

EXTRA_DIST = cpp_defs.sh

BUILT_SOURCES = cc_cppdefs.c

libILCCommon_a_SOURCES = cc_compat.c \
						 cc_cppdefs.c \
						 cc_errors.c \
						 cc_errors.h \
						 cc_intl.h \
						 cc_main.c \
						 cc_main.h \
						 cc_options.c \
						 cc_options.h \
						 cc_preproc.c \
						 cc_preproc.h

AM_CFLAGS = -I$(top_srcdir) -I$(top_builddir) -I$(top_srcdir)/include

cc_cppdefs.c: $(srcdir)/cpp_defs.sh
	$(SHELL) $(srcdir)/cpp_defs.sh >cc_cppdefs.c

CLEANFILES = cc_cppdefs.c
