
noinst_LIBRARIES = libCPwd.a

libCPwd_a_SOURCES = endgrent.c \
					endpwent.c \
					getgrent.c \
					getgrnam.c \
					getgrnam_r.c \
					getgrgid.c \
					getgrgid_r.c \
					getpwent.c \
					getpwnam.c \
					getpwnam_r.c \
					getpwuid.c \
					getpwuid_r.c \
					grent.c \
					grent.h \
					pwent.c \
					pwent.h \
					pwpersist.c \
					setgrent.c \
					setpwent.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-I$(top_srcdir)/libc/unistd \
			-imacros $(top_srcdir)/include/libc-symbols.h
