/*
 * debugger.c - debugger support for execution engine.
 *
 * Copyright (C) 2006  Southern Storm Software, Pty Ltd.
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

#include <stdio.h>
#include <ctype.h>
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif
#include "il_sysio.h"
#include "il_engine.h"
#include "il_debug.h"
#include "il_dumpasm.h"
#include "engine.h"
#include "cvm.h"
#include "lib_defs.h"
#include "debugger.h"

#ifdef IL_USE_JIT
#include <jit/jit-dump.h>
#include "jitc.h"
#endif

#ifdef IL_DEBUGGER

/*
 * Lock the debugger object.
 */
#define	LockDebugger(debugger)		ILMutexLock(debugger->lock);

/*
 * Unlock the debugger object.
 */
#define	UnlockDebugger(debugger)	ILMutexUnlock(debugger->lock);

/*
 * Extern functions.
 */
extern ILInt32 _IL_StackFrame_InternalGetTotalFrames(ILExecThread * _thread);

/*
 * Forward declarations.
 */
static void CommandLoop(ILDebugger *debugger);
static void Help(ILDebugger *debugger, FILE *stream);

/*
 * Print out of memory memory and exit program.
 */
static void ILDbOutOfMemory()
{
	fputs("debugger: virtual memory exhausted\n", stderr);
	exit(1);
}

/*
 * Open stream for writing and reading text. Return 0 on failure.
 */
static FILE *OpenTmpStream()
{
	return tmpfile();
}

/*
 * Return contents of stream from start to current position
 * as text allocated with ILMalloc() terminated with 0.
 * Returns 0 on error.
 */
static char *ReadStream(FILE *stream)
{
	int len;
	char *result;

	/* Terminate the stream with 0 */
	putc(0, stream);

	/* Allocate the string and read the stream into it */
	len = (int) ftell(stream);
	result = (char *) ILMalloc(len);
	if(result == 0)
	{
		return 0;
	}
	fseek(stream, 0, SEEK_SET);
	fread((void *) result, 1, len, stream);
	return result;
}

/*
 * Open socket IO.
 */
static int SocketIO_Open(ILDebuggerIO *io, const char *connectionString)
{
	ILSysIOHandle sockfd;
	int index = 0;
	const char *ch;
	unsigned int conn[5];
	unsigned char addr[16];

	/* Parse connection string */
	if(connectionString != 0)
	{
		ch = connectionString;
		ILMemSet(conn, 0, sizeof(conn));
		while(*ch != 0 && index <= 4)
		{
			if(*ch >= '0' && *ch <= '9')
			{
				conn[index] = (10 * conn[index]) + (*ch - '0');
			}
			else if(*ch == '.' || *ch == ':')
			{
				index++;
			}
			ch++;
		}
	}
	else
	{
		/* Default host */
		conn[0] = 127;
		conn[1] = 0;
		conn[2] = 0;
		conn[3] = 1;
	}

	/* Default port if we havent parsed it */
	if(index <= 3)
	{
		conn[4] = 4571;
	}

	/*
	 * Create socket descriptor
	 *  adress family = 2 (IL_AF_INET, AddressFamily.InterNetwork
	 *  socket type = 1 (IL_SOCK_STREAM, SocketType.Stream)
	 *  protocol type = 6 (ProtocolType.Tcp)
	 */
	sockfd = ILSysIOSocket(2, 1, 6);
	if(sockfd == ILSysIOHandle_Invalid)
	{
		perror("debugger create socket");
		return 0;
	}

	/* Convert the end point into a sockaddr buffer like in
	 * CombinedToSerialized() in socket.c */
	addr[0] = 2;								/* address family - AF_INET */
	addr[1] = 0; 
	addr[2] = (unsigned char)(conn[4] >> 8);	/* port */
	addr[3] = (unsigned char)(conn[4]);
	addr[4] = (unsigned char)(conn[0]);			/* end point 127.0.0.1 */
	addr[5] = (unsigned char)(conn[1]);
	addr[6] = (unsigned char)(conn[2]);
	addr[7] = (unsigned char)(conn[3]);
	addr[8] = addr[9] = addr[10] = addr[11] =
											 addr[12] = addr[13] =
											 addr[14]= addr[15] = 0;

	/* Connect, this connects socket for ipv4 */
	if(!ILSysIOSocketConnect(sockfd, addr, 16))
	{
		perror("debugger connect");
		return 0;
	}

	/* Setup io struct */
	io->sockfd = sockfd;

	return 1;
}

/*
 * Close socket IO.
 */
static void SocketIO_Close(ILDebuggerIO *io)
{
	ILSysIOSocketClose(io->sockfd);
}

/*
 * Recieve input data to input stream using socket IO.
 */
static int SocketIO_Recieve(ILDebuggerIO *io)
{
	char buffer[128];
	int len;

	/* Rewind input stream */
	fseek(io->input, 0, SEEK_SET);

	/* Read data from socket and write to the input stream */
	do
	{
		len = ILSysIOSocketReceive(io->sockfd, (void *)buffer, 128, 0);
		if(len < 0)
		{
			perror("debugger recieve");
			return 0;
		}
		if(len == 0)
		{
			fputs("debugger recieve: connection shutdown\n", stderr);
			return 0;
		}
		fwrite(buffer, 1, len, io->input);
	} while(len == 128 || len == 0 || buffer[len-1] != 0);

	return 1;
}

/*
 * Send data in output stream using socket IO.
 */
static int SocketIO_Send(ILDebuggerIO *io)
{
	int len;
	int count;
	char buffer[1024];

	/* Terminate output with 0 */
	putc(0, io->output);

	/* Remeber output length */
	len = (int) ftell(io->output);

	/* Rewind output stream */
	fseek(io->output, 0, SEEK_SET);

	/* Read data from output stream and send them to socket */
	do
	{
		if(len > 1024)
		{
			count = 1024;
		}
		else
		{
			count = len;
		}
		if(fread((void *)buffer, 1, count, io->output) != count)
		{
			perror("debugger send");
			return 0;
		}
		if(ILSysIOSocketSend(io->sockfd, buffer, count, 0) < 0)
		{
			perror("debugger send");
			return 0;
		}
		len -= 1024;
	}
	while(len > 0);

	/* Rewind output stream for next command */
	fseek(io->output, 0, SEEK_SET);

	return 1;
}

/*
 * Common recieving function std and file IO.
 */
static int StreamRecieve(ILDebuggerIO *io, FILE *stream, int isStd)
{
	int ch;

	/* Rewind input stream */
	fseek(io->input, 0, SEEK_SET);

	/* Display cursor */
	if(isStd)
	{
		fputs("\n> ", stdout);
	}

	/* Read line */
	for(;;)
	{
		ch = fgetc(stream);

		if(ch == EOF && !isStd)
		{
			fseek(stream, 0, SEEK_SET);		// rewind input
		}
		else if(ch != '\n')
		{
			fputc(ch, io->input);
			continue;
		}
		return 1;
	}
}

/*
 * Common sending function for std and file IO.
 */
static int StreamSend(ILDebuggerIO *io, FILE *stream, int isStd)
{
	int ch;

	/* Terminate output with 0 */
	putc(0, io->output);

	/* Rewind output stream */
	fseek(io->output, 0, SEEK_SET);

	/* Read data from output stream and send it out */
	while((ch = fgetc(io->output)) != 0)
	{
		fputc(ch, stream);
	}

	/* Rewind output stream for next command */
	fseek(io->output, 0, SEEK_SET);

	return 1;
}

/*
 * Open stdio IO.
 */
static int StdIO_Open(ILDebuggerIO *io, const char *connectionString)
{
	return 1;
}

/*
 * Close std IO.
 */
static void StdIO_Close(ILDebuggerIO *io)
{
	/* Nothing to do here */
}

/*
 * Recieve input data from input file.
 */
static int StdIO_Recieve(ILDebuggerIO *io)
{
	return StreamRecieve(io, stdin, 1);
}

/*
 * Send data in output stream out.
 */
static int StdIO_Send(ILDebuggerIO *io)
{
	return StreamSend(io, stdout, 1);
}

/*
 * Return next trace command.
 */
static int TraceIO_Recieve(ILDebuggerIO *io)
{
	const char *ch;
	static const char *commands = "show_position\nstep\nshow_locals";

	/* Rewind input stream */
	fseek(io->input, 0, SEEK_SET);

	/* Make ch point to command to be executed */
	if(io->data1 == 0)
	{
		ch = commands;
	}
	else
	{
		ch = (const char *) io->data1;
	}

	/* Output command to io->input */
	for(;;)
	{
		if(*ch == 0)
		{
			ch = commands;
			break;
		}
		if(*ch == '\n')
		{
			ch++;
			break;
		}
		fputc(*ch, io->input);
		fputc(*ch, stdout);
		ch++;
	}

	io->data1 = (void *) ch;
	fputc('\n', stdout);

	return 1;
}

/*
 * Return next argument for current command.
 * Call with prev=0 to obtain first argument.
 * Returns 0 after last argument is reached.
 */
static char *NextArg(ILDebugger *debugger, char *prev)
{
	if(prev == 0)
	{
		prev = debugger->cmd;
	}

	if(prev == debugger->lastArg)
	{
		return 0;
	}

	while(*prev != 0)
	{
		prev++;
	}
	prev++;

	return prev;
}

/*
 * Return first argument for current command.
 */
static char *FirstArg(ILDebugger *debugger)
{
	return NextArg(debugger, 0);
}

/*
 * Return first and second argument for current command.
 */
static void First2Args(ILDebugger *debugger, char **arg1, char **arg2)
{
	*arg1 = FirstArg(debugger);
	if(arg1)
	{
		*arg2 = NextArg(debugger, *arg1);
	}
	else
	{
		*arg2 = 0;
	}
}

/*
 * Check if we are watching given image.
 */
static int IsImageWatched(ILAssemblyWatch *watch, ILImage *image)
{
	if(watch == 0)
	{
		return 1;	/* watch all assemblies by default */
	}
	while(watch)
	{
		if(watch->image == image)
		{
			return 1;
		}
		watch = watch->next;
	}
	return 0;
}

int ILDebuggerIsAssemblyWatched(ILDebugger *debugger, ILMethod *method)
{
	return IsImageWatched(debugger->assemblyWatches,
									ILClassToImage(ILMethod_Owner(method)));
}

/*
 * Create entries for user data table.
 * Return 0 if out of memory.
 */
static ILUserDataEntry *ILUserDataCreateEntries(int size)
{
	ILUserDataEntry *entries;

	entries = (ILUserDataEntry *) ILMalloc(sizeof(ILUserDataEntry) * size);

	if(entries)
	{
		ILMemSet(entries, 0, sizeof(ILUserDataEntry) * size);
	}
	return entries;
}

/*
 * Create user data table with default size.
 * Return 0 if out of memory.
 */
static ILUserData *ILUserDataCreate()
{
	ILUserData *userData;

	userData = (ILUserData *) ILMalloc(sizeof(ILUserData));
	if(userData == 0)
	{
		return 0;
	}

	userData->count = 0;
	userData->size = IL_USER_DATA_TABLE_INIT_SIZE;
	userData->shiftsMax = 0;
	userData->entries = ILUserDataCreateEntries(userData->size);

	if(userData->entries)
	{
		return userData;
	}
	else
	{
		ILFree(userData);
		return 0;
	}
}

/*
 * Destroy user data table.
 */
static void ILUserDataDestroy(ILUserData *userData)
{
	ILFree(userData->entries);
	ILFree(userData);
}

#define ILUserDataStartIndex(userData, ptr, type) \
								(((ILNativeUInt)(ptr + type)) % userData->size)

/*
 * Find entry of given type for given pointer.
 * Return 0 if such entry does not exist.
 */
static ILUserDataEntry *ILUserDataFindEntry(ILUserData *userData,
											const void *ptr, int type)
{
	int i = 0;
	ILUserDataEntry *entry;
	ILNativeUInt index;

	/* Search for given pointer and type in entries */
	index = ILUserDataStartIndex(userData, ptr, type);
	while(1)
	{
		entry = userData->entries + index;
		if(entry->ptr == ptr && entry->type == type)
		{
			return entry;
		}
		if(++i > userData->shiftsMax)
		{
			return 0;		/* Entry not found */
		}
		index = (index + 1) % userData->size;
	}
}

/*
 * Assign data of given type to given pointer.
 * Return 0 if out of memory.
 */
static int ILUserDataSet(ILUserData *userData, const void *ptr, int type,
						 void *data)
{
	ILUserDataEntry *entry;
	ILUserDataEntry *oldEntries;
	ILUserDataEntry *newEntries;
	int oldSize;
	int newSize;
	int i;
	ILNativeUInt index;

	/* Do we have enough space? */
	if(userData->size < userData->count * 2)
	{
		/* Double the size */
		oldEntries = userData->entries;
		oldSize = userData->size;

		newSize = oldSize * 2 + 1;
		newEntries = ILUserDataCreateEntries(newSize);

		if(newEntries == 0)
		{
			return 0;
		}

		userData->count = 0;
		userData->shiftsMax = 0;
		userData->size = newSize;
		userData->entries = newEntries;

		/* Hash old entries into new table */
		for(i = 0; i < oldSize; i++)
		{
			entry = oldEntries + i;
			if(entry->ptr)
			{
				ILUserDataSet(userData, entry->ptr, entry->type, entry->data);
			}
		}
	}

	/* Find and change existing entry */
	entry = ILUserDataFindEntry(userData, ptr, type);
	if(!entry)
	{
		/* Find free entry and update maxShifts */
		index = ILUserDataStartIndex(userData, ptr, type);
		for(i = 0;; i++)
		{
			entry = userData->entries + index;
			if(entry->ptr == 0)
			{
				if(i > userData->shiftsMax)
				{
					userData->shiftsMax = i;
				}
				break;
			}
			index = (index + 1) % userData->size;
		}

		entry->ptr = ptr;
		entry->type = type;
	}

	/* Set data */
	entry->data = data;

	/* Icrease count */
	userData->count++;
	return 1;
}

/*
 * Set user with out of memory check.
 */
static void SetUserData(ILDebugger *debugger, const void *ptr, int type,
						void *data)
{
	if(!ILUserDataSet(debugger->userData, ptr, type, data))
	{
		ILDbOutOfMemory();
	}
}

/*
 * Return data of given type associated with given pointer.
 */
static void *GetUserData(ILDebugger *debugger, const void *ptr, int type)
{
	ILUserDataEntry *entry;

	entry = ILUserDataFindEntry(debugger->userData, ptr, type);

	if(entry != 0)
	{
		return entry->data;
	}
	else
	{
		return 0;
	}
}

/*
 * Return id assigned to given pointer.
 * If poineter has no id assigned then return and assign next free negative id.
 */
static int GetId(ILDebugger *debugger, const void *ptr, int type)
{
	ILNativeInt id = (ILNativeInt) GetUserData(debugger, ptr, type);
	if(id == 0)
	{
		id = --(debugger->minId);
		SetUserData(debugger, ptr, type, (void *)id);
	}
	return id;
}

/*
 * Get method's location.
 */
const char *GetLocation(ILMethod *method, ILInt32 offset,
						ILUInt32 *line, ILUInt32 *col)
{
	ILDebugContext *dbgc;
	const char *result;

	/* Dump empty location if the method's image
	 * does not have any debug information */
	if(ILDebugPresent(ILProgramItem_Image(method)))
	{
		/* Get the symbol debug information */
		if((dbgc = ILDebugCreate(ILProgramItem_Image(method))) == 0)
		{
			ILDbOutOfMemory();
		}
		result = ILDebugGetLineInfo(dbgc, ILMethod_Token(method),
									(ILUInt32)offset,
									line, col);
		ILDebugDestroy(dbgc);

		if(result)
		{
			/* Everything ok */
			return result;
		}
	}

	/* No debug info available */
	*line = 0;
	*col = 0;
	return 0;
}

/*
 * Dump a string, with XML quoting.
 */
static void DumpString(const char *str, FILE *stream)
{
	int ch;
	if(!str)
	{
		return;
	}
	while((ch = *str++) != '\0')
	{
		if(ch == '<')
		{
			fputs("&lt;", stream);
		}
		else if(ch == '>')
		{
			fputs("&gt;", stream);
		}
		else if(ch == '&')
		{
			fputs("&amp;", stream);
		}
		else if(ch == '"')
		{
			fputs("&quot;", stream);
		}
		else if(ch == '\'')
		{
			fputs("&apos;", stream);
		}
		else
		{
			putc(ch, stream);
		}
	}
}

 /*
 * Indent a line by a specific amount.
 */
static void Indent(FILE *stream, int indent)
{
	static char const spaces[] = "                ";
	while(indent >= 16)
	{
		fwrite(spaces, 1, 16, stream);
		indent -= 16;
	}
	if(indent > 0)
	{
		fwrite(spaces, 1, indent, stream);
	}
}

/*
 * Dump error message to outgoing buffer.
 */
static void DumpError(const char *message, FILE *stream)
{
	fputs("<ErrorMessage>", stream);
	fputs(message, stream);
	fputs("</ErrorMessage>", stream);
}

/*
 * Dump "out of memory" error.
 */
static void DumpOutOfMemoryError(FILE *stream)
{
	DumpError("out of memory", stream);
}

/*
 * Dump "parameter missing" error.
 */
static void DumpParamMissingError(FILE *stream)
{
	DumpError("parameter missing", stream);
}

/*
 * Dump message to outgoing buffer.
 */
static void DumpMessage(const char *message, FILE *stream)
{
	fputs("<Message>", stream);
	DumpString(message, stream);
	fputs("</Message>", stream);
}

/*
 * Dump xml node for source file.
 */
static void DumpSourceFile(ILDebugger *debugger, FILE *stream,
						   const char *path, int indent)
{
	int sourceFileId;
	int imageId;

	if(path)
	{
		Indent(stream, indent);

		sourceFileId = GetId(debugger, (void *) path,
												IL_USER_DATA_SOURCE_FILE_ID);

		imageId = GetId(debugger, (void *) path,
											IL_USER_DATA_SOURCE_FILE_IMAGE_ID);

		fprintf(stream,
						"<SourceFile Id=\"%d\" ProjectId=\"%d\" DebugPath=\"",
														sourceFileId, imageId);

		DumpString(path, stream);

		fputs("\" />\n", stream);
	}
}

/*
 * Dump current location. If debug info not available, dump empty location.
 */
static void DumpLocation(ILDebugger *debugger, FILE *stream,
						 const char *sourceFile, ILUInt32 line,ILUInt32 col,
						 int indent)
{
	int sourceFileId;

	Indent(stream, indent);
	fprintf(stream, "<Location Linenum=\"%d\" Col=\"%d\"", line, col);
	if(sourceFile)
	{
		sourceFileId = GetId(debugger, (void *) sourceFile,
											IL_USER_DATA_SOURCE_FILE_ID);

		fprintf(stream, " SourceFileId=\"%d\"", sourceFileId);
	}
	fputs(" />\n", stream);
}

/*
 * Generate documentation for a specific method definition.
 */
static void DumpMethod(ILDebugger *debugger, FILE *stream, ILMethod *method,
					   int dumpSignature, int indent)
{
	ILType *signature;
	int isConstructor;
	ILUInt32 line;
	ILUInt32 col;
	const char *sourceFile;
	int memberId;
	int classId;

	/* Bail out on null */
	if(!method)
	{
		return;
	}

	/* Get member and class id */
	memberId = GetId(debugger, (void *) method, IL_USER_DATA_MEMBER_ID);
	classId = GetId(debugger, (void *) ILMethod_Owner(method),
													IL_USER_DATA_CLASS_ID);

	/* Output the member header */
	Indent(stream, indent);
	fprintf(stream, "<Member Id=\"%d\" MemberName=\"", memberId);
	DumpString(ILMethod_Name(method), stream);
	fprintf(stream, "\" TypeId=\"%d\">\n", classId);

	/* Is this method a constructor? */
	isConstructor = (!strcmp(ILMethod_Name(method), ".ctor") ||
	   				 !strcmp(ILMethod_Name(method), ".cctor"));

	/* Output the signature in ILASM form */
	if(dumpSignature)
	{
		Indent(stream, indent + 2);
		fputs("<MemberSignature Language=\"ILASM\" Value=\"", stream);
		if(!isConstructor)
		{
			fputs(".method ", stream);
		}
		ILDumpFlags(stream, ILMethod_Attrs(method), ILMethodDefinitionFlags,
																			0);
		signature = ILMethod_Signature(method);
		ILDumpMethodType(stream, ILProgramItem_Image(method), signature,
						IL_DUMP_XML_QUOTING, 0, ILMethod_Name(method), method);
		putc(' ', stream);
		ILDumpFlags(stream, ILMethod_ImplAttrs(method),
					ILMethodImplementationFlags, 0);
		fputs("\"/>\n", stream);
	}

	/* Output the member kind */
	Indent(stream, indent + 2);
	if(isConstructor)
	{
		fputs("<MemberType>Constructor</MemberType>\n", stream);
	}
	else
	{
		fputs("<MemberType>Method</MemberType>\n", stream);
	}

	/* Output members location */
	sourceFile = GetLocation(method, 0, &line, &col);
	if(sourceFile)
	{
		DumpLocation(debugger, stream, sourceFile, line, col, indent + 2);
	}

	Indent(stream, indent);
	fputs("</Member>\n", stream);
}

/*
 * Dump position of execution.
 */
void DumpExecPosition(ILDebugger *debugger, FILE *stream, ILMethod *method,
							ILUInt32 offset, void *pc, const char *sourceFile,
							ILUInt32 line, ILUInt32 col, int indent)
{
	int memberId;
	memberId = GetId(debugger, (void *) method, IL_USER_DATA_MEMBER_ID);

	Indent(stream, indent);
	fprintf(stream, "<ExecPosition Offset=\"%d\" MemberId=\"%d\"",
															offset, memberId);
	if(pc != 0)
	{
		fprintf(stream, " PC=\"%p\"", pc);
	}
	fputs(">\n", stream);

	if(memberId < 0)
	{
		DumpMethod(debugger, stream, method, 1, indent + 2); 
	}
	DumpLocation(debugger, stream, sourceFile, line, col, indent + 2);
	Indent(stream, indent);
	fputs("</ExecPosition>", stream);
}

/*
 * Dump breakpoint.
 */
void DumpBreakpoint(ILDebugger *debugger, FILE *stream,
					ILBreakpoint *breakpoint, int indent)
{
	Indent(stream, indent);
	fprintf(stream, "<Breakpoint Id=\"%d\" >\n", breakpoint->id);
	DumpExecPosition(debugger,
							stream,
							breakpoint->method,
							breakpoint->offset,
							0,
							breakpoint->sourceFile,
							breakpoint->line,
							breakpoint->col, indent + 2);
	Indent(stream, indent);
	fputs("\n</Breakpoint>", stream);
}

#ifdef IL_USE_CVM
/*
 * Dump call frame (used only with CVM coder.)
 */
static void DumpCallFrame(FILE *stream, ILDebugger *debugger,
						  ILCallFrame *frame, int indent)
{
	unsigned char *start;
	ILInt32 offset;
	ILUInt32 line;
	ILUInt32 col;
	const char *sourceFile;

	if(frame->method && frame->pc != IL_INVALID_PC)
	{
		/* Find the start of the frame method */
		start = (unsigned char *)ILMethodGetUserData(frame->method);
		if(ILMethodIsConstructor(frame->method))
		{
			start -= ILCoderCtorOffset(debugger->process->coder);
		}

		offset = (ILInt32)ILCoderGetILOffset
				(debugger->process->coder, (void *)start,
				(ILUInt32)(frame->pc - start), 0);

		/* Read current position from debug info */
		sourceFile = GetLocation(frame->method, offset, &line, &col);
	}
	else
	{
		/* Probably a native method that does not have offsets */
		offset = -1;
		sourceFile = 0;
		line = 0;
		col = 0;
	}

	/* Dump the frame */
	DumpExecPosition(debugger, stream, frame->method,
						offset, 0, sourceFile,
						line, col, indent);
}
#endif	// IL_USE_CVM

/*
 * Create new ILDebuggerThreadInfo.
 */
ILDebuggerThreadInfo *ILDebuggerThreadInfo_Create()
{
	ILDebuggerThreadInfo *result;
	result = (ILDebuggerThreadInfo *) ILMalloc(sizeof(ILDebuggerThreadInfo));
	if(result == 0)
	{
		ILDbOutOfMemory();
	}
	memset(result, 0, sizeof(ILDebuggerThreadInfo));

	/* Initialize handle for recieving events from IO thread */
	result->event = ILWaitEventCreate(0, 0);
	if(result->event == 0)
	{
		ILDbOutOfMemory();
	}

	result->runType = IL_DEBUGGER_RUN_TYPE_CONTINUE;
	return result;
}

/*
 * Free thread info list.
 */
 void ILDebuggerThreadInfo_Destroy(ILDebuggerThreadInfo *info)
 {
	ILDebuggerThreadInfo *next;
	
	while(info)
	{
		next = info->next;
#ifdef IL_USE_JIT
		if(info->jitStackTrace)
		{
			jit_stack_trace_free(info->jitStackTrace);
		}
#endif
		if(info->event)
		{
			ILWaitHandleClose(info->event);
		}
		ILFree(info);
		info = next;
	}
 }

/*
 * Get debugger info associated with given exec thread.
 * If not found return free info.
 */
ILDebuggerThreadInfo *GetDbThreadInfo(ILDebugger *debugger,
									  ILExecThread *thread)
{
	ILDebuggerThreadInfo *info;

	/* Search for existing info */
	info = debugger->dbthread;
	do
	{
		if(info->execThread == thread)
		{
			return info;
		}
		info = info->next;
	} while(info);

	/* Search for free slot */
	info = debugger->dbthread;
	for(;;)
	{
		if(info->runType == IL_DEBUGGER_RUN_TYPE_CONTINUE)
		{
			return info;
		}
		if(info->next == 0)
		{
			/* Append new info to the end of list */
			info->next = ILDebuggerThreadInfo_Create();
			info->next->id = info->id + 1;
			return info->next;
		}
		info = info->next;
	}
}

/*
 * Update info about state of execution thread.
 * This function is called to remeber various info
 * when thread stops debugger.
 */
static void UpdateDbThreadInfo(ILDebuggerThreadInfo *info, void *userData,
								ILExecThread *thread, ILMethod *method,
								ILInt32 offset, int type,
								const char *sourceFile,
								ILUInt32 line, ILUInt32 col)
{
	info->userData = userData;
	info->execThread = thread;
	info->method = method;
	info->offset = offset;
	info->type = type;
	info->sourceFile = sourceFile;
	info->line = line;
	info->col = col;
	info->numFrames = _IL_StackFrame_InternalGetTotalFrames(thread);

#ifdef IL_USE_JIT
	/* Free previous jit stack trace */
	if(info->jitStackTrace)
	{
		jit_stack_trace_free(info->jitStackTrace);
	}
	/* Set current jit stack trace */
	info->jitStackTrace = jit_exception_get_stack_trace();
#endif
}

/*
 * Create new breakpoint.
 */
ILBreakpoint *ILBreakpoint_Create()
{
	ILBreakpoint *result;
	result = (ILBreakpoint *) ILMalloc(sizeof(ILBreakpoint));
	if(result == 0)
	{
		ILDbOutOfMemory();
	}
	memset(result, 0, sizeof(ILBreakpoint));
	return result;
}

/*
 * Free breakpoint list.
 */
 void DebuggerBreakpoint_Destroy(ILBreakpoint *breakpoint)
 {
	ILBreakpoint *next;

	while(breakpoint)
	{
		next = breakpoint->next;
		ILFree(breakpoint);
		breakpoint = next;
	}
 }

 /*
 * Get breakpoint in given method and given offset interval.
 * If not found return free breakpoint.
 */
ILBreakpoint *GetBreakpoint(ILDebugger *debugger, ILMethod *method,
							ILUInt32 offset)
{
	ILBreakpoint *breakpoint;

	/* Search for existing breakpoint */
	breakpoint = debugger->breakpoint;
	do
	{
		if(breakpoint->method == method && breakpoint->offset == offset)
		{
			return breakpoint;
		}
		breakpoint = breakpoint->next;
	} while(breakpoint);

	/* Search for free slot */
	breakpoint = debugger->breakpoint;
	for(;;)
	{
		if(breakpoint->method == 0)
		{
			return breakpoint;
		}
		if(breakpoint->next == 0)
		{
			/* Append new breakpoint to the end of the list */
			breakpoint->next = ILBreakpoint_Create();
			breakpoint->next->id = breakpoint->id + 1;
			return breakpoint->next;
		}
		breakpoint = breakpoint->next;
	}
}

static void DestroyAssemblyWatches(ILDebugger *debugger)
{
	ILAssemblyWatch *watch;
	while(debugger->assemblyWatches)
	{
		watch = debugger->assemblyWatches;
		debugger->assemblyWatches = debugger->assemblyWatches->next;
		ILFree(watch);
	}
}

/*
 * Find method for given source file and line.
 */
ILMethod *FindMethodByLocation(ILDebugger *debugger, const char *sourceFile,
								ILUInt32 reqLine, ILUInt32 *outLine,
								ILUInt32 *outCol, ILUInt32 *outOffset,
								const char **outFile)
{
	ILContext *context;
	ILImage *image;
	ILDebugContext *dbgc = 0;
	ILMethod *method;
	const char *filename;
	ILUInt32 line;
	ILUInt32 col;
	ILUInt32 offset;
	ILInt32 delta;
	ILInt32 bestDelta;
	ILUInt32 bestOffset;
	ILUInt32 bestLine;
	ILUInt32 bestCol;
	ILMethod *bestMethod;
	ILMethodCode methodCode;

	/* Initialize searching */
	bestDelta = IL_MAX_INT32;
	bestOffset = 0;
	bestMethod = 0;
	bestLine = 0;
	bestCol = 0;
	
	/* Iterate all images */
	context = ILExecProcessGetContext(debugger->process);
	image = 0;
	while((image = ILContextNextImage(context, image)) != 0)
	{
		/* Get debug context for image */
		if(!ILDebugPresent(image))
		{
			continue;
		}
		if((dbgc = ILDebugCreate(image)) == 0)
		{
			continue;
		}
		/* Iterate all methods */
		method = 0;
		while((method = (ILMethod *)ILImageNextToken
					(image, IL_META_TOKEN_METHOD_DEF, (void *)method)) != 0)
		{
			/* Find best matching offset in method */
			if(ILMethodGetCode(method, &methodCode))
			{
				for(offset = 0; offset < methodCode.codeLen; offset++)
				{
					/* Get debug info for current offset */
					filename = ILDebugGetLineInfo(dbgc,
														ILMethod_Token(method),
												 		offset, &line, &col);

					/* There is no line debug info for this offset */
					if(!filename)
					{
						continue;
					}
					/* Try next method if not in requested file */
					if(ILStrICmp(filename, sourceFile) != 0)
					{
						break;
					}
					delta = abs(reqLine - line);
					if(delta >= bestDelta)
					{
						continue;
					}
					/* Update the best debug info */
					bestMethod = method;
					bestDelta = delta;
					bestOffset = offset;
					bestLine = line;
					bestCol = col;
					*outFile = filename;
				}
			}
		}
	}
	/* No method found in given source file */
	if(bestMethod == 0)
	{
		return 0;
	}
	/* Fill output info and return method */
	*outLine = bestLine;
	*outCol = bestCol;
	*outOffset = bestOffset;

	return bestMethod;
}

/*
 * set_id command.
 */
void SetId(ILDebugger *debugger, FILE *stream)
{
	char *arg = 0;
	ILNativeInt oldId;
	ILNativeInt newId;
	int size;
	ILUserDataEntry *entry;
	int found = 0;
	int missingArg = 0;

	while(1)
	{
		arg = NextArg(debugger, arg);
		if(arg == 0)
		{
			break;
		}
		oldId = atoi(arg);

		arg = NextArg(debugger, arg);
		if(arg == 0)
		{
			missingArg = 1;
			break;
		}
		newId = atoi(arg);

		entry = debugger->userData->entries;
		for(size = debugger->userData->size; size > 0; size--)
		{
			/* Find matching id */
			if(entry->data == (void *) oldId)
			{
				/* Set new id when types match */
				if(entry->type == IL_USER_DATA_SOURCE_FILE_ID ||
					entry->type == IL_USER_DATA_SOURCE_FILE_IMAGE_ID ||
					entry->type == IL_USER_DATA_MEMBER_ID ||
					entry->type == IL_USER_DATA_CLASS_ID ||
					entry->type == IL_USER_DATA_IMAGE_ID)
				{
					entry->data = (void *) newId;
					found = 1;
				}
			}
			entry++;
		}

		if(!found)
		{
			break;
		}
	}

	if(missingArg)
	{
		DumpParamMissingError(stream);
	}
	else if(found)
	{
		DumpMessage("ok", stream);
	}
	else
	{
		DumpError("id not found", stream);
	}
}

/*
 * Insert, remove or toggle breakpoint. Actions 0, 1, 2
 */
void HandleBreakpointCommand(ILDebugger *debugger, FILE *stream, int command)
{
	char *arg0;
	char *arg1;
	ILUInt32 reqLine;
	ILMethod *method;
	ILUInt32 offset;
	ILUInt32 line;
	ILUInt32 col;
	const char *sourceFile;
	ILBreakpoint *breakpoint;

	/* Validate and parse arguments */
	First2Args(debugger, &arg0, &arg1);
	if(arg1 == 0)
	{
		DumpParamMissingError(stream);
		return;
	}

	reqLine = atoi(arg1);

	/* Find method for given source file and line */
	method = FindMethodByLocation(debugger, arg0, reqLine,
											&line, &col, &offset, &sourceFile);

	if(!method)
	{
		DumpError("Location not found, wrong params or debug info missing.",
																	stream);
		return;
	}

	/* Get breakpoint info - empty or existing */
	breakpoint = GetBreakpoint(debugger, method, offset);

	
	if(breakpoint->method)
	{
		/* Breakpoint exists */
		if(command == 0)
		{
			/* On insert */
			DumpError("breakpoint already exists", stream);
		}
		else
		{
			/* On remove or toggle */
			DumpBreakpoint(debugger, stream, breakpoint, 2);
			breakpoint->method = 0;
			ILExecProcessUnwatchMethod(debugger->process, method);
			DumpMessage("removed", stream);
		}
	}
	else
	{
		/* Breakpoint does not exist */
		if(command == 1)
		{
			/* On remove */
			DumpError("Breakpoint not found", stream);
		}
		else
		{
			/* Fill info */
			breakpoint->method = method;
			breakpoint->offset = offset;
			breakpoint->sourceFile = sourceFile;
			breakpoint->line = line;
			breakpoint->col = col;

			/* Dump breakpoint - it's usefull because location
			   may not be exactly the the same as user wanted */
			DumpBreakpoint(debugger, stream, breakpoint, 2);
			DumpMessage("inserted", stream);
		}
	}
}

/*
 * insert_breakpoint command.
 */
void InsertBreakpoint(ILDebugger *debugger, FILE *stream)
{
	HandleBreakpointCommand(debugger, stream, 0);
}

/*
 * remove_breakpoint command.
 * TODO: remove breakpoint by id
 */
void RemoveBreakpoint(ILDebugger *debugger, FILE *stream)
{
	HandleBreakpointCommand(debugger, stream, 1);
}

/*
 * toggle_breakpoint command.
 */
void ToggleBreakpoint(ILDebugger *debugger, FILE *stream)
{
	HandleBreakpointCommand(debugger, stream, 2);
}

/*
 * break command.
 */
void Break(ILDebugger *debugger, FILE *stream)
{
	/* Set stop flag */
	debugger->breakAll = 1;
}

/*
 * Call DebuggerHelper.ExpressionToString() method for given expression.
 * Returns result as GC allocated string.
 */
char *DebuggerHelper_ExpressionToString(ILExecThread *thread,
										const char *expression)
{
	ILString *str;
	char *result;

	str = ILStringCreateUTF8(thread, expression);
	if(!str)
	{
		return 0;
	}

	ILExecThreadCallNamed(thread, "System.Private.DebuggerHelper",
					"ExpressionToString", "(oSystem.String;)oSystem.String;",
					&str, str);

	result = ILStringToUTF8(thread, str);
	return result;
}

/*
 * Call DebuggerHelper.SetMethodOwner() method.
 */
void DebuggerHelper_SetMethodOwner(ILExecThread *thread, ILType *type)
{
	ILObject *clrType;

	clrType = _ILGetClrTypeForILType(thread, type);

	ILExecThreadCallNamed(thread, "System.Private.DebuggerHelper",
					"SetMethodOwner", "(oSystem.Type;)V",
					(void *)0, clrType);

}

/*
 * Call DebuggerHelper.ClearLocals() method.
 */
void DebuggerHelper_ClearLocals(ILExecThread *thread)
{
	ILExecThreadCallNamed(thread, "System.Private.DebuggerHelper",
					"ClearLocals", "()V", (void *)0);
}

/*
 * Call DebuggerHelper.AddLocal() method.
 */
void DebuggerHelper_AddLocal(ILExecThread *thread, const char *name,
							 ILType *type, void *ptr)
{
	ILString *str;
	ILObject *obj;
	ILObject *clrType;

	str = ILStringCreateUTF8(thread, name);

	if(ILType_IsPrimitive(type) || ILType_IsValueType(type))
	{
		obj = ILExecThreadBox(thread, type, ptr);
	}
	else
	{
		obj = *(ILObject **)(ptr);

		/* Box if class inherits from value type (e.g. struct) */
		if(ILType_IsClass(type) && ILClassIsValueType(ILType_ToClass(type)))
		{
			obj = ILExecThreadBox(thread,
									ILType_FromValueType(ILType_ToClass(type)),
																(void *) obj);
		}
	}

	clrType = _ILGetClrTypeForILType(thread, type);

	ILExecThreadCallNamed(thread, "System.Private.DebuggerHelper",
				"AddLocal", "(oSystem.String;oSystem.Type;oSystem.Object;)V",
				(void *)0, str, clrType, obj);

}

/*
 * Call DebuggerHelper.ShowLocals() method.
 * Returns result as GC allocated string.
 */
char *DebuggerHelper_ShowLocals(ILExecThread *thread)
{
	char *result;
	ILString *str;

	ILExecThreadCallNamed(thread, "System.Private.DebuggerHelper",
					"ShowLocals", "()oSystem.String;",
					&str);

	result = ILStringToUTF8(thread, str);
	return result;
}

static void UpdateThis(ILExecThread *thread, ILMethod *method, void *addr,
					   ILUInt32 *paramDebugIndex)
{
	ILType *type;

	type = ILType_FromClass(ILMethod_Owner(method));
	DebuggerHelper_AddLocal(thread, "this", type, addr);
	(*paramDebugIndex)++;
}

static void UpdatePram(ILExecThread *thread, ILMethod *method,
					   ILUInt32 *currentParam, ILUInt32 offset, void *addr,
					   ILUInt32 *paramDebugIndex, ILType *paramSignature,
					   ILDebugContext *dbgc)
{
	ILType *type;
	const char *name = 0;

	type = ILTypeGetParam(paramSignature, *currentParam);
	if(dbgc)
	{
		name = ILDebugGetVarName(dbgc, ILMethod_Token(method), offset,
											(*paramDebugIndex) | 0x80000000);
	}
	if(ILType_IsComplex(type) && ILType_Kind(type) == IL_TYPE_COMPLEX_BYREF)
	{
		DebuggerHelper_AddLocal(thread, name, ILType_Ref(type),
														*((void **)(addr)));
	}
	else
	{
		DebuggerHelper_AddLocal(thread, name, type, addr);
	}
	(*currentParam)++;
	(*paramDebugIndex)++;
}

static void UpdateLocal(ILExecThread *thread, ILMethod *method,
						ILUInt32 *currentLocal, ILUInt32 offset, void *addr,
						ILType *localSignature, ILDebugContext *dbgc)
{
	ILType *type;
	const char *name = 0;

	type = ILTypeGetLocal(localSignature, *currentLocal);
	if(dbgc)
	{
		name = ILDebugGetVarName(dbgc, ILMethod_Token(method), offset,
																*currentLocal);
	}
	if(name || dbgc == 0)
	{
		DebuggerHelper_AddLocal(thread, name, type, addr);
	}
	(*currentLocal)++;
}

/*
 * Get the size of a type in stack words, taking float expansion into account.
 * TODO: this function is already defined in cvmc.c but there is no way how
 * to include it now.
 */
static ILUInt32 GetStackTypeSize(ILExecProcess *process, ILType *type)
{
	ILUInt32 size;
	if(type == ILType_Float32 || type == ILType_Float64)
	{
		return CVM_WORDS_PER_NATIVE_FLOAT;
	}
	else
	{
		size = _ILSizeOfTypeLocked(process, type);
	}
	return (size + sizeof(CVMWord) - 1) / sizeof(CVMWord);
}

/*
 * Update locals in DebuggerHelper class.
 */
static void UpdateLocals(FILE *stream, ILExecThread *thread, ILMethod *method,
						 ILUInt32 offset)
{
	ILMethodCode code;
	ILType *localSignature;
	ILType *paramSignature;
	ILUInt32 currentLocal;
	ILUInt32 currentParam;
	ILUInt32 paramDebugIndex;
	ILDebugContext *dbgc = 0;
	ILType *type;

#ifdef IL_USE_JIT
	ILUInt32 i;
	ILLocalWatch *watch;
#else
	CVMWord *p;
	int hasThis;
	ILUInt32 numParams;
	ILUInt32 numLocals;
#endif

	/* Clear locals in helper class */
	DebuggerHelper_ClearLocals(thread);

	/* Get locals signature */
	if(!ILMethodGetCode(method, &code))
	{
		DumpError("Unable to get method code", stream);
		return;
	}
	if(code.localVarSig)
	{
		localSignature = ILStandAloneSigGetType(code.localVarSig);
	}
	else
	{
		localSignature = 0;
	}

	/* Get params signature */
	paramSignature = ILMethod_Signature(method);

	/* Debug context for local variable and parameter names */
	if(ILDebugPresent(ILProgramItem_Image(method)))
	{
		/* Get the symbol debug information */
		if((dbgc = ILDebugCreate(ILProgramItem_Image(method))) == 0)
		{
			DumpOutOfMemoryError(stream);
			return;
		}
	}

	/* Iterate watches in current thread */
	currentLocal = 0;
	currentParam = 1;
	paramDebugIndex = 0;

#if IL_USE_JIT
	for(i = 0; i < thread->numWatches; i++)
	{
		watch = &(thread->watchStack[i]);

		/* Skip variables that are not in current frame */
		if(watch->frame != thread->frame)
		{
			continue;
		}

		if(watch->type == IL_LOCAL_WATCH_TYPE_LOCAL_VAR)
		{
			UpdateLocal(thread, method, &currentLocal, offset, watch->addr,
														localSignature, dbgc);
		}
		else if(watch->type == IL_LOCAL_WATCH_TYPE_PARAM)
		{
			UpdatePram(thread, method, &currentParam, offset, watch->addr,
									&paramDebugIndex, paramSignature, dbgc);
		}
		else if(watch->type == IL_LOCAL_WATCH_TYPE_THIS)
		{
			UpdateThis(thread, method, watch->addr, &paramDebugIndex);
		}
	}
#else
	p = thread->frame;
	hasThis = ILType_HasThis(paramSignature);

	/* Handle the "this" parameter as necessary */
	if(hasThis)
	{
		UpdateThis(thread, method, (void *) p++, &paramDebugIndex);
	}

	/* Regular arguments */
	numParams = ILTypeNumParams(paramSignature);

	while(currentParam <= numParams)
	{
		type = ILTypeGetEnumType(ILTypeGetParam(paramSignature, currentParam));

		UpdatePram(thread, method, &currentParam, offset, (void *) p,
									&paramDebugIndex, paramSignature, dbgc);

		p += GetStackTypeSize(ILExecThreadGetProcess(thread), type);
	}

	/* Local variables */
	if(localSignature != 0)
	{
		numLocals = ILTypeNumLocals(localSignature);
		while(currentLocal < numLocals)
		{
			type = ILTypeGetEnumType(ILTypeGetLocal(localSignature,
																currentLocal));

			UpdateLocal(thread, method, &currentLocal, offset, (void *) p,
														localSignature, dbgc);

			p += GetStackTypeSize(ILExecThreadGetProcess(thread), type);
		}
	}

#endif	// IL_USE_JIT

	if(dbgc)
	{
		ILDebugDestroy(dbgc);
	}

	/* Update method's owner type */
	type = ILType_FromClass(ILMethod_Owner(method));
	DebuggerHelper_SetMethodOwner(thread, type);
}

/*
 * show_locals command.
 */
void ShowLocals(ILDebugger *debugger, FILE *stream)
{
	char *str;

	/* Update locals in DebuggerHelper class */
	UpdateLocals(stream, debugger->dbthread->execThread,
			debugger->dbthread->method, (ILUInt32) debugger->dbthread->offset);

	str = DebuggerHelper_ShowLocals(debugger->dbthread->execThread);
	if(str)
	{
		if(*str == '<')
		{
			fputs(str, stream);
		}
		else
		{
			DumpError(str, stream);
		}
	}
	else
	{
		DumpOutOfMemoryError(stream);
	}
}

/*
 * add_watch command.
 */
void AddWatch(ILDebugger *debugger, FILE *stream)
{
	char *arg;
	ILDebuggerWatch *watch;
	ILDebuggerWatch *tail;

	arg = FirstArg(debugger);
	if(arg == 0)
	{
		DumpParamMissingError(stream);
		return;
	}
	
	/* Allocate new watch */
	if((watch = (ILDebuggerWatch *)ILMalloc(sizeof(ILDebuggerWatch))) == 0)
	{
		DumpOutOfMemoryError(stream);
		return;
	}

	/* Append the watch to the end of the list */
	tail = debugger->watches;
	if(tail != 0)
	{
		while(tail->next != 0)
		{
			tail = tail->next;
		}
		tail->next = watch;
	}
	else
	{
		debugger->watches = watch;
	}
	
	watch->expression = ILDupString(arg);
	watch->next = 0;

	DumpMessage("ok", stream);
}

/*
 * remove_watch command.
 */
void RemoveWatch(ILDebugger *debugger, FILE *stream)
{
	char *arg;
	ILDebuggerWatch *watch;
	ILDebuggerWatch *prev;

	arg = FirstArg(debugger);
	if(arg == 0)
	{
		DumpParamMissingError(stream);
		return;
	}

	/* Search the watch list for watch with given name */
	prev = 0;
	watch = debugger->watches;
	while(watch != 0)
	{
		if(ILStrICmp(watch->expression, arg) == 0)
		{
			if(prev != 0)
			{
				prev->next = watch->next;
			}
			else
			{
				debugger->watches = watch->next;
			}
			ILFree(watch->expression);
			ILFree(watch);
			DumpMessage("ok", stream);
			return;
		}
		prev = watch;
		watch = watch->next;
	}
	
	DumpMessage("watch not found", stream);
}

/*
 * Destroy the watch list.
 */
static void DestroyWatchList(ILDebuggerWatch *watch)
{
	ILDebuggerWatch *next;
	while(watch)
	{
		next = watch->next;
		ILFree(watch->expression);
		ILFree(watch);
		watch = next;
	}
}

/*
 * remove_all_watches command.
 */
void RemoveAllWatches(ILDebugger *debugger, FILE *stream)
{
	DestroyWatchList(debugger->watches);
	debugger->watches = 0;
	DumpMessage("ok", stream);
}

/*
 * show_watches command.
 */
void ShowWatches(ILDebugger *debugger, FILE *stream)
{
	ILDebuggerWatch *watch;
	char *value;

	/* Update locals in DebuggerHelper class */
	UpdateLocals(stream, debugger->dbthread->execThread,
			debugger->dbthread->method, (ILUInt32) debugger->dbthread->offset);

	fputs("<Watches>\n", stream);

	/* Iterate all watches and dump their values */
	watch = debugger->watches;
	while(watch != 0)
	{
		fputs("  <Watch Expression=\"", stream);
		DumpString(watch->expression, stream);
		fputs("\" Value=\"", stream);
		value = DebuggerHelper_ExpressionToString(
						debugger->dbthread->execThread, watch->expression);
		if(value)
		{
			DumpString(value, stream);
		}
		fputs("\"/>\n", stream);
		watch = watch->next;
	}

	fputs("</Watches>", stream);
}

/*
 * Update location in current thread.
 * Line and sourceFile are set to 0 if no debug info available.
 */
void UpdateLocation(ILDebugger *debugger)
{
	debugger->dbthread->sourceFile = GetLocation(debugger->dbthread->method,
										debugger->dbthread->offset,
										&(debugger->dbthread->line),
										&(debugger->dbthread->col));
	
}

/*
 * Dump current location. If debug info not available, dump empty location.
 */
void DumpCurrentLocation(ILDebugger *debugger, FILE *stream, int indent)
{
	/* Update location in current thread info */
	UpdateLocation(debugger);

	DumpLocation(debugger, stream,
					 debugger->dbthread->sourceFile,
					 debugger->dbthread->line,
					 debugger->dbthread->col,
					 indent);
}

/*
 * show_position command.
 */
void ShowPosition(ILDebugger *debugger, FILE *stream)
{
	UpdateLocation(debugger);
	DumpExecPosition(debugger, stream, debugger->dbthread->method,
								debugger->dbthread->offset,
								0,
								debugger->dbthread->sourceFile,
								debugger->dbthread->line,
								debugger->dbthread->col,
								0);
}

/*
 * show_stack_trace command.
 */
void ShowStackTrace(ILDebugger *debugger, FILE *stream)
{
#ifdef IL_USE_CVM
	ILCallFrame *frame;
	ILExecThread *thread = debugger->dbthread->execThread;

	/* Push the current frame data onto the stack temporarily
		so that we can dump it */
	if(thread->numFrames < thread->maxFrames)
	{
		frame = &(thread->frameStack[(thread->numFrames)++]);
	}
	else if((frame = _ILAllocCallFrame(thread)) == 0)
	{
		/* We ran out of memory trying to push the frame */
		DumpOutOfMemoryError(stream);
		return;
	}
	frame->method = thread->method;
	frame->pc = thread->pc;
	frame->frame = thread->frame;
	frame->permissions = 0;

	fputs("  <StackTrace>\n", stream);
	frame = _ILGetCallFrame(thread, 0);

	while(frame != 0)
	{
		/* Dump the frame */
		DumpCallFrame(stream, debugger, frame, 4);
		putc('\n', stream);

		/* Next frame */
		frame = _ILGetNextCallFrame(thread, frame);
	}

	/* Pop current frame, that was previously pushed for dumping */
	--(thread->numFrames);

	/* Dump xml footer */
	fputs("  </StackTrace>", stream);
#endif	// IL_USE_CVM

#ifdef IL_USE_JIT
	jit_stack_trace_t stackTrace;
	ILUInt32 size;
	ILUInt32 current;
	jit_context_t jitContext;
	ILJitFunction jitFunction;
	ILMethod *method;
	ILUInt32 offset;
	ILUInt32 line;
	ILUInt32 col;
	const char *sourceFile;
	void *pc;

	/* Get jit stack trace */
	stackTrace = debugger->dbthread->jitStackTrace;
	if(stackTrace == 0)
	{
		DumpError("error getting stack trace", stream);
		return;
	}

	/* Dump xml header */
	fputs("  <StackTrace>\n", stream);

	/* Get info for reading stack trace */
	size = jit_stack_trace_get_size(stackTrace);
	jitContext = ILJitGetContext(debugger->process->coder);

	/* Walk the stack and dump managed frames */
	for(current = 0; current < size; ++current)
	{
		jitFunction = jit_stack_trace_get_function(jitContext,
										stackTrace, current);
		if(jitFunction)
		{
			if((method = jit_function_get_meta
									(jitFunction, IL_JIT_META_METHOD)) != 0)
			{
				/* Get IL offset */
				offset = jit_stack_trace_get_offset(jitContext,
													stackTrace,
													current);

				/* Read current position from debug info */
				sourceFile = GetLocation(method, offset, &line, &col);
				pc = jit_stack_trace_get_pc(stackTrace, current);

				/* Dump the frame */
				DumpExecPosition(debugger, stream, method, offset, pc,
												sourceFile, line, col, 4);
			}
		}
	}

	/* Dump xml footer */
	fputs("\n  </StackTrace>", stream);
#endif	// IL_USE_JIT
}

/*
 * watch_assembly command.
 */
void WatchAssembly(ILDebugger *debugger, FILE *stream)
{
	char *arg;
	ILContext *context;
	ILImage *image;
	ILAssemblyWatch *watch;
	int found = 0;

	arg = FirstArg(debugger);
	if(arg == 0)
	{
		DumpParamMissingError(stream);
		return;
	}

	/* Search for image with given assembly name */
	context = ILExecProcessGetContext(debugger->process);
	image = 0;
	while((image = ILContextNextImage(context, image)) != 0)
	{
		if(strcmp(ILImageGetAssemblyName(image), arg) == 0)
		{
			found = 1;
			break;
		}
	}
	
	if(!found)
	{
		DumpError("assembly not found", stream);
		return;
	}
	
	/* Search the watch list for the image */
	watch = debugger->assemblyWatches;
	while(watch != 0)
	{
		if(watch->image == image)
		{
			++(watch->count);
			return;
		}
		watch = watch->next;
	}
	
	/* Add the image to the watch list in the thread safe way */
	if((watch = (ILAssemblyWatch *)ILMalloc(sizeof(ILAssemblyWatch))) == 0)
	{
		DumpOutOfMemoryError(stream);
		return;
	}

	watch->image = image;
	watch->count = 1;
	watch->next = debugger->assemblyWatches;
	debugger->assemblyWatches = watch;

	DumpMessage("ok", stream);
}

/*
 * unwatch_all_assemblies command.
 */
void UnwatchAllAssemblies(ILDebugger *debugger, FILE *stream)
{
	DestroyAssemblyWatches(debugger);
	DumpMessage("ok", stream);
}

/*
 * start_profiling command.
 */
void StartProfiling(ILDebugger *debugger, FILE *stream)
{
	debugger->profiling = 1;
	DumpMessage("ok", stream);
}

/*
 * stop_profiling command.
 */
void StopProfiling(ILDebugger *debugger, FILE *stream)
{
	debugger->profiling = 0;
	DumpMessage("ok", stream);
}

/*
 * clear_profiling command.
 */
void ClearProfiling(ILDebugger *debugger, FILE *stream)
{
	int size;
	ILUserDataEntry *entry;
	ILDebuggerThreadInfo *threadInfo;

	/* Iterate userData table and clear profiling related info */
	entry = debugger->userData->entries;
	for(size = debugger->userData->size; size > 0; size--)
	{
		if(entry->type == IL_USER_DATA_METHOD_TIME ||
			entry->type == IL_USER_DATA_METHOD_CALL_COUNT)
		{
			SetUserData(debugger, entry->ptr, entry->type, (void *) 0);
		}
		entry++;
	}

	/* Iterate all threads and clear profiling info */
	threadInfo = debugger->dbthread;
	while(threadInfo)
	{
		threadInfo->profilerLastMethod = 0;
		threadInfo = threadInfo->next;
	}

	DumpMessage("ok", stream);
}

/*
 * show_profiling command.
 */
void ShowProfiling(ILDebugger *debugger, FILE *stream)
{
	int size;
	ILUserDataEntry *entry;
	ILNativeUInt time;
	ILNativeUInt count;
	int memberId;

	fputs("<Profiling>\n", stream);

	/* Iterate userData table and dump profiling related info */
	entry = debugger->userData->entries;
	for(size = debugger->userData->size; size > 0; size--)
	{
		/* Case with entry->ptr==0 is valid and must be skipped */
		if(entry->type == IL_USER_DATA_METHOD_TIME && entry->ptr != 0)
		{
			time = (ILNativeUInt) entry->data;
			count = (ILNativeUInt) GetUserData(debugger, entry->ptr,
											   IL_USER_DATA_METHOD_CALL_COUNT);
			memberId = GetId(debugger, entry->ptr, IL_USER_DATA_MEMBER_ID);
			
			fprintf(stream,
					"  <ProfilingEntry MemberId=\"%d\" CallCount=\"%d\" "
					"Time=\"%d\"", memberId, (ILUInt32)count, (ILUInt32)time);

			if(memberId >= 0)
			{
				fputs(" />\n", stream);
			}
			else
			{
				fputs(" >\n", stream);
				DumpMethod(debugger, stream, (ILMethod *) entry->ptr, 0, 4);
				fputs("  </ProfilingEntry>\n", stream);
			}
		}
		entry++;
	}

	fputs("</Profiling>", stream);
}

/*
 * show_threads command.
 */
void ShowThreads(ILDebugger *debugger, FILE *stream)
{
	ILDebuggerThreadInfo *info;

	fputs("<DebuggerThreads>\n", stream);

	info = debugger->dbthread;
	while(info)
	{
		if(info->runType != IL_DEBUGGER_RUN_TYPE_STOPPED)
		{
			continue;
		}
		Indent(stream, 2);
		fprintf(stream, "<DebuggerThread Id=\"%d\" Address=\"%p\""
									" RunType=\"%d\" Current=\"%d\" />\n",
									info->id, info->execThread,
									info->runType, info == debugger->dbthread);
		info = info->next;
	}
	
	fputs("</DebuggerThreads>", stream);
}

/*
 * show_breakpoints command.
 */
void ShowBreakpoints(ILDebugger *debugger, FILE *stream)
{
	ILBreakpoint *breakpoint;

	fputs("<DebuggerBreakpoints>\n", stream);

	breakpoint = debugger->breakpoint;
	while(breakpoint)
	{
		/* Skip removed breakpoints */
		if(breakpoint->method == 0)
		{
			continue;
		}

		/* Dump the breakpoint and move to next */
		DumpBreakpoint(debugger, stream, breakpoint, 2);
		breakpoint = breakpoint->next;
	}

	fputs("</DebuggerBreakpoints>", stream);
}

/*
 * Return directory where image was linked.
 */
static const char *GetLinkDir(ILImage *image)
{
	ILDebugContext *dbgc;
	ILToken token;
	ILDebugIter iter;
	ILMetaDataRead reader;
	const char *result = 0;

	/* Dump empty location if the method's image 
	 * does not have any debug information */
	if(ILDebugPresent(image))
	{
		/* Get the symbol debug information */
		if((dbgc = ILDebugCreate(image)) == 0)
		{
			ILDbOutOfMemory();
		}
		token = ILDebugGetPseudo("LDIR");
		ILDebugIterInit(&iter, dbgc, token);
		if(ILDebugIterNext(&iter))
		{
			/* Read the LDIR string */
			reader.data = iter.data;
			reader.len = iter.length;
			reader.error = 0;
			result = ILDebugGetString(dbgc, ILMetaUncompressData(&reader));
			if(reader.error)
			{
				result = 0;
			}
		}
		ILDebugDestroy(dbgc);
	}
	return result;
}

/*
 * show_projects command.
 */
void ShowProjects(ILDebugger *debugger, FILE *stream)
{
	ILContext *context;
	ILImage *image;
	int imageId;
	const char *linkDir;
	ILImage *entryImage;

	/* Dump xml header */
	fputs("  <Projects>\n", stream);

	/* Iterate all images */
	context = ILExecProcessGetContext(debugger->process);
	entryImage = ILClassToImage(ILMethod_Owner(ILExecProcessGetEntry
														(debugger->process)));
	image = 0;
	while((image = ILContextNextImage(context, image)) != 0)
	{
		/* Dump image xml header */
		fputs("    <Project AssemblyName=\"", stream);
		DumpString(ILImageGetAssemblyName(image), stream);

		imageId = GetId(debugger, (void *) image, IL_USER_DATA_IMAGE_ID);
		fprintf(stream, "\" Id=\"%d", imageId);

		linkDir = GetLinkDir(image);
		if(linkDir != 0)
		{
			fputs("\" ProjectDir=\"", stream);
			DumpString(linkDir, stream);
		}

		if(image == entryImage)
		{
			fputs("\" IsEntry=\"1", stream);
		}

		fputs("\" />\n", stream);
	}
	/* Output the library footer information */
	fputs("  </Projects>", stream);
}

/*
 * show_source_files command.
 */
void ShowSourceFiles(ILDebugger *debugger, FILE *stream)
{
	ILContext *context;
	ILImage *image;
	ILMember *member;
	ILClass *classInfo;
	ILUInt32 line;
	ILUInt32 col;
	const char *sourceFile;
	int imageId;
	int size;
	ILUserDataEntry *entry;

	/* Dump files in table and clear the table */
	fputs("<SourceFiles>\n", stream);

	/* Iterate all images */
	context = ILExecProcessGetContext(debugger->process);
	image = 0;
	while((image = ILContextNextImage(context, image)) != 0)
	{
		/* Skip assemblies that are not watched */
		if(!IsImageWatched(debugger->assemblyWatches, image))
		{
			continue;
		}

		imageId = GetId(debugger, (void *) image, IL_USER_DATA_IMAGE_ID);

		/* Scan the TypeDef table and dump each top-level type */
		classInfo = 0;
		while((classInfo = (ILClass *)ILImageNextToken
					(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
		{
			member = 0;
			while((member = ILClassNextMember(classInfo, member)) != 0)
			{
				if(ILMemberGetKind(member) == IL_META_MEMBERKIND_METHOD)
				{
					/* Get source file of method from debug info */
					sourceFile = GetLocation((ILMethod *) member, 0,
																&line, &col);

					/* Skip if no debug info
					* or if source file was already added */
					if(sourceFile == 0)
					{
						continue;
					}

					/* This assigns id if needed */
					GetId(debugger, (void *) sourceFile,
												IL_USER_DATA_SOURCE_FILE_ID);

					SetUserData(debugger, (void *) sourceFile,
											IL_USER_DATA_SOURCE_FILE_IMAGE_ID,
											(void *)(ILNativeInt) imageId);
				}
			}
		}

		/* Dump files in user data table */
		entry = debugger->userData->entries;
		for(size = debugger->userData->size; size > 0; size--)
		{
			if(entry->type == IL_USER_DATA_SOURCE_FILE_ID)
			{
				DumpSourceFile(debugger, stream, (const char *) entry->ptr,
																			4);
			}
			entry++;
		}
	}
	fputs("</SourceFiles>", stream);
}

/*
 * show_types command.
 */
void ShowTypes(ILDebugger *debugger, FILE *stream)
{
	ILContext *context;
	ILImage *image;
	ILClass *classInfo;
	int classId;
	int imageId;

	/* Dump xml header */
	fputs("  <Types>\n", stream);

	/* Iterate all images */
	context = ILExecProcessGetContext(debugger->process);
	image = 0;
	while((image = ILContextNextImage(context, image)) != 0)
	{
		/* Skip assemblies that are not watched */
		if(!IsImageWatched(debugger->assemblyWatches, image))
		{
			continue;
		}

		/* Image id */
		imageId = GetId(debugger, (void *) image, IL_USER_DATA_IMAGE_ID);

		/* Scan the TypeDef table and dump each top-level type */
		classInfo = 0;
		while((classInfo = (ILClass *)ILImageNextToken
					(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
		{
			classId = GetId(debugger, (void *) classInfo,
														IL_USER_DATA_CLASS_ID);
			Indent(stream, 4);
			fprintf(stream, "<Type Id=\"%d\" ProjectId=\"%d\" Name=\"",
															classId, imageId);
			DumpString(ILClass_Name(classInfo), stream);
			fputs("\" />\n", stream);
		}
	}

	fputs("  </Types>", stream);
}

/*
 * show_members command.
 */
void ShowMembers(ILDebugger *debugger, FILE *stream)
{
	ILContext *context;
	ILImage *image;
	ILMember *member;
	ILClass *classInfo;

	/* Dump xml header */
	fputs("  <Members>\n", stream);

	/* Iterate all images */
	context = ILExecProcessGetContext(debugger->process);
	image = 0;
	while((image = ILContextNextImage(context, image)) != 0)
	{
		/* Skip assemblies that are not watched */
		if(!IsImageWatched(debugger->assemblyWatches, image))
		{
			continue;
		}
		/* Scan the TypeDef table and dump each top-level type */
		classInfo = 0;
		while((classInfo = (ILClass *)ILImageNextToken
							(image, IL_META_TOKEN_TYPE_DEF, classInfo)) != 0)
		{
			member = 0;
			while((member = ILClassNextMember(classInfo, member)) != 0)
			{
				switch(ILMemberGetKind(member))
				{
					case IL_META_MEMBERKIND_METHOD:
					{
						DumpMethod
							(debugger, stream, (ILMethod *) member, 0, 4);
					}
					break;
				}
			}
		}
	}

	fputs("  </Members>", stream);
}

/*
 * Resume current thread with given runType. Other stopped threads
 * are resumend with IL_DEBUGGER_RUN_TYPE_CONTINUE.
 */
void Resume(ILDebugger *debugger, int runType)
{
	ILDebuggerThreadInfo *info;

	/* Handle situation after break command */
	if(debugger->breakAll)
	{
		debugger->breakAll = 0;
	}

	info = debugger->dbthread;
	do
	{
		if(info->runType == IL_DEBUGGER_RUN_TYPE_STOPPED)
		{
			/* Set status */
			if(info == debugger->dbthread)
			{
				info->runType = runType;
			}
			else
			{
				info->runType = IL_DEBUGGER_RUN_TYPE_CONTINUE;
			}
	
			/* Signal thread to continue */
			ILWaitEventSet(info->event);
		}
		info = info->next;
	} while(info);
}

/*
 * continue command.
 */
void Continue(ILDebugger *debugger, FILE *stream)
{
	/* Set run type, report ok and continue execution */
	Resume(debugger, IL_DEBUGGER_RUN_TYPE_CONTINUE);
	DumpMessage("ok", stream);
}

void Step(ILDebugger *debugger, FILE *stream)
{
	/* Set run type, report ok and continue execution */
	Resume(debugger, IL_DEBUGGER_RUN_TYPE_STEP);
	DumpMessage("ok", stream);
}

void Next(ILDebugger *debugger, FILE *stream)
{
	/* Set run type, report ok and continue execution */
	Resume(debugger, IL_DEBUGGER_RUN_TYPE_NEXT);
	DumpMessage("ok", stream);
}

void Until(ILDebugger *debugger, FILE *stream)
{
	/* Set run type, report ok and continue execution */
	Resume(debugger, IL_DEBUGGER_RUN_TYPE_UNTIL);
	DumpMessage("ok", stream);
}

void IsStopped(ILDebugger *debugger, FILE *stream)
{
	if(debugger->dbthread->runType == IL_DEBUGGER_RUN_TYPE_STOPPED)
	{
		DumpMessage("yes", stream);
	}
	else
	{
		DumpMessage("no", stream);
	}
}

/*
 * is_stopped_in_watched_assembly command.
 */
void IsStoppedInWatchedAssembly(ILDebugger *debugger, FILE *stream)
{
	if(debugger->dbthread->runType == IL_DEBUGGER_RUN_TYPE_STOPPED &&
			ILDebuggerIsAssemblyWatched(debugger, debugger->dbthread->method))
	{
		DumpMessage("yes", stream);
	}
	else
	{
		DumpMessage("no", stream);
	}
}

/*
 * show_dasm command.
 */
void ShowDasm(ILDebugger *debugger, FILE *stream)
{
	char *str;
	long pos;

	/* Remeber current position before dump */
	pos = ftell(stream);

#ifdef IL_USE_JIT
	ILJitFunction func = ILJitFunctionFromILMethod(debugger->dbthread->method);
	jit_dump_function(stream, func, ILMethod_Name(debugger->dbthread->method));
#else
	/* TODO: segfaults and dumps just one insn */
	_ILDumpCVMInsn(stream, debugger->dbthread->method,
										debugger->dbthread->execThread->pc);
#endif

	/* Read stream to memory so that we can dump with xml quoting */
	str = ReadStream(stream);

	/* Restore position */
	fseek(stream, pos, SEEK_SET);

	if(!str)
	{
		DumpOutOfMemoryError(stream);
		return;
	}

	fputs("<Dasm>\n", stream);
	fputs("<Text>\n", stream);
	DumpString(str + pos, stream);
	fputs("</Text>\n", stream);
	fputs("</Dasm>\n", stream);

	ILFree(str);
}

/*
 * show_ildasm command.
 */
void ShowIldasm(ILDebugger *debugger, FILE *stream)
{
	ILImage *image;

	char *str;
	long pos;

	/* Remeber current position before dump */
	pos = ftell(stream);

	image = ILClassToImage(ILMethod_Owner(debugger->dbthread->method));
	ILDAsmDumpMethod(image, stream, debugger->dbthread->method, 0, 0, 1);

	/* Read stream to memory so that we can dump with xml quoting */
	str = ReadStream(stream);

	/* Restore position */
	fseek(stream, pos, SEEK_SET);

	if(!str)
	{
		DumpOutOfMemoryError(stream);
		return;
	}

	fputs("<ILDasm>\n", stream);
	fputs("<Text>\n", stream);
	DumpString(str + pos, stream);
	fputs("</Text>\n", stream);
	fputs("</ILDasm>\n", stream);

	ILFree(str);
}

/*
 * quit command.
 */
void Quit(ILDebugger *debugger, FILE *stream)
{
	debugger->abort = 1;
	DumpMessage("ok", stream);
}

/*
 * Table of commands.
 */
typedef struct
{
	const char *name;
	void (*func)(ILDebugger *debugger, FILE *stream);
	char *usage;
	char *shortDest;
	char *longDesc;

} Cmd;

static Cmd const commands[] = {
	{ "watch_assembly",							WatchAssembly,
		"watch_assembly [assembly_name]",
		"Watch assembly.",
		"By default debugger breaks in every assembly. This can be slow, so you can specify just assemblies you are interested in (debugger can't break in other assemblies.) Only functions, that were not compiled yet are affected."
	},
	
	{ "unwatch_all_assemblies",					UnwatchAllAssemblies,
		"unwatch_all_assemblies",
		"Remove all assembly watches.",
		"Remove all assembly watches that were previously set with watch_assembly. This causes that debugger can break in any assembly again. Only functions, that were not compiled yet are affected."
	},

	{ "start_profiling",						StartProfiling,
		"start_profiling",
		"Start profiling.",
		"Starts profiling execution speed and memory usage."
	},

	{ "stop_profiling",							StopProfiling,
		"stop_profiling",
		"Stops profiling.",
		"Stops profiling."
	},

	{ "clear_profiling",						ClearProfiling,
		"clear_profiling",
		"Clear profiling results.",
		"Clears all previous profiling results."
	},

	{ "insert_breakpoint",						InsertBreakpoint,
		"insert_breakpoint [source_file] [line]",
		"Break when given location is reached.",
		"Break when given location is reached."
	},

	{ "remove_breakpoint",						RemoveBreakpoint,
		"remove_breakpoint [source_file/breakpoint_id] [line]",
		"Remove breakpoint.",
		"Remove breakpoint."
	},

	{ "toggle_breakpoint",						ToggleBreakpoint,
		"toggle_breakpoint [source_file/breakpoint_id] [line]",
		"Toggle breakpoint.",
		"Toggle breakpoint."
	},

	{ "add_watch",								AddWatch,
		"add_watch [expression]",
		"Add new watch.",
		"Add new watch. Specify variable, field, property or function call e.g. i, this.x, this.X, this.ToString"
	},

	{ "remove_watch",							RemoveWatch,
		"remove_watch [expression]",
		"Remove watch.",
		"Remove watch with given id."
	},

	{ "remove_all_watches",						RemoveAllWatches,
		"remove_all_watches",
		"Remove all watches.",
		"Remove all watches."
	},

	{ "set_id",									SetId,
		"set_id [old_id] [new_id] ...",
		"Set id.",
		"Set new id for given old id"
	},

	{ "break",									Break,
		"break",
		"Break execution.",
		"Break execution of all threads."
	},

	{ "continue",								Continue,
		"continue",
		"Continue running your program.",
		"Continue running your program."
	},

	{ "step",									Step,
		"step",
		"Step (into).",
		"Step program until it reaches a different source line (steps into functions)."
	},

	{ "next",									Next,
		"next",
		"Next (step over).",
		"Step program, over function calls."
	},

	{ "until",									Until,
		"until",
		"Until.",
		"Execute until the program reaches a source line greater than the current."
	},

	{ "is_stopped",								IsStopped,
		"is_stopped",
		"Report if execution is stopped.",
		"Execution is stopped e.g when breakpoint is reached or break command is issued."
	},

	{ "is_stopped_in_watched_assembly",			IsStoppedInWatchedAssembly,
		"is_stopped_in_watched_assembly",
		"Is execution stopped in assembly that we watch?",
		"This function is useful after initial break. We usually do step command to get to assembly that is user watching."
	},	

	{ "show_threads",							ShowThreads,
		"show_threads",
		"List threads stopped in debugger.",
		"List threads stopped in debugger."
	},

	{ "show_breakpoints",						ShowBreakpoints,
		"show_breakpoints",
		"Show breakpoints.",
		"Show breakpoints."
	},

	{ "show_locals",							ShowLocals,
		"show_locals",
		"Show local variables.",
		"Show local variables."
	},

	{ "show_watches",							ShowWatches,
		"show_watches",
		"Show all watches.",
		"Show all watches defined with add_watch command."
	},

	{ "show_position",							ShowPosition,
		"show_position",
		"Print position where execution stopped.",
		"Print info about breakpoint where execution stopped."
	},

	{ "show_projects",							ShowProjects,
		"show_projects",
		"Show projects.",
		"Show project information."
	},

	{ "show_source_files",						ShowSourceFiles,
		"show_source_files",
		"Show source files.",
		"Show source files in watched assemblies."
	},

	{ "show_types",								ShowTypes,
		"show_types",
		"Show types.",
		"Show all types in watched assemblies."
	},


	{ "show_members",							ShowMembers,
		"show_members",
		"Show members.",
		"Show members for all types in watched assemblies."
	},

	{ "show_stack_trace",						ShowStackTrace,
		"show_stack_trace",
		"Print stacktrace for current thread.",
		"Print stacktrace for current thread."
	},

	{ "show_dasm",								ShowDasm,
		"show_dasm",
		"Dissassemble current function in engine's internal format.",
		"Dissassemble current function in engine's internal format."
	},

	{ "show_ildasm",							ShowIldasm,
		"show_ildasm",
		"Dissassemble current function in IL.",
		"Dissassemble current function in IL."
	},

	{ "show_profiling",							ShowProfiling,
		"show_profiling",
		"Show current profiling results.",
		"Show current profiling results. Times are in microseconds."
	},

	{ "quit",									Quit,
		"quit",
		"Abort debugged program.",
		"Abort debugged program."
	},

	{ "help",									Help,
		"help [command]",
		"Display this information or command help.",
		"Display this information or command specific help."
	}
};

#define	num_commands	(sizeof(commands) / sizeof(commands[0]))

/*
 * Help command.
 */
static void Help(ILDebugger *debugger, FILE *stream)
{
	int i;
	const char *name;
	int size, maxSize;

	/* Print the help header */
	fputs("Usage: command [arguments]\n", stream);
	fputs("Commands:\n", stream);

	/* Scan the command table to determine the width of the tab column */
	maxSize = 0;
	for(i = 0; i < num_commands; ++i)
	{
		name = commands[i].name;
		if(!name)
		{
			continue;
		}
		size = strlen(name);
		if(size > maxSize)
		{
			maxSize = size;
		}
	}

	/* Dump the help messages in the command table */
	for(i = 0; i < num_commands; ++i)
	{
		if(i > 0)
		{
			putc('\n', stream);
		}
		name = commands[i].name;
		fputs(name, stream);
		size = strlen(name);
		while(size < maxSize)
		{
			putc(' ', stream);
			++size;
		}
		putc(' ', stream);
		putc(' ', stream);
		fputs(commands[i].usage, stream);
	}
}

/*
 * Parse current command and return command index or -1 on error.
 * Whitespaces that delimit args are replaced with nuls
 * and debugger->lastArg is set.
 */
int ParseCommand(ILDebugger *debugger)
{
	int i, j;
	char *cmd;
	const char *name;
	int match;

	cmd = debugger->cmd;

	for(i = 0; i < num_commands; i++)
	{
		name = commands[i].name;
		match = 0;

		/* Compare names */
		for(j = 0;; j++)
		{
			if(name[j] == cmd[j] && (name[j] != 0 || cmd[j] != 0))
			{
				continue;
			}
			if(cmd[j] == 0 || isspace(cmd[j]))
			{
				match = 1;
			}
			break;
		}

		/* If not match try next command */
		if(!match)
		{
			continue;
		}

		/* Set last arg to command so that NextArg() works */
		debugger->lastArg = cmd;

		while(1)
		{
			if(cmd[j] == 0)
			{
				return i;
			}
			/* Replace whitespace with nul
			 * so that arguments are well terminated,
			 * remember last argument */
			if(isspace(cmd[j]))
			{
				cmd[j] = 0;
				debugger->lastArg = cmd + j + 1;
			}
			j++;
		}
	}

	return -1;
}

/*
 * Parse, execute and free current debugger command.
 * Returns:
 *   1 if next command is expected
 *   0 to continue execution (e.g. after step command)
 */
static void ParseAndExecuteCurrentCommand(ILDebugger *debugger)
{
	int cmdIndex;
	FILE *outputStream = debugger->io->output;

	/* Handle aborting */
	if(debugger->abort)
	{
		DumpMessage("process is aborting", outputStream);
	}
	else
	{
		/* Parse command */
		cmdIndex = ParseCommand(debugger);

		if(cmdIndex >= 0)
		{
			/* Execute command */
			(*(commands[cmdIndex].func))(debugger, outputStream);
			
		}
		else
		{
			DumpError("Unknown command", outputStream);
		}
	}

	/* Free command text, since it was allocated on heap */
	ILFree(debugger->cmd);
	debugger->cmd = 0;
 }

/*
 * Run command loop.
 */
static void CommandLoop(ILDebugger *debugger)
{
	int nextCommand;

	do
	{
		/* Recieve command */
		if(!(debugger->io->recieve(debugger->io)))
		{
			debugger->abort = 1;
			break;
		}

		/* Read command text from input stream */
		debugger->cmd = ReadStream(debugger->io->input);
		if(debugger->cmd == 0)
		{
			debugger->abort = 1;
			break;
		}

		/* Dump response header */
		fputs("<DebuggerResponse>\n", debugger->io->output);

		/* Lock down the debugger for executing command */
		LockDebugger(debugger);

		/* Determine which thread should execute the command */
		if(debugger->dbthread->runType == IL_DEBUGGER_RUN_TYPE_STOPPED &&
												debugger->ioThread != 0)
		{
			/* Wake up current thread to execute the command */
			ILWaitEventSet(debugger->dbthread->event);

			/* Wait until the command is processed */
			ILWaitOne(debugger->event, -1);
		}
		else
		{
			/* Handle command from this thread */
			ParseAndExecuteCurrentCommand(debugger);
		}

		UnlockDebugger(debugger);

		/* Dump response footer */
		fputs("\n</DebuggerResponse>\n", debugger->io->output);
	
		/* Send response to client */
		if(!(debugger->io->send(debugger->io)))
		{
			debugger->abort = 1;
			break;
		}

		/* Is next command expected?*/
		nextCommand =
				debugger->dbthread->runType == IL_DEBUGGER_RUN_TYPE_STOPPED;
	}
	while(nextCommand && debugger->abort == 0);
}

/*
 * Do CommandLoop() until abort is requested.
 * This function is executing in IO thread.
 * When abort is requested, all thread have to
 * be resumed, so that they can terminate.
 */
static void IOThreadFn(void *arg)
{
	ILDebugger *debugger = (ILDebugger *) arg;
	do
	{
		CommandLoop(debugger);
	}
	while(debugger->abort == 0);
	
	/* Resume all threads so that they can terminate
	 * by returning IL_HOOK_ABORT */
	Resume(debugger, IL_DEBUGGER_RUN_TYPE_CONTINUE);
}

/*
 * Start command-loop in separate thread.
 */
int StartIOThread(ILDebugger *debugger)
{
	if(!ILHasThreads())
	{
		return 0;
	}

	if(debugger->dontStartIoThread)
	{
		return 0;
	}

	/* Create command loop thread  */
	debugger->ioThread = ILThreadCreate(IOThreadFn, (void *) debugger);

	if(debugger->ioThread == 0)
	{
		return 0;
	}

	/* Start command loop thread */
	if(!ILThreadStart(debugger->ioThread))
	{
		ILThreadDestroy(debugger->ioThread);
		debugger->ioThread = 0;
		return 0;
	}

	return 1;
}

/*
 * Debug hook.
 */
static int DebugHook(void *userData, ILExecThread *thread, ILMethod *method,
					 ILInt32 offset, int type)
{
	int stop;
	ILUInt32 line;
	ILUInt32 col;
	const char *sourceFile;
	ILDebugger *debugger;
	ILDebuggerThreadInfo *info;
	ILBreakpoint *breakpoint;
	ILCurrTime time;
	ILInt64 delta;
	ILInt64 total;
	ILNativeUInt count;

	/* Get debugger attached to thread's process */
	debugger = _ILExecThreadProcess(thread)->debugger;

	/* Dont break in methods whose images are not watched */
	if(!ILDebuggerIsAssemblyWatched(debugger, method))
	{
		return IL_HOOK_CONTINUE;
	}

	/* Read current position from debug info */
	sourceFile = GetLocation(method, offset, &line, &col);

	/* The most important printf if you want to debug this debugger */
	/* fprintf(stderr, "DebugHook method=%s.%s offset=%d sourceFile=%s line=%d\n",
							ILClass_Name(ILMethod_Owner(method)),
							ILMethod_Name(method),
							offset, sourceFile, line); */

	/* Continue if no debug info available */
	if(sourceFile == 0)
	{
		return IL_HOOK_CONTINUE;
	}

	/* Lock down the debugger */
	LockDebugger(debugger);

	/* Get or create debugger info about thread */
	info = GetDbThreadInfo(debugger, thread);

	/* fprintf(stderr, " runType=%d breakAll=%d\n", info->runType, debugger->breakAll); */

	/* Update profiling info */
	if(debugger->profiling)
	{
		ILGetSinceRebootTime(&time);

		/* Time difference between last profiler hit and current time */
		delta = time.nsecs;
		delta -= info->profilerLastStopTime.nsecs;
		delta /= 1000;
		delta += 1000000 *
					(time.secs - info->profilerLastStopTime.secs);

		/* Add time delta to previous hit */
		total = (ILNativeUInt) GetUserData(debugger,
										   (void *) info->profilerLastMethod,
										   IL_USER_DATA_METHOD_TIME);
		total += delta;
		SetUserData(debugger, (void *) info->profilerLastMethod,
					IL_USER_DATA_METHOD_TIME,
					(void *)(ILNativeUInt)total);

		/* Increase method call count */
		if(offset == 0)
		{
			count = (ILNativeUInt) GetUserData(debugger, (void *) method,
											   IL_USER_DATA_METHOD_CALL_COUNT);
			count++;
			SetUserData(debugger, (void *) method,
											IL_USER_DATA_METHOD_CALL_COUNT,
											(void *)count);
		}

		/* Record this profiler hit */
		info->profilerLastMethod = method;
		info->profilerLastStopTime = time;
	}

	if(debugger->breakAll)
	{
		/* Always stop when break requested (by break command) */
		stop = 1;
	}
	else
	{
		/* Determine if we should stop */
		switch(info->runType)
		{
			case IL_DEBUGGER_RUN_TYPE_CONTINUE:
			{
				/* Stop if we have valid breakpoint
				 * for this method and offset */
				breakpoint = GetBreakpoint(debugger, method, offset);
				stop = (breakpoint->method != 0);
			}
			break;

			case IL_DEBUGGER_RUN_TYPE_STEP:
			{
				/* Stop if current method or location changed */
				stop =  (info->method != method) ||
					(line != debugger->dbthread->line) ||
					(col != debugger->dbthread->col);
			}
			break;

			case IL_DEBUGGER_RUN_TYPE_NEXT:
			{
				/* Stop in the same method when location changed
				or if current stack trace height is smaller then before. */
				if(method == info->method)
				{
					stop = ((line != info->line) ||
						(col != info->col));
				}
				else
				{
					stop = (_IL_StackFrame_InternalGetTotalFrames(thread) <
															info->numFrames);
				}
			}
			break;

			case IL_DEBUGGER_RUN_TYPE_UNTIL:
			{
				/* Stop in the same method when location moves further
				or if current stack trace height is smaller then before. */
				if(method == debugger->dbthread->method)
				{
					stop = ((line > info->line) ||
						(col > info->col));
				}
				else
				{
					stop = (_IL_StackFrame_InternalGetTotalFrames(thread) <
															info->numFrames);
				}
			}
			break;

			case IL_DEBUGGER_RUN_TYPE_FINISH:
			{
				/* Stop on the last instrucion of current method. TODO */
				stop = 1;
			}
			break;

			default:
			{
				stop = 1;
			}
			break;
		}
	}

	/* Continue execution, if we dont need to stop */
	if(!stop)
	{
		/* Unlock the debugger */
		UnlockDebugger(debugger);

		return IL_HOOK_CONTINUE;
	}

	/* Update information about current exec thread state */
	UpdateDbThreadInfo(info, userData, thread, method, offset, type,
													sourceFile, line, col);

	/* Mark thread as stopped */
	info->runType = IL_DEBUGGER_RUN_TYPE_STOPPED;

	/* If we are the first thread that stopped, make this thread current */
	if(debugger->dbthread->runType != IL_DEBUGGER_RUN_TYPE_STOPPED)
	{
		debugger->dbthread = info;
	}

	/* Unlock the debugger */
	UnlockDebugger(debugger);

	/* Check command loop thread */
	if(debugger->ioThread || StartIOThread(debugger))
	{
		while(info->runType == IL_DEBUGGER_RUN_TYPE_STOPPED)
		{
			/* Wait for command or for resuming */
			ILWaitOne(info->event, -1);

			/* Check if we were resumed for executing current command */
			if(debugger->cmd != 0 && info == debugger->dbthread)
			{
				/* Execute and signal that the command was handled */
				ParseAndExecuteCurrentCommand(debugger);
				ILWaitEventSet(debugger->event);
			}
		}
	}
	else
	{
		/* Do commands in current thread */
		CommandLoop(debugger);
	}

	/* Return hook result */
	if(debugger->abort)
	{
		return IL_HOOK_ABORT;
	}

	if(debugger->profiling)
	{
		/* Refresh time to not count hanging inside debugger loop */
		ILGetSinceRebootTime(&time);
		info->profilerLastStopTime = time;
	}

	return IL_HOOK_CONTINUE;
}

/*
 * Destroy debugger IO.
 */
static void ILDebuggerIODestroy(ILDebuggerIO *io)
{
	/* Close and destroy implementation specific stuff */
	io->close(io);

	/* Destroy common stuff */
	if(io->input)
	{
		fclose(io->input);
	}
	if(io->output)
	{
		fclose(io->output);
	}
	ILFree(io);
}

void ILDebuggerSetIO(ILDebugger *debugger, ILDebuggerIO *io)
{
	debugger->io = io;
}

int ILDebuggerConnect(ILDebugger *debugger, char *connectionString)
{
	ILDebuggerIO *io = debugger->io;

	/* Check if custom IO was previously set */
	if(io == 0)
	{
		/* Create debugger IO structure */
		debugger->io = io = (ILDebuggerIO *) ILMalloc(sizeof(ILDebuggerIO));
		if(io == 0)
		{
			return 0;
		}

		/* Setup default socket based IO */
		io->open = &SocketIO_Open;
		io->close = &SocketIO_Close;
		io->recieve = &SocketIO_Recieve;
		io->send = &SocketIO_Send;

		if(connectionString != 0)
		{
			if(strcmp(connectionString, "stdio") == 0)
			{
				/* Reading and sending on stdio */
				io->open = &StdIO_Open;
				io->close = &StdIO_Close;
				io->recieve = &StdIO_Recieve;
				io->send = &StdIO_Send;
			}
			else if(strcmp(connectionString, "trace") == 0)
			{
				io->open = &StdIO_Open;
				io->close = &StdIO_Close;
				io->recieve = &TraceIO_Recieve;
				io->send = &StdIO_Send;

				/* When tracing we dont need command thread */
				debugger->dontStartIoThread = 1;
			}
		}
	}

	/* Open temporary seekable streams */
	io->input = OpenTmpStream();
	io->output = OpenTmpStream();
	if(io->input == 0 || io->output == 0)
	{
		perror("debugger open stream");
		return 0;
	}

	/* Try to connect to debugger client */
	return io->open(io, connectionString);
}

int ILDebuggerIsAttached(ILExecProcess *process)
{
	return process->debugger != 0;
}

ILDebugger *ILDebuggerFromProcess(ILExecProcess *process)
{
	return process->debugger;
}

void ILDebuggerRequestTerminate(ILDebugger *debugger)
{
	/* Notify IO thread to abort */
	debugger->abort = 1;
}

void ILDebuggerDestroy(ILDebugger *debugger)
{
	/* Destroy initialized members */
	if(debugger->ioThread)
	{
		ILThreadDestroy(debugger->ioThread);
	}
	if(debugger->event)
	{
		ILWaitHandleClose(debugger->event);
	}
	if(debugger->lock)
	{
		ILMutexDestroy(debugger->lock);
	}
	if(debugger->userData)
	{
		ILUserDataDestroy(debugger->userData);
	}
	if(debugger->dbthread)
	{
		ILDebuggerThreadInfo_Destroy(debugger->dbthread);
	}
	/* Destroy assembly watch list */
	DestroyAssemblyWatches(debugger);

	/* Destroy watch list */
	DestroyWatchList(debugger->watches);

	if(debugger->io)
	{
		ILDebuggerIODestroy(debugger->io);
	}
	/* Unregister hook function and unwatch all */
	ILExecProcessDebugHook(debugger->process, 0, 0);
	ILExecProcessWatchAll(debugger->process, 0);

	/* Detach debugger from process */
	if(debugger->process)
	{
		debugger->process->debugger = 0;
	}
	ILFree(debugger);
}

ILDebugger *ILDebuggerCreate(ILExecProcess *process)
{
	ILDebugger *debugger;

	/* Allocate and initialize debugger structure */
	debugger = (ILDebugger *) ILMalloc(sizeof(ILDebugger));
	if(debugger == 0)
	{
		return 0;
	}
	ILMemSet(debugger, 0, sizeof(ILDebugger));

	/* Assign reference to exec process */
	debugger->process = process;

	/* Create mutex for locking down debugger */
	debugger->lock = ILMutexCreate();
	if(!(debugger->lock))
	{
		fputs("debugger: failed to create mutex\n", stderr);
		ILDebuggerDestroy(debugger);
		return 0;
	}

	/* Create user data table */
	debugger->userData = ILUserDataCreate();
	if(debugger->userData == 0)
	{
		fputs("debugger: failed to create user data\n", stderr);
		ILDebuggerDestroy(debugger);
		return 0;
	}

	/* Assembly watch list */
	debugger->assemblyWatches = 0;

	/* Watch list */
	debugger->watches = 0;

	/* Default thread info */
	debugger->dbthread = ILDebuggerThreadInfo_Create();
	debugger->dbthread->runType = IL_DEBUGGER_RUN_TYPE_STEP;

	/* Set breakAll so that we stop on first program instruction */
	debugger->breakAll = 1;

	/* Initialize breakpoint list */
	debugger->breakpoint = ILBreakpoint_Create();

	/* Initialize command thread */
	debugger->ioThread = 0;

	/* Initialize handle for synchronizing command execution */
	debugger->event = ILWaitEventCreate(0, 0);
	if(!(debugger->event))
	{
		fputs("debugger: failed to create wait handle\n", stderr);
		ILDebuggerDestroy(debugger);
		return 0;
	}

	/* Register debug hook function */
	if(ILExecProcessDebugHook(process, DebugHook, 0) == 0)
	{
		fputs("debugger: the runtime engine does not support debugging.\n",
																	stderr);
		ILDebuggerDestroy(debugger);
		return 0;
	}

	/* This will call DebugHook()
	 * before the first program instruction executes */
	ILExecProcessWatchAll(process, 1);

	/* Attach debugger to process */
	process->debugger = debugger;

	return debugger;
}

#endif	/* IL_DEBUGGER */
