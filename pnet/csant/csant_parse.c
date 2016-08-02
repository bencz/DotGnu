/*
 * csant_parse.c - Parse the contents of a project build file.
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

#include "csant_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

char *CSAntBaseSrcDir    = 0;
char *CSAntBaseBuildDir  = 0;
char *CSAntProjectName   = 0;
char *CSAntDefaultTarget = 0;

CSAntTarget *CSAntTargetList = 0;
char **CSAntProfileDefines = 0;
char **CSAntProfileValues = 0;
int CSAntNumProfileDefines = 0;

/*
 * Find the descriptor for a particular target, or create
 * a new descriptor if necessary.
 */
static CSAntTarget *FindTarget(const char *target)
{
	CSAntTarget *temp;

	/* Do we have an existing target with this name? */
	temp = CSAntTargetList;
	while(temp != 0)
	{
		if(temp->name && target && !strcmp(temp->name, target))
		{
			return temp;
		}
		else if(!(temp->name) && !target)
		{
			return temp;
		}
		temp = temp->next;
	}

	/* Allocate a new target descriptor */
	temp = (CSAntTarget *)ILMalloc(sizeof(CSAntTarget));
	if(!temp)
	{
		CSAntOutOfMemory();
	}
	if(target)
	{
		temp->name = ILDupString(target);
		if(!(temp->name))
		{
			CSAntOutOfMemory();
		}
	}
	else
	{
		temp->name = 0;
	}
	temp->tasks = 0;
	temp->lastTask = 0;
	temp->dependsOn = 0;
	temp->numDependsOn = 0;
	temp->built = 0;
	temp->next = CSAntTargetList;
	CSAntTargetList = temp;
	return temp;
}

/*
 * Parse a particular task.
 */
static CSAntTask *ParseTask(ILXMLReader *reader)
{
	CSAntTask *task;
	CSAntTask *lastChild;
	CSAntTask *child;
	int paramLen;
	ILXMLItem item;

	/* Allocate space for the task block */
	paramLen = ILXMLGetPackedSize(reader);
	task = (CSAntTask *)ILMalloc(sizeof(CSAntTask) + paramLen - 1);
	if(!task)
	{
		CSAntOutOfMemory();
	}

	/* Copy the task parameters into the block */
	ILXMLGetPacked(reader, task->params);

	/* Initialize the other task fields */
	task->next = 0;
	task->name = ILDupString(ILXMLTagName(reader));
	if(!(task->name))
	{
		CSAntOutOfMemory();
	}
	task->taskChildren = 0;
	task->paramLen = paramLen;

	/* Parse the children of the task */
	if(ILXMLGetItem(reader) == ILXMLItem_StartTag)
	{
		lastChild = 0;
		while((item = ILXMLReadNext(reader)) != ILXMLItem_EOF &&
			  item != ILXMLItem_EndTag)
		{
			if(item == ILXMLItem_StartTag || item == ILXMLItem_SingletonTag)
			{
				child = ParseTask(reader);
				if(lastChild)
				{
					lastChild->next = child;
				}
				else
				{
					task->taskChildren = child;
				}
				lastChild = child;
			}
			else
			{
				ILXMLSkip(reader);
			}
		}
	}

	/* Done */
	return task;
}

/*
 * Parse a task for a particular target.  "target" is NULL
 * for the global target level.
 */
static void ParseTaskForTarget(ILXMLReader *reader, char *target)
{
	CSAntTarget *desc = FindTarget(target);
	CSAntTask *task = ParseTask(reader);
	if(desc->lastTask)
	{
		desc->lastTask->next = task;
	}
	else
	{
		desc->tasks = task;
	}
	desc->lastTask = task;
}

/*
 * Determine if a target depends upon another, either
 * directly or indirectly.  This is used to detect cycles.
 */
static int IncludesDependency(CSAntTarget *desc1, CSAntTarget *desc2)
{
	int posn;
	if(desc1 == desc2)
	{
		return 1;
	}
	for(posn = 0; posn < desc1->numDependsOn; ++posn)
	{
		if(IncludesDependency(desc1->dependsOn[posn], desc2))
		{
			return 1;
		}
	}
	return 0;
}

/*
 * Record a dependency between two targets.  Returns
 * zero if a cycle has been detected.
 */
static int RecordDependency(char *target, const char *dependsOn)
{
	const char* pos;
  	const char* commaPosition;
	char *dependsFileName;
	CSAntTarget *desc1;
	CSAntTarget *desc2;
	
	for (pos = dependsOn; ; pos = commaPosition + 1)
	{
	  	commaPosition = strchr(pos, ',');
		if(!commaPosition)
			commaPosition = strchr(pos, '\0');
		dependsFileName = ILDupNString(pos, commaPosition - pos);
		if (!dependsFileName)
		{
			CSAntOutOfMemory();
		}
		desc1 = FindTarget(target);
		desc2 = FindTarget(dependsFileName);
		ILFree(dependsFileName);
		if(IncludesDependency(desc2, desc1))
		{
			return 0;
		}
		desc1->dependsOn = (CSAntTarget **)ILRealloc
			(desc1->dependsOn, sizeof(CSAntTarget *) * (desc1->numDependsOn + 1));
		if(!(desc1->dependsOn))
		{
			CSAntOutOfMemory();
		}
		desc1->dependsOn[(desc1->numDependsOn)++] = desc2;
		if (*commaPosition == '\0')
			break;
	}
	return 1;
}

int CSAntParseFile(ILXMLReader *reader, const char *filename)
{
	ILXMLItem item;
	const char *arg;
	int targetLevel;
	char *targetName;

	/* Read the first item, which should be the top-level "project" tag */
	item = ILXMLReadNext(reader);
	if(!ILXMLIsTag(reader, "project"))
	{
		fprintf(stderr, "%s: could not locate the <project> element\n",
				filename);
		return 0;
	}

	/* Extract the project name */
	arg = ILXMLGetParam(reader, "name");
	if(arg)
	{
		CSAntProjectName = ILDupString(arg);
		if(!CSAntProjectName)
		{
			CSAntOutOfMemory();
		}
	}

	/* Extract the default build target name */
	arg = ILXMLGetParam(reader, "default");
	if(arg)
	{
		CSAntDefaultTarget = ILDupString(arg);
		if(!CSAntDefaultTarget)
		{
			CSAntOutOfMemory();
		}
	}

	/* Set the default base directories, if the command-line hasn't yet */
	if(!CSAntBaseSrcDir)
	{
		arg = ILXMLGetParam(reader, "srcdir");
		if(!arg)
		{
			arg = ILXMLGetParam(reader, "basedir");
		}
		if(arg)
		{
			CSAntBaseSrcDir = ILDupString(arg);
			if(!CSAntBaseSrcDir)
			{
				CSAntOutOfMemory();
			}
		}
		else
		{
			CSAntBaseSrcDir = ".";
		}
	}
	if(!CSAntBaseBuildDir)
	{
		arg = ILXMLGetParam(reader, "builddir");
		if(!arg)
		{
			arg = ILXMLGetParam(reader, "basedir");
		}
		if(arg)
		{
			CSAntBaseBuildDir = ILDupString(arg);
			if(!CSAntBaseBuildDir)
			{
				CSAntOutOfMemory();
			}
		}
		else
		{
			CSAntBaseBuildDir = ".";
		}
	}

	/* Parse the rules within the project */
	if(item == ILXMLItem_StartTag)
	{
		item = ILXMLReadNext(reader);
		targetLevel = 0;
		targetName = 0;
		while(item != ILXMLItem_EOF &&
			  (targetLevel > 0 || item != ILXMLItem_EndTag))
		{
			if(ILXMLIsTag(reader, "target"))
			{
				/* Start of a new target level */
				arg = ILXMLGetParam(reader, "name");
				if(arg)
				{
					targetName = ILDupString(arg);
					if(!targetName)
					{
						CSAntOutOfMemory();
					}
				}
				else
				{
					fprintf(stderr, "%s: <target> used without a name\n",
							filename);
					return 0;
				}
				arg = ILXMLGetParam(reader, "depends");
				if(arg)
				{
					if(!RecordDependency(targetName, arg))
					{
						fprintf(stderr,
								"%s: target dependencies contain a cycle\n",
								filename);
						return 0;
					}
				}
				++targetLevel;
				if(targetLevel > 1)
				{
					fprintf(stderr, "%s: cannot use nested <target> elements\n",
							filename);
					return 0;
				}
				if(item == ILXMLItem_SingletonTag)
				{
					/* No contents for this task, so go back out a level */
					--targetLevel;
					ILFree(targetName);
					targetName = 0;
				}
			}
			else if(item == ILXMLItem_StartTag ||
			        item == ILXMLItem_SingletonTag)
			{
				/* Parse a task for the current target */
				ParseTaskForTarget(reader, targetName);
			}
			else if(item == ILXMLItem_EndTag)
			{
				/* End of the current target level */
				--targetLevel;
				ILFree(targetName);
				targetName = 0;
			}
			else
			{
				/* Don't know what this is, so skip it */
				ILXMLSkip(reader);
			}
			item = ILXMLReadNext(reader);
		}
	}

	/* Done */
	return 1;
}

CSAntTarget *CSAntFindTarget(const char *target)
{
	CSAntTarget *temp = CSAntTargetList;
	while(temp != 0)
	{
		if(temp->name && target && !strcmp(temp->name, target))
		{
			return temp;
		}
		else if(!(temp->name) && !target)
		{
			return temp;
		}
		temp = temp->next;
	}
	return 0;
}

const char *CSAntGetProfileValue(const char *name, int len)
{
	int index;
	if(!name)
	{
		return 0;
	}
	for(index = 0; index < CSAntNumProfileDefines; index++)
	{
		if(!strncmp(name, CSAntProfileDefines[index], len) &&
		   CSAntProfileDefines[index][len] == '\0')
		{
			return CSAntProfileValues[index];
		}
	}
	return 0;
}

const char *CSAntTaskParam(CSAntTask *task, const char *name)
{
	const char *value;
	char *buffer;
	int bufLen;
	int bufMax;
	int nameLen;
	const char *propValue;
#define	ADD_BUFFER_CH(ch)	\
		do { \
			if(bufLen >= bufMax) \
			{ \
				if((buffer = (char *)ILRealloc(buffer, bufMax + 32)) == 0) \
				{ \
					CSAntOutOfMemory(); \
				} \
				bufMax += 32; \
			} \
			buffer[bufLen++] = (char)(ch); \
		} while (0)

	/* Get the parameter value */
	value = ILXMLGetPackedParam(task->params, task->paramLen, name);

	/* Bail out if no value, or we don't need any property substitution */
	if(!value || !strchr(value, '$'))
	{
		return value;
	}

	/* Perform property substitution on the value */
	bufLen = 0;
	bufMax = strlen(value) + 1;
	if((buffer = (char *)ILMalloc(bufMax)) == 0)
	{
		CSAntOutOfMemory();
	}
	while(*value != '\0')
	{
		if(*value =='$' && value[1] == '{')
		{
			value += 2;
			nameLen = 0;
			while(value[nameLen] != '\0' && value[nameLen] != '}')
			{
				++nameLen;
			}
			propValue = CSAntGetProperty(value, nameLen);
			if(propValue)
			{
				while(*propValue != '\0')
				{
					ADD_BUFFER_CH(*propValue);
					++propValue;
				}
			}
			else
			{
				propValue = CSAntGetProfileValue(value,nameLen);
				if(propValue)
				{
					while(*propValue != '\0')
					{
						ADD_BUFFER_CH(*propValue);
						++propValue;
					}
				}
			}
			if(value[nameLen] == '}')
			{
				value += nameLen + 1;
			}
			else
			{
				value += nameLen;
			}
		}
		else if(*value =='$' && value[1] == '$')
		{
			ADD_BUFFER_CH('$');
			value += 2;
		}
		else
		{
			ADD_BUFFER_CH(*value);
			++value;
		}
	}
	ADD_BUFFER_CH('\0');
	return buffer;
}

int CSAntParseProfileFile(ILXMLReader *reader, const char *filename)
{
	ILXMLItem item;
	const char *name;
	const char *value;
	char **newDefines;
	char **newValues;

	/* Read the first item, which should be the top-level "profile" tag */
	item = ILXMLReadNext(reader);
	if(!ILXMLIsTag(reader, "profile"))
	{
		fprintf(stderr, "%s: could not locate the <profile> element\n",
				filename);
		return 0;
	}

	/* Look for "define" tags within the "profile" tag */
	if(item == ILXMLItem_StartTag)
	{
		item = ILXMLReadNext(reader);
		while(item != ILXMLItem_EOF && item != ILXMLItem_EndTag)
		{
			if(ILXMLIsTag(reader, "define"))
			{
				/* Process a "define" tag */
				name = ILXMLGetParam(reader, "name");
				value = ILXMLGetParam(reader, "value");
				if(name && value)
				{
					newDefines = (char **)ILRealloc
						(CSAntProfileDefines,
						 (CSAntNumProfileDefines + 1) * sizeof(char *));
					if(!newDefines)
					{
						CSAntOutOfMemory();
					}
					if((newDefines[CSAntNumProfileDefines] =
							ILDupString(name)) == 0)
					{
						CSAntOutOfMemory();
					}
					CSAntProfileDefines = newDefines;
					newValues = (char **)ILRealloc
						(CSAntProfileValues,
						 (CSAntNumProfileDefines + 1) * sizeof(char *));
					if(!newValues)
					{
						CSAntOutOfMemory();
					}
					if((newValues[CSAntNumProfileDefines] =
							ILDupString(value)) == 0)
					{
						CSAntOutOfMemory();
					}
					CSAntProfileValues = newValues;
					++CSAntNumProfileDefines;
				}
			}
			ILXMLSkip(reader);
			item = ILXMLReadNext(reader);
		}
	}

	/* Done */
	return 1;
}

#ifdef	__cplusplus
};
#endif
