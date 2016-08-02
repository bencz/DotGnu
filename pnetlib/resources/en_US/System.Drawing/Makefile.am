
.PHONY: System.Drawing.resources

all-local: System.Drawing.resources

System.Drawing.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = System.Drawing.resources

EXTRA_DIST = System.Drawing.asterisk.ico \
		System.Drawing.exclamation.ico \
		System.Drawing.hand.ico \
		System.Drawing.question.ico \
		SystemIcons.error.ico \
		SystemIcons.information.ico \
		SystemIcons.warning.ico \
		SystemIcons.windowslogo.ico \
		SystemIcons.application.ico

