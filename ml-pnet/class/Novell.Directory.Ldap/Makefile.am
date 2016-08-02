
.PHONY: Novell.Directory.Ldap.dll

all-local: Novell.Directory.Ldap.dll

Novell.Directory.Ldap.dll:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-b "$(srcdir)" -B . -f Novell.Directory.Ldap.build all

CLEANFILES = Novell.Directory.Ldap.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/Novell.Directory.Ldap.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/Novell.Directory.Ldap.build" -u all
