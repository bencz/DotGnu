
.PHONY: DotGNU.XmlRpc.dll

all-local: DotGNU.XmlRpc.dll

DotGNU.XmlRpc.dll:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/DotGNU.XmlRpc.build" all

CLEANFILES = DotGNU.XmlRpc.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/DotGNU.XmlRpc.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/DotGNU.XmlRpc.build" -u all
