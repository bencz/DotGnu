
.PHONY: runtime.resources

all-local: runtime.resources

runtime.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = runtime.resources
