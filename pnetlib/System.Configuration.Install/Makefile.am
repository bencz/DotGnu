
.PHONY: System.Configuration.Install.dll

all-local: System.Configuration.Install.dll

System.Configuration.Install.dll:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/System.Configuration.Install.build" all

CLEANFILES = System.Configuration.Install.dll ilinstall.exe

install-exec-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/System.Configuration.Install.build" -i all
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/ilinstall.exe
	rm -f $(DESTDIR)$(bindir)/ilinstall
	$(SHELL) $(top_builddir)/tools/wrapper.sh \
			$(DESTDIR)$(bindir)/ilinstall$(EXEEXT)

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/System.Configuration.Install.build" -u all
	rm -f $(DESTDIR)$(bindir)/ilinstall.exe
	rm -f $(DESTDIR)$(bindir)/ilinstall
