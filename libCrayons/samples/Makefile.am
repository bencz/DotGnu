## Process this file with automake to produce Makefile.in
noinst_PROGRAMS = sampleX11

if CRAYONS_X11_ENABLED
sampleX11_SOURCES = sampleX11.c
endif

sampleX11_LDFLAGS      = $(CRAYONS_LDFLAGS)
sampleX11_LDADD        = $(top_builddir)/src/libCrayons.la
sampleX11_DEPENDENCIES = $(top_builddir)/src/libCrayons.la
sampleX11_CFLAGS       = $(CRAYONS_CFLAGS)
