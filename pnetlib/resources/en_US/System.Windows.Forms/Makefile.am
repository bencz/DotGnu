
.PHONY: System.Windows.Forms.resources

all-local: System.Windows.Forms.resources

System.Windows.Forms.resources:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/resources.build" all

CLEANFILES = System.Windows.Forms.resources

EXTRA_DIST = System.Windows.Forms.small_application.ico \
			 System.Windows.Forms.small_dll.ico \
			 System.Windows.Forms.small_document.ico \
			 System.Windows.Forms.small_folder.ico \
			 System.Windows.Forms.small_text.ico
