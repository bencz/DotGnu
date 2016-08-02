bin_PROGRAMS = ildd
man_MANS     = ildd.1
EXTRA_DIST   = $(man_MANS)
ildd_SOURCES = ildd.c
ildd_LDADD   = ../image/libILImage.a ../support/libILSupport.a \
			   $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include
