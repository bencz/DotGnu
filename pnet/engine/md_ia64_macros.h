/*
 * md_ia64_macros.h - Code generation macros for the Itanium processor
 * 
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by :CH Gowri Kumar <gkumar@csa.iisc.ernet.in>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#ifndef _MD_IA64_MACROS_H
#define _MD_IA64_MACROS_H

#ifdef __cplusplus
extern "C" {
#endif

#include <unistd.h>
#include <fcntl.h>
#include <sys/mman.h>
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <stdarg.h>
#include <string.h>

typedef enum {
	IA64_R0=0,
	IA64_R1,
	IA64_R2,
	IA64_R3,
	IA64_R4,
	IA64_R5,
	IA64_R6,
	IA64_R7,
	IA64_R8,
	IA64_R9,
	IA64_R10,
	IA64_R11,
	IA64_R12,
	IA64_R13,
	IA64_R14,
	IA64_R15,
	IA64_R16,
	IA64_R17,
	IA64_R18,
	IA64_R19,
	IA64_R20,
	IA64_R21,
	IA64_R22,
	IA64_R23,
	IA64_R24,
	IA64_R25,
	IA64_R26,
	IA64_R27,
	IA64_R28,
	IA64_R29,
	IA64_R30,
	IA64_R31,
	IA64_R32,
	IA64_R33,
	IA64_R34,
	IA64_R35,
	IA64_R36,
	IA64_R37,
	IA64_R38,
	IA64_R39,
	IA64_R40,
	IA64_R41,
	IA64_R42,
	IA64_R43,
	IA64_R44,
	IA64_R45,
	IA64_R46,
	IA64_R47,
	IA64_R48,
	IA64_R49,
	IA64_R50,
	IA64_R51,
	IA64_R52,
	IA64_R53,
	IA64_R54,
	IA64_R55,
	IA64_R56,
	IA64_R57,
	IA64_R58,
	IA64_R59,
	IA64_R60,
	IA64_R61,
	IA64_R62,
	IA64_R63,
	IA64_R64,
	IA64_R65,
	IA64_R66,
	IA64_R67,
	IA64_R68,
	IA64_R69,
	IA64_R70,
	IA64_R71,
	IA64_R72,
	IA64_R73,
	IA64_R74,
	IA64_R75,
	IA64_R76,
	IA64_R77,
	IA64_R78,
	IA64_R79,
	IA64_R80,
	IA64_R81,
	IA64_R82,
	IA64_R83,
	IA64_R84,
	IA64_R85,
	IA64_R86,
	IA64_R87,
	IA64_R88,
	IA64_R89,
	IA64_R90,
	IA64_R91,
	IA64_R92,
	IA64_R93,
	IA64_R94,
	IA64_R95,
	IA64_R96,
	IA64_R97,
	IA64_R98,
	IA64_R99,
	IA64_R100,
	IA64_R101,
	IA64_R102,
	IA64_R103,
	IA64_R104,
	IA64_R105,
	IA64_R106,
	IA64_R107,
	IA64_R108,
	IA64_R109,
	IA64_R110,
	IA64_R111,
	IA64_R112,
	IA64_R113,
	IA64_R114,
	IA64_R115,
	IA64_R116,
	IA64_R117,
	IA64_R118,
	IA64_R119,
	IA64_R120,
	IA64_R121,
	IA64_R122,
	IA64_R123,
	IA64_R124,
	IA64_R125,
	IA64_R126,
	IA64_R127,
	IA64_GP = IA64_R1,
	IA64_RET = IA64_R8,
	IA64_SP = IA64_R12,
	IA64_TP = IA64_R13
}IA64GeneralRegisters;

typedef enum {
	IA64_F0=0,
	IA64_F1,
	IA64_F2,
	IA64_F3,
	IA64_F4,
	IA64_F5,
	IA64_F6,
	IA64_F7,
	IA64_F8,
	IA64_F9,
	IA64_F10,
	IA64_F11,
	IA64_F12,
	IA64_F13,
	IA64_F14,
	IA64_F15,
	IA64_F16,
	IA64_F17,
	IA64_F18,
	IA64_F19,
	IA64_F20,
	IA64_F21,
	IA64_F22,
	IA64_F23,
	IA64_F24,
	IA64_F25,
	IA64_F26,
	IA64_F27,
	IA64_F28,
	IA64_F29,
	IA64_F30,
	IA64_F31,
	IA64_F32,
	IA64_F33,
	IA64_F34,
	IA64_F35,
	IA64_F36,
	IA64_F37,
	IA64_F38,
	IA64_F39,
	IA64_F40,
	IA64_F41,
	IA64_F42,
	IA64_F43,
	IA64_F44,
	IA64_F45,
	IA64_F46,
	IA64_F47,
	IA64_F48,
	IA64_F49,
	IA64_F50,
	IA64_F51,
	IA64_F52,
	IA64_F53,
	IA64_F54,
	IA64_F55,
	IA64_F56,
	IA64_F57,
	IA64_F58,
	IA64_F59,
	IA64_F60,
	IA64_F61,
	IA64_F62,
	IA64_F63,
	IA64_F64,
	IA64_F65,
	IA64_F66,
	IA64_F67,
	IA64_F68,
	IA64_F69,
	IA64_F70,
	IA64_F71,
	IA64_F72,
	IA64_F73,
	IA64_F74,
	IA64_F75,
	IA64_F76,
	IA64_F77,
	IA64_F78,
	IA64_F79,
	IA64_F80,
	IA64_F81,
	IA64_F82,
	IA64_F83,
	IA64_F84,
	IA64_F85,
	IA64_F86,
	IA64_F87,
	IA64_F88,
	IA64_F89,
	IA64_F90,
	IA64_F91,
	IA64_F92,
	IA64_F93,
	IA64_F94,
	IA64_F95,
	IA64_F96,
	IA64_F97,
	IA64_F98,
	IA64_F99,
	IA64_F100,
	IA64_F101,
	IA64_F102,
	IA64_F103,
	IA64_F104,
	IA64_F105,
	IA64_F106,
	IA64_F107,
	IA64_F108,
	IA64_F109,
	IA64_F110,
	IA64_F111,
	IA64_F112,
	IA64_F113,
	IA64_F114,
	IA64_F115,
	IA64_F116,
	IA64_F117,
	IA64_F118,
	IA64_F119,
	IA64_F120,
	IA64_F121,
	IA64_F122,
	IA64_F123,
	IA64_F124,
	IA64_F125,
	IA64_F126,
	IA64_F127
}IA64FloatingPointRegisters;

typedef enum {
	IA64_AR0=0,
	IA64_AR1,
	IA64_AR2,
	IA64_AR3,
	IA64_AR4,
	IA64_AR5,
	IA64_AR6,
	IA64_AR7,
	IA64_AR8,
	IA64_AR9,
	IA64_AR10,
	IA64_AR11,
	IA64_AR12,
	IA64_AR13,
	IA64_AR14,
	IA64_AR15,
	IA64_AR16,
	IA64_AR17,
	IA64_AR18,
	IA64_AR19,
	IA64_AR20,
	IA64_AR21,
	IA64_AR22,
	IA64_AR23,
	IA64_AR24,
	IA64_AR25,
	IA64_AR26,
	IA64_AR27,
	IA64_AR28,
	IA64_AR29,
	IA64_AR30,
	IA64_AR31,
	IA64_AR32,
	IA64_AR33,
	IA64_AR34,
	IA64_AR35,
	IA64_AR36,
	IA64_AR37,
	IA64_AR38,
	IA64_AR39,
	IA64_AR40,
	IA64_AR41,
	IA64_AR42,
	IA64_AR43,
	IA64_AR44,
	IA64_AR45,
	IA64_AR46,
	IA64_AR47,
	IA64_AR48,
	IA64_AR49,
	IA64_AR50,
	IA64_AR51,
	IA64_AR52,
	IA64_AR53,
	IA64_AR54,
	IA64_AR55,
	IA64_AR56,
	IA64_AR57,
	IA64_AR58,
	IA64_AR59,
	IA64_AR60,
	IA64_AR61,
	IA64_AR62,
	IA64_AR63,
	IA64_AR64,
	IA64_AR65,
	IA64_AR66,
	IA64_AR67,
	IA64_AR68,
	IA64_AR69,
	IA64_AR70,
	IA64_AR71,
	IA64_AR72,
	IA64_AR73,
	IA64_AR74,
	IA64_AR75,
	IA64_AR76,
	IA64_AR77,
	IA64_AR78,
	IA64_AR79,
	IA64_AR80,
	IA64_AR81,
	IA64_AR82,
	IA64_AR83,
	IA64_AR84,
	IA64_AR85,
	IA64_AR86,
	IA64_AR87,
	IA64_AR88,
	IA64_AR89,
	IA64_AR90,
	IA64_AR91,
	IA64_AR92,
	IA64_AR93,
	IA64_AR94,
	IA64_AR95,
	IA64_AR96,
	IA64_AR97,
	IA64_AR98,
	IA64_AR99,
	IA64_AR100,
	IA64_AR101,
	IA64_AR102,
	IA64_AR103,
	IA64_AR104,
	IA64_AR105,
	IA64_AR106,
	IA64_AR107,
	IA64_AR108,
	IA64_AR109,
	IA64_AR110,
	IA64_AR111,
	IA64_AR112,
	IA64_AR113,
	IA64_AR114,
	IA64_AR115,
	IA64_AR116,
	IA64_AR117,
	IA64_AR118,
	IA64_AR119,
	IA64_AR120,
	IA64_AR121,
	IA64_AR122,
	IA64_AR123,
	IA64_AR124,
	IA64_AR125,
	IA64_AR126,
	IA64_AR127,
	IA64_KR0 = IA64_AR0,
	IA64_KR1 = IA64_AR1,
	IA64_KR2 = IA64_AR2,
	IA64_KR3 = IA64_AR3,
	IA64_KR4 = IA64_AR4,
	IA64_KR5 = IA64_AR5,
	IA64_KR6 = IA64_AR6,
	IA64_KR7 = IA64_AR7,
	IA64_AR_RSC = IA64_AR16,
	IA64_AR_BSP = IA64_AR17,
	IA64_AR_BSPSTORE = IA64_AR18,
	IA64_AR_RNAT = IA64_AR19,
	IA64_AR_FCR = IA64_AR21,
	IA64_AR_EFLAG = IA64_AR24,
	IA64_AR_CCV = IA64_AR32,
	IA64_AR_UNAT = IA64_AR36,
	IA64_AR_FPSR = IA64_AR40,
	IA64_AR_ITC = IA64_AR44,
	IA64_AR_PFS = IA64_AR64,
	IA64_AR_LC = IA64_AR65,
	IA64_AR_EC = IA64_AR66

}IA64ApplicationRegisters;

typedef enum {
	IA64_P0=0,
	IA64_P1,
	IA64_P2,
	IA64_P3,
	IA64_P4,
	IA64_P5,
	IA64_P6,
	IA64_P7,
	IA64_P8,
	IA64_P9,
	IA64_P10,
	IA64_P11,
	IA64_P12,
	IA64_P13,
	IA64_P14,
	IA64_P15,
	IA64_P16,
	IA64_P17,
	IA64_P18,
	IA64_P19,
	IA64_P20,
	IA64_P21,
	IA64_P22,
	IA64_P23,
	IA64_P24,
	IA64_P25,
	IA64_P26,
	IA64_P27,
	IA64_P28,
	IA64_P29,
	IA64_P30,
	IA64_P31,
	IA64_P32,
	IA64_P33,
	IA64_P34,
	IA64_P35,
	IA64_P36,
	IA64_P37,
	IA64_P38,
	IA64_P39,
	IA64_P40,
	IA64_P41,
	IA64_P42,
	IA64_P43,
	IA64_P44,
	IA64_P45,
	IA64_P46,
	IA64_P47,
	IA64_P48,
	IA64_P49,
	IA64_P50,
	IA64_P51,
	IA64_P52,
	IA64_P53,
	IA64_P54,
	IA64_P55,
	IA64_P56,
	IA64_P57,
	IA64_P58,
	IA64_P59,
	IA64_P60,
	IA64_P61,
	IA64_P62,
	IA64_P63
}IA64PredicateRegisters;

typedef enum {
	IA64_B0=0,
	IA64_B1,
	IA64_B2,
	IA64_B3,
	IA64_B4,
	IA64_B5,
	IA64_B6,
	IA64_B7,
	IA64_RP = IA64_B0
}IA64BranchRegisters;






typedef enum {
	MII=0,
	MII_,
	MI_I,
	MI_I_,
	MLX,
	MLX_,
	MMI=0X8,
	MMI_,
	M_MI,
	M_MI_,
	MFI,
	MFI_,
	MMF,
	MMF_,
	MIB,
	MIB_,
	MBB,
	MBB_,
	BBB=0X16,
	BBB_,
	MMB,
	MMB_,
	MFB=0X1C,
	MFB_
}Template;

typedef enum
{
	IA64_AUNIT=0,
	IA64_IUNIT,
	IA64_MUNIT,
	IA64_BUNIT,
	IA64_FUNIT,
	IA64_XUNIT
}IA64Units;

#define CODESIZE 256
#define r(N) (0+(N))
#define p(N) (0+(N))
#define f(N) (0+(N))
#define b(N) (0+(N))
#define ar(N) (0+(N))


typedef unsigned long UL;
#define MASK(N) ((UL)(((((UL)1)<<(N)))-1))

#define CHECK(I,N) (((UL)(I))&(MASK(N)))
		
#define FIELD1(I) (CHECK(I,1))
#define FIELD2(I) (CHECK(I,2))
#define FIELD3(I) (CHECK(I,3))
#define FIELD4(I) (CHECK(I,4))
#define FIELD5(I) (CHECK(I,5))
#define FIELD6(I) (CHECK(I,6))
#define FIELD7(I) (CHECK(I,7))
#define FIELD8(I) (CHECK(I,8))
#define FIELD9(I) (CHECK(I,9))
#define FIELD10(I) (CHECK(I,1))

#define FIELD11(I) (CHECK(I,11))
#define FIELD12(I) (CHECK(I,12))
#define FIELD13(I) (CHECK(I,13))
#define FIELD14(I) (CHECK(I,14))
#define FIELD15(I) (CHECK(I,15))
#define FIELD16(I) (CHECK(I,16))
#define FIELD17(I) (CHECK(I,17))
#define FIELD18(I) (CHECK(I,18))
#define FIELD19(I) (CHECK(I,19))
#define FIELD20(I) (CHECK(I,20))

#define FIELD31(I) (CHECK(I,31))
#define FIELD32(I) (CHECK(I,32))
#define FIELD33(I) (CHECK(I,33))
#define FIELD34(I) (CHECK(I,34))
#define FIELD35(I) (CHECK(I,35))
#define FIELD36(I) (CHECK(I,36))
#define FIELD37(I) (CHECK(I,37))
#define FIELD38(I) (CHECK(I,38))
#define FIELD39(I) (CHECK(I,39))
#define FIELD40(I) (CHECK(I,40))
#define FIELD41(I) (CHECK(I,41))

#define INRANGE(val,width) \
	(((val)<(~((~0)<<((width)-1)))) && ((val) > -(~((~0)<<((width)-1)))))

/* A-UNIT encoding macros */
/* A1 */
#define A1(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 29) |\
	(((unsigned long)(P06)) << 27) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* A2 */
#define A2(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 29) |\
	(((unsigned long)(P06)) << 27) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* A3 */
#define A3(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 29) |\
	(((unsigned long)(P06)) << 27) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* A4 */
#define A4(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* A5 */
#define A5(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 27) |\
	(((unsigned long)(P04)) << 22) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* A6 */
#define A6(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) << 12) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* A7 */
#define A7(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) << 12) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* A8 */
#define A8(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) << 12) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* A9 */
#define A9(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 29) |\
	(((unsigned long)(P06)) << 27) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* A10 */
#define A10(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 29) |\
	(((unsigned long)(P06)) << 27) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* 
 * A-Unit Instructions 
 */

/*A1 Integer ALU - Register - Register*/
#define ADD_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,0,0,r3,r2,r1,pr)

#define ADDINC_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,0,1,r3,r2,r1,pr)

#define SUB_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,1,1,r3,r2,r1,pr)

#define SUBDEC_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,1,0,r3,r2,r1,pr)

#define ADDP4_rrr(pr,r1,r2,r3) \
   	A1(8,0,0,0,2,0,r3,r2,r1,pr)

#define AND_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,3,0,r3,r2,r1,pr)

#define ANDCM_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,3,1,r3,r2,r1,pr)

#define OR_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,3,2,r3,r2,r1,pr)

#define XOR_rrr(pr,r1,r2,r3) \
	A1(8,0,0,0,3,3,r3,r2,r1,pr)

/*A2 Shift Left and Add*/
#define SHLADD_rrcnt2r(pr,r1,r2,count2,r3) \
	A2(8,0,0,0,4,count2-1,r3,r2,r1,pr)
	
#define SHLADDP4_rrcnt2r(pr,r1,r2,count2,r3) \
	A2(8,0,0,0,6,count2-1,r3,r2,r1,pr)

/*A3 Integer ALU-Immediate8-Register*/
#define SUB_rimm8r(pr,r1,imm8,r3) \
	A3(8,((imm8>>7)&MASK(1)),0,0,9,1,r3,(imm8&MASK(7)),r1,pr)

#define AND_rimm8r(pr,r1,imm8,r3) \
	A3(8,((imm8>>7)&MASK(1)),0,0,0xB,0,r3,(imm8&MASK(7)),r1,pr)

#define ANDCM_rimm8r(pr,r1,imm8,r3) \
	A3(8,((imm8>>7)&MASK(1)),0,0,0xB,1,r3,(imm8&MASK(7)),r1,pr)

#define OR_rimm8r(pr,r1,imm8,r3) \
	A3(8,((imm8>>7)&MASK(1)),0,0,0xB,2,r3,(imm8&MASK(7)),r1,pr)

#define XOR_rimm8r(pr,r1,imm8,r3) \
	A3(8,((imm8>>7)&MASK(1)),0,0,0xB,3,r3,(imm8&MASK(7)),r1,pr)

/*A4 Add Immediate14*/
#define ADDS_rimm14r(pr,r1,imm14,r3) \
	A4(8,(imm14>>13)&MASK(1),2,0,((imm14>>7)&MASK(6)),r3,(imm14&MASK(7)),r1,pr)

#define ADDP4_rimm14r(pr,r1,imm14,r3) \
	A4(8,(imm14>>13)&MASK(1),3,0,((imm14>>7)&MASK(6)),r3,(imm14&MASK(7)),r1,pr)

#define MOV_rr(pr,r1,r2) \
	ADDS_rimm14r(pr,r1,0,r2)

/* A5 Add Immediate22 */
#define ADDL_rimm22r(pr,r1,imm22,r3) \
	A5(9,((imm22>>21)&MASK(1)),((imm22>>7)&MASK(9)),((imm22>>16)&MASK(5)),r3,((imm22)&MASK(7)),r1,pr)	

#define MOV_rimm22(pr,r1,imm22) \
	ADDL_rimm22r(pr,r1,imm22,0)

/* A6 Integer Compare - Register - Register */

#define CMP_LT_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,0,0,p2,r3,r2,0,p1,pr)

#define CMP_LTU_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,0,0,p2,r3,r2,0,p1,pr)

#define CMP_EQ_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,0,0,p2,r3,r2,0,p1,pr)

#define CMP_LT_UNC_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,0,0,p2,r3,r2,1,p1,pr)

#define CMP_LTU_UNC_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,0,0,p2,r3,r2,1,p1,pr)

#define CMP_EQ_UNC_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,0,0,p2,r3,r2,1,p1,pr)

#define CMP_EQ_AND_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,0,1,p2,r3,r2,0,p1,pr)

#define CMP_EQ_OR_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,0,1,p2,r3,r2,0,p1,pr)

#define CMP_EQ_OR_ANDCM_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,0,1,p2,r3,r2,0,p1,pr)

#define CMP_NE_AND_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,0,1,p2,r3,r2,1,p1,pr)

#define CMP_NE_OR_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,0,1,p2,r3,r2,1,p1,pr)

#define CMP_NE_OR_ANDCM_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,0,1,p2,r3,r2,1,p1,pr)

#define CMP4_LT_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,1,0,p2,r3,r2,0,p1,pr)

#define CMP4_LTU_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,1,0,p2,r3,r2,0,p1,pr)

#define CMP4_EQ_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,1,0,p2,r3,r2,0,p1,pr)

#define CMP4_LT_UNC_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,1,0,p2,r3,r2,1,p1,pr)

#define CMP4_LTU_UNC_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,1,0,p2,r3,r2,1,p1,pr)

#define CMP4_EQ_UNC_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,1,0,p2,r3,r2,1,p1,pr)

#define CMP4_EQ_AND_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,1,1,p2,r3,r2,0,p1,pr)

#define CMP4_EQ_OR_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,1,1,p2,r3,r2,0,p1,pr)

#define CMP4_EQ_OR_ANDCM_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,1,1,p2,r3,r2,0,p1,pr)

#define CMP4_NE_AND_pprr(pr,p1,p2,r2,r3) \
	A6(0xC,0,1,1,p2,r3,r2,1,p1,pr)

#define CMP4_NE_OR_pprr(pr,p1,p2,r2,r3) \
	A6(0xD,0,1,1,p2,r3,r2,1,p1,pr)

#define CMP4_NE_OR_ANDCM_pprr(pr,p1,p2,r2,r3) \
	A6(0xE,0,1,1,p2,r3,r2,1,p1,pr)

/* A7 - Integer Compare to Zero - Register */

#define CMP_GT_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,0,0,p2,r3,0,0,p1,pr)

#define CMP_GT_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,0,0,p2,r3,0,0,p1,pr)

#define CMP_GT_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,0,0,p2,r3,0,0,p1,pr)

#define CMP_LE_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,0,0,p2,r3,0,1,p1,pr)

#define CMP_LE_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,0,0,p2,r3,0,1,p1,pr)

#define CMP_LE_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,0,0,p2,r3,0,1,p1,pr)

#define CMP_GE_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,0,1,p2,r3,0,0,p1,pr)

#define CMP_GE_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,0,1,p2,r3,0,0,p1,pr)

#define CMP_GE_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,0,1,p2,r3,0,0,p1,pr)

#define CMP_LT_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,0,1,p2,r3,0,1,p1,pr)

#define CMP_LT_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,0,1,p2,r3,0,1,p1,pr)

#define CMP_LT_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,0,1,p2,r3,0,1,p1,pr)

#define CMP4_GT_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,1,0,p2,r3,0,0,p1,pr)

#define CMP4_GT_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,1,0,p2,r3,0,0,p1,pr)

#define CMP4_GT_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,0,0,p2,r3,0,0,p1,pr)

#define CMP4_LE_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,0,0,p2,r3,0,1,p1,pr)

#define CMP4_LE_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,1,0,p2,r3,0,1,p1,pr)

#define CMP4_LE_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,1,0,p2,r3,0,1,p1,pr)

#define CMP4_GE_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,1,1,p2,r3,0,0,p1,pr)

#define CMP4_GE_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,1,1,p2,r3,0,0,p1,pr)

#define CMP4_GE_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,1,1,p2,r3,0,0,p1,pr)

#define CMP4_LT_AND_pprr(pr,p1,p2,r0,r3) \
	A7(0xC,1,1,1,p2,r3,0,1,p1,pr)

#define CMP4_LT_OR_pprr(pr,p1,p2,r0,r3) \
	A7(0xD,1,1,1,p2,r3,0,1,p1,pr)

#define CMP4_LT_OR_ANDCM_pprr(pr,p1,p2,r0,r3) \
	A7(0xE,1,1,1,p2,r3,0,1,p1,pr)

/* A8 Integer Compare - Immediate - Register */
#define CMP_LT_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),2,0,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP_LTU_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),2,0,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP_EQ_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),2,0,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP_LT_UNC_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),2,0,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP_LTU_UNC_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),2,0,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP_EQ_UNC_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),2,0,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP_EQ_AND_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),2,1,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP_EQ_OR_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),2,1,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP_EQ_OR_ANDCM_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),2,1,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP_NE_AND_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),2,1,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP_NE_OR_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),2,1,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP_NE_OR_ANDCM_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),2,1,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP4_LT_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),3,0,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP4_LTU_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),3,0,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP4_EQ_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),3,0,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP4_LT_UNC_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),3,0,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP4_LTU_UNC_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),3,0,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP4_EQ_UNC_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),3,0,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP4_EQ_AND_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),3,1,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP4_EQ_OR_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),3,1,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP4_EQ_OR_ANDCM_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),3,1,p2,r3,((imm8) & MASK(7)),0,p1,pr)

#define CMP4_NE_AND_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xC,((imm8>>7)&MASK(1)),3,1,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP4_NE_OR_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xD,((imm8>>7)&MASK(1)),3,1,p2,r3,((imm8) & MASK(7)),1,p1,pr)

#define CMP4_NE_OR_ANDCM_ppimm8r(pr,p1,p2,imm8,r3) \
	A8(0xE,((imm8>>7)&MASK(1)),3,1,p2,r3,((imm8) & MASK(7)),1,p1,pr)



/* I-UNIT encoding macros */
/* I1 */
#define I1(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 27) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 13) |\
	(((unsigned long)(P11)) <<  6) |\
	(((unsigned long)(P12)) <<  0) \
	)

/* I2 */
#define I2(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 27) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 13) |\
	(((unsigned long)(P11)) <<  6) |\
	(((unsigned long)(P12)) <<  0) \
	)

/* I3 */
#define I3(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 24) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 13) |\
	(((unsigned long)(P11)) <<  6) |\
	(((unsigned long)(P12)) <<  0) \
	)

/* I4 */
#define I4(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 20) |\
	(((unsigned long)(P09)) << 13) |\
	(((unsigned long)(P10)) <<  6) |\
	(((unsigned long)(P11)) <<  0) \
	)

/* I5 */
#define I5(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 27) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 13) |\
	(((unsigned long)(P11)) <<  6) |\
	(((unsigned long)(P12)) <<  0) \
	)

/* I6 */
#define I6(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12,P13,P14)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 27) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 19) |\
	(((unsigned long)(P11)) << 14) |\
	(((unsigned long)(P12)) << 13) |\
	(((unsigned long)(P13)) <<  6) |\
	(((unsigned long)(P14)) <<  0) \
	)

/* I7 */
#define I7(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 27) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 13) |\
	(((unsigned long)(P11)) <<  6) |\
	(((unsigned long)(P12)) <<  0) \
	)

/* I8 */
#define I8(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 25) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 13) |\
	(((unsigned long)(P11)) <<  6) |\
	(((unsigned long)(P12)) <<  0) \
	)

/* I9 */
#define I9(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11,P12)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 32) |\
	(((unsigned long)(P06)) << 30) |\
	(((unsigned long)(P07)) << 28) |\
	(((unsigned long)(P08)) << 27) |\
	(((unsigned long)(P09)) << 20) |\
	(((unsigned long)(P10)) << 13) |\
	(((unsigned long)(P11)) <<  6) |\
	(((unsigned long)(P12)) <<  0) \
	)

/* I10 */
#define I10(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* I11 */
#define I11(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 14) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* I12 */
#define I12(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 26) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* I13 */
#define I13(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 26) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* I14 */
#define I14(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 14) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* I15 */
#define I15(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 31) |\
	(((unsigned long)(P03)) << 27) |\
	(((unsigned long)(P04)) << 20) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* I16 */
#define I16(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 14) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) << 12) |\
	(((unsigned long)(P10)) <<  6) |\
	(((unsigned long)(P11)) <<  0) \
	)

/* I17 */
#define I17(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 14) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) << 12) |\
	(((unsigned long)(P10)) <<  6) |\
	(((unsigned long)(P11)) <<  0) \
	)

/* I18 */
#define I18(P01)   \
	(\
	(((unsigned long)(P01)) << 41) \
	)

/* I19 */
#define I19(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 26) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* I20 */
#define I20(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 20) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* I21 */
#define I21(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 24) |\
	(((unsigned long)(P05)) << 23) |\
	(((unsigned long)(P06)) << 22) |\
	(((unsigned long)(P07)) << 20) |\
	(((unsigned long)(P08)) << 13) |\
	(((unsigned long)(P09)) <<  9) |\
	(((unsigned long)(P10)) <<  6) |\
	(((unsigned long)(P11)) <<  0) \
	)

/* I22 */
#define I22(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 16) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* I23 */
#define I23(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 32) |\
	(((unsigned long)(P05)) << 24) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* I24 */
#define I24(P01,P02,P03,P04,P05)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) <<  6) |\
	(((unsigned long)(P05)) <<  0) \
	)

/* I25 */
#define I25(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* I26 */
#define I26(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* I27 */
#define I27(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* I28 */
#define I28(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* I29 */
#define I29(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)



/* I1 - Multimedia Multiply and Shift */

#define PMPYSHR2_rrrcount2(qp,r1,r2,r3,count2) \
	I1(7,0,0,1,0,((count2 == 0)?0:(count2 == 7)?1:(count2 == 15)?2:3),3,0,r3,r2,r1,qp)
	
#define PMPYSHR2_U_rrrcount2(qp,r1,r2,r3,count2) \
	I1(7,0,0,1,0,((count2 == 0)?0:(count2 == 7)?1:(count2 == 15)?2:3),1,0,r3,r2,r1,qp)

/* I2 - Multimedia Multiply/Mix/Pack/UnPack */

#define PMPY2_R_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,3,1,0,r3,r2,r1,qp)

#define PMPY2_L_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,3,3,0,r3,r2,r1,qp)

#define MIX1_R_rrr(qp,r1,r2,r3) \
	I2(7,0,2,0,0,2,0,0,r3,r2,r1,qp)

#define MIX2_R_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,2,0,0,r3,r2,r1,qp)

#define MIX4_R_rrr(qp,r1,r2,r3) \
	I2(7,1,2,0,0,2,0,0,r3,r2,r1,qp)

#define MIX1_L_rrr(qp,r1,r2,r3) \
	I2(7,0,2,0,0,2,2,0,r3,r2,r1,qp)

#define MIX2_L_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,2,2,0,r3,r2,r1,qp)

#define MIX4_L_rrr(qp,r1,r2,r3) \
	I2(7,1,2,0,0,2,2,0,r3,r2,r1,qp)

#define PACK2_USS_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,0,0,0,r3,r2,r1,qp)

#define PACK2_SSS_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,0,2,0,r3,r2,r1,qp)

#define PACK4_SSS_rrr(qp,r1,r2,r3) \
	I2(7,1,2,0,0,0,2,0,r3,r2,r1,qp)

#define UNPACK1_H_rrr(qp,r1,r2,r3) \
	I2(7,0,2,0,0,1,0,0,r3,r2,r1,qp)

#define UNPACK2_H_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,1,0,0,r3,r2,r1,qp)

#define UNPACK4_H_rrr(qp,r1,r2,r3) \
	I2(7,1,2,0,0,1,0,0,r3,r2,r1,qp)

#define UNPACK1_L_rrr(qp,r1,r2,r3) \
	I2(7,0,2,0,0,1,2,0,r3,r2,r1,qp)

#define UNPACK2_L_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,1,2,0,r3,r2,r1,qp)

#define UNPACK4_L_rrr(qp,r1,r2,r3) \
	I2(7,1,2,0,0,1,2,0,r3,r2,r1,qp)

#define PMIN1_U_rrr(qp,r1,r2,r3) \
	I2(7,0,2,0,0,0,1,0,r3,r2,r1,qp)

#define PMAX1_U_rrr(qp,r1,r2,r3) \
	I2(7,0,2,0,0,1,1,0,r3,r2,r1,qp)

#define PMIN2_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,0,3,0,r3,r2,r1,qp)

#define PMAX2_rrr(qp,r1,r2,r3) \
	I2(7,0,2,1,0,1,3,0,r3,r2,r1,qp)

#define PSAD1_rrr(qp,r1,r2,r3) \
	I2(7,0,2,0,0,2,3,0,r3,r2,r1,qp)

/* I3 - Multimedia Mux1 */

#define MUX1_rrmbtype4(qp,r1,r2,mbtype4) \
	I3(7,0,3,0,0,2,2,0,0/*TODO*/,r2,r1,qp)

/* I4 - Multemedia Mux2 */

#define MUX2_rrmhtype8(qp,r1,r2,mhtype8) \
	I4(7,0,3,1,0,2,2,mhtype8,r2,r1,qp)

/* I5 - Shift Right - Variable */

#define PSHR2_rrr(qp,r1,r3,r2) \
	I5(7,0,0,1,0,0,2,0,r3,r2,r1,qp)

#define PSHR4_rrr(qp,r1,r3,r2) \
	I5(7,1,0,0,0,0,2,0,r3,r2,r1,qp)

#define SHR_rrr(qp,r1,r3,r2) \
	I5(7,1,0,1,0,0,2,0,r3,r2,r1,qp)

#define PSHR2_U_rrr(qp,r1,r3,r2) \
	I5(7,0,0,1,0,0,0,0,r3,r2,r1,qp)

#define PSHR4_U_rrr(qp,r1,r3,r2) \
	I5(7,1,0,0,0,0,0,0,r3,r2,r1,qp)

#define SHR_U_rrr(qp,r1,r3,r2) \
	I5(7,1,0,1,0,0,0,0,r3,r2,r1,qp)

/* I6 - Multimedia Shift Right - Fixed */

#define PSHR2_rrcount5(qp,r1,r3,count5) \
	I6(7,0,1,1,0,3,0,r3,0,count5,0,r1,qp)

#define PSHR4_rrcount5(qp,r1,r3,count5) \
	I6(7,1,1,0,0,3,0,r3,0,count5,0,r1,qp)

#define PSHR2_U_rrcount5(qp,r1,r3,count5) \
	I6(7,0,1,1,0,1,0,r3,0,count5,0,r1,qp)

#define PSHR4_U_rrcount5(qp,r1,r3,count5) \
	I6(7,1,1,0,0,1,0,r3,0,count5,0,r1,qp)
	
/* I7 - Shift Left - Variable */

#define PSHL2_rrr(qp,r1,r2,r3) \
	I7(7,0,0,1,0,1,0,0,r3,r2,r1,qp)

#define PSHL4_rrr(qp,r1,r2,r3) \
	I7(7,1,0,0,0,1,0,0,r3,r2,r1,qp)

#define SHL_rrr(qp,r1,r2,r3) \
	I7(7,1,0,1,0,1,0,0,r3,r2,r1,qp)
	
/* I8 - Multimedia Shift Left - Fixed */

#define PSHL2_rrcount5(qp,r1,r2,count5) \
	I8(7,0,3,1,0,1,1,0,(31-count5),r2,r1,qp)

#define PSHL4_rrcount5(qp,r1,r2,count5) \
	I8(7,1,3,0,0,1,1,0,(31-count5),r2,r1,qp)

/* I9 - Population Count */

#define POPCNT_rr(qp,r1,r3) \
	I9(7,0,1,1,0,2,1,0,r3,0,r1,qp)

/* I10 - Shift Right Pair */

#define SHRP(qp,r1,r2,r3,count6) \
	I10(5,0,3,0,count6,r3,r2,r1,qp)

/* I11 - Extract */

#define EXTR_U_rrpos6len6(qp,r1,r3,pos6,len6) \
	I11(5,0,1,0,(len6 - 1),r3,pos6,0,r1,qp)

#define EXTR_rrpos6len6(qp,r1,r3,pos6,len6) \
	I11(5,0,1,0,(len6 - 1),r3,pos6,1,r1,qp)

/* I12 - Zero and Deposit */

#define DEP_Z_rrpos6len6(qp,r1,r2,pos6,len6) \
	I12(5,0,1,1,(len6-1),0,(63-pos6),r2,r1,qp)
	
/* I13 - Zero and Deposit Immediate8 */

#define DEP_Z_rimm8pos6len6(qp,r1,imm8,pos6,len6) \
	I13(5,((imm8>>7)&MASK(1)),1,1,(len6-1),1,(63-pos6),((imm8)&MASK(7)),r1,qp)

/* I14 - Deposit Immediate1 */

#define DEP_rimm1rpos6len6(qp,r1,imm1,r3,pos6,len6) \
	I14(5,imm1,3,1,(len6-1),r3,(63-pos6),0,r1,qp)
	
/* I15 - Deposit */

#define DEP_rrrpos6len6(qp,r1,r2,r3,pos6,len4) \
	I15(4,(63-pos6),(len4-1),r3,r2,r1,qp)

/* I16 - Test Bit */

#define TBIT_Z_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,0,0,0,p2,r3,pos6,0,0,p1,qp)
	
#define TBIT_Z_UNC_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,0,0,0,p2,r3,pos6,0,1,p1,qp)
	
#define TBIT_Z_AND_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,1,0,0,p2,r3,pos6,0,0,p1,qp)
	
#define TBIT_NZ_AND_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,1,0,0,p2,r3,pos6,0,1,p1,qp)
	
#define TBIT_Z_OR_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,0,0,1,p2,r3,pos6,0,0,p1,qp)
	
#define TBIT_NZ_OR_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,0,0,1,p2,r3,pos6,0,1,p1,qp)
	
#define TBIT_Z_OR_ANDCM_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,1,0,1,p2,r3,pos6,0,0,p1,qp)
	
#define TBIT_NZ_OR_ANDCM_pprpos6(qp,p1,p2,r3,pos6) \
	I16(5,1,0,1,p2,r3,pos6,0,1,p1,qp)

/* I17 - TestNat */

#define TNAT_Z_ppr(qp,p1,p2,r3) \
	I17(5,0,0,0,p2,r3,0,1,0,p1,qp)	

#define TNAT_Z_UNC_ppr(qp,p1,p2,r3) \
	I17(5,0,0,0,p2,r3,0,1,1,p1,qp)	

#define TNAT_Z_AND_ppr(qp,p1,p2,r3) \
	I17(5,1,0,0,p2,r3,0,1,0,p1,qp)	

#define TNAT_NZ_AND_ppr(qp,p1,p2,r3) \
	I17(5,1,0,0,p2,r3,0,1,1,p1,qp)	

#define TNAT_Z_OR_ppr(qp,p1,p2,r3) \
	I17(5,0,0,1,p2,r3,0,1,0,p1,qp)	

#define TNAT_NZ_OR_ppr(qp,p1,p2,r3) \
	I17(5,0,0,1,p2,r3,0,1,1,p1,qp)	

#define TNAT_Z_OR_ANDCM_ppr(qp,p1,p2,r3) \
	I17(5,1,0,1,p2,r3,0,1,0,p1,qp)	

#define TNAT_NZ_OR_ANDCM_ppr(qp,p1,p2,r3) \
	I17(5,1,0,1,p2,r3,0,1,1,p1,qp)	

/* I18 - Is not present in the manual */

/* I19 - Break/Nop ( I-Unit) */

#define BREAK_I_imm21(qp,imm21) \
	I19(0,((imm21>>20)&MASK(1)),0,0,0,(imm21&MASK(20)),qp)

#define NOP_I_imm21(qp,imm21) \
	I19(0,((imm21>>20)&MASK(1)),0,1,0,(imm21&MASK(20)),qp)

/* I20 - Integer Speculation Check (I-Unit) */

/*TODO*/
#define CHK_S_I_rtarget25(qp,r2,target25) \
	fprintf(stderr,"CHK_I_S not yet implemented\n")

/* I21 - Move to BR */
/* TODO : Test the imm13 extraction field */

#define MOV_SPTK_brtag13(qp,b1,r2,tag13) \
	I21(0,0,7,(((tag13)>>4)&MASK(9)),0,0,0,r2,0,b1,qp)

#define MOV_brtag13(qp,b1,r2,tag13) \
	I21(0,0,7,((tag13>>4)&MASK(9)),0,0,1,r2,0,b1,qp)

#define MOV_DPTKbrtag13(qp,b1,r2,tag13) \
	I21(0,0,7,((tag13>>4)&MASK(9)),0,0,2,r2,0,b1,qp)

#define MOV_SPTK_IMP_brtag13(qp,b1,r2,tag13) \
	I21(0,0,7,((tag13>>4)&MASK(9)),1,0,0,r2,0,b1,qp)

#define MOV_IMP_brtag13(qp,b1,r2,tag13) \
	I21(0,0,7,((tag13>>4)&MASK(9)),1,0,1,r2,0,b1,qp)

#define MOV_DPTK_IMP_brtag13(qp,b1,r2,tag13) \
	I21(0,0,7,((tag13>>4)&MASK(9)),1,0,2,r2,0,b1,qp)

/* I22 - Move From BR */

#define MOV_rb(qp,r1,b2) \
	I22(0,0,0,0x31,0,b2,r1,qp)

/* I23 - Move tO Predicates - Register */

#define MOV_pr(qp,pr,r2,mask17) \
	I23(0,((mask17>>16)&MASK(1)),3,0,((mask17>>8)&MASK(8)),0,r2,((mask17>>1)&MASK(7)),qp)

/* I24 - Move to Predicates - Immediate44 */

#define MOV_pimm14(qp,pr_rot,imm44) \
	I24(0,((imm44>>43)&MASK(1)),2,((imm44>>16)&MASK(27)),qp)

/* I25 - Move From Predicates/IP */

#define MOV_rip(qp,r1) \
	I25(0,0,0,0x30,0,r1,qp)

#define MOV_rp(qp) \
	I25(0,0,0,0x33,0,r1,qp)

/* I26 - Move to AR - Register (I-Unit)	*/
	
#define MOV_I_ar(qp,ar3,r2) \
	I26(0,1,0,0x2A,ar3,r2,0,qp)

/* I27 - Move to Ar - Immediate8 (I-Unit) */

#define MOV_I_aimm8(qp,ar3,imm8) \
	I27(0,((imm8>>7)&MASK(1)),0,0X0A,ar3,((imm8)&MASK(7)),0,qp)

/* I28 - Move from AR(I-Unit) */

#define MOV_I_ra(qp,r1,ar3) \
	I28(0,0,0,0x32,ar3,0,r1,qp)

/* I29 - Sign/Zero Extend/Compute Zero Index */

#define ZXT1_rr(qp,r1,r3) \
	I29(0,0,0,0x10,r3,0,r1,qp)

#define ZXT2_rr(qp,r1,r3) \
	I29(0,0,0,0x11,r3,0,r1,qp)

#define ZXT4_rr(qp,r1,r3) \
	I29(0,0,0,0x12,r3,0,r1,qp)

#define SXT1_rr(qp,r1,r3) \
	I29(0,0,0,0x14,r3,0,r1,qp)

#define SXT2_rr(qp,r1,r3) \
	I29(0,0,0,0x15,r3,0,r1,qp)

#define SXT4_rr(qp,r1,r3) \
	I29(0,0,0,0x16,r3,0,r1,qp)

#define CZX1_L_rr(qp,r1,r3) \
	I29(0,0,0,0x18,r3,0,r1,qp)

#define CZX2_L_rr(qp,r1,r3) \
	I29(0,0,0,0x19,r3,0,r1,qp)

#define CZX1_R_rr(qp,r1,r3) \
	I29(0,0,0,0x1C,r3,0,r1,qp)

#define CZX2_R_rr(qp,r1,r3) \
	I29(0,0,0,0x1D,r3,0,r1,qp)



/* M-UNIT encoding macros */
/* M1 */
#define M1(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M2 */
#define M2(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M3 */
#define M3(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M4 */
#define M4(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M5 */
#define M5(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M6 */
#define M6(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M7 */
#define M7(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M8 */
#define M8(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M9 */
#define M9(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M10 */
#define M10(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M11 */
#define M11(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M12 */
#define M12(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M13 */
#define M13(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M14 */
#define M14(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M15 */
#define M15(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M16 */
#define M16(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M17 */
#define M17(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 16) |\
	(((unsigned long)(P08)) << 15) |\
	(((unsigned long)(P09)) << 13) |\
	(((unsigned long)(P10)) <<  6) |\
	(((unsigned long)(P11)) <<  0) \
	)

/* M18 */
#define M18(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M19 */
#define M19(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 30) |\
	(((unsigned long)(P04)) << 28) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M20 */
#define M20(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 20) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* M21 */
#define M21(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 20) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* M22 */
#define M22(P01,P02,P03,P04,P05,P06)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 13) |\
	(((unsigned long)(P05)) <<  6) |\
	(((unsigned long)(P06)) <<  0) \
	)

/* M23 */
#define M23(P01,P02,P03,P04,P05,P06)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 13) |\
	(((unsigned long)(P05)) <<  6) |\
	(((unsigned long)(P06)) <<  0) \
	)

/* M24 */
#define M24(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* M25 */
#define M25(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* M26 */
#define M26(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M27 */
#define M27(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M28 */
#define M28(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* M29 */
#define M29(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M30 */
#define M30(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M31 */
#define M31(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M32 */
#define M32(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M33 */
#define M33(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M34 */
#define M34(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M35 */
#define M35(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M36 */
#define M36(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* M37 */
#define M37(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 26) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M38 */
#define M38(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M39 */
#define M39(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 15) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M40 */
#define M40(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 15) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* M41 */
#define M41(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M42 */
#define M42(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M43 */
#define M43(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M44 */
#define M44(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 31) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* M45 */
#define M45(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* M46 */
#define M46(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)



typedef enum {
	ldhNONE=0,
	ldhNT1=1,
	ldhNTA=3
}Ldhint;

/* M1 - Integer Load */

#define LD1_rr(qp,r1,r3) \
	M1(4,0,0x00,ldhNONE,0,r3,0,r1,qp)

#define LD1_NT1_rr(qp,r1,r3) \
	M1(4,0,0x00,ldhNT1,0,r3,0,r1,qp)

#define LD1_NTA_rr(qp,r1,r3) \
	M1(4,0,0x00,ldhNTA,0,r3,0,r1,qp)

#define LD2_rr(qp,r1,r3) \
	M1(4,0,0x01,ldhNONE,0,r3,0,r1,qp)

#define LD2_NT1_rr(qp,r1,r3) \
	M1(4,0,0x01,ldhNT1,0,r3,0,r1,qp)

#define LD2_NTA_rr(qp,r1,r3) \
	M1(4,0,0x01,ldhNTA,0,r3,0,r1,qp)

#define LD4_rr(qp,r1,r3) \
	M1(4,0,0x02,ldhNONE,0,r3,0,r1,qp)

#define LD4_NT1_rr(qp,r1,r3) \
	M1(4,0,0x02,ldhNT1,0,r3,0,r1,qp)

#define LD4_NTA_rr(qp,r1,r3) \
	M1(4,0,0x02,ldhNTA,0,r3,0,r1,qp)

#define LD8_rr(qp,r1,r3) \
	M1(4,0,0x03,ldhNONE,0,r3,0,r1,qp)

#define LD8_NT1_rr(qp,r1,r3) \
	M1(4,0,0x03,ldhNT1,0,r3,0,r1,qp)

#define LD8_NTA_rr(qp,r1,r3) \
	M1(4,0,0x03,ldhNTA,0,r3,0,r1,qp)

#define LD1_S_rr(qp,r1,r3) \
	M1(4,0,0x04,ldhNONE,0,r3,0,r1,qp)

#define LD1_S_NT1_rr(qp,r1,r3) \
	M1(4,0,0x04,ldhNT1,0,r3,0,r1,qp)

#define LD1_S_NTA_rr(qp,r1,r3) \
	M1(4,0,0x04,ldhNTA,0,r3,0,r1,qp)

#define LD2_S_rr(qp,r1,r3) \
	M1(4,0,0x05,ldhNONE,0,r3,0,r1,qp)

#define LD2_S_NT1_rr(qp,r1,r3) \
	M1(4,0,0x05,ldhNT1,0,r3,0,r1,qp)

#define LD2_S_NTA_rr(qp,r1,r3) \
	M1(4,0,0x05,ldhNTA,0,r3,0,r1,qp)

#define LD4_S_rr(qp,r1,r3) \
	M1(4,0,0x06,ldhNONE,0,r3,0,r1,qp)

#define LD4_S_NT1_rr(qp,r1,r3) \
	M1(4,0,0x06,ldhNT1,0,r3,0,r1,qp)

#define LD4_S_NTA_rr(qp,r1,r3) \
	M1(4,0,0x06,ldhNTA,0,r3,0,r1,qp)

#define LD8_S_rr(qp,r1,r3) \
	M1(4,0,0x07,ldhNONE,0,r3,0,r1,qp)

#define LD8_S_NT1_rr(qp,r1,r3) \
	M1(4,0,0x07,ldhNT1,0,r3,0,r1,qp)

#define LD8_S_NTA_rr(qp,r1,r3) \
	M1(4,0,0x07,ldhNTA,0,r3,0,r1,qp)

#define LD1_A_rr(qp,r1,r3) \
	M1(4,0,0x08,ldhNONE,0,r3,0,r1,qp)

#define LD1_A_NT1_rr(qp,r1,r3) \
	M1(4,0,0x08,ldhNT1,0,r3,0,r1,qp)

#define LD1_A_NTA_rr(qp,r1,r3) \
	M1(4,0,0x08,ldhNTA,0,r3,0,r1,qp)

#define LD2_A_rr(qp,r1,r3) \
	M1(4,0,0x09,ldhNONE,0,r3,0,r1,qp)

#define LD2_A_NT1_rr(qp,r1,r3) \
	M1(4,0,0x09,ldhNT1,0,r3,0,r1,qp)

#define LD2_A_NTA_rr(qp,r1,r3) \
	M1(4,0,0x09,ldhNTA,0,r3,0,r1,qp)

#define LD4_A_rr(qp,r1,r3) \
	M1(4,0,0x0A,ldhNONE,0,r3,0,r1,qp)

#define LD4_A_NT1_rr(qp,r1,r3) \
	M1(4,0,0x0A,ldhNT1,0,r3,0,r1,qp)

#define LD4_A_NTA_rr(qp,r1,r3) \
	M1(4,0,0x0A,ldhNTA,0,r3,0,r1,qp)

#define LD8_A_rr(qp,r1,r3) \
	M1(4,0,0x0B,ldhNONE,0,r3,0,r1,qp)

#define LD8_A_NT1_rr(qp,r1,r3) \
	M1(4,0,0x0B,ldhNT1,0,r3,0,r1,qp)

#define LD8_A_NTA_rr(qp,r1,r3) \
	M1(4,0,0x0B,ldhNTA,0,r3,0,r1,qp)

#define LD1_SA_rr(qp,r1,r3) \
	M1(4,0,0x0C,ldhNONE,0,r3,0,r1,qp)

#define LD1_SA_NT1_rr(qp,r1,r3) \
	M1(4,0,0x0C,ldhNT1,0,r3,0,r1,qp)

#define LD1_SA_NTA_rr(qp,r1,r3) \
	M1(4,0,0x0C,ldhNTA,0,r3,0,r1,qp)

#define LD2_SA_rr(qp,r1,r3) \
	M1(4,0,0x0D,ldhNONE,0,r3,0,r1,qp)

#define LD2_SA_NT1_rr(qp,r1,r3) \
	M1(4,0,0x0D,ldhNT1,0,r3,0,r1,qp)

#define LD2_SA_NTA_rr(qp,r1,r3) \
	M1(4,0,0x0D,ldhNTA,0,r3,0,r1,qp)

#define LD4_SA_rr(qp,r1,r3) \
	M1(4,0,0x0E,ldhNONE,0,r3,0,r1,qp)

#define LD4_SA_NT1_rr(qp,r1,r3) \
	M1(4,0,0x0E,ldhNT1,0,r3,0,r1,qp)

#define LD4_SA_NTA_rr(qp,r1,r3) \
	M1(4,0,0x0E,ldhNTA,0,r3,0,r1,qp)

#define LD8_SA_rr(qp,r1,r3) \
	M1(4,0,0x0F,ldhNONE,0,r3,0,r1,qp)

#define LD8_SA_NT1_rr(qp,r1,r3) \
	M1(4,0,0x0F,ldhNT1,0,r3,0,r1,qp)

#define LD8_SA_NTA_rr(qp,r1,r3) \
	M1(4,0,0x0F,ldhNTA,0,r3,0,r1,qp)

#define LD1_BIAS_rr(qp,r1,r3) \
	M1(4,0,0x10,ldhNONE,0,r3,0,r1,qp)

#define LD1_BIAS_NT1_rr(qp,r1,r3) \
	M1(4,0,0x10,ldhNT1,0,r3,0,r1,qp)

#define LD1_BIAS_NTA_rr(qp,r1,r3) \
	M1(4,0,0x10,ldhNTA,0,r3,0,r1,qp)

#define LD2_BIAS_rr(qp,r1,r3) \
	M1(4,0,0x11,ldhNONE,0,r3,0,r1,qp)

#define LD2_BIAS_NT1_rr(qp,r1,r3) \
	M1(4,0,0x11,ldhNT1,0,r3,0,r1,qp)

#define LD2_BIAS_NTA_rr(qp,r1,r3) \
	M1(4,0,0x11,ldhNTA,0,r3,0,r1,qp)

#define LD4_BIAS_rr(qp,r1,r3) \
	M1(4,0,0x12,ldhNONE,0,r3,0,r1,qp)

#define LD4_BIAS_NT1_rr(qp,r1,r3) \
	M1(4,0,0x12,ldhNT1,0,r3,0,r1,qp)

#define LD4_BIAS_NTA_rr(qp,r1,r3) \
	M1(4,0,0x12,ldhNTA,0,r3,0,r1,qp)

#define LD8_BIAS_rr(qp,r1,r3) \
	M1(4,0,0x13,ldhNONE,0,r3,0,r1,qp)

#define LD8_BIAS_NT1_rr(qp,r1,r3) \
	M1(4,0,0x13,ldhNT1,0,r3,0,r1,qp)

#define LD8_BIAS_NTA_rr(qp,r1,r3) \
	M1(4,0,0x13,ldhNTA,0,r3,0,r1,qp)

#define LD1_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x14,ldhNONE,0,r3,0,r1,qp)

#define LD1_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x14,ldhNT1,0,r3,0,r1,qp)

#define LD1_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x14,ldhNTA,0,r3,0,r1,qp)

#define LD2_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x15,ldhNONE,0,r3,0,r1,qp)

#define LD2_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x15,ldhNT1,0,r3,0,r1,qp)

#define LD2_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x15,ldhNTA,0,r3,0,r1,qp)

#define LD4_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x16,ldhNONE,0,r3,0,r1,qp)

#define LD4_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x16,ldhNT1,0,r3,0,r1,qp)

#define LD4_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x16,ldhNTA,0,r3,0,r1,qp)

#define LD8_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x17,ldhNONE,0,r3,0,r1,qp)

#define LD8_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x17,ldhNT1,0,r3,0,r1,qp)

#define LD8_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x17,ldhNTA,0,r3,0,r1,qp)

#define LD8_FILL_rr(qp,r1,r3) \
	M1(4,0,0x1B,ldhNONE,0,r3,0,r1,qp)

#define LD8_FILL_NT1_rr(qp,r1,r3) \
	M1(4,0,0x1B,ldhNT1,0,r3,0,r1,qp)

#define LD8_FILL_NTA_rr(qp,r1,r3) \
	M1(4,0,0x1B,ldhNTA,0,r3,0,r1,qp)

#define LD1_C_CLR_rr(qp,r1,r3) \
	M1(4,0,0x20,ldhNONE,0,r3,0,r1,qp)

#define LD1_C_CLR_NT1_rr(qp,r1,r3) \
	M1(4,0,0x20,ldhNT1,0,r3,0,r1,qp)

#define LD1_C_CLR_NTA_rr(qp,r1,r3) \
	M1(4,0,0x20,ldhNTA,0,r3,0,r1,qp)

#define LD2_C_CLR_rr(qp,r1,r3) \
	M1(4,0,0x21,ldhNONE,0,r3,0,r1,qp)

#define LD2_C_CLR_NT1_rr(qp,r1,r3) \
	M1(4,0,0x21,ldhNT1,0,r3,0,r1,qp)

#define LD2_C_CLR_NTA_rr(qp,r1,r3) \
	M1(4,0,0x21,ldhNTA,0,r3,0,r1,qp)

#define LD4_C_CLR_rr(qp,r1,r3) \
	M1(4,0,0x22,ldhNONE,0,r3,0,r1,qp)

#define LD4_C_CLR_NT1_rr(qp,r1,r3) \
	M1(4,0,0x22,ldhNT1,0,r3,0,r1,qp)

#define LD4_C_CLR_NTA_rr(qp,r1,r3) \
	M1(4,0,0x22,ldhNTA,0,r3,0,r1,qp)

#define LD8_C_CLR_rr(qp,r1,r3) \
	M1(4,0,0x23,ldhNONE,0,r3,0,r1,qp)

#define LD8_C_CLR_NT1_rr(qp,r1,r3) \
	M1(4,0,0x23,ldhNT1,0,r3,0,r1,qp)

#define LD8_C_CLR_NTA_rr(qp,r1,r3) \
	M1(4,0,0x23,ldhNTA,0,r3,0,r1,qp)

#define LD1_C_NC_rr(qp,r1,r3) \
	M1(4,0,0x24,ldhNONE,0,r3,0,r1,qp)

#define LD1_C_NC_NT1_rr(qp,r1,r3) \
	M1(4,0,0x24,ldhNT1,0,r3,0,r1,qp)

#define LD1_C_NC_NTA_rr(qp,r1,r3) \
	M1(4,0,0x24,ldhNTA,0,r3,0,r1,qp)

#define LD2_C_NC_rr(qp,r1,r3) \
	M1(4,0,0x25,ldhNONE,0,r3,0,r1,qp)

#define LD2_C_NC_NT1_rr(qp,r1,r3) \
	M1(4,0,0x25,ldhNT1,0,r3,0,r1,qp)

#define LD2_C_NC_NTA_rr(qp,r1,r3) \
	M1(4,0,0x25,ldhNTA,0,r3,0,r1,qp)

#define LD4_C_NC_rr(qp,r1,r3) \
	M1(4,0,0x26,ldhNONE,0,r3,0,r1,qp)

#define LD4_C_NC_NT1_rr(qp,r1,r3) \
	M1(4,0,0x26,ldhNT1,0,r3,0,r1,qp)

#define LD4_C_NC_NTA_rr(qp,r1,r3) \
	M1(4,0,0x26,ldhNTA,0,r3,0,r1,qp)

#define LD8_C_NC_rr(qp,r1,r3) \
	M1(4,0,0x27,ldhNONE,0,r3,0,r1,qp)

#define LD8_C_NC_NT1_rr(qp,r1,r3) \
	M1(4,0,0x27,ldhNT1,0,r3,0,r1,qp)

#define LD8_C_NC_NTA_rr(qp,r1,r3) \
	M1(4,0,0x27,ldhNTA,0,r3,0,r1,qp)

#define LD1_C_CLR_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x28,ldhNONE,0,r3,0,r1,qp)

#define LD1_C_CLR_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x28,ldhNT1,0,r3,0,r1,qp)

#define LD1_C_CLR_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x28,ldhNTA,0,r3,0,r1,qp)

#define LD2_C_CLR_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x29,ldhNONE,0,r3,0,r1,qp)

#define LD2_C_CLR_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x29,ldhNT1,0,r3,0,r1,qp)

#define LD2_C_CLR_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x29,ldhNTA,0,r3,0,r1,qp)

#define LD4_C_CLR_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x2A,ldhNONE,0,r3,0,r1,qp)

#define LD4_C_CLR_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x2A,ldhNT1,0,r3,0,r1,qp)

#define LD4_C_CLR_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x2A,ldhNTA,0,r3,0,r1,qp)

#define LD8_C_CLR_ACQ_rr(qp,r1,r3) \
	M1(4,0,0x2B,ldhNONE,0,r3,0,r1,qp)

#define LD8_C_CLR_ACQ_NT1_rr(qp,r1,r3) \
	M1(4,0,0x2B,ldhNT1,0,r3,0,r1,qp)

#define LD8_C_CLR_ACQ_NTA_rr(qp,r1,r3) \
	M1(4,0,0x2B,ldhNTA,0,r3,0,r1,qp)

/* M2 - Integer Load - Increment by Register */

#define LD1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x00,ldhNONE,0,r3,r2,r1,qp)

#define LD1_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x00,ldhNT1,0,r3,r2,r1,qp)

#define LD1_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x00,ldhNTA,0,r3,r2,r1,qp)

#define LD2_rrr(qp,r1,r3,r2) \
	M2(4,1,0x01,ldhNONE,0,r3,r2,r1,qp)

#define LD2_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x01,ldhNT1,0,r3,r2,r1,qp)

#define LD2_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x01,ldhNTA,0,r3,r2,r1,qp)

#define LD4_rrr(qp,r1,r3,r2) \
	M2(4,1,0x02,ldhNONE,0,r3,r2,r1,qp)

#define LD4_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x02,ldhNT1,0,r3,r2,r1,qp)

#define LD4_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x02,ldhNTA,0,r3,r2,r1,qp)

#define LD8_rrr(qp,r1,r3,r2) \
	M2(4,1,0x03,ldhNONE,0,r3,r2,r1,qp)

#define LD8_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x03,ldhNT1,0,r3,r2,r1,qp)

#define LD8_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x03,ldhNTA,0,r3,r2,r1,qp)

#define LD1_S_rrr(qp,r1,r3,r2) \
	M2(4,1,0x04,ldhNONE,0,r3,r2,r1,qp)

#define LD1_S_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x04,ldhNT1,0,r3,r2,r1,qp)

#define LD1_S_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x04,ldhNTA,0,r3,r2,r1,qp)

#define LD2_S_rrr(qp,r1,r3,r2) \
	M2(4,1,0x05,ldhNONE,0,r3,r2,r1,qp)

#define LD2_S_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x05,ldhNT1,0,r3,r2,r1,qp)

#define LD2_S_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x05,ldhNTA,0,r3,r2,r1,qp)

#define LD4_S_rrr(qp,r1,r3,r2) \
	M2(4,1,0x06,ldhNONE,0,r3,r2,r1,qp)

#define LD4_S_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x06,ldhNT1,0,r3,r2,r1,qp)

#define LD4_S_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x06,ldhNTA,0,r3,r2,r1,qp)

#define LD8_S_rrr(qp,r1,r3,r2) \
	M2(4,1,0x07,ldhNONE,0,r3,r2,r1,qp)

#define LD8_S_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x07,ldhNT1,0,r3,r2,r1,qp)

#define LD8_S_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x07,ldhNTA,0,r3,r2,r1,qp)

#define LD1_A_rrr(qp,r1,r3,r2) \
	M2(4,1,0x08,ldhNONE,0,r3,r2,r1,qp)

#define LD1_A_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x08,ldhNT1,0,r3,r2,r1,qp)

#define LD1_A_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x08,ldhNTA,0,r3,r2,r1,qp)

#define LD2_A_rrr(qp,r1,r3,r2) \
	M2(4,1,0x09,ldhNONE,0,r3,r2,r1,qp)

#define LD2_A_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x09,ldhNT1,0,r3,r2,r1,qp)

#define LD2_A_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x09,ldhNTA,0,r3,r2,r1,qp)

#define LD4_A_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0A,ldhNONE,0,r3,r2,r1,qp)

#define LD4_A_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0A,ldhNT1,0,r3,r2,r1,qp)

#define LD4_A_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0A,ldhNTA,0,r3,r2,r1,qp)

#define LD8_A_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0B,ldhNONE,0,r3,r2,r1,qp)

#define LD8_A_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0B,ldhNT1,0,r3,r2,r1,qp)

#define LD8_A_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0B,ldhNTA,0,r3,r2,r1,qp)

#define LD1_SA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0C,ldhNONE,0,r3,r2,r1,qp)

#define LD1_SA_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0C,ldhNT1,0,r3,r2,r1,qp)

#define LD1_SA_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0C,ldhNTA,0,r3,r2,r1,qp)

#define LD2_SA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0D,ldhNONE,0,r3,r2,r1,qp)

#define LD2_SA_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0D,ldhNT1,0,r3,r2,r1,qp)

#define LD2_SA_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0D,ldhNTA,0,r3,r2,r1,qp)

#define LD4_SA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0E,ldhNONE,0,r3,r2,r1,qp)

#define LD4_SA_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0E,ldhNT1,0,r3,r2,r1,qp)

#define LD4_SA_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0E,ldhNTA,0,r3,r2,r1,qp)

#define LD8_SA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0F,ldhNONE,0,r3,r2,r1,qp)

#define LD8_SA_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0F,ldhNT1,0,r3,r2,r1,qp)

#define LD8_SA_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x0F,ldhNTA,0,r3,r2,r1,qp)

#define LD1_BIAS_rrr(qp,r1,r3,r2) \
	M2(4,1,0x10,ldhNONE,0,r3,r2,r1,qp)

#define LD1_BIAS_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x10,ldhNT1,0,r3,r2,r1,qp)

#define LD1_BIAS_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x10,ldhNTA,0,r3,r2,r1,qp)

#define LD2_BIAS_rrr(qp,r1,r3,r2) \
	M2(4,1,0x11,ldhNONE,0,r3,r2,r1,qp)

#define LD2_BIAS_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x11,ldhNT1,0,r3,r2,r1,qp)

#define LD2_BIAS_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x11,ldhNTA,0,r3,r2,r1,qp)

#define LD4_BIAS_rrr(qp,r1,r3,r2) \
	M2(4,1,0x12,ldhNONE,0,r3,r2,r1,qp)

#define LD4_BIAS_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x12,ldhNT1,0,r3,r2,r1,qp)

#define LD4_BIAS_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x12,ldhNTA,0,r3,r2,r1,qp)

#define LD8_BIAS_rrr(qp,r1,r3,r2) \
	M2(4,1,0x13,ldhNONE,0,r3,r2,r1,qp)

#define LD8_BIAS_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x13,ldhNT1,0,r3,r2,r1,qp)

#define LD8_BIAS_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x13,ldhNTA,0,r3,r2,r1,qp)

#define LD1_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x14,ldhNONE,0,r3,r2,r1,qp)

#define LD1_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x14,ldhNT1,0,r3,r2,r1,qp)

#define LD1_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x14,ldhNTA,0,r3,r2,r1,qp)

#define LD2_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x15,ldhNONE,0,r3,r2,r1,qp)

#define LD2_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x15,ldhNT1,0,r3,r2,r1,qp)

#define LD2_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x15,ldhNTA,0,r3,r2,r1,qp)

#define LD4_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x16,ldhNONE,0,r3,r2,r1,qp)

#define LD4_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x16,ldhNT1,0,r3,r2,r1,qp)

#define LD4_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x16,ldhNTA,0,r3,r2,r1,qp)

#define LD8_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x17,ldhNONE,0,r3,r2,r1,qp)

#define LD8_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x17,ldhNT1,0,r3,r2,r1,qp)

#define LD8_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x17,ldhNTA,0,r3,r2,r1,qp)

#define LD8_FILL_rrr(qp,r1,r3,r2) \
	M2(4,1,0x1B,ldhNONE,0,r3,r2,r1,qp)

#define LD8_FILL_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x1B,ldhNT1,0,r3,r2,r1,qp)

#define LD8_FILL_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x1B,ldhNTA,0,r3,r2,r1,qp)

#define LD1_C_CLR_rrr(qp,r1,r3,r2) \
	M2(4,1,0x20,ldhNONE,0,r3,r2,r1,qp)

#define LD1_C_CLR_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x20,ldhNT1,0,r3,r2,r1,qp)

#define LD1_C_CLR_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x20,ldhNTA,0,r3,r2,r1,qp)

#define LD2_C_CLR_rrr(qp,r1,r3,r2) \
	M2(4,1,0x21,ldhNONE,0,r3,r2,r1,qp)

#define LD2_C_CLR_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x21,ldhNT1,0,r3,r2,r1,qp)

#define LD2_C_CLR_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x21,ldhNTA,0,r3,r2,r1,qp)

#define LD4_C_CLR_rrr(qp,r1,r3,r2) \
	M2(4,1,0x22,ldhNONE,0,r3,r2,r1,qp)

#define LD4_C_CLR_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x22,ldhNT1,0,r3,r2,r1,qp)

#define LD4_C_CLR_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x22,ldhNTA,0,r3,r2,r1,qp)

#define LD8_C_CLR_rrr(qp,r1,r3,r2) \
	M2(4,1,0x23,ldhNONE,0,r3,r2,r1,qp)

#define LD8_C_CLR_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x23,ldhNT1,0,r3,r2,r1,qp)

#define LD8_C_CLR_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x23,ldhNTA,0,r3,r2,r1,qp)

#define LD1_C_NC_rrr(qp,r1,r3,r2) \
	M2(4,1,0x24,ldhNONE,0,r3,r2,r1,qp)

#define LD1_C_NC_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x24,ldhNT1,0,r3,r2,r1,qp)

#define LD1_C_NC_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x24,ldhNTA,0,r3,r2,r1,qp)

#define LD2_C_NC_rrr(qp,r1,r3,r2) \
	M2(4,1,0x25,ldhNONE,0,r3,r2,r1,qp)

#define LD2_C_NC_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x25,ldhNT1,0,r3,r2,r1,qp)

#define LD2_C_NC_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x25,ldhNTA,0,r3,r2,r1,qp)

#define LD4_C_NC_rrr(qp,r1,r3,r2) \
	M2(4,1,0x26,ldhNONE,0,r3,r2,r1,qp)

#define LD4_C_NC_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x26,ldhNT1,0,r3,r2,r1,qp)

#define LD4_C_NC_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x26,ldhNTA,0,r3,r2,r1,qp)

#define LD8_C_NC_rrr(qp,r1,r3,r2) \
	M2(4,1,0x27,ldhNONE,0,r3,r2,r1,qp)

#define LD8_C_NC_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x27,ldhNT1,0,r3,r2,r1,qp)

#define LD8_C_NC_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x27,ldhNTA,0,r3,r2,r1,qp)

#define LD1_C_CLR_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x28,ldhNONE,0,r3,r2,r1,qp)

#define LD1_C_CLR_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x28,ldhNT1,0,r3,r2,r1,qp)

#define LD1_C_CLR_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x28,ldhNTA,0,r3,r2,r1,qp)

#define LD2_C_CLR_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x29,ldhNONE,0,r3,r2,r1,qp)

#define LD2_C_CLR_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x29,ldhNT1,0,r3,r2,r1,qp)

#define LD2_C_CLR_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x29,ldhNTA,0,r3,r2,r1,qp)

#define LD4_C_CLR_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x2A,ldhNONE,0,r3,r2,r1,qp)

#define LD4_C_CLR_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x2A,ldhNT1,0,r3,r2,r1,qp)

#define LD4_C_CLR_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x2A,ldhNTA,0,r3,r2,r1,qp)

#define LD8_C_CLR_ACQ_rrr(qp,r1,r3,r2) \
	M2(4,1,0x2B,ldhNONE,0,r3,r2,r1,qp)

#define LD8_C_CLR_ACQ_NT1_rrr(qp,r1,r3,r2) \
	M2(4,1,0x2B,ldhNT1,0,r3,r2,r1,qp)

#define LD8_C_CLR_ACQ_NTA_rrr(qp,r1,r3,r2) \
	M2(4,1,0x2B,ldhNTA,0,r3,r2,r1,qp)

/* M3 - Integer Load - Increment by immediate */


#define LD1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x00,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x00,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x00,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x01,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x01,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x01,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x02,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x02,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x02,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x03,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x03,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x03,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_S_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x04,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_S_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x04,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_S_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x04,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_S_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x05,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_S_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x05,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_S_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x05,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_S_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x06,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_S_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x06,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_S_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x06,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_S_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x07,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_S_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x07,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_S_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x07,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_A_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x08,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_A_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x08,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_A_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x08,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_A_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x09,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_A_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x09,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_A_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x09,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_A_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0a,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_A_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0a,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_A_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0a,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_A_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0b,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_A_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0b,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_A_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0b,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_SA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0c,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_SA_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0c,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_SA_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0c,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_SA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0d,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_SA_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0d,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_SA_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0d,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_SA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0e,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_SA_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0e,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_SA_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0e,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_SA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0f,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_SA_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0f,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_SA_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x0f,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_BIAS_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x10,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_BIAS_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x10,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_BIAS_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x10,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_BIAS_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x11,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_BIAS_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x11,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_BIAS_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x11,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_BIAS_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x12,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_BIAS_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x12,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_BIAS_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x12,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_BIAS_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x13,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_BIAS_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x13,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_BIAS_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x13,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x14,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x14,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x14,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x15,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x15,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x15,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x16,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x16,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x16,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x17,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x17,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x17,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_FILL_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x1B,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_FILL_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x1B,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_FILL_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x1B,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_CLR_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x20,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_CLR_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x20,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_CLR_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x20,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_CLR_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x21,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_CLR_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x21,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_CLR_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x21,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_CLR_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x22,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_CLR_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x22,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_CLR_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x22,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_CLR_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x23,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_CLR_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x23,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_CLR_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x23,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_NC_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x24,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_NC_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x24,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_NC_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x24,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_NC_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x25,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_NC_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x25,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_NC_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x25,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_NC_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x26,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_NC_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x26,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_NC_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x26,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_NC_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x27,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_NC_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x27,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_NC_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x27,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_CLR_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x28,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_CLR_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x28,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD1_C_CLR_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x28,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_CLR_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x29,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_CLR_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x29,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD2_C_CLR_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x29,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_CLR_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x2a,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_CLR_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x2a,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD4_C_CLR_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x2a,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_CLR_ACQ_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x2b,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_CLR_ACQ_NT1_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x2b,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

#define LD8_C_CLR_ACQ_NTA_rrimm9(qp,r1,r3,imm9) \
	M3(5,((imm9>>8)&MASK(1)),0x2b,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),r1,qp)

typedef enum{
	sthNONE=0,
	sthNTA=3
}Sthint;

/* M4 - Integer Store */
#define ST1_rr(qp,r3,r2) \
	M4(4,0,0x30,sthNONE,0,r3,r2,0,qp)

#define ST1_NTA_rr(qp,r3,r2) \
	M4(4,0,0x30,sthNTA,0,r3,r2,0,qp)

#define ST2_rr(qp,r3,r2) \
	M4(4,0,0x31,sthNONE,0,r3,r2,0,qp)

#define ST2_NTA_rr(qp,r3,r2) \
	M4(4,0,0x31,sthNTA,0,r3,r2,0,qp)

#define ST4_rr(qp,r3,r2) \
	M4(4,0,0x32,sthNONE,0,r3,r2,0,qp)

#define ST4_NTA_rr(qp,r3,r2) \
	M4(4,0,0x32,sthNTA,0,r3,r2,0,qp)

#define ST8_rr(qp,r3,r2) \
	M4(4,0,0x33,sthNONE,0,r3,r2,0,qp)

#define ST8_NTA_rr(qp,r3,r2) \
	M4(4,0,0x33,sthNTA,0,r3,r2,0,qp)

#define ST1_REL_rr(qp,r3,r2) \
	M4(4,0,0x34,sthNONE,0,r3,r2,0,qp)

#define ST1_REL_NTA_rr(qp,r3,r2) \
	M4(4,0,0x34,sthNTA,0,r3,r2,0,qp)

#define ST2_REL_rr(qp,r3,r2) \
	M4(4,0,0x35,sthNONE,0,r3,r2,0,qp)

#define ST2_REL_NTA_rr(qp,r3,r2) \
	M4(4,0,0x35,sthNTA,0,r3,r2,0,qp)

#define ST4_REL_rr(qp,r3,r2) \
	M4(4,0,0x36,sthNONE,0,r3,r2,0,qp)

#define ST4_REL_NTA_rr(qp,r3,r2) \
	M4(4,0,0x36,sthNTA,0,r3,r2,0,qp)

#define ST8_REL_rr(qp,r3,r2) \
	M4(4,0,0x37,sthNONE,0,r3,r2,0,qp)

#define ST8_REL_NTA_rr(qp,r3,r2) \
	M4(4,0,0x37,sthNTA,0,r3,r2,0,qp)

#define ST8_SPILL_rr(qp,r3,r2) \
	M4(4,0,0x3B,sthNONE,0,r3,r2,0,qp)

#define ST8_SPILL_NTA_rr(qp,r3,r2) \
	M4(4,0,0x3B,sthNTA,0,r3,r2,0,qp)

/* M5 - Integer Store - Increment by Immediate */

#define ST1_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x30,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST1_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x30,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST2_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x31,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST2_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x31,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST4_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x32,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST4_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x32,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST8_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x33,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST8_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x33,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST1_REL_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x34,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST1_REL_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x34,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST2_REL_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x35,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST2_REL_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x35,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST4_REL_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x36,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST4_REL_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x36,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST8_REL_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x37,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST8_REL_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x37,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST8_SPILL_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x3B,sthNONE,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

#define ST8_SPILL_NTA_rrimm9(qp,r3,r2,imm9) \
	M5(5,((imm9>>8)&MASK(1)),0x3B,sthNTA,((imm9>>7)&MASK(1)),r3,r2,((imm9)&MASK(7)),qp)

/*  M6 - floating-point Load */

#define LDFE_fr(qp,f1,r3) \
	M6(6,0,0x00,ldhNONE,0,r3,0,f1,qp)

#define LDFE_NT1_fr(qp,f1,r3) \
	M6(6,0,0x00,ldhNT1,0,r3,0,f1,qp)

#define LDFE_NTA_fr(qp,f1,r3) \
	M6(6,0,0x00,ldhNTA,0,r3,0,f1,qp)

#define LDF8_fr(qp,f1,r3) \
	M6(6,0,0x01,ldhNONE,0,r3,0,f1,qp)

#define LDF8_NT1_fr(qp,f1,r3) \
	M6(6,0,0x01,ldhNT1,0,r3,0,f1,qp)

#define LDF8_NTA_fr(qp,f1,r3) \
	M6(6,0,0x01,ldhNTA,0,r3,0,f1,qp)

#define LDFS_fr(qp,f1,r3) \
	M6(6,0,0x02,ldhNONE,0,r3,0,f1,qp)

#define LDFS_NT1_fr(qp,f1,r3) \
	M6(6,0,0x02,ldhNT1,0,r3,0,f1,qp)

#define LDFS_NTA_fr(qp,f1,r3) \
	M6(6,0,0x02,ldhNTA,0,r3,0,f1,qp)

#define LDFD_fr(qp,f1,r3) \
	M6(6,0,0x03,ldhNONE,0,r3,0,f1,qp)

#define LDFD_NT1_fr(qp,f1,r3) \
	M6(6,0,0x03,ldhNT1,0,r3,0,f1,qp)

#define LDFD_NTA_fr(qp,f1,r3) \
	M6(6,0,0x03,ldhNTA,0,r3,0,f1,qp)

#define LDFE_S_fr(qp,f1,r3) \
	M6(6,0,0x04,ldhNONE,0,r3,0,f1,qp)

#define LDFE_S_NT1_fr(qp,f1,r3) \
	M6(6,0,0x04,ldhNT1,0,r3,0,f1,qp)

#define LDFE_S_NTA_fr(qp,f1,r3) \
	M6(6,0,0x04,ldhNTA,0,r3,0,f1,qp)

#define LDF8_S_fr(qp,f1,r3) \
	M6(6,0,0x05,ldhNONE,0,r3,0,f1,qp)

#define LDF8_S_NT1_fr(qp,f1,r3) \
	M6(6,0,0x05,ldhNT1,0,r3,0,f1,qp)

#define LDF8_S_NTA_fr(qp,f1,r3) \
	M6(6,0,0x05,ldhNTA,0,r3,0,f1,qp)

#define LDFS_S_fr(qp,f1,r3) \
	M6(6,0,0x06,ldhNONE,0,r3,0,f1,qp)

#define LDFS_S_NT1_fr(qp,f1,r3) \
	M6(6,0,0x06,ldhNT1,0,r3,0,f1,qp)

#define LDFS_S_NTA_fr(qp,f1,r3) \
	M6(6,0,0x06,ldhNTA,0,r3,0,f1,qp)

#define LDFD_S_fr(qp,f1,r3) \
	M6(6,0,0x07,ldhNONE,0,r3,0,f1,qp)

#define LDFD_S_NT1_fr(qp,f1,r3) \
	M6(6,0,0x07,ldhNT1,0,r3,0,f1,qp)

#define LDFD_S_NTA_fr(qp,f1,r3) \
	M6(6,0,0x07,ldhNTA,0,r3,0,f1,qp)

#define LDFE_A_fr(qp,f1,r3) \
	M6(6,0,0x08,ldhNONE,0,r3,0,f1,qp)

#define LDFE_A_NT1_fr(qp,f1,r3) \
	M6(6,0,0x08,ldhNT1,0,r3,0,f1,qp)

#define LDFE_A_NTA_fr(qp,f1,r3) \
	M6(6,0,0x08,ldhNTA,0,r3,0,f1,qp)

#define LDF8_A_fr(qp,f1,r3) \
	M6(6,0,0x09,ldhNONE,0,r3,0,f1,qp)

#define LDF8_A_NT1_fr(qp,f1,r3) \
	M6(6,0,0x09,ldhNT1,0,r3,0,f1,qp)

#define LDF8_A_NTA_fr(qp,f1,r3) \
	M6(6,0,0x09,ldhNTA,0,r3,0,f1,qp)

#define LDFS_A_fr(qp,f1,r3) \
	M6(6,0,0x0a,ldhNONE,0,r3,0,f1,qp)

#define LDFS_A_NT1_fr(qp,f1,r3) \
	M6(6,0,0x0a,ldhNT1,0,r3,0,f1,qp)

#define LDFS_A_NTA_fr(qp,f1,r3) \
	M6(6,0,0x0a,ldhNTA,0,r3,0,f1,qp)

#define LDFD_A_fr(qp,f1,r3) \
	M6(6,0,0x0b,ldhNONE,0,r3,0,f1,qp)

#define LDFD_A_NT1_fr(qp,f1,r3) \
	M6(6,0,0x0b,ldhNT1,0,r3,0,f1,qp)

#define LDFD_A_NTA_fr(qp,f1,r3) \
	M6(6,0,0x0b,ldhNTA,0,r3,0,f1,qp)

#define LDFE_SA_fr(qp,f1,r3) \
	M6(6,0,0x0c,ldhNONE,0,r3,0,f1,qp)

#define LDFE_SA_NT1_fr(qp,f1,r3) \
	M6(6,0,0x0c,ldhNT1,0,r3,0,f1,qp)

#define LDFE_SA_NTA_fr(qp,f1,r3) \
	M6(6,0,0x0c,ldhNTA,0,r3,0,f1,qp)

#define LDF8_SA_fr(qp,f1,r3) \
	M6(6,0,0x0d,ldhNONE,0,r3,0,f1,qp)

#define LDF8_SA_NT1_fr(qp,f1,r3) \
	M6(6,0,0x0d,ldhNT1,0,r3,0,f1,qp)

#define LDF8_SA_NTA_fr(qp,f1,r3) \
	M6(6,0,0x0d,ldhNTA,0,r3,0,f1,qp)

#define LDFS_SA_fr(qp,f1,r3) \
	M6(6,0,0x0e,ldhNONE,0,r3,0,f1,qp)

#define LDFS_SA_NT1_fr(qp,f1,r3) \
	M6(6,0,0x0e,ldhNT1,0,r3,0,f1,qp)

#define LDFS_SA_NTA_fr(qp,f1,r3) \
	M6(6,0,0x0e,ldhNTA,0,r3,0,f1,qp)

#define LDFD_SA_fr(qp,f1,r3) \
	M6(6,0,0x0f,ldhNONE,0,r3,0,f1,qp)

#define LDFD_SA_NT1_fr(qp,f1,r3) \
	M6(6,0,0x0f,ldhNT1,0,r3,0,f1,qp)

#define LDFD_SA_NTA_fr(qp,f1,r3) \
	M6(6,0,0x0f,ldhNTA,0,r3,0,f1,qp)

#define LDF_FILL_fr(qp,f1,r3) \
	M6(6,0,0x1B,ldhNONE,0,r3,0,f1,qp)

#define LDF_FILL_NT1_fr(qp,f1,r3) \
	M6(6,0,0x1B,ldhNT1,0,r3,0,f1,qp)

#define LDF_FILL_NTA_fr(qp,f1,r3) \
	M6(6,0,0x1B,ldhNTA,0,r3,0,f1,qp)

#define LDFE_C_CLR_fr(qp,f1,r3) \
	M6(6,0,0x20,ldhNONE,0,r3,0,f1,qp)

#define LDFE_C_CLR_NT1_fr(qp,f1,r3) \
	M6(6,0,0x20,ldhNT1,0,r3,0,f1,qp)

#define LDFE_C_CLR_NTA_fr(qp,f1,r3) \
	M6(6,0,0x20,ldhNTA,0,r3,0,f1,qp)

#define LDF8_C_CLR_fr(qp,f1,r3) \
	M6(6,0,0x21,ldhNONE,0,r3,0,f1,qp)

#define LDF8_C_CLR_NT1_fr(qp,f1,r3) \
	M6(6,0,0x21,ldhNT1,0,r3,0,f1,qp)

#define LDF8_C_CLR_NTA_fr(qp,f1,r3) \
	M6(6,0,0x21,ldhNTA,0,r3,0,f1,qp)

#define LDFS_C_CLR_fr(qp,f1,r3) \
	M6(6,0,0x22,ldhNONE,0,r3,0,f1,qp)

#define LDFS_C_CLR_NT1_fr(qp,f1,r3) \
	M6(6,0,0x22,ldhNT1,0,r3,0,f1,qp)

#define LDFS_C_CLR_NTA_fr(qp,f1,r3) \
	M6(6,0,0x22,ldhNTA,0,r3,0,f1,qp)

#define LDFD_C_CLR_fr(qp,f1,r3) \
	M6(6,0,0x23,ldhNONE,0,r3,0,f1,qp)

#define LDFD_C_CLR_NT1_fr(qp,f1,r3) \
	M6(6,0,0x23,ldhNT1,0,r3,0,f1,qp)

#define LDFD_C_CLR_NTA_fr(qp,f1,r3) \
	M6(6,0,0x23,ldhNTA,0,r3,0,f1,qp)

#define LDFE_C_NC_fr(qp,f1,r3) \
	M6(6,0,0x24,ldhNONE,0,r3,0,f1,qp)

#define LDFE_C_NC_NT1_fr(qp,f1,r3) \
	M6(6,0,0x24,ldhNT1,0,r3,0,f1,qp)

#define LDFE_C_NC_NTA_fr(qp,f1,r3) \
	M6(6,0,0x24,ldhNTA,0,r3,0,f1,qp)

#define LDF8_C_NC_fr(qp,f1,r3) \
	M6(6,0,0x25,ldhNONE,0,r3,0,f1,qp)

#define LDF8_C_NC_NT1_fr(qp,f1,r3) \
	M6(6,0,0x25,ldhNT1,0,r3,0,f1,qp)

#define LDF8_C_NC_NTA_fr(qp,f1,r3) \
	M6(6,0,0x25,ldhNTA,0,r3,0,f1,qp)

#define LDFS_C_NC_fr(qp,f1,r3) \
	M6(6,0,0x26,ldhNONE,0,r3,0,f1,qp)

#define LDFS_C_NC_NT1_fr(qp,f1,r3) \
	M6(6,0,0x26,ldhNT1,0,r3,0,f1,qp)

#define LDFS_C_NC_NTA_fr(qp,f1,r3) \
	M6(6,0,0x26,ldhNTA,0,r3,0,f1,qp)

#define LDFD_C_NC_fr(qp,f1,r3) \
	M6(6,0,0x27,ldhNONE,0,r3,0,f1,qp)

#define LDFD_C_NC_NT1_fr(qp,f1,r3) \
	M6(6,0,0x27,ldhNT1,0,r3,0,f1,qp)

#define LDFD_C_NC_NTA_fr(qp,f1,r3) \
	M6(6,0,0x27,ldhNTA,0,r3,0,f1,qp)

/* M7 - Floating-point Load - Increment by register */

#define LDFE_frr(qp,f1,r3,r2) \
	M7(6,0,0x00,ldhNONE,0,r3,r2,f1,qp)

#define LDFE_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x00,ldhNT1,0,r3,r2,f1,qp)

#define LDFE_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x00,ldhNTA,0,r3,r2,f1,qp)

#define LDF8_frr(qp,f1,r3,r2) \
	M7(6,0,0x01,ldhNONE,0,r3,r2,f1,qp)

#define LDF8_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x01,ldhNT1,0,r3,r2,f1,qp)

#define LDF8_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x01,ldhNTA,0,r3,r2,f1,qp)

#define LDFS_frr(qp,f1,r3,r2) \
	M7(6,0,0x02,ldhNONE,0,r3,r2,f1,qp)

#define LDFS_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x02,ldhNT1,0,r3,r2,f1,qp)

#define LDFS_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x02,ldhNTA,0,r3,r2,f1,qp)

#define LDFD_frr(qp,f1,r3,r2) \
	M7(6,0,0x03,ldhNONE,0,r3,r2,f1,qp)

#define LDFD_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x03,ldhNT1,0,r3,r2,f1,qp)

#define LDFD_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x03,ldhNTA,0,r3,r2,f1,qp)

#define LDFE_S_frr(qp,f1,r3,r2) \
	M7(6,0,0x04,ldhNONE,0,r3,r2,f1,qp)

#define LDFE_S_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x04,ldhNT1,0,r3,r2,f1,qp)

#define LDFE_S_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x04,ldhNTA,0,r3,r2,f1,qp)

#define LDF8_S_frr(qp,f1,r3,r2) \
	M7(6,0,0x05,ldhNONE,0,r3,r2,f1,qp)

#define LDF8_S_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x05,ldhNT1,0,r3,r2,f1,qp)

#define LDF8_S_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x05,ldhNTA,0,r3,r2,f1,qp)

#define LDFS_S_frr(qp,f1,r3,r2) \
	M7(6,0,0x06,ldhNONE,0,r3,r2,f1,qp)

#define LDFS_S_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x06,ldhNT1,0,r3,r2,f1,qp)

#define LDFS_S_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x06,ldhNTA,0,r3,r2,f1,qp)

#define LDFD_S_frr(qp,f1,r3,r2) \
	M7(6,0,0x07,ldhNONE,0,r3,r2,f1,qp)

#define LDFD_S_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x07,ldhNT1,0,r3,r2,f1,qp)

#define LDFD_S_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x07,ldhNTA,0,r3,r2,f1,qp)

#define LDFE_A_frr(qp,f1,r3,r2) \
	M7(6,0,0x08,ldhNONE,0,r3,r2,f1,qp)

#define LDFE_A_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x08,ldhNT1,0,r3,r2,f1,qp)

#define LDFE_A_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x08,ldhNTA,0,r3,r2,f1,qp)

#define LDF8_A_frr(qp,f1,r3,r2) \
	M7(6,0,0x09,ldhNONE,0,r3,r2,f1,qp)

#define LDF8_A_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x09,ldhNT1,0,r3,r2,f1,qp)

#define LDF8_A_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x09,ldhNTA,0,r3,r2,f1,qp)

#define LDFS_A_frr(qp,f1,r3,r2) \
	M7(6,0,0x0a,ldhNONE,0,r3,r2,f1,qp)

#define LDFS_A_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x0a,ldhNT1,0,r3,r2,f1,qp)

#define LDFS_A_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0a,ldhNTA,0,r3,r2,f1,qp)

#define LDFD_A_frr(qp,f1,r3,r2) \
	M7(6,0,0x0b,ldhNONE,0,r3,r2,f1,qp)

#define LDFD_A_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x0b,ldhNT1,0,r3,r2,f1,qp)

#define LDFD_A_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0b,ldhNTA,0,r3,r2,f1,qp)

#define LDFE_SA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0c,ldhNONE,0,r3,r2,f1,qp)

#define LDFE_SA_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x0c,ldhNT1,0,r3,r2,f1,qp)

#define LDFE_SA_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0c,ldhNTA,0,r3,r2,f1,qp)

#define LDF8_SA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0d,ldhNONE,0,r3,r2,f1,qp)

#define LDF8_SA_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x0d,ldhNT1,0,r3,r2,f1,qp)

#define LDF8_SA_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0d,ldhNTA,0,r3,r2,f1,qp)

#define LDFS_SA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0e,ldhNONE,0,r3,r2,f1,qp)

#define LDFS_SA_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x0e,ldhNT1,0,r3,r2,f1,qp)

#define LDFS_SA_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0e,ldhNTA,0,r3,r2,f1,qp)

#define LDFD_SA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0f,ldhNONE,0,r3,r2,f1,qp)

#define LDFD_SA_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x0f,ldhNT1,0,r3,r2,f1,qp)

#define LDFD_SA_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x0f,ldhNTA,0,r3,r2,f1,qp)

#define LDF_FILL_frr(qp,f1,r3,r2) \
	M7(6,0,0x1B,ldhNONE,0,r3,r2,f1,qp)

#define LDF_FILL_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x1B,ldhNT1,0,r3,r2,f1,qp)

#define LDF_FILL_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x1B,ldhNTA,0,r3,r2,f1,qp)

#define LDFE_C_CLR_frr(qp,f1,r3,r2) \
	M7(6,0,0x20,ldhNONE,0,r3,r2,f1,qp)

#define LDFE_C_CLR_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x20,ldhNT1,0,r3,r2,f1,qp)

#define LDFE_C_CLR_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x20,ldhNTA,0,r3,r2,f1,qp)

#define LDF8_C_CLR_frr(qp,f1,r3,r2) \
	M7(6,0,0x21,ldhNONE,0,r3,r2,f1,qp)

#define LDF8_C_CLR_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x21,ldhNT1,0,r3,r2,f1,qp)

#define LDF8_C_CLR_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x21,ldhNTA,0,r3,r2,f1,qp)

#define LDFS_C_CLR_frr(qp,f1,r3,r2) \
	M7(6,0,0x22,ldhNONE,0,r3,r2,f1,qp)

#define LDFS_C_CLR_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x22,ldhNT1,0,r3,r2,f1,qp)

#define LDFS_C_CLR_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x22,ldhNTA,0,r3,r2,f1,qp)

#define LDFD_C_CLR_frr(qp,f1,r3,r2) \
	M7(6,0,0x23,ldhNONE,0,r3,r2,f1,qp)

#define LDFD_C_CLR_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x23,ldhNT1,0,r3,r2,f1,qp)

#define LDFD_C_CLR_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x23,ldhNTA,0,r3,r2,f1,qp)

#define LDFE_C_NC_frr(qp,f1,r3,r2) \
	M7(6,0,0x24,ldhNONE,0,r3,r2,f1,qp)

#define LDFE_C_NC_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x24,ldhNT1,0,r3,r2,f1,qp)

#define LDFE_C_NC_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x24,ldhNTA,0,r3,r2,f1,qp)

#define LDF8_C_NC_frr(qp,f1,r3,r2) \
	M7(6,0,0x25,ldhNONE,0,r3,r2,f1,qp)

#define LDF8_C_NC_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x25,ldhNT1,0,r3,r2,f1,qp)

#define LDF8_C_NC_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x25,ldhNTA,0,r3,r2,f1,qp)

#define LDFS_C_NC_frr(qp,f1,r3,r2) \
	M7(6,0,0x26,ldhNONE,0,r3,r2,f1,qp)

#define LDFS_C_NC_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x26,ldhNT1,0,r3,r2,f1,qp)

#define LDFS_C_NC_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x26,ldhNTA,0,r3,r2,f1,qp)

#define LDFD_C_NC_frr(qp,f1,r3,r2) \
	M7(6,0,0x27,ldhNONE,0,r3,r2,f1,qp)

#define LDFD_C_NC_NT1_frr(qp,f1,r3,r2) \
	M7(6,0,0x27,ldhNT1,0,r3,r2,f1,qp)

#define LDFD_C_NC_NTA_frr(qp,f1,r3,r2) \
	M7(6,0,0x27,ldhNTA,0,r3,r2,f1,qp)

/* M8 - Floating-point Load - Increment by Immediate */
	
#define LDFE_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x00,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x00,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x00,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x01,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x01,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x01,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x02,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x02,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x02,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x03,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x03,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x03,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_S_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x04,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_S_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x04,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_S_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x04,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_S_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x05,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_S_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x05,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_S_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x05,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_S_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x06,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_S_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x06,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_S_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x06,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_S_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x07,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_S_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x07,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_S_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x07,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_A_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x08,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_A_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x08,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_A_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x08,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_A_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x09,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_A_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x09,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_A_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x09,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_A_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0a,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_A_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0a,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_A_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0a,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_A_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0b,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_A_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0b,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_A_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0b,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_SA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0c,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_SA_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0c,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_SA_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0c,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_SA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0d,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_SA_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0d,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_SA_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0d,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_SA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0e,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_SA_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0e,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_SA_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0e,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_SA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0f,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_SA_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0f,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_SA_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x0f,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF_FILL_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x1B,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF_FILL_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x1B,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF_FILL_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x1B,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_C_CLR_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x20,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_C_CLR_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x20,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_C_CLR_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x20,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_C_CLR_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x21,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_C_CLR_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x21,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_C_CLR_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x21,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_C_CLR_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x22,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_C_CLR_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x22,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_C_CLR_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x22,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_C_CLR_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x23,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_C_CLR_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x23,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_C_CLR_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x23,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_C_NC_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x24,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_C_NC_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x24,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFE_C_NC_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x24,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_C_NC_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x25,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_C_NC_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x25,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDF8_C_NC_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x25,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_C_NC_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x26,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_C_NC_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x26,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFS_C_NC_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x26,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_C_NC_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x27,ldhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_C_NC_NT1_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x27,ldhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

#define LDFD_C_NC_NTA_frimm9(qp,f1,r3,imm9) \
	M8(7,((imm9>>8)&MASK(1)),0x27,ldhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),f1,qp)

/* M9 - Floating-point Store */
#define STFS_rf(qp,r3,f2) \
	M9(6,0,0x32,sthNONE,0,r3,f2,0,qp)

#define STFS_NTA_rf(qp,r3,f2) \
	M9(6,0,0x32,sthNTA,0,r3,f2,0,qp)

#define STFD_rf(qp,r3,f2) \
	M9(6,0,0x33,sthNONE,0,r3,f2,0,qp)

#define STFD_NTA_rf(qp,r3,f2) \
	M9(6,0,0x33,sthNTA,0,r3,f2,0,qp)

#define STF8_rf(qp,r3,f2) \
	M9(6,0,0x31,sthNONE,0,r3,f2,0,qp)

#define STF8_NTA_rf(qp,r3,f2) \
	M9(6,0,0x31,sthNTA,0,r3,f2,0,qp)

#define STFE_rf(qp,r3,f2) \
	M9(6,0,0x30,sthNONE,0,r3,f2,0,qp)

#define STFE_NTA_rf(qp,r3,f2) \
	M9(6,0,0x30,sthNTA,0,r3,f2,0,qp)

#define STF_SPILL_rf(qp,r3,f2) \
	M9(6,0,0x3B,sthNONE,0,r3,f2,0,qp)

#define STF_SPILL_NTA_rf(qp,r3,f2) \
	M9(6,0,0x3B,sthNTA,0,r3,f2,0,qp)

/* M10 - Floating-point Store - Increment by Immediate */
#define STFS_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x32,sthNONE,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STFS_NTA_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x32,sthNTA,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STFD_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x33,sthNONE,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STFD_NTA_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x33,sthNTA,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STF8_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x31,sthNONE,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STF8_NTA_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x31,sthNTA,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STFE_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x30,sthNONE,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STFE_NTA_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x30,sthNTA,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STF_SPILL_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x3B,sthNONE,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)

#define STF_SPILL_rfimm9(qp,r3,f2,imm9) \
	M10(7,((imm9>>8)&MASK(1)),0x3B,sthNTA,((imm9>>7)&MASK(1)),r3,f2,((imm9)&MASK(7)),qp)


/* M11 - Floating-point Load Pair */
/* M12 - Floating-point Load Pair - Increment by Immediate */
typedef enum {
	lfhNONE=0,
	lfhNT1,
	lfhNT2,
	lfhNTA
}Lfhint;

/* M13 - Line Prefetch */

#define LFETCH_r(qp,r3) \
	M13(6,0,0x2c,lfhNONE,0,r3,0,qp)

#define LFETCH_NT1_r(qp,r3) \
	M13(6,0,0x2c,lfhNT1,0,r3,0,qp)

#define LFETCH_NT2_r(qp,r3) \
	M13(6,0,0x2c,lfhNT2,0,r3,0,qp)

#define LFETCH_NTA_r(qp,r3) \
	M13(6,0,0x2c,lfhNTA,0,r3,0,qp)

#define LFETCH_EXCL_r(qp,r3) \
	M13(6,0,0x2d,lfhNONE,0,r3,0,qp)

#define LFETCH_EXCL_NT1_r(qp,r3) \
	M13(6,0,0x2d,lfhNT1,0,r3,0,qp)

#define LFETCH_EXCL_NT2_r(qp,r3) \
	M13(6,0,0x2d,lfhNT2,0,r3,0,qp)

#define LFETCH_EXCL_NTA_r(qp,r3) \
	M13(6,0,0x2d,lfhNTA,0,r3,0,qp)

#define LFETCH_FAULT_r(qp,r3) \
	M13(6,0,0x2e,lfhNONE,0,r3,0,qp)

#define LFETCH_FAULT_NT1_r(qp,r3) \
	M13(6,0,0x2e,lfhNT1,0,r3,0,qp)

#define LFETCH_FAULT_NT2_r(qp,r3) \
	M13(6,0,0x2e,lfhNT2,0,r3,0,qp)

#define LFETCH_FAULT_NTA_r(qp,r3) \
	M13(6,0,0x2e,lfhNTA,0,r3,0,qp)

#define LFETCH_FAULT_EXCL_r(qp,r3) \
	M13(6,0,0x2f,lfhNONE,0,r3,0,qp)

#define LFETCH_FAULT_EXCL_NT1_r(qp,r3) \
	M13(6,0,0x2f,lfhNT1,0,r3,0,qp)

#define LFETCH_FAULT_EXCL_NT2_r(qp,r3) \
	M13(6,0,0x2f,lfhNT2,0,r3,0,qp)

#define LFETCH_FAULT_EXCL_NTA_r(qp,r3) \
	M13(6,0,0x2f,lfhNTA,0,r3,0,qp)

/* M14 - Line Prefetch - Increment by Register */

#define LFETCH_rr(qp,r3,r2) \
	M14(6,1,0x2c,lfhNONE,0,r3,r2,0,qp)

#define LFETCH_NT1_rr(qp,r3,r2) \
	M14(6,1,0x2c,lfhNT1,0,r3,r2,0,qp)

#define LFETCH_NT2_rr(qp,r3,r2) \
	M14(6,1,0x2c,lfhNT2,0,r3,r2,0,qp)

#define LFETCH_NTA_rr(qp,r3,r2) \
	M14(6,1,0x2c,lfhNTA,0,r3,r2,0,qp)

#define LFETCH_EXCL_rr(qp,r3,r2) \
	M14(6,1,0x2d,lfhNONE,0,r3,r2,0,qp)

#define LFETCH_EXCL_NT1_rr(qp,r3,r2) \
	M14(6,1,0x2d,lfhNT1,0,r3,r2,0,qp)

#define LFETCH_EXCL_NT2_rr(qp,r3,r2) \
	M14(6,1,0x2d,lfhNT2,0,r3,r2,0,qp)

#define LFETCH_EXCL_NTA_rr(qp,r3,r2) \
	M14(6,1,0x2d,lfhNTA,0,r3,r2,0,qp)

#define LFETCH_FAULT_rr(qp,r3,r2) \
	M14(6,1,0x2e,lfhNONE,0,r3,r2,0,qp)

#define LFETCH_FAULT_NT1_rr(qp,r3,r2) \
	M14(6,1,0x2e,lfhNT1,0,r3,r2,0,qp)

#define LFETCH_FAULT_NT2_rr(qp,r3,r2) \
	M14(6,1,0x2e,lfhNT2,0,r3,r2,0,qp)

#define LFETCH_FAULT_NTA_rr(qp,r3,r2) \
	M14(6,1,0x2e,lfhNTA,0,r3,r2,0,qp)

#define LFETCH_FAULT_EXCL_rr(qp,r3,r2) \
	M14(6,1,0x2f,lfhNONE,0,r3,r2,0,qp)

#define LFETCH_FAULT_EXCL_NT1_rr(qp,r3,r2) \
	M14(6,1,0x2f,lfhNT1,0,r3,r2,0,qp)

#define LFETCH_FAULT_EXCL_NT2_rr(qp,r3,r2) \
	M14(6,1,0x2f,lfhNT2,0,r3,r2,0,qp)

#define LFETCH_FAULT_EXCL_NTA_rr(qp,r3,r2) \
	M14(6,1,0x2f,lfhNTA,0,r3,r2,0,qp)

/* M15 - Line Prefetch - Increment by Immediate */

#define LFETCH_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2c,lfhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_NT1_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2c,lfhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_NT2_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2c,lfhNT2,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_NTA_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2c,lfhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_EXCL_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2d,lfhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_EXCL_NT1_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2d,lfhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_EXCL_NT2_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2d,lfhNT2,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_EXCL_NTA_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2d,lfhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2e,lfhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_NT1_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2e,lfhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_NT2_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2e,lfhNT2,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_NTA_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2e,lfhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_EXCL_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2f,lfhNONE,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_EXCL_NT1_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2f,lfhNT1,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_EXCL_NT2_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2f,lfhNT2,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

#define LFETCH_FAULT_EXCL_NTA_rimm9(qp,r3,imm9) \
	M15(7,((imm9>>8)&MASK(1)),0x2f,lfhNTA,((imm9>>7)&MASK(1)),r3,((imm9)&MASK(7)),0,qp)

/* M16 - Exchange/Compare and Exchange */
/* M17 - Fetch and Add - Immediate */

/* M18 - Set FR */

#define SETF_SIG_fr(qp,f1,r2) \
	M18(6,0,0x1C,0,1,0,r2,f1,qp)

#define SETF_EXP_fr(qp,f1,r2) \
	M18(6,0,0x1D,0,1,0,r2,f1,qp)

#define SETF_S_fr(qp,f1,r2) \
	M18(6,0,0x1E,0,1,0,r2,f1,qp)

#define SETF_D_fr(qp,f1,r2) \
	M18(6,0,0x1F,0,1,0,r2,f1,qp)

/* M19 - Get FR */

#define GETF_SIG_rf(qp,r1,f2) \
	M19(4,0,0x1C,0,1,0,f2,r1,qp)

#define GETF_EXP_rf(qp,r1,f2) \
	M19(4,0,0x1D,0,1,0,f2,r1,qp)

#define GETF_S_rf(qp,r1,f2) \
	M19(4,0,0x1E,0,1,0,f2,r1,qp)

#define GETF_D_rf(qp,r1,f2) \
	M19(4,0,0x1F,0,1,0,f2,r1,qp)

/* M20 - Integer speculation Check (M-Unit) */
/* M21 - floating-point Speculation Check */
/* M22 - Integer Advance Load Check */
/* M23 - Floating-point Advanced Load Check */
/* M24 - Sync/Fence/Serialize/ALAT Control */

#define INVALA(qp) \
	M24(0,0,0,1,0,0,qp)

#define FWB(qp) \
	M24(0,0,0,2,0,0,qp)

#define MF(qp) \
	M24(0,0,0,2,2,0,qp)

#define MF_A(qp) \
	M24(0,0,0,2,3,0,qp)

#define SRLZ_D(qp) \
	M24(0,0,0,3,0,0,qp)

#define SRLZ_I(qp) \
	M24(0,0,0,3,1,0,qp)

#define SYNC_I(qp) \
	M24(0,0,0,3,3,0,qp)

/* M25 - RSE Control */
#define FLUSHRS() \
	M25(0,0,0,0,0x0C,0,0)

#define LOADRS() \
	M25(0,0,0,0,0x0A,0,0)

/* M26 - Integer ALAT Entry Invalidate */
#define INVALA_E_r(qp,r1) \
	M26(0,0,0,1,2,0,r1,qp)

/* M27 - Floating-point ALAT Entry Invalidate */
#define INVALA_E_f(qp,f1) \
	M27(0,0,0,1,3,0,f1,qp)

/* M28 - Flush Cache/Purge Translation Cache Entry */
#define FC_r(qp,r3) \
	M28(1,0,0,0x30,r3,0,qp)

#define PTC_E_r(qp,r3) \
	M28(1,0,0,0x34,r3,0,qp)

/* M29 - Move to AR - Register(M-Unit) */
#define MOV_M_ar(qp,ar3,r2) \
	M29(1,0,0,0x2A,ar3,r2,0,qp)

/* M30 - Move to AR- Immediate8(M-Unit) */
#define MOV_M_aimm8(qp,ar3,imm8) \
	M30(0,((imm8>>7)&MASK(1)),0,2,8,ar3,((imm8)&MASK(7)),0,qp)

/* M31 - Move from AR(M-Unit) */
#define MOV_M_ra(qp,r1,ar3) \
	M31(1,0,0,0x22,ar3,0,r1,qp)

/* M32 - Move to CR */
#define MOV_cr(qp,cr3,r2) \
	M32(1,0,0,0x2C,cr3,r2,0,qp)

/* M33 - Move from CR */
#define MOV_rc(qp,r1,cr3) \
	M33(1,0,0,0x24,cr3,0,r1,qp)
	
/* M34 - Allocate Register Stack Frame */
/* TODO: May be we have to set sof,sol ..variables
 * so that OUT0,OUT1..,IN0,IN1...etc will work
 */ 
#define ALLOC_rarpfsilor(qp,r1,ar_pfs,i,l,o,r) \
	M34(1,0,0x06,0,(r>>3),(i+l),(i+l+o),r1,qp); \
	do {\
	assert((i+l+o) <= 96);\
	assert((r) <= (i+l+o));\
	assert((i+l) <= (i+l+o));\
	assert(qp == 0);\
	soi = i;\
	sol = l;\
	soo = o;\
	sor = r;\
	}while(0);
	
/* M35 - Move to PSR */
#define MOV_psrlr(qp,r2) \
	M35(1,0,0,0x2D,0,r2,0,qp)

#define MOV_psrumr(qp,r2) \
	M35(1,0,0,0x29,0,r2,0,qp)

/* M36 - Move from PSR */
#define MOV_rpsr(qp,r1) \
	M36(1,0,0,0x25,0,r1,qp)

#define MOV_rpsrum(qp,r1) \
	M36(1,0,0,0x21,0,r1,qp)

/* M37 - Break/NOP(M-Unit) */
#define BREAK_M_imm21(qp,imm21) \
	M37(0,((imm21>>20)&MASK(1)),0,0,0,0,(imm21&MASK(20)),qp)
			
#define NOP_M_imm21(qp,imm21) \
	M37(0,((imm21>>20)&MASK(1)),0,0,1,0,(imm21&MASK(20)),qp)

/* M38 - Probe - Register */
#define PROBE_R_rrr(qp,r1,r3,r2) \
	M38(1,0,0,0x38,r3,r2,r1,qp)	

#define PROBE_W_rrr(qp,r1,r3,r2) \
	M38(1,0,0,0x39,r3,r2,r1,qp)	

/* M39 - Probe - Immediate2 */
#define PROBE_R_rrimm2(qp,r1,r3,imm2) \
	M39(1,0,0,0x18,r3,0,imm2,r1,qp)

#define PROBE_W_rrimm2(qp,r1,r3,imm2) \
	M39(1,0,0,0x19,r3,0,imm2,r1,qp)

/* M40 - Probe Fault-Immediate2 */
#define PROBE_RW_FAULT(qp,r2,imm2) \
	M40(1,0,0,0x31,r3,0,imm2,0,qp)
	
#define PROBE_R_FAULT(qp,r2,imm2) \
	M40(1,0,0,0x32,r3,0,imm2,0,qp)
	
#define PROBE_W_FAULT(qp,r2,imm2) \
	M40(1,0,0,0x33,r3,0,imm2,0,qp)

/* M41 - Translation Cache Insert */
#define LTC_D_r(qp,r2) \
	M41(1,0,0,0x2E,0,r2,0,qp)

#define LTC_I_r(qp,r2) \
	M41(1,0,0,0x2F,0,r2,0,qp)

/* TODO: M42 - Move to Indirect Register/Translation Register Insert*/
/* TODO: M43 - Move from Indirect Register */

/* M44 - Set/Reset User/System Mask */
#define SUM_imm24(qp,imm24) \
	M44(0,((imm24>>23)&MASK(1)),0,((imm24>>21)&MASK(2)),0x4,((imm24)&MASK(21)),qp)

#define RUM_imm24(qp,imm24) \
	M44(0,((imm24>>23)&MASK(1)),0,((imm24>>21)&MASK(2)),0x5,((imm24)&MASK(21)),qp)

#define SSM_imm24(qp,imm24) \
	M44(0,((imm24>>23)&MASK(1)),0,((imm24>>21)&MASK(2)),0x6,((imm24)&MASK(21)),qp)

#define RSM_imm24(qp,imm24) \
	M44(0,((imm24>>23)&MASK(1)),0,((imm24>>21)&MASK(2)),0x7,((imm24)&MASK(21)),qp)

/* M45 - Translation Purge */
#define PTC_I_rr(qp,r3,r2) \
	M45(1,0,0,0x09,r3,r2,0,qp)

#define PTC_G_rr(qp,r3,r2) \
	M45(1,0,0,0x0A,r3,r2,0,qp)

#define PTC_GA_rr(qp,r3,r2) \
	M45(1,0,0,0x0B,r3,r2,0,qp)

#define PTR_D_rr(qp,r3,r2) \
	M45(1,0,0,0x0C,r3,r2,0,qp)

#define PTR_I_rr(qp,r3,r2) \
	M45(1,0,0,0x0D,r3,r2,0,qp)

/* M46 - Translation Access */
#define THASH_rr(qp,r1,r3) \
	M46(1,0,0,0x1A,r3,0,r1,qp)

#define TTAG_rr(qp,r1,r3) \
	M46(1,0,0,0x1B,r3,0,r1,qp)

#define TPA_rr(qp,r1,r3) \
	M46(1,0,0,0x1E,r3,0,r1,qp)

#define TAK_rr(qp,r1,r3) \
	M46(1,0,0,0x1F,r3,0,r1,qp)



/* B-UNIT encoding macros */
/* B1 */
#define B1(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) << 12) |\
	(((unsigned long)(P07)) <<  9) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* B2 */
#define B2(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) << 12) |\
	(((unsigned long)(P07)) <<  9) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* B3 */
#define B3(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) << 12) |\
	(((unsigned long)(P07)) <<  9) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* B4 */
#define B4(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 16) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) << 12) |\
	(((unsigned long)(P09)) <<  9) |\
	(((unsigned long)(P10)) <<  6) |\
	(((unsigned long)(P11)) <<  0) \
	)

/* B5 */
#define B5(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 32) |\
	(((unsigned long)(P05)) << 16) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) << 12) |\
	(((unsigned long)(P08)) <<  9) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* B6 */
#define B6(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  5) |\
	(((unsigned long)(P08)) <<  3) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* B7 */
#define B7(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 16) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  5) |\
	(((unsigned long)(P10)) <<  3) |\
	(((unsigned long)(P11)) <<  0) \
	)

/* B8 */
#define B8(P01,P02,P03,P04,P05)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 33) |\
	(((unsigned long)(P03)) << 27) |\
	(((unsigned long)(P04)) <<  6) |\
	(((unsigned long)(P05)) <<  0) \
	)

/* B9 */
#define B9(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 26) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)



/* B1 - IP-Relative Branch */
#define BR_COND_SPTK_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_SPTK_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_COND_SPNT_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_SPNT_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_COND_DPTK_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_DPTK_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_COND_DPNT_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_DPNT_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_COND_SPTK_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_SPTK_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_COND_SPNT_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_SPNT_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_COND_DPTK_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_DPTK_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_COND_DPNT_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),0,0,0,qp)

#define BR_COND_DPNT_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),1,0,0,qp)

#define BR_WEXIT_SPTK_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_SPTK_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WEXIT_SPNT_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_SPNT_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WEXIT_DPTK_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_DPTK_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WEXIT_DPNT_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_DPNT_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WEXIT_SPTK_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_SPTK_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WEXIT_SPNT_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_SPNT_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WEXIT_DPTK_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_DPTK_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WEXIT_DPNT_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),0,0,2,qp)

#define BR_WEXIT_DPNT_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),1,0,2,qp)

#define BR_WTOP_SPTK_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_SPTK_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),1,0,3,qp)

#define BR_WTOP_SPNT_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_SPNT_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),1,0,3,qp)

#define BR_WTOP_DPTK_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_DPTK_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),1,0,3,qp)

#define BR_WTOP_DPNT_FEW_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_DPNT_MANY_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),1,0,3,qp)

#define BR_WTOP_SPTK_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_SPTK_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),1,0,3,qp)

#define BR_WTOP_SPNT_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_SPNT_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),1,0,3,qp)

#define BR_WTOP_DPTK_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_DPTK_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),1,0,3,qp)

#define BR_WTOP_DPNT_FEW_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),0,0,3,qp)

#define BR_WTOP_DPNT_MANY_CLR_target25(qp,target25) \
	B1(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),1,0,3,qp)
	
/* B2 - IP-Relative Counted Branch */
#define BR_CLOOP_SPTK_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_SPTK_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CLOOP_SPNT_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_SPNT_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CLOOP_DPTK_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_DPTK_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CLOOP_DPNT_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_DPNT_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CLOOP_SPTK_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_SPTK_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CLOOP_SPNT_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_SPNT_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CLOOP_DPTK_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_DPTK_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CLOOP_DPNT_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),0,0,5,0)

#define BR_CLOOP_DPNT_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),1,0,5,0)

#define BR_CEXIT_SPTK_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_SPTK_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),1,0,6)

#define BR_CEXIT_SPNT_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_SPNT_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),1,0,6)

#define BR_CEXIT_DPTK_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_DPTK_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),1,0,6)

#define BR_CEXIT_DPNT_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_DPNT_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),1,0,6)

#define BR_CEXIT_SPTK_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_SPTK_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),1,0,6)

#define BR_CEXIT_SPNT_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_SPNT_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),1,0,6)

#define BR_CEXIT_DPTK_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_DPTK_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),1,0,6)

#define BR_CEXIT_DPNT_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),0,0,6)

#define BR_CEXIT_DPNT_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),1,0,6)

#define BR_CTOP_SPTK_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_SPTK_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),1,0,7)

#define BR_CTOP_SPNT_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_SPNT_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),1,0,7)

#define BR_CTOP_DPTK_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_DPTK_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),1,0,7)

#define BR_CTOP_DPNT_FEW_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_DPNT_MANY_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),1,0,7)

#define BR_CTOP_SPTK_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_SPTK_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),1,0,7)

#define BR_CTOP_SPNT_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_SPNT_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),1,0,7)

#define BR_CTOP_DPTK_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_DPTK_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),1,0,7)

#define BR_CTOP_DPNT_FEW_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),0,0,7)

#define BR_CTOP_DPNT_MANY_CLR_target25(target25) \
	B2(4,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),1,0,7)

	/* B3 - IP-Relative Call*/
#define BR_CALL_SPTK_FEW_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_SPTK_MANY_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,0,((target25>>4)&MASK(20)),1,0,r,qp)

#define BR_CALL_SPNT_FEW_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_SPNT_MANY_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,1,((target25>>4)&MASK(20)),1,0,r,qp)

#define BR_CALL_DPTK_FEW_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_DPTK_MANY_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,2,((target25>>4)&MASK(20)),1,0,r,qp)

#define BR_CALL_DPNT_FEW_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_DPNT_MANY_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),0,3,((target25>>4)&MASK(20)),1,0,r,qp)

#define BR_CALL_SPTK_FEW_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_SPTK_MANY_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,0,((target25>>4)&MASK(20)),1,0,r,qp)

#define BR_CALL_SPNT_FEW_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_SPNT_MANY_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,1,((target25>>4)&MASK(20)),1,0,r,qp)

#define BR_CALL_DPTK_FEW_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_DPTK_MANY_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,2,((target25>>4)&MASK(20)),1,0,r,qp)

#define BR_CALL_DPNT_FEW_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),0,0,r,qp)

#define BR_CALL_DPNT_MANY_CLR_btarget25(qp,r,target25) \
	B3(5,((target25 >> 24)&MASK(1)),1,3,((target25>>4)&MASK(20)),1,0,r,qp)

	/* B4 - Indirect Branch */
	
#define BR_COND_SPTK_FEW_b(qp,br) \
	B4(0,0,0,0,0x20,0,br,0,0,0,qp)

#define BR_COND_SPTK_MANY_b(qp,br) \
	B4(0,0,0,0,0x20,0,br,1,0,0,qp)

#define BR_COND_SPNT_FEW_b(qp,br) \
	B4(0,0,0,1,0x20,0,br,0,0,0,qp)

#define BR_COND_SPNT_MANY_b(qp,br) \
	B4(0,0,0,1,0x20,0,br,1,0,0,qp)

#define BR_COND_DPTK_FEW_b(qp,br) \
	B4(0,0,0,2,0x20,0,br,0,0,0,qp)

#define BR_COND_DPTK_MANY_b(qp,br) \
	B4(0,0,0,2,0x20,0,br,1,0,0,qp)

#define BR_COND_DPNT_FEW_b(qp,br) \
	B4(0,0,0,3,0x20,0,br,0,0,0,qp)

#define BR_COND_DPNT_MANY_b(qp,br) \
	B4(0,0,0,3,0x20,0,br,1,0,0,qp)

#define BR_COND_SPTK_FEW_CLR_b(qp,br) \
	B4(0,0,1,0,0x20,0,br,0,0,0,qp)

#define BR_COND_SPTK_MANY_CLR_b(qp,br) \
	B4(0,0,1,0,0x20,0,br,1,0,0,qp)

#define BR_COND_SPNT_FEW_CLR_b(qp,br) \
	B4(0,0,1,1,0x20,0,br,0,0,0,qp)

#define BR_COND_SPNT_MANY_CLR_b(qp,br) \
	B4(0,0,1,1,0x20,0,br,1,0,0,qp)

#define BR_COND_DPTK_FEW_CLR_b(qp,br) \
	B4(0,0,1,2,0x20,0,br,0,0,0,qp)

#define BR_COND_DPTK_MANY_CLR_b(qp,br) \
	B4(0,0,1,2,0x20,0,br,1,0,0,qp)

#define BR_COND_DPNT_FEW_CLR_b(qp,br) \
	B4(0,0,1,3,0x20,0,br,0,0,0,qp)

#define BR_COND_DPNT_MANY_CLR_b(qp,br) \
	B4(0,0,1,3,0x20,0,br,1,0,0,qp)

#define BR_IA_SPTK_FEW_b(qp,br) \
	B4(0,0,0,0,0x20,0,br,0,0,1,qp)

#define BR_IA_SPTK_MANY_b(qp,br) \
	B4(0,0,0,0,0x20,0,br,1,0,1,qp)

#define BR_IA_SPNT_FEW_b(qp,br) \
	B4(0,0,0,1,0x20,0,br,0,0,1,qp)

#define BR_IA_SPNT_MANY_b(qp,br) \
	B4(0,0,0,1,0x20,0,br,1,0,1,qp)

#define BR_IA_DPTK_FEW_b(qp,br) \
	B4(0,0,0,2,0x20,0,br,0,0,1,qp)

#define BR_IA_DPTK_MANY_b(qp,br) \
	B4(0,0,0,2,0x20,0,br,1,0,1,qp)

#define BR_IA_DPNT_FEW_b(qp,br) \
	B4(0,0,0,3,0x20,0,br,0,0,1,qp)

#define BR_IA_DPNT_MANY_b(qp,br) \
	B4(0,0,0,3,0x20,0,br,1,0,1,qp)

#define BR_IA_SPTK_FEW_CLR_b(qp,br) \
	B4(0,0,1,0,0x20,0,br,0,0,1,qp)

#define BR_IA_SPTK_MANY_CLR_b(qp,br) \
	B4(0,0,1,0,0x20,0,br,1,0,1,qp)

#define BR_IA_SPNT_FEW_CLR_b(qp,br) \
	B4(0,0,1,1,0x20,0,br,0,0,1,qp)

#define BR_IA_SPNT_MANY_CLR_b(qp,br) \
	B4(0,0,1,1,0x20,0,br,1,0,1,qp)

#define BR_IA_DPTK_FEW_CLR_b(qp,br) \
	B4(0,0,1,2,0x20,0,br,0,0,1,qp)

#define BR_IA_DPTK_MANY_CLR_b(qp,br) \
	B4(0,0,1,2,0x20,0,br,1,0,1,qp)

#define BR_IA_DPNT_FEW_CLR_b(qp,br) \
	B4(0,0,1,3,0x20,0,br,0,0,1,qp)

#define BR_IA_DPNT_MANY_CLR_b(qp,br) \
	B4(0,0,1,3,0x20,0,br,1,0,1,qp)

#define BR_RET_SPTK_FEW_b(qp,br) \
	B4(0,0,0,0,0x21,0,br,0,0,4,qp)

#define BR_RET_SPTK_MANY_b(qp,br) \
	B4(0,0,0,0,0x21,0,br,1,0,4,qp)

#define BR_RET_SPNT_FEW_b(qp,br) \
	B4(0,0,0,1,0x21,0,br,0,0,4,qp)

#define BR_RET_SPNT_MANY_b(qp,br) \
	B4(0,0,0,1,0x21,0,br,1,0,4,qp)

#define BR_RET_DPTK_FEW_b(qp,br) \
	B4(0,0,0,2,0x21,0,br,0,0,4,qp)

#define BR_RET_DPTK_MANY_b(qp,br) \
	B4(0,0,0,2,0x21,0,br,1,0,4,qp)

#define BR_RET_DPNT_FEW_b(qp,br) \
	B4(0,0,0,3,0x21,0,br,0,0,4,qp)

#define BR_RET_DPNT_MANY_b(qp,br) \
	B4(0,0,0,3,0x21,0,br,1,0,4,qp)

#define BR_RET_SPTK_FEW_CLR_b(qp,br) \
	B4(0,0,1,0,0x21,0,br,0,0,4,qp)

#define BR_RET_SPTK_MANY_CLR_b(qp,br) \
	B4(0,0,1,0,0x21,0,br,1,0,4,qp)

#define BR_RET_SPNT_FEW_CLR_b(qp,br) \
	B4(0,0,1,1,0x21,0,br,0,0,4,qp)

#define BR_RET_SPNT_MANY_CLR_b(qp,br) \
	B4(0,0,1,1,0x21,0,br,1,0,4,qp)

#define BR_RET_DPTK_FEW_CLR_b(qp,br) \
	B4(0,0,1,2,0x21,0,br,0,0,4,qp)

#define BR_RET_DPTK_MANY_CLR_b(qp,br) \
	B4(0,0,1,2,0x21,0,br,1,0,4,qp)

#define BR_RET_DPNT_FEW_CLR_b(qp,br) \
	B4(0,0,1,3,0x21,0,br,0,0,4,qp)

#define BR_RET_DPNT_MANY_CLR_b(qp,br) \
	B4(0,0,1,3,0x21,0,br,1,0,4,qp)

	/* B5 - Indirect Call */

#define BR_CALL_SPTK_FEW_bb(qp,b1,b2) \
	B5(1,0,0,1,0,b2,0,0,b1,qp)

#define BR_CALL_SPTK_MANY_bb(qp,b1,b2) \
	B5(1,0,0,1,0,b2,1,0,b1,qp)

#define BR_CALL_SPNT_FEW_bb(qp,b1,b2) \
	B5(1,0,0,3,0,b2,0,0,b1,qp)

#define BR_CALL_SPNT_MANY_bb(qp,b1,b2) \
	B5(1,0,0,3,0,b2,1,0,b1,qp)

#define BR_CALL_DPTK_FEW_bb(qp,b1,b2) \
	B5(1,0,0,5,0,b2,0,0,b1,qp)

#define BR_CALL_DPTK_MANY_bb(qp,b1,b2) \
	B5(1,0,0,5,0,b2,1,0,b1,qp)

#define BR_CALL_DPNT_FEW_bb(qp,b1,b2) \
	B5(1,0,0,7,0,b2,0,0,b1,qp)

#define BR_CALL_DPNT_MANY_bb(qp,b1,b2) \
	B5(1,0,0,7,0,b2,1,0,b1,qp)

#define BR_CALL_SPTK_FEW_CLR_bb(qp,b1,b2) \
	B5(1,0,1,1,0,b2,0,0,b1,qp)

#define BR_CALL_SPTK_MANY_CLR_bb(qp,b1,b2) \
	B5(1,0,1,1,0,b2,1,0,b1,qp)

#define BR_CALL_SPNT_FEW_CLR_bb(qp,b1,b2) \
	B5(1,0,1,3,0,b2,0,0,b1,qp)

#define BR_CALL_SPNT_MANY_CLR_bb(qp,b1,b2) \
	B5(1,0,1,3,0,b2,1,0,b1,qp)

#define BR_CALL_DPTK_FEW_CLR_bb(qp,b1,b2) \
	B5(1,0,1,5,0,b2,0,0,b1,qp)

#define BR_CALL_DPTK_MANY_CLR_bb(qp,b1,b2) \
	B5(1,0,1,5,0,b2,1,0,b1,qp)

#define BR_CALL_DPNT_FEW_CLR_bb(qp,b1,b2) \
	B5(1,0,1,7,0,b2,0,0,b1,qp)

#define BR_CALL_DPNT_MANY_CLR_bb(qp,b1,b2) \
	B5(1,0,1,7,0,b2,1,0,b1,qp)
/* B6 - IP-Relative Predict */
/* B7 - Indirect Predict */

/* B8 - Miscellaneous (B-Unit) */
#define COVER() \
	B8(0,0,0x02,0,0)

#define CLRRRB() \
	B8(0,0,0x04,0,0)

#define CLRRRB_PR() \
	B8(0,0,0x05,0,0)

#define RFI() \
	B8(0,0,0x08,0,0)

#define BSW_0() \
	B8(0,0,0x0C,0,0)

#define BSW_1() \
	B8(0,0,0x0D,0,0)

#define EPC() \
	B8(0,0,0x10,0,0)

/* B9 - Break/Nop (B-Unit) */
#define BREAK_B_imm21(qp,imm21) \
	B9(0,((imm21>>20)&MASK(1)),0,0x00,0,(imm21&MASK(20)),qp)
	
#define NOP_B_imm21(qp,imm21) \
	B9(2,((imm21>>20)&MASK(1)),0,0x00,0,(imm21&MASK(20)),qp)
	

/* F-UNIT encoding macros */
/* F1 */
#define F1(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* F2 */
#define F2(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* F3 */
#define F3(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* F4 */
#define F4(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) << 12) |\
	(((unsigned long)(P09)) <<  6) |\
	(((unsigned long)(P10)) <<  0) \
	)

/* F5 */
#define F5(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 35) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) << 12) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* F6 */
#define F6(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* F7 */
#define F7(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* F8 */
#define F8(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* F9 */
#define F9(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 34) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* F10 */
#define F10(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* F11 */
#define F11(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 34) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 20) |\
	(((unsigned long)(P06)) << 13) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* F12 */
#define F12(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* F13 */
#define F13(P01,P02,P03,P04,P05,P06,P07)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* F14 */
#define F14(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 26) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)

/* F15 */
#define F15(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 34) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 27) |\
	(((unsigned long)(P06)) << 26) |\
	(((unsigned long)(P07)) <<  6) |\
	(((unsigned long)(P08)) <<  0) \
	)



typedef enum
{
	sfS0=0,
	sfS1,
	sfS2,
	sfS3
}StatusField;

/* F1 - Floating-point Multiply Add */
#define FMA_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,0,sfS0,f4,f3,f2,f1,qp)

#define FMA_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,0,sfS1,f4,f3,f2,f1,qp)

#define FMA_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,0,sfS2,f4,f3,f2,f1,qp)

#define FMA_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,0,sfS3,f4,f3,f2,f1,qp)

#define FMA_S_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,1,sfS0,f4,f3,f2,f1,qp)

#define FMA_S_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,1,sfS1,f4,f3,f2,f1,qp)

#define FMA_S_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,1,sfS2,f4,f3,f2,f1,qp)

#define FMA_S_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x08,1,sfS3,f4,f3,f2,f1,qp)

#define FMA_D_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,0,sfS0,f4,f3,f2,f1,qp)

#define FMA_D_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,0,sfS1,f4,f3,f2,f1,qp)

#define FMA_D_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,0,sfS2,f4,f3,f2,f1,qp)

#define FMA_D_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,0,sfS3,f4,f3,f2,f1,qp)

#define FPMA_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,1,sfS0,f4,f3,f2,f1,qp)

#define FPMA_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,1,sfS1,f4,f3,f2,f1,qp)

#define FPMA_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,1,sfS2,f4,f3,f2,f1,qp)

#define FPMA_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x09,1,sfS3,f4,f3,f2,f1,qp)

#define FMS_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,0,sfS0,f4,f3,f2,f1,qp)

#define FMS_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,0,sfS1,f4,f3,f2,f1,qp)

#define FMS_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,0,sfS2,f4,f3,f2,f1,qp)

#define FMS_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,0,sfS3,f4,f3,f2,f1,qp)

#define FMS_S_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,1,sfS0,f4,f3,f2,f1,qp)

#define FMS_S_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,1,sfS1,f4,f3,f2,f1,qp)

#define FMS_S_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,1,sfS2,f4,f3,f2,f1,qp)

#define FMS_S_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0A,1,sfS3,f4,f3,f2,f1,qp)

#define FMS_D_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,0,sfS0,f4,f3,f2,f1,qp)

#define FMS_D_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,0,sfS1,f4,f3,f2,f1,qp)

#define FMS_D_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,0,sfS2,f4,f3,f2,f1,qp)

#define FMS_D_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,0,sfS3,f4,f3,f2,f1,qp)

#define FPMS_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,1,sfS0,f4,f3,f2,f1,qp)

#define FPMS_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,1,sfS1,f4,f3,f2,f1,qp)

#define FPMS_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,1,sfS2,f4,f3,f2,f1,qp)

#define FPMS_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0B,1,sfS3,f4,f3,f2,f1,qp)

#define FNMA_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,0,sfS0,f4,f3,f2,f1,qp)

#define FNMA_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,0,sfS1,f4,f3,f2,f1,qp)

#define FNMA_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,0,sfS2,f4,f3,f2,f1,qp)

#define FNMA_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,0,sfS3,f4,f3,f2,f1,qp)

#define FNMA_S_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,1,sfS0,f4,f3,f2,f1,qp)

#define FNMA_S_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,1,sfS1,f4,f3,f2,f1,qp)

#define FNMA_S_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,1,sfS2,f4,f3,f2,f1,qp)

#define FNMA_S_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0C,1,sfS3,f4,f3,f2,f1,qp)

#define FNMA_D_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,0,sfS0,f4,f3,f2,f1,qp)

#define FNMA_D_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,0,sfS1,f4,f3,f2,f1,qp)

#define FNMA_D_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,0,sfS2,f4,f3,f2,f1,qp)

#define FNMA_D_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,0,sfS3,f4,f3,f2,f1,qp)

#define FPNMA_SFS0_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,1,sfS0,f4,f3,f2,f1,qp)

#define FPNMA_SFS1_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,1,sfS1,f4,f3,f2,f1,qp)

#define FPNMA_SFS2_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,1,sfS2,f4,f3,f2,f1,qp)

#define FPNMA_SFS3_ffff(qp,f1,f3,f4,f2) \
	F1(0x0D,1,sfS3,f4,f3,f2,f1,qp)

/* F2 - Fixed-point Multiply Add */
#define XMA_L_ffff(qp,f1,f3,f4,f2) \
	F2(0xE,1,0,f4,f3,f2,f1,qp)

#define XMA_H_ffff(qp,f1,f3,f4,f2) \
	F2(0xE,3,0,f4,f3,f2,f1,qp)

#define XMA_HU_ffff(qp,f1,f3,f4,f2) \
	F2(0xE,2,0,f4,f3,f2,f1,qp)

/* F3 - Parallel Floating-point Select */
#define FSELECT_ffff(qp,f1,f3,f4,f2) \
	F3(0x0E,0,0,f4,f3,f2,f1,qp)

/* F4 - Floating-point Compare */
#define FCMP_EQ_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS0,0,p2,f3,f2,0,p1,qp)

#define FCMP_EQ_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS1,0,p2,f3,f2,0,p1,qp)

#define FCMP_EQ_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS2,0,p2,f3,f2,0,p1,qp)

#define FCMP_EQ_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS3,0,p2,f3,f2,0,p1,qp)

#define FCMP_LT_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS0,0,p2,f3,f2,0,p1,qp)

#define FCMP_LT_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS1,0,p2,f3,f2,0,p1,qp)

#define FCMP_LT_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS2,0,p2,f3,f2,0,p1,qp)

#define FCMP_LT_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS3,0,p2,f3,f2,0,p1,qp)

#define FCMP_LE_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS0,1,p2,f3,f2,0,p1,qp)

#define FCMP_LE_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS1,1,p2,f3,f2,0,p1,qp)

#define FCMP_LE_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS2,1,p2,f3,f2,0,p1,qp)

#define FCMP_LE_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS3,1,p2,f3,f2,0,p1,qp)

#define FCMP_UNORD_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS0,1,p2,f3,f2,0,p1,qp)

#define FCMP_UNORD_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS1,1,p2,f3,f2,0,p1,qp)

#define FCMP_UNORD_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS2,1,p2,f3,f2,0,p1,qp)

#define FCMP_UNORD_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS3,1,p2,f3,f2,0,p1,qp)

#define FCMP_EQ_UNC_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS0,0,p2,f3,f2,0,p1,qp)

#define FCMP_EQ_UNC_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS1,0,p2,f3,f2,0,p1,qp)

#define FCMP_EQ_UNC_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS2,0,p2,f3,f2,0,p1,qp)

#define FCMP_EQ_UNC_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS3,0,p2,f3,f2,0,p1,qp)

#define FCMP_LT_UNC_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS0,0,p2,f3,f2,1,p1,qp)

#define FCMP_LT_UNC_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS1,0,p2,f3,f2,1,p1,qp)

#define FCMP_LT_UNC_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS2,0,p2,f3,f2,1,p1,qp)

#define FCMP_LT_UNC_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS3,0,p2,f3,f2,1,p1,qp)

#define FCMP_LE_UNC_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS0,1,p2,f3,f2,1,p1,qp)

#define FCMP_LE_UNC_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS1,1,p2,f3,f2,1,p1,qp)

#define FCMP_LE_UNC_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS2,1,p2,f3,f2,1,p1,qp)

#define FCMP_LE_UNC_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,0,sfS3,1,p2,f3,f2,1,p1,qp)

#define FCMP_UNORD_UNC_S0_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS0,1,p2,f3,f2,1,p1,qp)

#define FCMP_UNORD_UNC_S1_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS1,1,p2,f3,f2,1,p1,qp)

#define FCMP_UNORD_UNC_S2_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS2,1,p2,f3,f2,1,p1,qp)

#define FCMP_UNORD_UNC_S3_ppff(qp,p1,p2,f2,f3) \
	F4(4,1,sfS3,1,p2,f3,f2,1,p1,qp)

/* F5 - Floating-point Class */
#define FCLASS_M_ppffclass9(qp,p1,p2,f2,fclass9) \
	F5(5,0,((fclass9)&MASK(2)),p2,((fclass9>>2)&MASK(7)),f2,0,p1,qp)

#define FCLASS_M_UNC_ppffclass9(qp,p1,p2,f2,fclass9) \
	F5(5,0,((fclass9)&MASK(2)),p2,((fclass9>>2)&MASK(7)),f2,1,p1,qp)


/* F6 - Floating-point Reciprocal Approximation */
#define FRCPA_S0_fpff(qp,f1,p2,f2,f3) \
	F6(0,0,sfS0,1,p2,f3,f2,f1,qp)

#define FRCPA_S1_fpff(qp,f1,p2,f2,f3) \
	F6(0,0,sfS1,1,p2,f3,f2,f1,qp)

#define FRCPA_S2_fpff(qp,f1,p2,f2,f3) \
	F6(0,0,sfS2,1,p2,f3,f2,f1,qp)

#define FRCPA_S3_fpff(qp,f1,p2,f2,f3) \
	F6(0,0,sfS3,1,p2,f3,f2,f1,qp)

#define FPRCPA_S0_fpff(qp,f1,p2,f2,f3) \
	F6(1,0,sfS0,1,p2,f3,f2,f1,qp)

#define FPRCPA_S1_fpff(qp,f1,p2,f2,f3) \
	F6(1,0,sfS1,1,p2,f3,f2,f1,qp)

#define FPRCPA_S2_fpff(qp,f1,p2,f2,f3) \
	F6(1,0,sfS2,1,p2,f3,f2,f1,qp)

#define FPRCPA_S3_fpff(qp,f1,p2,f2,f3) \
	F6(1,0,sfS3,1,p2,f3,f2,f1,qp)

/* F7 - Floating-point Reciprocal Square Root Approximation */
#define FRSQRTA_S0_fpf(qp,f1,p2,f3) \
	F7(0,1,sfS0,1,p2,f3,0,f1,qp)

#define FRSQRTA_S1_fpf(qp,f1,p2,f3) \
	F7(0,1,sfS1,1,p2,f3,0,f1,qp)

#define FRSQRTA_S2_fpf(qp,f1,p2,f3) \
	F7(0,1,sfS2,1,p2,f3,0,f1,qp)

#define FRSQRTA_S3_fpf(qp,f1,p2,f3) \
	F7(0,1,sfS3,1,p2,f3,0,f1,qp)

#define FPRSQRTA_S0_fpf(qp,f1,p2,f3) \
	F7(1,1,sfS0,1,p2,f3,0,f1,qp)

#define FPRSQRTA_S1_fpf(qp,f1,p2,f3) \
	F7(1,1,sfS1,1,p2,f3,0,f1,qp)

#define FPRSQRTA_S2_fpf(qp,f1,p2,f3) \
	F7(1,1,sfS2,1,p2,f3,0,f1,qp)

#define FPRSQRTA_S3_fpf(qp,f1,p2,f3) \
	F7(1,1,sfS3,1,p2,f3,0,f1,qp)

/* F8  - Minimum/Maximum and Parallel Compare */
#define FMIN_S0_fff(qp,f1,f2,f3) \
	F8(0,0,sfS0,0,0x14,f3,f2,f1,qp)

#define FMIN_S1_fff(qp,f1,f2,f3) \
	F8(0,0,sfS1,0,0x14,f3,f2,f1,qp)

#define FMIN_S2_fff(qp,f1,f2,f3) \
	F8(0,0,sfS2,0,0x14,f3,f2,f1,qp)

#define FMIN_S3_fff(qp,f1,f2,f3) \
	F8(0,0,sfS3,0,0x14,f3,f2,f1,qp)

#define FMAX_S0_fff(qp,f1,f2,f3) \
	F8(0,0,sfS0,0,0x15,f3,f2,f1,qp)

#define FMAX_S1_fff(qp,f1,f2,f3) \
	F8(0,0,sfS1,0,0x15,f3,f2,f1,qp)

#define FMAX_S2_fff(qp,f1,f2,f3) \
	F8(0,0,sfS2,0,0x15,f3,f2,f1,qp)

#define FMAX_S3_fff(qp,f1,f2,f3) \
	F8(0,0,sfS3,0,0x15,f3,f2,f1,qp)

#define FAMIN_S0_fff(qp,f1,f2,f3) \
	F8(0,0,sfS0,0,0x16,f3,f2,f1,qp)

#define FAMIN_S1_fff(qp,f1,f2,f3) \
	F8(0,0,sfS1,0,0x16,f3,f2,f1,qp)

#define FAMIN_S2_fff(qp,f1,f2,f3) \
	F8(0,0,sfS2,0,0x16,f3,f2,f1,qp)

#define FAMIN_S3_fff(qp,f1,f2,f3) \
	F8(0,0,sfS3,0,0x16,f3,f2,f1,qp)

#define FAMAX_S0_fff(qp,f1,f2,f3) \
	F8(0,0,sfS0,0,0x17,f3,f2,f1,qp)

#define FAMAX_S1_fff(qp,f1,f2,f3) \
	F8(0,0,sfS1,0,0x17,f3,f2,f1,qp)

#define FAMAX_S2_fff(qp,f1,f2,f3) \
	F8(0,0,sfS2,0,0x17,f3,f2,f1,qp)

#define FAMAX_S3_fff(qp,f1,f2,f3) \
	F8(0,0,sfS3,0,0x17,f3,f2,f1,qp)

#define FPMIN_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x14,f3,f2,f1,qp)

#define FPMIN_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x14,f3,f2,f1,qp)

#define FPMIN_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x14,f3,f2,f1,qp)

#define FPMIN_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x14,f3,f2,f1,qp)

#define FPMAX_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x15,f3,f2,f1,qp)

#define FPMAX_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x15,f3,f2,f1,qp)

#define FPMAX_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x15,f3,f2,f1,qp)

#define FPMAX_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x15,f3,f2,f1,qp)

#define FPAMIN_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x16,f3,f2,f1,qp)

#define FPAMIN_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x16,f3,f2,f1,qp)

#define FPAMIN_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x16,f3,f2,f1,qp)

#define FPAMIN_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x16,f3,f2,f1,qp)

#define FPAMAX_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x17,f3,f2,f1,qp)

#define FPAMAX_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x17,f3,f2,f1,qp)

#define FPAMAX_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x17,f3,f2,f1,qp)

#define FPAMAX_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x17,f3,f2,f1,qp)

#define FPCMP_EQ_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x30,f3,f2,f1,qp)

#define FPCMP_EQ_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x30,f3,f2,f1,qp)

#define FPCMP_EQ_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x30,f3,f2,f1,qp)

#define FPCMP_EQ_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x30,f3,f2,f1,qp)

#define FPCMP_LT_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x31,f3,f2,f1,qp)

#define FPCMP_LT_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x31,f3,f2,f1,qp)

#define FPCMP_LT_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x31,f3,f2,f1,qp)

#define FPCMP_LT_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x31,f3,f2,f1,qp)

#define FPCMP_LE_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x32,f3,f2,f1,qp)

#define FPCMP_LE_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x32,f3,f2,f1,qp)

#define FPCMP_LE_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x32,f3,f2,f1,qp)

#define FPCMP_LE_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x32,f3,f2,f1,qp)

#define FPCMP_UNORD_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x33,f3,f2,f1,qp)

#define FPCMP_UNORD_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x33,f3,f2,f1,qp)

#define FPCMP_UNORD_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x33,f3,f2,f1,qp)

#define FPCMP_UNORD_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x33,f3,f2,f1,qp)

#define FPCMP_NEQ_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x34,f3,f2,f1,qp)

#define FPCMP_NEQ_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x34,f3,f2,f1,qp)

#define FPCMP_NEQ_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x34,f3,f2,f1,qp)

#define FPCMP_NEQ_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x34,f3,f2,f1,qp)

#define FPCMP_NLT_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x35,f3,f2,f1,qp)

#define FPCMP_NLT_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x35,f3,f2,f1,qp)

#define FPCMP_NLT_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x35,f3,f2,f1,qp)

#define FPCMP_NLT_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x35,f3,f2,f1,qp)

#define FPCMP_NLE_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x36,f3,f2,f1,qp)

#define FPCMP_NLE_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x36,f3,f2,f1,qp)

#define FPCMP_NLE_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x36,f3,f2,f1,qp)

#define FPCMP_NLE_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x36,f3,f2,f1,qp)

#define FPCMP_ORD_S0_fff(qp,f1,f2,f3) \
	F8(1,0,sfS0,0,0x37,f3,f2,f1,qp)

#define FPCMP_ORD_S1_fff(qp,f1,f2,f3) \
	F8(1,0,sfS1,0,0x37,f3,f2,f1,qp)

#define FPCMP_ORD_S2_fff(qp,f1,f2,f3) \
	F8(1,0,sfS2,0,0x37,f3,f2,f1,qp)

#define FPCMP_ORD_S3_fff(qp,f1,f2,f3) \
	F8(1,0,sfS3,0,0x37,f3,f2,f1,qp)

/* F9 - Merge and Logical */
#define FMERGE_S_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x10,f3,f2,f1,qp) 
	
#define FMERGE_NS_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x11,f3,f2,f1,qp) 
	
#define FMERGE_SE_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x12,f3,f2,f1,qp) 

#define FMIX_LR_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x39,f3,f2,f1,qp) 

#define FMIX_R_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x3A,f3,f2,f1,qp) 

#define FMIX_L_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x3B,f3,f2,f1,qp) 

#define FSXT_R_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x3C,f3,f2,f1,qp) 

#define FSXT_L_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x3D,f3,f2,f1,qp) 

#define FPACK_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x28,f3,f2,f1,qp) 

#define FSWAP_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x34,f3,f2,f1,qp) 

#define FSWAP_NL_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x35,f3,f2,f1,qp) 

#define FSWAP_NR_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x36,f3,f2,f1,qp) 

#define FAND_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x2C,f3,f2,f1,qp) 

#define FANDCM_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x2D,f3,f2,f1,qp) 

#define FOR_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x2E,f3,f2,f1,qp) 

#define FXOR_fff(qp,f1,f2,f3) \
	F9(0,0,0,0x2F,f3,f2,f1,qp) 

#define FPMERGE_S_fff(qp,f1,f2,f3) \
	F9(1,0,0,0x10,f3,f2,f1,qp) 
	
#define FPMERGE_NS_fff(qp,f1,f2,f3) \
	F9(1,0,0,0x11,f3,f2,f1,qp) 
	
#define FPMERGE_SE_fff(qp,f1,f2,f3) \
	F9(1,0,0,0x12,f3,f2,f1,qp) 

/* F10 - Convert Floating-point to Fixed-point */
#define FCVT_FX_S0_ff(qp,f1,f2) \
	F10(0,0,sfS0,0,0x18,0,f2,f1,qp)

#define FCVT_FX_S1_ff(qp,f1,f2) \
	F10(0,0,sfS1,0,0x18,0,f2,f1,qp)

#define FCVT_FX_S2_ff(qp,f1,f2) \
	F10(0,0,sfS2,0,0x18,0,f2,f1,qp)

#define FCVT_FX_S3_ff(qp,f1,f2) \
	F10(0,0,sfS3,0,0x18,0,f2,f1,qp)

#define FCVT_FXU_S0_ff(qp,f1,f2) \
	F10(0,0,sfS0,0,0x19,0,f2,f1,qp)

#define FCVT_FXU_S1_ff(qp,f1,f2) \
	F10(0,0,sfS1,0,0x19,0,f2,f1,qp)

#define FCVT_FXU_S2_ff(qp,f1,f2) \
	F10(0,0,sfS2,0,0x19,0,f2,f1,qp)

#define FCVT_FXU_S3_ff(qp,f1,f2) \
	F10(0,0,sfS3,0,0x19,0,f2,f1,qp)

#define FCVT_FX_TRUNC_S0_ff(qp,f1,f2) \
	F10(0,0,sfS0,0,0x1A,0,f2,f1,qp)

#define FCVT_FX_TRUNC_S1_ff(qp,f1,f2) \
	F10(0,0,sfS1,0,0x1A,0,f2,f1,qp)

#define FCVT_FX_TRUNC_S2_ff(qp,f1,f2) \
	F10(0,0,sfS2,0,0x1A,0,f2,f1,qp)

#define FCVT_FX_TRUNC_S3_ff(qp,f1,f2) \
	F10(0,0,sfS3,0,0x1A,0,f2,f1,qp)

#define FCVT_FXU_TRUNC_S0_ff(qp,f1,f2) \
	F10(0,0,sfS0,0,0x1B,0,f2,f1,qp)

#define FCVT_FXU_TRUNC_S1_ff(qp,f1,f2) \
	F10(0,0,sfS1,0,0x1B,0,f2,f1,qp)

#define FCVT_FXU_TRUNC_S2_ff(qp,f1,f2) \
	F10(0,0,sfS2,0,0x1B,0,f2,f1,qp)

#define FCVT_FXU_TRUNC_S3_ff(qp,f1,f2) \
	F10(0,0,sfS3,0,0x1B,0,f2,f1,qp)

#define FPCVT_FX_S0_ff(qp,f1,f2) \
	F10(1,0,sfS0,0,0x18,0,f2,f1,qp)

#define FPCVT_FX_S1_ff(qp,f1,f2) \
	F10(1,0,sfS1,0,0x18,0,f2,f1,qp)

#define FPCVT_FX_S2_ff(qp,f1,f2) \
	F10(1,0,sfS2,0,0x18,0,f2,f1,qp)

#define FPCVT_FX_S3_ff(qp,f1,f2) \
	F10(1,0,sfS3,0,0x18,0,f2,f1,qp)

#define FPCVT_FXU_S0_ff(qp,f1,f2) \
	F10(1,0,sfS0,0,0x19,0,f2,f1,qp)

#define FPCVT_FXU_S1_ff(qp,f1,f2) \
	F10(1,0,sfS1,0,0x19,0,f2,f1,qp)

#define FPCVT_FXU_S2_ff(qp,f1,f2) \
	F10(1,0,sfS2,0,0x19,0,f2,f1,qp)

#define FPCVT_FXU_S3_ff(qp,f1,f2) \
	F10(1,0,sfS3,0,0x19,0,f2,f1,qp)

#define FPCVT_FX_TRUNC_S0_ff(qp,f1,f2) \
	F10(1,0,sfS0,0,0x1A,0,f2,f1,qp)

#define FPCVT_FX_TRUNC_S1_ff(qp,f1,f2) \
	F10(1,0,sfS1,0,0x1A,0,f2,f1,qp)

#define FPCVT_FX_TRUNC_S2_ff(qp,f1,f2) \
	F10(1,0,sfS2,0,0x1A,0,f2,f1,qp)

#define FPCVT_FX_TRUNC_S3_ff(qp,f1,f2) \
	F10(1,0,sfS3,0,0x1A,0,f2,f1,qp)

#define FPCVT_FXU_TRUNC_S0_ff(qp,f1,f2) \
	F10(1,0,sfS0,0,0x1B,0,f2,f1,qp)

#define FPCVT_FXU_TRUNC_S1_ff(qp,f1,f2) \
	F10(1,0,sfS1,0,0x1B,0,f2,f1,qp)

#define FPCVT_FXU_TRUNC_S2_ff(qp,f1,f2) \
	F10(1,0,sfS2,0,0x1B,0,f2,f1,qp)

#define FPCVT_FXU_TRUNC_S3_ff(qp,f1,f2) \
	F10(1,0,sfS3,0,0x1B,0,f2,f1,qp)

/* F11 - Convert Fixed-point to Floating-point */
#define FCVT_XF_ff(qp,f1,f2) \
	F11(0,0,0,0x1C,0,f2,f1,qp)

/* F12 - Floating-point Set Controls */
#define FSETC_S0_amaskomask(qp,amask7,omask7) \
	F12(0,0,sfS0,0,0x04,omask7,amask7,0,qp)

#define FSETC_S1_amaskomask(qp,amask7,omask7) \
	F12(0,0,sfS1,0,0x04,omask7,amask7,0,qp)

#define FSETC_S2_amaskomask(qp,amask7,omask7) \
	F12(0,0,sfS2,0,0x04,omask7,amask7,0,qp)

#define FSETC_S3_amaskomask(qp,amask7,omask7) \
	F12(0,0,sfS3,0,0x04,omask7,amask7,0,qp)

/* F13 - Floating-point Clear Flags */
#define FCLRF_S0(qp) \
	F13(0,0,sfS0,0,0x05,0,qp)

#define FCLRF_S1(qp) \
	F13(0,0,sfS1,0,0x05,0,qp)

#define FCLRF_S2(qp) \
	F13(0,0,sfS2,0,0x05,0,qp)

#define FCLRF_S3(qp) \
	F13(0,0,sfS3,0,0x05,0,qp)

/* TODO : F14 - Floating-point Check Flags */

/* F15 - Break/Nop(F-Unit) */
#define BREAK_F_imm21(qp,imm21) \
	F15(0,(((imm21)>>20)&MASK(1)),0,0,1,0,(imm21&(MASK(20))),qp)

#define NOP_F_imm21(qp,imm21) \
	F15(0,(((imm21)>>20)&MASK(1)),0,0,1,0,(imm21&(MASK(20))),qp)


/* X-UNIT encoding macros */
/* X1 */
#define X1(P01,P02,P03,P04,P05,P06,P07,P08)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 33) |\
	(((unsigned long)(P04)) << 27) |\
	(((unsigned long)(P05)) << 26) |\
	(((unsigned long)(P06)) <<  6) |\
	(((unsigned long)(P07)) <<  0) \
	)

/* X2 */
#define X2(P01,P02,P03,P04,P05,P06,P07,P08,P09)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 27) |\
	(((unsigned long)(P04)) << 22) |\
	(((unsigned long)(P05)) << 21) |\
	(((unsigned long)(P06)) << 20) |\
	(((unsigned long)(P07)) << 13) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* X3 */
#define X3(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) << 12) |\
	(((unsigned long)(P07)) <<  9) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* X4 */
#define X4(P01,P02,P03,P04,P05,P06,P07,P08,P09,P10,P11)   \
	(\
	(((unsigned long)(P01)) << 37) |\
	(((unsigned long)(P02)) << 36) |\
	(((unsigned long)(P03)) << 35) |\
	(((unsigned long)(P04)) << 33) |\
	(((unsigned long)(P05)) << 13) |\
	(((unsigned long)(P06)) << 12) |\
	(((unsigned long)(P07)) <<  9) |\
	(((unsigned long)(P08)) <<  6) |\
	(((unsigned long)(P09)) <<  0) \
	)

/* TODO: X-Unit Instructions */
#define MOVL_rimm23(qp,r1,imm23) \
	X2(6,((imm23>>22)&MASK(1)),((imm23>>7)&MASK(9)),((imm23>>16)&MASK(5)),((imm23>>21)&MASK(1)),0,((imm23)&MASK(7)),r1,qp)	
	
#define MAKE_BUNDLE(bundle,template,instr0,instr1,instr2) \
	do { \
		bundle->a =(template)|(instr0<<5) | ((instr1&MASK(18))<<46);\
		bundle->b =(instr1>>18)|(instr2 << 23);\
	}while(0);

/* Psuedo ops */
#define PUSH_r(qp,r) \
	ST8_rrimm9(qp,IA64_SP,r,-16)

#define POP_r(qp,r) \
	LD8_rr(qp,r,IA64_SP)

#define IA64_IN(n) 	(IA64_R32+n)
#define IA64_LOC(n)	(IA64_R32+soi+n)
#define IA64_OUT(n)	(IA64_R32+soi+sol+n)

#define IA64ERROR(MSG) IA64_Error(MSG,__FILE__, __LINE__, __FUNCTION__)

typedef struct _fp
{
	long addr;
	long gp;
}IA64_FUNCTION;

typedef struct Bundle
{
	unsigned long a;
	unsigned long b;
}Bundle;

typedef Bundle* IA64_inst_ptr;
typedef Bundle* ia64_inst_ptr;
Bundle MOVL_rimm64(int,int,UL);
Bundle BRL_imm64(Bundle*,int,UL);
Bundle ALLOC_rilor(int r1,int i,int l,int o,int r);
void IA64_FlushCache(void *addr, unsigned long len);
void IA64_AddInstruction(Bundle*,IA64Units unit,unsigned long instr);
void IA64_DumpCode(unsigned char*,int);
int  IA64_Error(char*,char*,int,char*);
int  IA64_Execute(Bundle* code);
int  IA64_Dummy();
void  IA64_DebugMessageBox(char*,char*,int,char*);

extern int soi,sol,soo,sor;
extern unsigned long instr0,instr1,instr2,template;

#ifdef __cplusplus
};
#endif

#endif /* _MD_IA64_MACROS_H */
