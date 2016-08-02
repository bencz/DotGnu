/*
 * il_engine.h - Interface to the runtime engine.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#ifndef	_IL_ENGINE_H
#define	_IL_ENGINE_H

#include "il_program.h"
#include "il_thread.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 *	Structure used by the EE for storing monitors.
 */
typedef struct _tagILExecMonitor ILExecMonitor;

/*
 * Execution control context for a process.
 */
typedef struct _tagILExecProcess ILExecProcess;

/*
 * Execution control context for a single thread.
 */
typedef struct _tagILExecThread ILExecThread;

/*
 * Object and string handles.
 */
typedef struct _tagILObject ILObject;
typedef struct _tagILString ILString;

/*
 * Types that are useful for passing values through va lists.
 */
#if SIZEOF_INT > 4
typedef int				ILVaInt;
typedef unsigned int	ILVaUInt;
#else
typedef ILInt32			ILVaInt;
typedef ILUInt32		ILVaUInt;
#endif
typedef double			ILVaDouble;

/*
 * The structure that is used to pass typed references around.
 */
typedef struct
{
	void *type;
	void *value;

} ILTypedRef;

/*
 * A structure that can be used to represent any execution engine value.
 * This is typically used when it is inconvenient to use vararg calls.
 */
typedef union
{
	ILInt32			int32Value;
	ILUInt32		uint32Value;
	ILInt64			int64Value;
	ILUInt64		uint64Value;
	ILNativeInt		nintValue;
	ILNativeUInt	nuintValue;
	ILNativeFloat	floatValue;
	ILTypedRef		typedRefValue;
	void		   *ptrValue;
	ILObject	   *objValue;
	ILString	   *strValue;

} ILExecValue;

/*
 * Structure of an "internalcall" method table entry.
 */
typedef struct
{
	const char	   *methodName;
	const char	   *signature;
	void           *func;
#if !defined(HAVE_LIBFFI)
	void           *marshal;
#endif

} ILMethodTableEntry;

/*
 * Structure that keeps track of a process local internalcall
 * NOTE: Keep in sync with int_table.c declaration
 */
typedef struct _tagILEngineInternalClassInfo ILEngineInternalClassInfo;
struct _tagILEngineInternalClassInfo
{
	char *name;
	char *nspace;
	ILMethodTableEntry *entry;
};

/*
 * Simple linked list for storing multiple blocks of internal calls
 */
typedef struct _tagILEngineInternalClassList ILEngineInternalClassList;
struct _tagILEngineInternalClassList
{
	const ILEngineInternalClassInfo *list;
	int size;
	struct _tagILEngineInternalClassList* next;
};


/*
 * Helper macros for defining "internalcall" method tables.
 */
#define	IL_METHOD_BEGIN(name)	\
			static ILMethodTableEntry const name[] = {
#if defined(HAVE_LIBFFI)
#define	IL_METHOD(name,sig,func,marshal)	\
			{(name), (sig), (void *)(func)},
#define	IL_CONSTRUCTOR(name,sig,func,marshal,allocFunc,allocMarshal)	\
			{(name), (sig), (void *)(func)}, \
			{(name), 0, (void *)(allocFunc)},
#define	IL_METHOD_END			\
			{0, 0, 0}};
#else
#define	IL_METHOD(name,sig,func,marshal)	\
			{(name), (sig), (void *)(func), (void *)(marshal)},
#define	IL_CONSTRUCTOR(name,sig,func,marshal,allocFunc,allocMarshal)	\
			{(name), (sig), (void *)(func), (void *)(marshal)}, \
			{(name), 0, (void *)(allocFunc), (void *)(allocMarshal)},
#define	IL_METHOD_END			\
			{0, 0, 0, 0}};
#endif

/*
 * Function type that is used by debuggers for hooking breakpoints.
 */
typedef int (*ILExecDebugHookFunc)(void *userData,
								   ILExecThread *thread,
								   ILMethod *method,
								   ILInt32 offset, int type);

/*
 * Breakpoint types.
 */
#define	IL_BREAK_ENTER_METHOD		0
#define	IL_BREAK_EXIT_METHOD		1
#define	IL_BREAK_DEBUG_LINE			2
#define	IL_BREAK_EXPLICIT			3
#define	IL_BREAK_THROW_CALLER		4

/*
 * Debug hook actions.
 */
#define	IL_HOOK_CONTINUE			0
#define	IL_HOOK_ABORT				1

#define IL_EXEC_INIT_OK				(0)
#define IL_EXEC_INIT_OUTOFMEMORY	(1)

/*
 * Return values of the ILExecProcessExecute functions.
 * They must be kept in sync with the IL_IMAGE_LOADERR values.
 */
#define IL_EXECUTE_OK						0
#define	IL_EXECUTE_LOADERR_TRUNCATED 		IL_LOADERR_TRUNCATED
#define	IL_EXECUTE_LOADERR_NOT_PE			IL_LOADERR_NOT_PE
#define	IL_EXECUTE_LOADERR_NOT_IL			IL_LOADERR_NOT_IL
#define	IL_EXECUTE_LOADERR_VERSION			IL_LOADERR_VERSION
#define	IL_EXECUTE_LOADERR_32BIT_ONLY		IL_LOADERR_32BIT_ONLY
#define	IL_EXECUTE_LOADERR_BACKWARDS		IL_LOADERR_BACKWARDS
#define	IL_EXECUTE_LOADERR_MEMORY			IL_LOADERR_MEMORY
#define	IL_EXECUTE_LOADERR_BAD_ADDR			IL_LOADERR_BAD_ADDR
#define	IL_EXECUTE_LOADERR_BAD_META			IL_LOADERR_BAD_META
#define	IL_EXECUTE_LOADERR_UNDOC_META		IL_LOADERR_UNDOC_META
#define	IL_EXECUTE_LOADERR_UNRESOLVED		IL_LOADERR_UNRESOLVED
#define	IL_EXECUTE_LOADERR_ARCHIVE			IL_LOADERR_ARCHIVE
#define	IL_EXECUTE_ERR_MEMORY				(IL_LOADERR_MAX + 1)	/* out of memory */
#define	IL_EXECUTE_ERR_FILE_OPEN			(IL_LOADERR_MAX + 2)	/* could not open the file */
#define	IL_EXECUTE_ERR_NO_ENTRYPOINT		(IL_LOADERR_MAX + 3)	/* there is no entry point */
#define	IL_EXECUTE_ERR_INVALID_ENTRYPOINT	(IL_LOADERR_MAX + 4)	/* the entry point is invalid */
#define	IL_EXECUTE_ERR_EXCEPTION			(IL_LOADERR_MAX + 5)	/* finish with an exception */

/*
 * Initialize the engine and set a default maximum heap size.
 * If the size is zero, then use all of memory for the heap.
 * This should be called only once per application.
 *
 * Returns 0 if the engine was successfully initialized.
 */
int ILExecInit(unsigned long maxSize);

/*
 *	Deinitialize the engine.
 */
void ILExecDeinit();

/*
 *	Registers an ILThread and allow it to execute managed code. 
 */
ILExecThread *ILThreadRegisterForManagedExecution(ILExecProcess *process, ILThread *thread);

/*
 *	Unregisters an ILThread that no longer needs to execute managed code.
 */
void ILThreadUnregisterForManagedExecution(ILThread *thread);

/*
 * Create a new process where code can be executed.
 */
ILExecProcess *ILExecProcessCreate(unsigned long frameStackSize, unsigned long cachePageSize);

/*
 * Create an ILExecProcess associated with the null coder.
 * In this process no code can be executed.
 */
ILExecProcess *ILExecProcessCreateNull(void);

/*
 * Destroy a process and all threads associated with it.
 * If the thread that calls this method belongs to the process
 * the thread will no longer be able to execute managed code
 * when the function exits.  For a safer version that can
 * be called from managed code, see ILExecProcessUnload.
 */
void ILExecProcessDestroy(ILExecProcess *process);

/*
 * Unloads a process and all threads associated with it.
 * The process may not unload immediately (or even ever).
 * If the current thread exists inside the process, a
 * new thread will be created to unload & destroy the
 * process.  When this function exits, the thread that
 * calls this method will still be able to execute managed
 * code even if it resides within the process it tried to
 * destroy.  The process will eventually be destroyed
 * when the thread (and all other process-owned threads)
 * exit.
 */
void ILExecProcessUnload(ILExecProcess *process);

/*
 * Set the list of directories to be used for library path
 * searching, before inspecting the standard directories.
 * It is assumed that the list will persist for the lifetime
 * of the process.
 */
void ILExecProcessSetLibraryDirs(ILExecProcess *process,
								 char **libraryDirs,
								 int numLibraryDirs);

/*
 *Set the flags for profiling, debugging etc
 */
void ILExecProcessSetCoderFlags(ILExecProcess *process,
								int flags);

/*
 * Get the IL context associated with a process.
 */
ILContext *ILExecProcessGetContext(ILExecProcess *process);

/*
 * Set the display name if this AppDomain.
 * The old friendlyName will be freed.
 */
void ILExecProcessSetFriendlyName(ILExecProcess *process, char *friendlyName);

/*
 * Get the display name if this AppDomain.
 * The caller has to make sure that this string exists for the
 * time it is used. It might be destroyed if the Set function is
 * called by an other thread.
 */
char *ILExecProcessGetFriendlyName(ILExecProcess *process);

/*
 * Get the "main" thread for a process.
 * This function is obsolete.
 * It worked only correctly if called from the thread that created the
 * ILExecProcess with calling ILExecProcessCreate.
 * Failing to do so resulted the same ILExecThread being used by multiple
 * threads and causing trouble as soon as both of them started executing
 * managed code.
 * Simply use ILExecThreadCurrent() instead.
 */
ILExecThread *ILExecProcessGetMain(ILExecProcess *process);

#ifndef REDUCED_STDIO

/*
 * Load an image into a process.  Returns 0 if OK, or
 * an image load error code otherwise (see "il_image.h").
 */
int ILExecProcessLoadImage(ILExecProcess *process, FILE *file);

#endif

/*
 * Load the contents of a file as an image into a process.
 * Returns 0 if OK, -1 if the file could not be opened,
 * or an image load error code otherwise.
 */
int ILExecProcessLoadFile(ILExecProcess *process, const char *filename);

/*
 * Load an assembly into the ILExecProcess and execute the entypoint.
 */
int ILExecProcessExecuteFile(ILExecProcess *process,
							 const char *filename,
							 char *argv[],
							 int* retval);

/*
 * Set the load flags to use with "ILImageLoad".
 */
void ILExecProcessSetLoadFlags(ILExecProcess *process, int mask, int flags);

/*
 * Get the exit status for a process.
 */
int ILExecProcessGetStatus(ILExecProcess *process);

/*
 * Get the entry point method for a process.
 */
ILMethod *ILExecProcessGetEntry(ILExecProcess *process);

/*
 * Entry point types.
 */
#define	IL_ENTRY_INVALID		0		/* Invalid entry point */
#define	IL_ENTRY_NOARGS_VOID	1		/* static void Main() */
#define	IL_ENTRY_NOARGS_INT		2		/* static int Main() */
#define	IL_ENTRY_ARGS_VOID		3		/* static void Main(String[]) */
#define	IL_ENTRY_ARGS_INT		4		/* static int Main(String[]) */

/*
 * Validate a program entry point and return its type.
 */
int ILExecProcessEntryType(ILMethod *method);

/*
 * Type values for "ILExecProcessGetParam".
 */
#define	IL_EXEC_PARAM_GC_SIZE		1	/* Size of the GC heap */
#define	IL_EXEC_PARAM_MC_SIZE		2	/* Size of the method cache */
#define	IL_EXEC_PARAM_MALLOC_MAX	3	/* Maximum malloc usage */

/*
 * Get parameter information about a process.  Returns -1 if
 * the type is invalid.
 */
long ILExecProcessGetParam(ILExecProcess *process, int type);

/*
 * Set the command-line arguments.  Returns the parameter
 * that should be passed to "Main".
 */
ILObject *ILExecProcessSetCommandLine(ILExecProcess *process,
									  const char *progName, char *args[]);

/*
 * Register a debug hook with a process.  This should be called just
 * after "ILExecProcessCreate" and before loading the application.
 * Returns zero if the engine does not support debugging.
 */
int ILExecProcessDebugHook(ILExecProcess *process,
						   ILExecDebugHookFunc func,
						   void *data);

/*
 * Watch a particular method for breakpoints.  Returns zero
 * if out of memory.  Multiple watches can be registered for
 * the same method, and must be individually unregistered
 * using "ILExecProcessUnwatchMethod".
 */
int ILExecProcessWatchMethod(ILExecProcess *process, ILMethod *method);

/*
 * Remove a method from the breakpoint watch list.
 */
void ILExecProcessUnwatchMethod(ILExecProcess *process, ILMethod *method);

/*
 * Enable or disable the "watch all breakpoints" flag.
 */
void ILExecProcessWatchAll(ILExecProcess *process, int flag);

/* 
 * Add a new set of internal calls to the process's lookup table
 */
int ILExecProcessAddInternalCallTable(ILExecProcess* process, 
					const ILEngineInternalClassInfo* internalClassTable,
					int internalClassCount);
/*
 * Get the current ILExecThread.
 */
ILExecThread *ILExecThreadCurrent(void);

/*
 *	Gets the current managed thread object.
 */
ILObject *ILExecThreadCurrentClrThread();

/*
 * Get the process that corresponds to a thread.
 */
ILExecProcess *ILExecThreadGetProcess(ILExecThread *thread);

/*
 * Get the method that is executing "num" steps up
 * the thread stack.  Zero indicates the currently
 * executing method.  Returns NULL if "num" is invalid.
 */
ILMethod *ILExecThreadStackMethod(ILExecThread *thread, unsigned long num);

/*
 * Determine the size of a type's values in bytes.
 */
ILUInt32 ILSizeOfType(ILExecThread *thread, ILType *type);

/*
 * Look up a class name within a particular thread's context.
 * Returns NULL if the name could not be found.
 */
ILClass *ILExecThreadLookupClass(ILExecThread *thread, const char *className);

/*
 * Look up a type name within a particular thread's context.
 * Returns NULL if the name could not be found.
 */
ILType *ILExecThreadLookupType(ILExecThread *thread, const char *typeName);

/*
 * Look up a method by type name, method name, and signature.
 * Returns NULL if the method could not be found.  This function
 * will search ancestor classes if the method is not found in
 * a child class.
 */
ILMethod *ILExecThreadLookupMethod(ILExecThread *thread,
								   const char *typeName,
								   const char *methodName,
								   const char *signature);

/*
 * Look up a method in a particular class.  Returns NULL if
 * the method could not be found.
 */
ILMethod *ILExecThreadLookupMethodInClass(ILExecThread *thread,
										  ILClass *classInfo,
								   		  const char *methodName,
								   		  const char *signature);

/*
 * Look up a field by type name, field name, and signature.
 * Returns NULL if the field could not be found.  This function
 * will search ancestor classes if the field is not found in
 * a child class.
 */
ILField *ILExecThreadLookupField(ILExecThread *thread,
								 const char *typeName,
								 const char *fieldName,
								 const char *signature);

/*
 * Look up a field in a  particular class.  Returns NULL if
 * the field could not be found.
 */
ILField *ILExecThreadLookupFieldInClass(ILExecThread *thread,
								 ILClass *classInfo,
								 const char *fieldName,
								 const char *signature);

/*
 * Call a particular method within a thread's context.
 * Returns non-zero if an exception was thrown during
 * the processing of the method.  Zero otherwise.
 * The return value, if any, is placed in "*result".
 */
int ILExecThreadCall(ILExecThread *thread, ILMethod *method,
					 void *result, ...);

/*
 * Call a particular method within a thread's context,
 * passing the parameter and return values from
 * "ILExecValue" structures.  Returns non-zero if an
 * exception was thrown.  Zero otherwise.  The return
 * value, if any, is placed in "*result".
 */
int ILExecThreadCallV(ILExecThread *thread, ILMethod *method,
					  ILExecValue *result, ILExecValue *args);

/*
 * Call a constructor within a thread's context, passing
 * the parameter and return values from "ILExecValue"
 * structures.  Returns the new object, or zero if an
 * exception was thrown.
 */
ILObject *ILExecThreadCallCtorV(ILExecThread *thread, ILMethod *ctor,
								ILExecValue *args);

/*
 * Call a particular virtual method within a thread's context.
 * If the method isn't marked as virtual, then an ordinary
 * method call will be used instead.
 */
int ILExecThreadCallVirtual(ILExecThread *thread, ILMethod *method,
							void *result, void *_this, ...);

/*
 * Call a particular virtual method using "ILExecValue" parameters.
 */
int ILExecThreadCallVirtualV(ILExecThread *thread, ILMethod *method,
							 ILExecValue *result, void *_this,
							 ILExecValue *args);

/*
 * Look up a method by name and then call it.  If the
 * method is not found, an exception will be thrown.
 */
int ILExecThreadCallNamed(ILExecThread *thread, const char *typeName,
						  const char *methodName, const char *signature,
						  void *result, ...);

/*
 * Look up a method by name and call it with "ILExecValue" parameters.
 */
int ILExecThreadCallNamedV(ILExecThread *thread, const char *typeName,
						   const char *methodName, const char *signature,
						   ILExecValue *result, ILExecValue *args);

/*
 * Look up a method by name and then call it using a
 * virtual call.  If the method is not found, an exception
 * will be thrown.
 */
int ILExecThreadCallNamedVirtual(ILExecThread *thread, const char *typeName,
						         const char *methodName, const char *signature,
						         void *result, void *_this, ...);

/*
 * Look up a virtual method by name and call it with
 * "ILExecValue" parameters.
 */
int ILExecThreadCallNamedVirtualV(ILExecThread *thread, const char *typeName,
						          const char *methodName, const char *signature,
						          ILExecValue *result, void *_this,
								  ILExecValue *args);

/*
 * Create a new object instance of a class and call its constructor.
 * Returns NULL if an exception occurred.
 */
ILObject *ILExecThreadNew(ILExecThread *thread, const char *typeName,
						  const char *signature, ...);

/*
 * Create a new object instance of a class and call its constructor
 * using "ILExecValue" parameters.
 */
ILObject *ILExecThreadNewV(ILExecThread *thread, const char *typeName,
						   const char *signature, ILExecValue *args);

/*
 * Determine if there is a last-occuring exception
 * for a thread.  Returns non-zero if so.
 */
int ILExecThreadHasException(ILExecThread *thread);

/*
 * Get the last-occurring exception for a thread.
 * Returns NULL if there is no exception object.
 */
ILObject *ILExecThreadGetException(ILExecThread *thread);

/*
 * Set the last-occuring exception for a thread.
 */
void ILExecThreadSetException(ILExecThread *thread, ILObject *obj);

/*
 * Clear the last-occuring exception for a thread.  If an
 * exception is not cleared, it will be re-thrown from the
 * current method.
 */
void ILExecThreadClearException(ILExecThread *thread);

/*
 * Throws the given exception.
 */
void ILExecThreadThrow(ILExecThread *thread, ILObject *obj);

/*
 * Throw a system exception with a particular type and
 * resource name.
 */
void ILExecThreadThrowSystem(ILExecThread *thread, const char *typeName,
							 const char *resourceName);

/*
 * Throw an "ArgumentOutOfRangeException" for a particular
 * parameter and resource name.
 */
void ILExecThreadThrowArgRange(ILExecThread *thread, const char *paramName,
							   const char *resourceName);

/*
 * Throw an "ArgumentNullException" for a particular
 * parameter name.
 */
void ILExecThreadThrowArgNull(ILExecThread *thread, const char *paramName);

/*
 * Throw an "OutOfMemoryException".
 */
void ILExecThreadThrowOutOfMemory(ILExecThread *thread);

/*
 * Create a string from a NUL-terminated C string.  Returns
 * NULL if out of memory, with the thread's current exception
 * set to an "OutOfMemoryException" instance.
 */
ILString *ILStringCreate(ILExecThread *thread, const char *str);

/*
 * Create a string from a length-delimited C string.
 */
ILString *ILStringCreateLen(ILExecThread *thread, const char *str, int len);

/*
 * Create a string from a NUL-terminated UTF-8 string.
 */
ILString *ILStringCreateUTF8(ILExecThread *thread, const char *str);

/*
 * Create a string from a length-deliminated UTF-8 string.
 */
ILString *ILStringCreateUTF8Len(ILExecThread *thread, const char *str, int len);

/*
 * Create a string from a zero-terminated wide character string.
 */
ILString *ILStringWCreate(ILExecThread *thread, const ILUInt16 *str);

/*
 * Create a string from a length-delimited wide character string.
 */
ILString *ILStringWCreateLen(ILExecThread *thread,
							 const ILUInt16 *str, int len);

/*
 * Compare two strings, returning -1, 0, or 1, depending
 * upon the relationship between the values.
 */
int ILStringCompare(ILExecThread *thread, ILString *strA, ILString *strB);

/*
 * Compare two strings, while ignoring case.
 */
int ILStringCompareIgnoreCase(ILExecThread *thread, ILString *strA,
							  ILString *strB);

/*
 * Compare the ordinal values of two strings.
 */
int ILStringCompareOrdinal(ILExecThread *thread, ILString *strA,
						   ILString *strB);

/*
 * Determine if two strings are equal.
 */
int ILStringEquals(ILExecThread *thread, ILString *strA, ILString *strB);

/*
 * Concatenate two strings.
 */
ILString *ILStringConcat(ILExecThread *thread, ILString *strA, ILString *strB);

/*
 * Concatenate three strings.
 */
ILString *ILStringConcat3(ILExecThread *thread, ILString *strA,
						  ILString *strB, ILString *strC);

/*
 * Concatenate four strings.
 */
ILString *ILStringConcat4(ILExecThread *thread, ILString *strA,
						  ILString *strB, ILString *strC, ILString *strD);

/*
 * Convert an object into a string, using "ToString".
 * NULL is returned if "object" is NULL.
 */
ILString *ILObjectToString(ILExecThread *thread, ILObject *object);

/*
 * Internalize a string.
 */
ILString *ILStringIntern(ILExecThread *thread, ILString *str);

/*
 * Convert a string into a NUL-terminated UTF-8 buffer,
 * allocated within the garbage collected heap.  If "str"
 * is NULL, then NULL will be returned.
 */
char *ILStringToUTF8(ILExecThread *thread, ILString *str);

/*
 * Convert a string into a NUL-terminated UTF-16 buffer,
 * allocated within the garbage collected heap.  If "str"
 * is NULL, then NULL will be returned.
 */
ILUInt16 *ILStringToUTF16(ILExecThread *thread, ILString *str);

/*
 * Convert a string into an "ANSI" string in the local
 * character set, allocated within the garbage collected heap.
 * If "str" is NULL, then NULL will be returned.
 */
char *ILStringToAnsi(ILExecThread *thread, ILString *str);

/*
 * Convert a string into "ANSI" and normalize any pathname
 * separators to the system's standard character.  If "str"
 * is NULL, then NULL will be returned.
 */
char *ILStringToPathname(ILExecThread *thread, ILString *str);

/*
 * Get an element of an array.  Returns non-zero if an exception occurred.
 */
int ILExecThreadGetElem(ILExecThread *thread, void *value,
						ILObject *array, ILInt32 index);

/*
 * Set an element of an array.  Returns non-zero if an exception occurred.
 */
int ILExecThreadSetElem(ILExecThread *thread, ILObject *array,
						ILInt32 index, ...);

/*
 * Create a box for a type and initialize the box with no value.
 */
ILObject *ILExecThreadBoxNoValue(ILExecThread *thread, ILType *type);

/*
 * Box the value at a pointer using a specific primitive or value type.
 */
ILObject *ILExecThreadBox(ILExecThread *thread, ILType *type, void *ptr);

/*
 * Box the value at a pointer using a specific primitive or value type,
 * and handle the case where float/double values are stored as ILNativeFloat.
 */
ILObject *ILExecThreadBoxFloat(ILExecThread *thread, ILType *type, void *ptr);

/*
 * Unbox an object using a specific primitive or value type and
 * write the contents to a pointer.  Returns zero if the object
 * is NULL or is not of the correct type.
 */
int ILExecThreadUnbox(ILExecThread *thread, ILType *type,
					  ILObject *object, void *ptr);

/*
 * Unbox an object using a specific primitive or value type and
 * write the contents to a pointer and handle the case where
 * float/double values are stored as ILNativeFloat.
 */
int ILExecThreadUnboxFloat(ILExecThread *thread, ILType *type,
					       ILObject *object, void *ptr);

/*
 * Box the value at a pointer using specific primitive or subsequent 
 * promotions. Handle the cases where an Int16 is being unboxed to an 
 * Int32 or Int64.
 */
int ILExecThreadPromoteAndUnbox(ILExecThread *thread, ILType *type, 
								ILObject *object, void *ptr);

/*
 * Print the current exception object to standard error.
 */
void ILExecThreadPrintException(ILExecThread *thread);

/*
 * Checks if the given object is a ThreadAbortException.
 */
int ILExecThreadIsThreadAbortException(ILExecThread *thread, ILObject *object);

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_ENGINE_H */
