
.PHONY: developers.exe

all-local: developers.exe

developers.exe:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/developers.build" all

CLEANFILES = *.exe
