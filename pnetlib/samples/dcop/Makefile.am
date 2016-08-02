
.PHONY: samples

all-local: samples

samples:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/dcop.build" all

CLEANFILES = *.exe
