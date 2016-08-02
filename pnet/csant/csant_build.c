/*
 * csant_build.c - Build all targets.
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

#include "csant_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int   CSAntJustPrint     = 0;
int   CSAntKeepGoing     = 0;
int   CSAntSilent        = 0;
int   CSAntRedirectCsc   = 0;
int   CSAntDummyDoc      = 0;
int   CSAntForceCorLib   = 0;
int   CSAntInstallMode   = 0;
int   CSAntUninstallMode = 0;
char *CSAntCompiler      = 0;
char *CSAntCacheDir      = 0;

/*
 * List of non-global targets that are registered to be built.
 */
static int numTargets = 0;
static int maxTargets = 0;
static char **targets = 0;

void CSAntAddBuildTarget(const char *target)
{
	if(numTargets >= maxTargets)
	{
		targets = (char **)ILRealloc
			(targets, sizeof(char *) * (numTargets + 4));
		if(!targets)
		{
			CSAntOutOfMemory();
		}
		maxTargets += 4;
	}
	if((targets[numTargets++] = ILDupString(target)) == 0)
	{
		CSAntOutOfMemory();
	}
}

/*
 * Initialize the standard properties.
 */
static void StdProps(const char *buildFilename)
{
	extern char **environ;
	char **temp;
	char *split;
	char *copyName;

	/* Define directory and project name properties */
	CSAntDefineProperty("csant.buildfile", -1, buildFilename, 0);
	if(CSAntProjectName)
	{
		CSAntDefineProperty("csant.project.name", -1, CSAntProjectName, 0);
	}
	CSAntDefineProperty("csant.src.dir", -1, CSAntBaseSrcDir, 0);
	CSAntDefineProperty("csant.build.dir", -1, CSAntBaseBuildDir, 0);
	if(CSAntDefaultTarget)
	{
		CSAntDefineProperty("csant.default.name", -1,
							CSAntDefaultTarget, 0);
	}
	CSAntDefineProperty("csant.compiler", -1, CSAntCompiler, 0);

	/* Copy the contents of the environment to the property list */
	temp = environ;
	while(*temp != 0)
	{
		split = strchr(*temp, '=');
		if(split)
		{
			copyName = (char *)ILMalloc((int)(split - *temp) + 11);
			if(!copyName)
			{
				CSAntOutOfMemory();
			}
			strcpy(copyName, "csant.env.");
			ILMemCpy(copyName + 10, *temp, (int)(split - *temp));
			copyName[10 + (int)(split - *temp)] = '\0';
			CSAntDefineProperty(copyName, -1, split + 1, 0);
			ILFree(copyName);
		}
		++temp;
	}
}

/*
 * Forward declaration.
 */
static int BuildTarget(const char *name, CSAntTarget *target);

/*
 * Process a specific task.  Returns zero on failure.
 */
static int ProcessTask(CSAntTask *task)
{
	const char *arg;
	int posn;

	/* Trap the "call" task as a special case */
	if(!strcmp(task->name, "call"))
	{
		arg = CSAntTaskParam(task, "target");
		if(!arg)
		{
			fprintf(stderr, "<call> missing target argument\n");
			return 0;
		}
		return BuildTarget(arg, 0);
	}

	/* Look for the task in the global name table */
	for(posn = 0; posn < CSAntNumTasks; ++posn)
	{
		if(!strcmp(CSAntTasks[posn].name, task->name))
		{
			/* Validate that the task can be executed in the
			   install/uninstall modes of csant */
			if((!CSAntInstallMode && !CSAntUninstallMode) ||
			   CSAntTasks[posn].installMode)
			{
				return (*(CSAntTasks[posn].func))(task);
			}
			else
			{
				/* Ignore non-installable tasks in install/uninstall mode */
				return 1;
			}
		}
	}

	/* Could not determine how to process this task */
	fprintf(stderr, "unknown task element type <%s>\n", task->name);
	return 0;
}

/*
 * Build a specific target.  Returns zero on failure.
 */
static int BuildTarget(const char *name, CSAntTarget *target)
{
	int success = 1;
	int error = 0;
	int posn;
	CSAntTask *task;

	/* Find the target by name */
	if(!target)
	{
		target = CSAntFindTarget(name);
		if(!target)
		{
			/* If the global target doesn't exist, it is OK */
			if(!name)
			{
				return 1;
			}
			fprintf(stderr, "%s: no such target\n", name);
			return 0;
		}
	}

	/* Trap cycles in the build order */
	if(target->built)
	{
		return 1;
	}
	target->built = 1;

	/* Print a debug message indicating the target that we are building */
	if(!CSAntSilent)
	{
		if(target->name)
		{
			if(CSAntProjectName)
			{
				printf("Building target `%s' for project `%s'\n",
					   target->name, CSAntProjectName);
			}
			else
			{
				printf("Building target `%s'\n", target->name);
			}
		}
	}

	/* Build all of the targets that we depend upon */
	for(posn = 0; !error && posn < target->numDependsOn; ++posn)
	{
		if(!BuildTarget(0, target->dependsOn[posn]))
		{
			if(CSAntKeepGoing)
			{
				success = 0;
			}
			else
			{
				error = 1;
			}
		}
	}

	/* Process the tasks within the target */
	task = target->tasks;
	while(!error && task != 0)
	{
		if(!ProcessTask(task))
		{
			if(CSAntKeepGoing)
			{
				success = 0;
			}
			else
			{
				error = 1;
			}
		}
		task = task->next;
	}

	/* Print a debug message indicating the target that we are done with */
	if(!CSAntSilent)
	{
		if(!error && success)
		{
			if(target->name)
			{
				if(CSAntProjectName)
				{
					printf("Leaving target `%s' for project `%s'\n",
						   target->name, CSAntProjectName);
				}
				else
				{
					printf("Leaving target `%s'\n", target->name);
				}
			}
		}
		else
		{
			if(target->name)
			{
				if(CSAntProjectName)
				{
					printf("*** Target `%s' for project `%s' failed ***\n",
						   target->name, CSAntProjectName);
				}
				else
				{
					printf("*** Target `%s' failed ***\n", target->name);
				}
			}
		}
	}

	/* Done */
	if(error)
	{
		return 0;
	}
	else
	{
		return success;
	}
}

int CSAntBuild(const char *buildFilename)
{
	int posn;
	int success = 1;
	int error = 0;

	/* Initialize standard properties */
	StdProps(buildFilename);

	/* Set the default build target */
	if(!numTargets && CSAntDefaultTarget)
	{
		CSAntAddBuildTarget(CSAntDefaultTarget);
	}

	/* Print a debug message indicating the project that we are building */
	if(!CSAntSilent)
	{
		if(CSAntProjectName)
		{
			printf("Building project `%s'\n", CSAntProjectName);
		}
		else
		{
			printf("Building project\n");
		}
	}

	/* Build the global target */
	if(!BuildTarget(0, 0))
	{
		if(CSAntKeepGoing)
		{
			success = 0;
		}
		else
		{
			error = 1;
		}
	}

	/* Build the specified targets */
	for(posn = 0; !error && posn < numTargets; ++posn)
	{
		if(!BuildTarget(targets[posn], 0))
		{
			if(CSAntKeepGoing)
			{
				success = 0;
			}
			else
			{
				error = 1;
			}
		}
	}

	/* Print a debug message indicating the project that we are done with */
	if(!CSAntSilent)
	{
		if(!error && success)
		{
			if(CSAntProjectName)
			{
				printf("Ending project `%s'\n", CSAntProjectName);
			}
			else
			{
				printf("Ending project\n");
			}
		}
		else
		{
			if(CSAntProjectName)
			{
				printf("*** Project `%s' build failed ***\n",
					   CSAntProjectName);
			}
			else
			{
				printf("*** Project build failed ***\n");
			}
		}
	}

	/* Done */
	if(error)
	{
		return 0;
	}
	else
	{
		return success;
	}
}

#ifdef	__cplusplus
};
#endif
