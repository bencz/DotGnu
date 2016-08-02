/*
 * md_ia64.h - Machine-dependent definitions of IA-64
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Contributed by : CH Gowri Kumar <gkumar@csa.iisc.ernet.in>
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

#include "cvm_config.h"

#ifdef __cplusplus
extern "C" {
#endif

#ifdef CVM_IA64

#include "md_ia64_macros.h"

int soi,sol,soo,sor;
unsigned long int instr0,instr1,instr2,template;

static void MessageBox(char* msg1,char* msg2)
{
	int sw,tw,bw,lm,tw1,tw2;
	int i;
	
	sw = 80;
	tw1 = strlen(msg1);
	tw2 = strlen(msg2);
	tw = (tw1>tw2)?tw1:tw2;
	bw = tw+6;
	lm = (sw-bw)/2;

	printf("\n");
	for(i = 0;i < lm; i++)
		printf(" ");
	
	printf("+");
	
	for(i = 0;i < (bw-2); i++)
		printf("-");
	
	printf("+\n");
	
	for(i = 0;i < lm;i++)
		printf(" ");
	
	printf("|  ");
	
	for(i = 0;i < tw;i++)
		printf(" ");
	printf("  |\n");

	for(i = 0;i < lm; i++)
		printf(" ");
	
	printf("|  ");
	printf("%s",msg1);
	
	for(i = 0;i < (tw-tw1);i++)
		printf(" ");
	printf("  |\n");
	

	for(i = 0;i < lm; i++)
		printf(" ");
	
	printf("|  ");
	printf("%s",msg2);
	for(i = 0;i < (tw-tw2);i++)
		printf(" ");
	printf("  |\n");

	for(i = 0;i < lm; i++)
		printf(" ");
	
	printf("|  ");
	for(i = 0;i < tw; i++)
		printf(" ");
	printf("  |\n");

	for(i = 0;i < lm; i++)
		printf(" ");
	printf("+");
	for(i = 0;i < (bw-2); i++)
		printf("-");
	printf("+\n");
}
	
void IA64_DebugMessageBox(char* msg,char *file,int line,char *function)
{
#define SIZE 80
#define FANCY
#if defined(FANCY)
	char msg1[SIZE];
	char msg2[SIZE];
	msg1[SIZE-1] = '\0';
	msg2[SIZE-1] = '\0';
	snprintf(msg1,SIZE-1,"%s: In function `%s'",file,function);
	snprintf(msg2,SIZE-1,"%s:%d: %s is not implemented",file,line,msg);
	MessageBox(msg1,msg2);
#else
	fprintf(stderr,"%s: In function `%s':\n",file,function);
	fprintf(stderr,"%s:%d: %s\n",file,line,msg);
#endif
	exit(0);

}

int IA64_Error(char* msg,char *file,int line,char *function)
{
	fprintf(stderr,"%s: In function `%s':\n",file,function);
	fprintf(stderr,"%s:%d: %s\n",file,line,msg);
	abort();
	return 0;
}

int IA64_Dummy()
{
	return 0;
}

int IA64_Execute(Bundle *code)
{
	
	IA64_FUNCTION *fp,newfp;
	int (*pSubRoutine)(void);
	int i;
	
	fp = (IA64_FUNCTION*)IA64_Dummy;
	newfp.gp = fp->gp;
	newfp.addr = (long)code;
	pSubRoutine = (int(*)(void))&newfp;

	/* Make the page containing the code  executable */
	mprotect((void *) ((long) code & ~(getpagesize () - 1)),
	 		             getpagesize(), PROT_READ | PROT_WRITE | PROT_EXEC);
	
	IA64_FlushCache(code,CODESIZE);
	i = (*pSubRoutine)();
	
	return i;
}


void IA64_FlushCache(void *addr,unsigned long len)
{
	void *end = (char*)addr + len;
	while(addr < end)
	{
		asm volatile("fc %0" :: "r"(addr));
		addr = (char*)addr + 32;
	}
	asm volatile(";;sync.i;;srlz.i;;");
}

Bundle ALLOC_rilor(int r1,int i,int l,int o,int r)
{
	Bundle b;
	Bundle* a;
	UL instr0,instr1,instr2;
	int sol,sor,sof;
	
	a = &b;
	
	sof = i+l+o;
	sol = i+l;
	sor = r>>3;

	assert(sof<=96);
	assert(sor<=sof);
	assert(sol<=sof);

	instr0 = ALLOC_rarpfsilor(0,r1,IA64_AR_PFS,i,l,o,r);
	instr1 = NOP_M_imm21(0,0);
	instr2 = NOP_I_imm21(0,0);
	MAKE_BUNDLE(a,M_MI_,instr0,instr1,instr2);
	return b;
}

Bundle BRL_imm64(Bundle* inst,int qp,UL label)
{
	UL target64;
	UL imm39,imm20b;
	int template,sign;
	Bundle *a,b;
	
	target64 = (UL)inst - label;
	target64 = target64 >> 4;
	
	sign = MASK(1) & (target64 >>59);
	imm39 = MASK(39) & (target64>>20);
	imm20b = MASK(20) & (target64);

	template = MLX_;
	instr0 = NOP_M_imm21(IA64_P0,0);
	instr1 = imm39 << 2;
	instr2 = B1(0X0C,sign,0,0,imm20b,0,0,0,qp);
	a = &b;
	MAKE_BUNDLE(a,template,instr0,instr1,instr2);
	return b;
	
		
}
	
Bundle MOVL_rimm64(int qp,int r1,UL imm64)
{
	UL instr0,instr1,instr2;
	UL target23,target41;
	Bundle b;
	Bundle* a;
	int template;
	int sign;
	
	sign = MASK(1) &(imm64>>63);
	target23 = ((imm64)&MASK(22));
	target23 = target23 | sign<<22;
	target41 = (imm64>>22)&MASK(41);
	
	template = MLX_;
	instr0 = NOP_M_imm21(IA64_P0,0);
	instr1 = target41;
	instr2 = MOVL_rimm23(qp,r1,target23);
	a = &b;
	MAKE_BUNDLE(a,template,instr0,instr1,instr2);
	return b;
}

void IA64_AddInstruction(Bundle* bundle,IA64Units unit,UL instr)
{
	UL instr0,instr1,instr2;
	int template;
	
	instr0=instr1=instr2=template=0;
	switch(unit)
	{
		case IA64_AUNIT:
			template = MII_;
			instr0 = NOP_M_imm21(IA64_P0,0);
			instr1 = NOP_I_imm21(IA64_P0,0);
			instr2 = instr;
			break;
			
		case IA64_IUNIT:
			template = MII_;
			instr0 = NOP_M_imm21(IA64_P0,0);
			instr1 = NOP_I_imm21(IA64_P0,0);
			instr2 = instr;
		   break;	

		case IA64_MUNIT:
			template = MII_;
			instr0 = instr;
			instr1 = NOP_I_imm21(IA64_P0,0);
			instr2 = NOP_I_imm21(IA64_P0,0);
			break;

		case IA64_BUNIT:
			template = MFB_;
			instr0 = NOP_M_imm21(IA64_P0,0);
			instr1 = NOP_F_imm21(IA64_P0,0);
			instr2 = instr;
		   break;	
		   
		case IA64_FUNIT:
			template = MFB_;
			instr0 = NOP_M_imm21(IA64_P0,0);
			instr1 = instr;
			instr2 = NOP_B_imm21(IA64_P0,0);
			break;
		case IA64_XUNIT:
			printf("X-UNIT Not Yet Implemented\n");
			assert(0);
		   break;	
	}
	bundle->a =(template)|(instr0<<5) | ((instr1&MASK(18))<<46);\
	bundle->b =(instr1>>18)|(instr2 << 23);
}

void IA64_DumpCode(unsigned char *start, int len)
{
	char cmdline[128];
	FILE *file = fopen("/tmp/unroll.s", "w");
	if(!file)
	{
		return;
	}
	while(len > 0)
	{
		fprintf(file, ".byte %d\n", (int)(*start));
		++start;
		--len;
	}
	fclose(file);
	sprintf(cmdline, "as /tmp/unroll.s -o /tmp/unroll.o;objdump --adjust-vma=%ld -d /tmp/unroll.o", (long)start);
	system(cmdline);
	unlink("/tmp/unroll.s");
	unlink("/tmp/unroll.o");
	putc('\n', stdout);
	fflush(stdout);
}

#endif /* CVM_IA64 */

#ifdef __cplusplus
};
#endif
