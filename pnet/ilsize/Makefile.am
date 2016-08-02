bin_PROGRAMS   = ilsize
man_MANS       = ilsize.1
EXTRA_DIST     = $(man_MANS)
ilsize_SOURCES = ilsize.c ilsize_est.c
ilsize_LDADD   = ../dumpasm/libILDumpAsm.a \
				 ../image/libILImage.a ../support/libILSupport.a $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include

install-data-local:
	@$(NORMALL_INSTALL)
	rm -f $(DESTDIR)$(mandir)/man1/cli-unknown-size.1.gz
	$(LN_S) -f ilsize.1.gz $(DESTDIR)$(mandir)/man1/cli-unknown-size.1.gz

install-exec-local:
	@$(NORMALL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-size$(EXEEXT)
	$(LN_S) -f ilsize$(EXEEXT) $(DESTDIR)$(bindir)/cli-unknown-size$(EXEEXT)

uninstall-local:
	@$(NORMALL_UNINSTALL)
	rm -f $(DESTDIR)$(mandir)/man1/cli-unknown-size.1.gz
	rm -f $(DESTDIR)$(bindir)/cli-unknown-size$(EXEEXT)
