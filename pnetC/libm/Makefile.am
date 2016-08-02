
all-local: libm.dll

noinst_LIBRARIES = libMath.a

libMath_a_SOURCES = fmax.c \
					fmin.c \
					fmod.c \
					redirect.c \
					test.c

AM_CFLAGS = -I$(top_srcdir)/include \
			-imacros $(top_srcdir)/include/libc-symbols.h

libm.dll: libMath.a
	$(LINK) libMath.a -L../libc -llibc

CLEANFILES = libm.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(CYG_CACHE)
	$(ILGAC) -c "$(DESTDIR)$(CYG_CACHE)" -f -d -i libm.dll

uninstall-local:
	@$(NORMAL_UNINSTALL)
	$(ILGAC) -c "$(DESTDIR)$(CYG_CACHE)" -f -d -u libm $(MSCORLIB_VERSION)
