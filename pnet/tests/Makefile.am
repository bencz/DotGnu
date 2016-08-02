noinst_PROGRAMS = test_thread test_crypt

test_thread_SOURCES = test_thread.c \
					  ilunit.c \
					  ilunit.h
test_thread_LDADD   = ../image/libILImage.a ../support/libILSupport.a \
			  		  $(GCLIBS)

test_crypt_SOURCES  = test_crypt.c \
					  ilunit.c
test_crypt_LDADD    = ../image/libILImage.a ../support/libILSupport.a \
					  $(GCLIBS)	

AM_CFLAGS = -I$(top_srcdir)/include -I$(top_srcdir)/libgc/include

TESTS = test_thread test_crypt

