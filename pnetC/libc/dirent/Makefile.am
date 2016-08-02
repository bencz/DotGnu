
noinst_LIBRARIES = libCDirent.a

SUFFIXES = .cs

libCDirent_a_SOURCES = closedir.c \
                       dirent-glue.cs \
                       opendir.c \
                       readdir.c \
                       readdir_r.c \
                       rewinddir.c \
                       seekdir.c \
                       telldir.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h \
			-lOpenSystem.C

.cs.o:
	$(COMPILE) -x cs -o $@ -c $<
