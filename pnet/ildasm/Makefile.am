bin_PROGRAMS   = ildasm
man_MANS       = ildasm.1
EXTRA_DIST     = $(man_MANS)
ildasm_SOURCES = ildasm_main.c
ildasm_LDADD   = ../dumpasm/libILDumpAsm.a ../image/libILImage.a \
				 ../support/libILSupport.a $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include
