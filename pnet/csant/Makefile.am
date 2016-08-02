bin_PROGRAMS   = csant
man_MANS       = csant.1
EXTRA_DIST     = $(man_MANS)
csant_SOURCES  = csant.c \
				 csant_build.c \
				 csant_cscc.c \
				 csant_defs.h \
				 csant_dir.c \
				 csant_dir.h \
				 csant_fileset.c \
				 csant_fileset.h \
				 csant_parse.c \
				 csant_prop.c \
				 csant_task.c
csant_LDADD    = ../support/libILSupport.a $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir)/include
