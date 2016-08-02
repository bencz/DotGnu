bin_PROGRAMS     = ilheader
man_MANS         = ilheader.1
EXTRA_DIST       = $(man_MANS)

ilheader_SOURCES = ilheader.c
ilheader_LDADD   = ../image/libILImage.a ../support/libILSupport.a \
				   $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include
