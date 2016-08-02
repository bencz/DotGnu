
.PHONY: sqlsharp-pnet.exe

all-local: sqlsharp-pnet.exe

sqlsharp-pnet.exe:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-b "$(srcdir)" -B . -f SqlSharp.build all

CLEANFILES = sqlsharp-pnet.exe

install-exec-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/SqlSharp.build" -i all
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/sqlsharp-pnet.exe
	rm -f $(DESTDIR)$(bindir)/sqlsharp-pnet
	$(LN_S) clrwrap $(DESTDIR)$(bindir)/sqlsharp-pnet

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/SqlSharp.build" -u all
	rm -f $(DESTDIR)$(bindir)/sqlsharp-pnet.exe
	rm -f $(DESTDIR)$(bindir)/sqlsharp-pnet
