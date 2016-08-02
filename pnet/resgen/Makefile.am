bin_PROGRAMS   = resgen
man_MANS       = resgen.1
EXTRA_DIST     = $(man_MANS)

resgen_SOURCES = resgen.c \
				 resgen.h \
				 resgen_binary.c \
				 resgen_po.c \
				 resgen_text.c \
				 resgen_xml.c
resgen_LDADD   = ../image/libILImage.a ../support/libILSupport.a \
				 $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include
