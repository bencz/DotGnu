
REGION_ASSEMBLIES = I18N.CJK.dll I18N.MidEast.dll \
					I18N.Other.dll I18N.Rare.dll \
					I18N.West.dll
HANDLER_FILE = I18N-handlers.def

.PHONY: build-assemblies

all-local: build-assemblies

build-assemblies:
	test -d first || mkdir first
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/I18N.build" all
	@$(MAKE) $(AM_MAKEFLAGS) $(HANDLER_FILE)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/I18N.build" phase-two

$(HANDLER_FILE): first/I18N.dll $(REGION_ASSEMBLIES)
	$(SHELL) "$(srcdir)/mklist.sh" "$(ILFIND)" $(HANDLER_FILE) \
			first/I18N.dll $(REGION_ASSEMBLIES)

CLEANFILES = $(REGION_ASSEMBLIES) $(HANDLER_FILE) I18N.dll first/I18N.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . -f "$(srcdir)/I18N.build" \
			-i all phase-two

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . -f "$(srcdir)/I18N.build" \
			-u all phase-two
