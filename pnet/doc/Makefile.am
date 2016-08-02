info_TEXINFOS = pnettools.texi
pnettools_TEXINFOS = pnetasm.texi
CLEANFILES = pnettools.info pnettools.info-1 pnettools.info-2 \
			 pnettools.info-3 pnettools.info-4 \
			 cvmdoc/cvm.xml cvmdoc/index.html

EXTRA_DIST = cvmdoc.py mkcvmdoc.sh

all-local: cvmdoc/index.html

CVM_INPUTS = $(top_srcdir)/engine/cvm.c \
             $(top_srcdir)/engine/cvm_arith.c \
             $(top_srcdir)/engine/cvm_branch.c \
             $(top_srcdir)/engine/cvm_call.c \
             $(top_srcdir)/engine/cvm_compare.c \
             $(top_srcdir)/engine/cvm_const.c \
             $(top_srcdir)/engine/cvm_conv.c \
             $(top_srcdir)/engine/cvm_except.c \
             $(top_srcdir)/engine/cvm_inline.c \
             $(top_srcdir)/engine/cvm_ptr.c \
             $(top_srcdir)/engine/cvm_stack.c \
             $(top_srcdir)/engine/cvm_var.c

cvmdoc/index.html: $(CVM_INPUTS) cvmdoc.py
	$(SHELL) "$(srcdir)/mkcvmdoc.sh" "$(top_srcdir)" "$(top_builddir)"

clean-local:
	rm -rf $(srcdir)/cvmdoc
