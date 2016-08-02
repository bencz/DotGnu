
.PHONY: System.Configuration.Install.resources

all-local: System.Configuration.Install.resources

System.Configuration.Install.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = System.Configuration.Install.resources
