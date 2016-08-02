
.PHONY: I18N.resources

all-local: I18N.resources

I18N.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = I18N.resources
