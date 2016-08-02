
.PHONY: Mono.Data.Tds.dll

all-local: Mono.Data.Tds.dll

Mono.Data.Tds.dll:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-b "$(srcdir)" -B . -f Mono.Data.Tds.build all

CLEANFILES = Mono.Data.Tds.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/Mono.Data.Tds.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/Mono.Data.Tds.build" -u all
