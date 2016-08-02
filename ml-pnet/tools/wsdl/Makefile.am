
.PHONY: wsdl-pnet.exe

all-local: wsdl-pnet.exe

wsdl-pnet.exe:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-Dpnet.libdir="$(PNETLIB_PATH)" \
			-b "$(srcdir)" -B . -f wsdl.build all

CLEANFILES = wsdl-pnet.exe

install-exec-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-Dpnet.libdir="$(PNETLIB_PATH)" \
			-f "$(srcdir)/wsdl.build" -i all
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/wsdl-pnet.exe
	rm -f $(DESTDIR)$(bindir)/wsdl-pnet
	$(LN_S) clrwrap$(EXEEXT) $(DESTDIR)$(bindir)/wsdl-pnet$(EXEEXT)

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-Dpnet.libdir="$(PNETLIB_PATH)" \
			-f "$(srcdir)/wsdl.build" -u all
	rm -f $(DESTDIR)$(bindir)/wsdl-pnet.exe
	rm -f $(DESTDIR)$(bindir)/wsdl-pnet
