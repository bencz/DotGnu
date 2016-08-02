bin_PROGRAMS  = ilasm
lib_LIBRARIES = libILAsm.a
man_MANS      = ilasm.1
EXTRA_DIST    = $(man_MANS)
BUILT_SOURCES = ilasm_grammar.c ilasm_grammar.h ilasm_scanner.c

libILAsm_a_SOURCES =	ilasm_build.c \
			ilasm_build.h \
		 	ilasm_data.c \
		 	ilasm_data.h \
			ilasm_grammar.y \
			ilasm_main.c \
			ilasm_output.c \
			ilasm_output.h \
			ilasm_rename.h \
			ilasm_scanner.l

ilasm_SOURCES = ilasm.c
ilasm_LDADD   = libILAsm.a ../image/libILImage.a ../support/libILSupport.a $(GCLIBS)

ilasm_scanner.l: ilasm_grammar.c

AM_YFLAGS = -d
CLEANFILES = ilasm_grammar.c ilasm_scanner.c ilasm_grammar.h

AM_CFLAGS = -I$(top_srcdir)/include

install-data-local:
	rm -f $(DESTDIR)$(man1dir)/al.1.gz
	$(mkdir_p) $(DESTDIR)$(man1dir)
	$(LN_S) ilasm.1.gz $(DESTDIR)$(man1dir)/al.1.gz

uninstall-local:
	rm -f $(DESTDIR)$(man1dir)/al.1.gz
