
noinst_LIBRARIES = libCPthread.a

SUFFIXES = .cs

libCPthread_a_SOURCES = monitor.cs \
						self.c \
						thread.cs

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h \
			-lOpenSystem.C

.cs.o:
	$(COMPILE) -funsafe -x cs -o $@ -c $<
