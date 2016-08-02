
noinst_LIBRARIES = libCTime.a

SUFFIXES = .cs

libCTime_a_SOURCES = clock.c \
                     cpuclock.c \
					 gettimeofday.c \
                     gmtime.c \
					 itimer.c \
					 nanosleep.c \
					 settimeofday.c \
					 sleep.c \
                     time.c \
                     time-defs.h \
					 time-glue.cs \
					 time-vars.c \
					 tzset.c \
					 usleep.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h \
			-lOpenSystem.C

.cs.o:
	$(COMPILE) -x cs -o $@ -c $<
