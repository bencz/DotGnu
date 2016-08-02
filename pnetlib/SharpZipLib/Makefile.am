
.PHONY: ICSharpCode.SharpZipLib.dll

all-local: ICSharpCode.SharpZipLib.dll

ICSharpCode.SharpZipLib.dll:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/SharpZipLib.build" all

CLEANFILES = ICSharpCode.SharpZipLib.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/SharpZipLib.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/SharpZipLib.build" -u all
