
.PHONY: System.Deployment.resources

all-local: System.Deployment.resources

System.Deployment.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = System.Deployment.resources
