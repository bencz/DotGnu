
noinst_LIBRARIES = libCStdlib.a

SUFFIXES = .cs

libCStdlib_a_SOURCES = \
	abort.c \
	atof.c \
	atoi.c \
	_exit.c \
	getenv-glue.cs \
	getenv.c \
	exit.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-I$(top_srcdir)/libc \
			-imacros $(top_srcdir)/include/libc-symbols.h \
			-lOpenSystem.C

.cs.o:
	$(COMPILE) -x cs -o $@ -c $<
