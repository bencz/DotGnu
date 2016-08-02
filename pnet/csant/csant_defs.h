/*
 * csant_defs.h - Internal definitions for "csant".
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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

#ifndef	_CSANT_DEFS_H
#define	_CSANT_DEFS_H

#include <stdio.h>
#include "il_system.h"
#include "il_utils.h"
#include "il_sysio.h"
#include "il_xml.h"
#include "csant_dir.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Build options.
 */
extern int   CSAntJustPrint;
extern int   CSAntKeepGoing;
extern int   CSAntSilent;
extern int   CSAntRedirectCsc;
extern int   CSAntDummyDoc;
extern int   CSAntForceCorLib;
extern int   CSAntInstallMode;
extern int   CSAntUninstallMode;
extern char *CSAntCompiler;
extern char *CSAntBaseSrcDir;
extern char *CSAntBaseBuildDir;
extern char *CSAntProjectName;
extern char *CSAntDefaultTarget;
extern char *CSAntCacheDir;

/*
 * Information that is stored about a particular task.
 */
typedef struct _tagCSAntTask CSAntTask;
struct _tagCSAntTask
{
	CSAntTask	   *next;
	char		   *name;
	CSAntTask	   *taskChildren;
	int				paramLen;
	char			params[1];

};

/*
 * Information that is stored about a particular target.
 */
typedef struct _tagCSAntTarget CSAntTarget;
struct _tagCSAntTarget
{
	char		   *name;
	CSAntTask      *tasks;
	CSAntTask      *lastTask;
	CSAntTarget	  **dependsOn;
	int				numDependsOn;
	int				built;
	CSAntTarget	   *next;

};

/*
 * List of all target description blocks in the system.
 */
extern CSAntTarget *CSAntTargetList;

/*
 * List of all profile definitions.
 */
extern char **CSAntProfileDefines;
extern char **CSAntProfileValues;
extern int CSAntNumProfileDefines;

/*
 * Task dispatch function.
 */
typedef int (*CSAntTaskFunc)(CSAntTask *task);

/*
 * List of task dispatch functions.
 */
typedef struct
{
	const char     *name;
	int				installMode;
	CSAntTaskFunc	func;

} CSAntTaskInfo;
extern CSAntTaskInfo const CSAntTasks[];
extern int           const CSAntNumTasks;

/*
 * Report out of memory and abort the program.
 */
void CSAntOutOfMemory(void);

/*
 * Get the name of this program execution instance.
 */
char *CSAntGetProgramName(void);

/*
 * Define a property value.
 */
void CSAntDefineProperty(const char *name, int nameLen,
						 const char *value, int fromCmdLine);

/*
 * Get the value of a property.  Returns NULL if no such property.
 */
const char *CSAntGetProperty(const char *name, int nameLen);

/*
 * Add a target to the list of targets to be built.
 */
void CSAntAddBuildTarget(const char *target);

/*
 * Parse the contents of a build project file.
 * Returns zero if a parse error was detected.
 */
int CSAntParseFile(ILXMLReader *reader, const char *filename);

/*
 * Parse the contents of a profile definition file.
 * Returns zero if a parse error was detected.
 */
int CSAntParseProfileFile(ILXMLReader *reader, const char *filename);

/*
 * Find the descriptor for a particular target.  Returns
 * NULL if the target name could not be found.  If the
 * target name is NULL, then return the global target.
 */
CSAntTarget *CSAntFindTarget(const char *target);

/*
 * Get the value of a particular task parameter.
 */
const char *CSAntTaskParam(CSAntTask *task, const char *name);

/*
 * Build all specified targets.  Returns zero on failure.
 */
int CSAntBuild(const char *buildFilename);

/*
 * Handle a "cscc" task, which invokes the Portable.NET C# compiler.
 */
int CSAntTask_Cscc(CSAntTask *task);

/*
 * Handle a "csc" task, which invokes the Microsoft C# compiler.
 */
int CSAntTask_Csc(CSAntTask *task);

/*
 * Handle a "mcs" task, which invokes the Mono C# compiler.
 */
int CSAntTask_Mcs(CSAntTask *task);

/*
 * Handle a "compile" task, which invokes the configured C# compiler.
 */
int CSAntTask_Compile(CSAntTask *task);

/*
 * Handle a "csdoc" task, which invokes the documentation generator.
 */
int CSAntTask_Csdoc(CSAntTask *task);

/*
 * Handle a "resgen" task, which invokes the resource converter.
 */
int CSAntTask_ResGen(CSAntTask *task);

/*
 * Handle a "reslink" task, which links resources into a standalone assembly.
 */
int CSAntTask_ResLink(CSAntTask *task);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSANT_DEFS_H */
