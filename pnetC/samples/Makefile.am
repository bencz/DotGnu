
noinst_PROGRAMS = dirent.exe hello.exe hello2.exe stack.exe pwd.exe

dirent_exe_SOURCES = dirent.c
dirent_exe_LDADD = -L../libc -lOpenSystem.C -llibc

hello_exe_SOURCES = hello.c
hello_exe_LDADD = -L../libc -lOpenSystem.C -llibc

hello2_exe_SOURCES = hello2.c
hello2_exe_LDADD = -L../libc -lOpenSystem.C -llibc

stack_exe_SOURCES = stack.c
stack_exe_LDADD = -L../libc -lOpenSystem.C -llibc

pwd_exe_SOURCES = pwd.c
pwd_exe_LDADD = -L../libc -lOpenSystem.C -llibc

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h
