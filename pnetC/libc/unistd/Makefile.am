
noinst_LIBRARIES = libCUnistd.a

SUFFIXES = .cs

libCUnistd_a_SOURCES = chown-stub.c \
                       close.c \
					   dup.c \
					   dup2.c \
					   fcntl.c \
					   getlogin.c \
					   guid-stub.c \
					   isatty.c \
					   lseek.c \
					   mman-stub.c \
					   open.c \
					   pid-stub.c \
					   pread.c \
					   pwrite.c \
					   read.c \
					   readv.c \
					   write.c \
					   writev.c \
					   unistd-glue.cs

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h \
			-lSystem -lDotGNU.SSL -lOpenSystem.C

.cs.o:
	$(COMPILE) -x cs -o $@ -c $<
