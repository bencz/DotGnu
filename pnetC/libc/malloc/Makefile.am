
noinst_LIBRARIES = libCMalloc.a

libCMalloc_a_SOURCES = calloc.c \
					   free.c \
					   malloc.c \
					   realloc.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h
