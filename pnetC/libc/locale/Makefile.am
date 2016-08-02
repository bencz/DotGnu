
noinst_LIBRARIES = libCLocale.a

libCLocale_a_SOURCES = \
	localeconv.c \
	localeinfo.h \
	nl_langinfo.c \
	setlocale.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-I$(top_srcdir)/libc \
			-imacros $(top_srcdir)/include/libc-symbols.h
