
.PHONY: DotGNU.Terminal.dll

all-local: DotGNU.Terminal.dll

DotGNU.Terminal.dll:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/DotGNU.Terminal.build" all

CLEANFILES = DotGNU.Terminal.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/DotGNU.Terminal.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/DotGNU.Terminal.build" -u all
