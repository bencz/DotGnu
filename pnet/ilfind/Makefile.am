bin_PROGRAMS   = ilfind
man_MANS       = ilfind.1
EXTRA_DIST     = $(man_MANS)
ilfind_SOURCES = ilfind.c
ilfind_LDADD   = ../image/libILImage.a ../support/libILSupport.a \
				 $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include
