
.PHONY: samples

all-local: samples

samples:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/xsharp.build" all

CLEANFILES = *.exe
