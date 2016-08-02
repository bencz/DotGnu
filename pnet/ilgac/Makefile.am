bin_PROGRAMS   = ilgac
man_MANS       = ilgac.1
EXTRA_DIST     = $(man_MANS)
ilgac_SOURCES  = ilgac.c
ilgac_LDADD    = ../image/libILImage.a ../support/libILSupport.a \
				 $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include
