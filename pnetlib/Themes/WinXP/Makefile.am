
.PHONY: System.Windows.Forms.Themes.XP.dll

all-local: System.Windows.Forms.Themes.XP.dll

System.Windows.Forms.Themes.XP.dll:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/Themes.XP.build" all

CLEANFILES = System.Windows.Forms.Themes.XP.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/Themes.XP.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/Themes.XP.build" -u all
