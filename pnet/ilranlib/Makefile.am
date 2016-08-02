bin_PROGRAMS     = ilranlib
man_MANS         = ilranlib.1
EXTRA_DIST       = $(man_MANS)
ilranlib_SOURCES = ilranlib.c
ilranlib_LDADD   = ../support/libILSupport.a

AM_CFLAGS = -I$(top_srcdir)/include

install-data-local:
	@$(NORMALL_INSTALL)
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-ar.1.gz
	$(LN_S) ar.1.gz $(DESTDIR)$(man1dir)/cli-unknown-ar.1.gz
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-ranlib.1.gz
	$(LN_S) ilranlib.1.gz $(DESTDIR)$(man1dir)/cli-unknown-ranlib.1.gz

install-exec-local:
	@$(NORMALL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-ranlib$(EXEEXT)
	$(LN_S) ilranlib$(EXEEXT) $(DESTDIR)$(bindir)/cli-unknown-ranlib$(EXEEXT)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-ar$(EXEEXT)
	a="$(bindir)/cli-unknown-ar$(EXEEXT)"; b=.$${a%/*}/b; c="$(SYSTEM_AR)"; d=.$${c%/*}//; e=; \
	  while [ ."$${b%%/*}" = ."$${d%%/*}" ]; do b=$${b#*/}; d=$${d#*/}; done; \
	  while [ ."$$b" != ."b" ]; do b=$${b#*/}; e=../$$e; done;		\
	  $(LN_S) "$$e$${d%/}$${c##*/}" "$(DESTDIR)$$a"

uninstall-local:
	@$(NORMALL_UNINSTALL)
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-ar.1.gz
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-ranlib.1.gz
	rm -f $(DESTDIR)$(bindir)/cli-unknown-ranlib$(EXEEXT)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-ar$(EXEEXT)
