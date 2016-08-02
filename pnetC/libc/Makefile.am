
SUBDIRS = ctype \
		  dirent \
		  locale \
		  malloc \
		  misc \
		  pthread \
		  pwd \
		  signal \
		  socket \
		  stdio \
		  stdlib \
		  string \
		  time \
		  unistd

all-local: libc.dll

LIBC_COMPONENTS = ctype/libCType.a \
				  dirent/libCDirent.a \
				  locale/libCLocale.a \
				  malloc/libCMalloc.a \
				  misc/libCMisc.a \
				  pthread/libCPthread.a \
				  pwd/libCPwd.a \
				  signal/libCSignal.a \
				  socket/libCSocket.a \
				  stdio/libCStdio.a \
				  stdlib/libCStdlib.a \
				  string/libCString.a \
				  time/libCTime.a \
				  unistd/libCUnistd.a

libc.dll: $(LIBC_COMPONENTS)
	$(LINK) $(LIBC_COMPONENTS) -lSystem

CLEANFILES = libc.dll

## Build a dummy library, which causes automake to output the
## correct definition for the "LINK" variable into "Makefile".
noinst_LIBRARIES = libCDummy.a
libCDummy_a_SOURCES = dummy.c

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(CYG_CACHE)
	$(ILGAC) -c "$(DESTDIR)$(CYG_CACHE)" -f -d -i libc.dll

uninstall-local:
	@$(NORMAL_UNINSTALL)
	$(ILGAC) -c "$(DESTDIR)$(CYG_CACHE)" -f -d -u libc $(MSCORLIB_VERSION)
