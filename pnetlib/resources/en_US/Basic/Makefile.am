
.PHONY: Microsoft.VisualBasic.resources

all-local: Microsoft.VisualBasic.resources

Microsoft.VisualBasic.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
		-f "$(srcdir)/resources.build" all

CLEANFILES = Microsoft.VisualBasic.resources
