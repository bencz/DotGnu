
.PHONY: OpenSystem.Platform.dll

all-local: OpenSystem.Platform.dll

OpenSystem.Platform.dll:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/OpenSystem.Platform.build" all

CLEANFILES = OpenSystem.Platform.dll
DISTCLEANFILES = Platform.cs

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/OpenSystem.Platform.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/OpenSystem.Platform.build" -u all
