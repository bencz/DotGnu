
.PHONY: csunit

all-local: csunit

csunit: TestVersion.cs
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/csunit.build" all

CLEANFILES = cstest.dll csunit.exe
DISTCLEANFILES = TestVersion.cs

install-exec-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/csunit.build" -i all
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/csunit.exe
	rm -f $(DESTDIR)$(bindir)/csunit
	$(SHELL) $(top_builddir)/tools/wrapper.sh \
			$(DESTDIR)$(bindir)/csunit$(EXEEXT)

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/csunit.build" -u all
	rm -f $(DESTDIR)$(bindir)/csunit.exe
	rm -f $(DESTDIR)$(bindir)/csunit
