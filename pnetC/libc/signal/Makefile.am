
noinst_LIBRARIES = libCSignal.a

SUFFIXES = .cs

libCSignal_a_SOURCES = \
			kill.c \
			sigaction.c \
			sighold.c \
			sigignore.c \
			signal.c \
			signal-glue.cs \
			sigpause.c \
			sigrelse.c \
			sigset.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h \
			-lOpenSystem.C

.cs.o:
	$(COMPILE) -x cs -o $@ -c $<
