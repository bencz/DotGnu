
.PHONY: System.Deployment.dll

all-local: System.Deployment.dll

System.Deployment.dll:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/System.Deployment.build" all

CLEANFILES = System.Deployment.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/System.Deployment.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/System.Deployment.build" -u all
