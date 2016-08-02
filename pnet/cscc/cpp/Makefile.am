
bin_PROGRAMS = cscc-cpp
man_MANS     = cscc-cpp.1
EXTRA_DIST   = $(man_MANS)

cscc_cpp_SOURCES = \
	ansidecl.h \
	cppdefault.c \
	cppdefault.h \
	cpperror.c \
	cppexp.c \
	cppfiles.c \
	cpphash.c \
	cpphash.h \
	cppinit.c \
	cpplex.c \
	cpplib.c \
	cpplib.h \
	cppmacro.c \
	cppmain.c \
	concat.c \
	config.h \
	hashtable.c \
	hashtable.h \
	hex.c \
	hwint.h \
	intl.h \
	lbasename.c \
	libiberty.h \
	line-map.c \
	line-map.h \
	mbchar.c \
	mbchar.h \
	mkdeps.c \
	mkdeps.h \
	obstack.c \
	obstack.h \
	prefix.c \
	prefix.h \
	safe-ctype.c \
	safe-ctype.h \
	splay-tree.c \
	splay-tree.h \
	symcat.h \
	system.h \
	version.c \
	version.h \
	xexit.c \
	xmalloc.c \
	xstrdup.c \
	xstrerror.c

cscc_cpp_LDADD = ../../support/libILSupport.a

AM_CFLAGS = -I. -I$(srcdir) -I$(top_srcdir)/include \
			-I$(top_builddir)/include -DANSI_PROTOTYPES -DCHAR_BIT=8 \
			-DWCHAR_UNSIGNED=1 -DSUPPORTS_ONE_ONLY=0 -DCROSS_COMPILE=1 \
			-DCHAR_TYPE_SIZE=8 -DWCHAR_TYPE_SIZE=16 -DPREFIX=\"$(prefix)\" \
			-DCROSS_INCLUDE_DIR=\"$(datadir)/cscc/include\" \
			-DDEFAULT_SIGNED_CHAR=1 -DCSCC_VERSION=\"$(VERSION)\"

## Link "cli-unknown-cpp" to "cscc-cpp" for cross-compile environments.

install-data-local:
	@$(NORMALL_INSTALL)
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-cpp.1.gz
	$(LN_S) cscc-cpp.1.gz $(DESTDIR)$(man1dir)/cli-unknown-cpp.1.gz

install-exec-local:
	@$(NORMALL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-cpp$(EXEEXT)
	$(LN_S) cscc-cpp$(EXEEXT) $(DESTDIR)$(bindir)/cli-unknown-cpp$(EXEEXT)

uninstall-local:
	@$(NORMALL_UNINSTALL)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-cpp$(EXEEXT)
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-cpp.1.gz
