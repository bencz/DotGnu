
.PHONY: Xsharp.resources

all-local: Xsharp.resources

Xsharp.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = Xsharp.resources
