## Process this file with automake to produce Makefile.in
lib_LTLIBRARIES = libCrayons.la

libCrayons_la_LDFLAGS = -version-info $(CRAYONS_LIBRARY_VERSION) -no-undefined $(CRAYONS_LDFLAGS)
libCrayons_la_LIBADD  = $(CRAYONS_LIBS)
libCrayons_la_CFLAGS  = $(CRAYONS_CFLAGS)

libCrayons_la_SOURCES = \
	$(top_srcdir)/include/Crayons.h \
	$(top_srcdir)/include/CrayonsFeatures.h \
	$(top_srcdir)/include/CrayonsInternal.h \
	$(top_srcdir)/include/CrayonsConfig.h \
	$(top_srcdir)/include/CMutex.h \
	CMemory.c \
	CAffineTransform.c   $(top_srcdir)/include/CAffineTransform.h \
	CBezier.c            $(top_srcdir)/include/CBezier.h \
	CBitmap.c            $(top_srcdir)/include/CBitmap.h \
	CBitmapSurface.c     $(top_srcdir)/include/CBitmapSurface.h \
	CBlend.c             $(top_srcdir)/include/CBlend.h \
	CBrush.c             $(top_srcdir)/include/CBrush.h \
	CCache.c             $(top_srcdir)/include/CCache.h \
	CColorPalette.c      $(top_srcdir)/include/CColorPalette.h \
	CFiller.c            $(top_srcdir)/include/CFiller.h \
	CFlattener.c         $(top_srcdir)/include/CFlattener.h \
	CFont.c              $(top_srcdir)/include/CFont.h \
	CFontCollection.c    $(top_srcdir)/include/CFontCollection.h \
	CFontFace.c          $(top_srcdir)/include/CFontFace.h \
	CFontFamily.c        $(top_srcdir)/include/CFontFamily.h \
	CFontFamilyTable.c   $(top_srcdir)/include/CFontFamilyTable.h \
	CGlyphCache.c        $(top_srcdir)/include/CGlyphCache.h \
	CGraphics.c          $(top_srcdir)/include/CGraphics.h \
	CGraphicsPipeline.c  $(top_srcdir)/include/CGraphicsPipeline.h \
	CHashTable.c         $(top_srcdir)/include/CHashTable.h \
	CHatchBrush.c        $(top_srcdir)/include/CHatchBrush.h \
	CImage.c             $(top_srcdir)/include/CImage.h \
	CLineBrush.c         $(top_srcdir)/include/CLineBrush.h \
	CMath.c              $(top_srcdir)/include/CMath.h \
	CMatrix.c            $(top_srcdir)/include/CMatrix.h \
	CPathBrush.c         $(top_srcdir)/include/CPathBrush.h \
	CPath.c              $(top_srcdir)/include/CPath.h \
	CPathInterpreter.c   $(top_srcdir)/include/CPathInterpreter.h \
	CPen.c               $(top_srcdir)/include/CPen.h \
	CPointArray.c        $(top_srcdir)/include/CPointArray.h \
	CPolygon.c           $(top_srcdir)/include/CPolygon.h \
	CRegion.c            $(top_srcdir)/include/CRegion.h \
	CRegionCloner.c      $(top_srcdir)/include/CRegionCloner.h \
	CRegionDisposer.c    $(top_srcdir)/include/CRegionDisposer.h \
	CRegionInterpreter.c $(top_srcdir)/include/CRegionInterpreter.h \
	CRegionRasterizer.c  $(top_srcdir)/include/CRegionRasterizer.h \
	CRegionStack.c       $(top_srcdir)/include/CRegionStack.h \
	CRegionTransformer.c $(top_srcdir)/include/CRegionTransformer.h \
	CRegionTranslator.c  $(top_srcdir)/include/CRegionTranslator.h \
	CSolidBrush.c        $(top_srcdir)/include/CSolidBrush.h \
	CStringFormat.c      $(top_srcdir)/include/CStringFormat.h \
	CStroker.c           $(top_srcdir)/include/CStroker.h \
	CSurface.c           $(top_srcdir)/include/CSurface.h \
	CTempFileList.c      $(top_srcdir)/include/CTempFileList.h \
	CTextureBrush.c      $(top_srcdir)/include/CTextureBrush.h \
	CTrapezoids.c        $(top_srcdir)/include/CTrapezoids.h \
	CUnicodeHashTable.c  $(top_srcdir)/include/CUnicodeHashTable.h \
	CUtils.c             $(top_srcdir)/include/CUtils.h

if CRAYONS_PTHREADS_ENABLED
libCrayons_la_SOURCES += CPThreadMutex.c $(top_srcdir)/include/CPThreadMutex.h
else
libCrayons_la_SOURCES += $(top_srcdir)/include/CNoThreadMutex.h
endif

if CRAYONS_X11_ENABLED
libCrayons_la_SOURCES += \
	CX11Surface.c \
	$(top_srcdir)/include/CX11Surface.h \
	$(top_srcdir)/include/CrayonsSurfaceX11.h
endif
