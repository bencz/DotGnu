
all-local: libpthread.dll

SUFFIXES = .cs

noinst_LIBRARIES = libPthread.a

libPthread_a_SOURCES = \
			pthread_barrier.c \
			pthread_cond.c \
			pthread_glue.cs \
			pthread_key.c \
			pthread_kill.c \
			pthread_mutex.c \
			pthread_once.c \
			pthread_other.c \
			pthread_rwlock.c \
			pthread_sem.c \
			pthread_thread.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h \
			-lOpenSystem.C

libpthread.dll: libPthread.a
	$(LINK) libPthread.a -L../libc -llibc

CLEANFILES = libpthread.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(CYG_CACHE)
	$(ILGAC) -c "$(DESTDIR)$(CYG_CACHE)" -f -d -i libpthread.dll

uninstall-local:
	@$(NORMAL_UNINSTALL)
	$(ILGAC) -c "$(DESTDIR)$(CYG_CACHE)" -f -d \
			-u libpthread $(MSCORLIB_VERSION)

.cs.o:
	$(COMPILE) -funsafe -x cs -o $@ -c $<
