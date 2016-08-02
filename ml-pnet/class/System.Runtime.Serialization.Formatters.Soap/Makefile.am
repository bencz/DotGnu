
.PHONY: System.Runtime.Serialization.Formatters.Soap.dll

all-local: System.Runtime.Serialization.Formatters.Soap.dll

System.Runtime.Serialization.Formatters.Soap.dll:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-b "$(srcdir)" -B . \
			-f System.Runtime.Serialization.Formatters.Soap.build all

CLEANFILES = System.Runtime.Serialization.Formatters.Soap.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/System.Runtime.Serialization.Formatters.Soap.build" \
			-i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/System.Runtime.Serialization.Formatters.Soap.build" \
			-u all
