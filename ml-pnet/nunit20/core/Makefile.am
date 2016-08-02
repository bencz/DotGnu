
.PHONY: nunit.core.dll

all-local: nunit.core.dll

nunit.core.dll:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-Dpnet.libdir="$(PNETLIB_PATH)" \
			-b "$(srcdir)" -B . -f nunit.core.build all

CLEANFILES = nunit.core.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-Dpnet.libdir="$(PNETLIB_PATH)" \
			-f "$(srcdir)/nunit.core.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-Dpnet.libdir="$(PNETLIB_PATH)" \
			-f "$(srcdir)/nunit.core.build" -u all
