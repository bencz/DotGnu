lib_LIBRARIES = libILSupport.a
noinst_PROGRAMS = mkcategory mknumber mkcase
EXTRA_DIST = gen_errno.sh

UNICODE_INCLUDES = unicase.c unicat.c uninum.c

EXTRA_libILSupport_a_SOURCES = $(UNICODE_INCLUDES)

BUILT_SOURCES = errno_map.c

libILSupport_a_SOURCES = aes.c \
						 allocate.c \
						 ansi.c \
						 bignum.c \
						 clflush.c \
						 cmdline.c \
						 console.c \
						 cvt_float.c \
						 decimal.c \
						 def_gc.c \
						 des.c \
						 dir.c \
						 dns.c \
						 dynlib.c \
						 errno_map.c \
						 expand.c \
						 file.c \
						 filemap.c \
						 getcwd.c \
						 guid.c \
						 hash.c \
						 hashtab.c \
						 hb_gc.c \
						 interlocked.c \
						 interlocked.h \
						 interlocked_any.h \
						 interlocked_arm.h \
						 interlocked_mips.h \
						 interlocked_ppc.h \
						 interlocked_x86.h \
						 interlocked_slist.c \
						 interlocked_slist.h \
						 intern.c \
						 interrupt.c \
						 interrupt.h \
						 interrupt_posix.c \
						 interrupt_win32.c \
						 list.c \
						 locale.c \
						 md5.c \
						 mem_debug.h \
						 memory.c \
						 mempool.c \
						 memstack.c \
						 monitor.c \
						 mul_long.c \
						 mutex.c \
						 no_defs.c \
						 no_defs.h \
						 path.c \
						 pt_defs.c \
						 pt_defs.h \
						 queue.c \
						 rc2.c \
						 read_float.c \
						 regex.c \
						 rem_float.c \
						 ripemd160.c \
						 semaphore.c \
						 serial.c \
						 sha1.c \
						 sha256.c \
						 sha384.c \
						 sha512.c \
						 socket.c \
						 spawn.c \
						 strings.c \
						 test_float.c \
						 thr_choose.h \
						 thr_defs.h \
						 thread.c \
						 time.c \
						 unicode.c \
						 utf8.c \
						 utf16.c \
						 w32_defs.c \
						 w32_defs.h \
						 wait.c \
						 wait_mutex.c \
						 wait_mutex.h \
						 wait_event.c \
						 wakeup.c \
						 write_float.c \
						 xml.c

mkcategory_SOURCES = mkcategory.c

mknumber_SOURCES = mknumber.c

mkcase_SOURCES = mkcase.c

AM_CFLAGS = -I$(top_srcdir)/libgc/include \
			-I$(top_builddir)/libgc/libatomic_ops/src \
			-I$(top_srcdir)/include \
			-DCSCC_LIB_PREFIX=\"$(libdir)\" \
		    -DCSCC_BIN_PREFIX=\"$(bindir)\" \
		    -DCSCC_DATA_PREFIX=\"$(datadir)\" \
			-DCSCC_HOST_TRIPLET=\"$(host_triplet)\"

unicode.$(OBJEXT): unicode.c $(UNICODE_INCLUDES)

CLEANFILES = errno_map.c

errno_map.c: $(srcdir)/gen_errno.sh
	$(SHELL) $(srcdir)/gen_errno.sh ${top_srcdir}/include/il_errno.h \
		$(COMPILE) >errno_map.c
