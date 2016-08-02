
.PHONY: System.Xml.resources

all-local: System.Xml.resources

System.Xml.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = System.Xml.resources
