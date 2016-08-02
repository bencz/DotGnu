/*
 * csant_task.c - Dispatch functions for csant task elements.
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
#include "il_errno.h"
#ifdef HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#ifdef HAVE_SYS_STAT_H
#include <sys/stat.h>
#endif
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif
#include <errno.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Handle a property set task.
 */
static int Task_Property(CSAntTask *task)
{
	const char *name;
	const char *value;
	name = CSAntTaskParam(task, "name");
	value = CSAntTaskParam(task, "value");
	if(!name)
	{
		fprintf(stderr, "<property> element with no property name\n");
		return 0;
	}
	if(!value)
	{
		fprintf(stderr, "%s: no value specified for property\n", name);
	}
	CSAntDefineProperty(name, -1, value, 0);
	return 1;
}

/*
 * Echo a message to stdout.
 */
static int Task_Echo(CSAntTask *task)
{
	const char *msg = CSAntTaskParam(task, "message");
	if(!msg)
	{
		fprintf(stderr, "<echo> element with no message\n");
		return 0;
	}
	puts(msg);
	return 1;
}

/*
 * Echo a message to stdout and fail the build.
 */
static int Task_Fail(CSAntTask *task)
{
	const char *msg = CSAntTaskParam(task, "message");
	if(msg)
	{
		puts(msg);
	}
	return CSAntJustPrint;
}

/*
 * Do an "rm -r" on a directory.  Returns 0 on success, 1 on fail.
 * If it fails, fileFile may contain an ILMalloc'ed string to the
 * file that caused it to fail.  Then again, it may just contain 0.
 */
static int Rm_R(const char* dir, char** failFile)
{
	int		error;
  	ILDir*		ilDir;
	ILDirEnt*	ilDirEnt;
	char*		pathname;
	int		retval;

	ilDir = ILOpenDir(dir);
	if(!ilDir)
	{
	  	*failFile = ILDupString(dir);
		goto failed;
	}
	/*
	 * Loop through all files in the directory.
	 */
	while((ilDirEnt = ILReadDir(ilDir)))
	{
	  	/*
		 * Ignore '.' and '..'.
		 */
	  	if(!strcmp(ILDirEntName(ilDirEnt), ".") || !strcmp(ILDirEntName(ilDirEnt), ".."))
		{
		  	continue;
		}
	  	/*
		 * We need the full path name, so cat the directory name
		 * and path name together.
		 */
	  	pathname = CSAntDirCombine(dir, ILDirEntName(ilDirEnt));
		if(!pathname)
		{
		  	goto failed;
		}
		/*
		 * Normal file or sub-directory?
		 */
	  	if(ILDirEntType(ilDirEnt) == ILFileType_REG)
		{
		  	retval = ILDeleteFile(pathname);
			if(retval)
			{
			  	*failFile = pathname;
				goto failed;
			}
		}
		else
		{
		  	retval = Rm_R(pathname, failFile);
			if(retval)
			{
				ILFree(pathname);
			  	goto failed;
			}
		}
		ILFree(pathname);
	}
	/*
	 * Done - close the directory iterator and delete the directory.
	 */
	retval = ILCloseDir(ilDir);
	ilDir = 0;
	if(retval)
	{
		retval = ILDeleteDir(dir);
	}
	if(retval)
	{
	  	*failFile = ILDupString(dir);
		goto failed;
	}
	return 0;

	/*
	 * Opps - something went wrong.  Free all resources we have open.
	 */
failed:
	error = errno;
	if(ilDir != 0)
	  	ILCloseDir(ilDir);
	errno = error;
	return 1;
}

/*
 * Delete a file from the build (for "clean" targets mainly)
 */
static int Task_Delete(CSAntTask *task)
{
	const char *dirParam = CSAntTaskParam(task, "dir");
	const char *fileParam = CSAntTaskParam(task, "file");
	const char *failnoerror = CSAntTaskParam(task, "failonerror");
	char *dir;
	char *file;
	char* failFile;
	int retval;
	if(!fileParam && !dirParam)
	{
		fprintf(stderr, "no file or dir to delete in <delete>\n");
		return 0;
	}
	if(fileParam)
	{
		file = CSAntDirCombine(CSAntBaseBuildDir, fileParam);
		if(!CSAntSilent)
		{
			if(!failnoerror || ILStrICmp(failnoerror, "true") != 0)
			{
				printf("rm -f %s\n", file);
			}
			else
			{
				printf("rm %s\n", file);
			}
		}
		retval = ILDeleteFile(file);
		if(retval && failnoerror && !ILStrICmp(failnoerror, "true"))
		{
			perror(file);
			ILFree(file);
			return 0;
		}
		ILFree(file);
	}
	if(dirParam)
	{
		dir = CSAntDirCombine(CSAntBaseBuildDir, dirParam);
		if(!CSAntSilent)
		{
			if(!failnoerror || ILStrICmp(failnoerror, "true") != 0)
			{
				printf("rm -rf %s\n", dir);
			}
			else
			{
				printf("rm -r %s\n", dir);
			}
		}
		failFile = 0;
		retval = Rm_R(dir, &failFile);
		if(retval && failnoerror && !ILStrICmp(failnoerror, "true"))
		{
			perror(failFile ? failFile : dir);
			if(failFile)
			{
			  	ILFree(dir);
				ILFree(failFile);
			}
			return 0;
		}
		if(failFile)
		{
		  	ILFree(failFile);
		}
		ILFree(dir);
	}
	return 1;
}

/*
 * Make a directory, and all parent directories.
 */
static int Mkdir_P(const char *base, const char *dir)
{
      char *path = CSAntDirCombine(base, dir);
      char* slashpos = &path[strlen(base)];
      char* bslashpos;
      int retval = 0;
      char save;

      if(!path)
      {
		return errno;
      }
      while(*slashpos != '\0')
      {
		bslashpos = strchr(slashpos + 1, '\\');
		slashpos = strchr(slashpos + 1, '/');
		if (slashpos == 0 || (bslashpos != 0 && slashpos > bslashpos))
		  	slashpos = bslashpos;
		if (slashpos == 0)
		  	slashpos = strchr(path, '\0');
		save = *slashpos;
		*slashpos = '\0';
		retval = ILCreateDir(path);
		*slashpos = save;
		if (retval != IL_ERRNO_Success && retval != IL_ERRNO_EEXIST)
		  	break;
      }
      if(retval == IL_ERRNO_EEXIST)
		retval = IL_ERRNO_Success;
      ILFree(path);
      return retval;
}

/*
 * Make a directory.
 */
static int Task_Mkdir(CSAntTask *task)
{
	const char *dirParam = CSAntTaskParam(task, "dir");
	char *dir;
	int retval;
	if(!dirParam)
	{
		fprintf(stderr, "no directory to make in <mkdir>\n");
		return 0;
	}
	dir = CSAntDirCombine(CSAntBaseBuildDir, dirParam);
	if(!CSAntSilent)
	{
		printf("mkdir -p %s\n", dir);
	}
	retval = Mkdir_P(CSAntBaseBuildDir, dirParam);
	if(retval != IL_ERRNO_Success)
	{
		ILSysIOSetErrno(retval);
		perror(dir);
		ILFree(dir);
		return 0;
	}
	ILFree(dir);
	return 1;
}

/*
 * Copy a file.
 */
static int Task_Copy(CSAntTask *task)
{
	const char *fromfileParam = CSAntTaskParam(task, "file");
	const char *tofileParam = CSAntTaskParam(task, "tofile");
	const char* todirParam = CSAntTaskParam(task, "todir");
	char *fromfile;
	char *topath;
	const char *slashpos;
	const char *bslashpos;
	FILE *instream;
	FILE *outstream;
	char buffer[BUFSIZ];
	int len;
	char *path;
	int retval;

	/* Validate the parameters */
	if(!fromfileParam)
	{
		fprintf(stderr, "no source file in <copy>\n");
		return 0;
	}
	if(!tofileParam && !todirParam)
	{
		fprintf(stderr, "no destination in <copy>\n");
		return 0;
	}
	if(tofileParam && todirParam)
	{
		fprintf(stderr, "ambigious destination in <copy>\n");
		return 0;
	}
	fromfile = CSAntDirCombine(CSAntBaseBuildDir, fromfileParam);
	if (!fromfile)
	{
	  	return 0;
	}
	if (tofileParam)
	{
	  	topath = CSAntDirCombine(CSAntBaseBuildDir, tofileParam);
	}
	else
	{
	  	path = CSAntDirCombine(CSAntBaseBuildDir, todirParam);
		if (!path)
		  	topath = 0;
	        else
		{
			retval = Mkdir_P(CSAntBaseBuildDir, todirParam);
			if (retval != IL_ERRNO_Success)
			{
				ILSysIOSetErrno(retval);
				perror(path);
				ILFree(path);
				ILFree(fromfile);
				return 0;

			}
			slashpos = strrchr(fromfileParam, '/');
			bslashpos = strrchr(fromfileParam, '\\');
			if (bslashpos > slashpos)
			  	slashpos = bslashpos;
			if (slashpos == 0)
			  	slashpos = fromfileParam;
			else
			  	slashpos += 1;
			topath = CSAntDirCombine(path, slashpos);
			ILFree(path);
		}
	}
	if (!topath)
	{
		ILFree(fromfile);
	  	return 0;
	}

	/* Report the command that we will be executing */
	if(!CSAntSilent)
	{
		printf("cp %s %s\n", fromfile, topath);
	}

	/* Attempt to open the source file */
	if((instream = fopen(fromfile, "rb")) == NULL)
	{
		if((instream = fopen(fromfile, "r")) == NULL)
		{
			perror(fromfile);
		  	ILFree(fromfile);
			ILFree(topath);
			return 0;
		}
	}

	/* Attempt to open the destination file */
	if((outstream = fopen(topath, "wb")) == NULL)
	{
		if((outstream = fopen(topath, "w")) == NULL)
		{
			perror(topath);
			fclose(instream);
		  	ILFree(fromfile);
			ILFree(topath);
			return 0;
		}
	}

	/* Copy the file's contents */
	while((len = (int)fread(buffer, 1, BUFSIZ, instream)) >= BUFSIZ)
	{
		fwrite(buffer, 1, len, outstream);
	}
	if(len > 0)
	{
		fwrite(buffer, 1, len, outstream);
	}
	if(ferror(outstream))
	{
		perror(topath);
		fclose(instream);
		fclose(outstream);
		ILDeleteFile(topath);
		ILFree(fromfile);
		ILFree(topath);
		return 0;
	}

	/* Done */
	fclose(instream);
	fclose(outstream);
	ILFree(fromfile);
	ILFree(topath);
	return 1;
}

/*
 * Invoke a sub-process containing another invocation of "csant".
 */
static int Task_CSAnt(CSAntTask *task)
{
	char *baseSrcDir;
	char *baseBuildDir;
	const char *buildFile;
	const char *target;
	const char *compiler;
	char *argv[40];
	int argc, posn;
	int result;

	/* Adjust the base directories if necessary */
	baseSrcDir = CSAntDirCombine
		(CSAntBaseSrcDir, CSAntTaskParam(task, "basedir"));
	baseBuildDir = CSAntDirCombine
		(CSAntBaseBuildDir, CSAntTaskParam(task, "basedir"));

	/* Locate the new build file and target */
	buildFile = CSAntTaskParam(task, "buildfile");
	target = CSAntTaskParam(task, "target");

	/* Locate the compiler, which may have been overridden by properties */
	compiler = CSAntGetProperty("csant.compiler", -1);

	/* Construct the command-line to be spawned */
	argv[0] = CSAntGetProgramName();
	argc = 1;
	if(baseSrcDir)
	{
		argv[argc++] = "-b";
		argv[argc++] = baseSrcDir;
	}
	if(baseBuildDir)
	{
		argv[argc++] = "-B";
		argv[argc++] = baseBuildDir;
	}
	if(buildFile)
	{
		argv[argc++] = "-f";
		argv[argc++] = (char *)buildFile;
	}
	if(CSAntKeepGoing)
	{
		argv[argc++] = "-k";
	}
	if(CSAntJustPrint)
	{
		argv[argc++] = "-n";
	}
	if(CSAntSilent)
	{
		argv[argc++] = "-s";
	}
	if(CSAntRedirectCsc)
	{
		argv[argc++] = "-c";
	}
	if(CSAntForceCorLib)
	{
		argv[argc++] = "-m";
	}
	if(CSAntInstallMode)
	{
		argv[argc++] = "-i";
	}
	if(CSAntUninstallMode)
	{
		argv[argc++] = "-u";
	}
	argv[argc++] = "-C";
	argv[argc++] = (char *)compiler;
	if(target)
	{
		argv[argc++] = (char *)target;
	}
	argv[argc] = 0;

	/* Print the spawn command-line */
	if(!CSAntSilent)
	{
		for(posn = 0; posn < argc; ++posn)
		{
			fputs(argv[posn], stdout);
			if(posn < (argc - 1))
			{
				putc(' ', stdout);
			}
		}
		putc('\n', stdout);
	}

	/* Spawn the sub-process */
	result = (ILSpawnProcess(argv) == 0);

	/* Clean up and exit */
	ILFree(baseSrcDir);
	ILFree(baseBuildDir);
	return result;
}

CSAntTaskInfo const CSAntTasks[] = {
	{"compile",			1, CSAntTask_Compile},
	{"cscc",			1, CSAntTask_Cscc},
	{"csc",				1, CSAntTask_Csc},
	{"mcs",				1, CSAntTask_Mcs},
	{"csdoc",			0, CSAntTask_Csdoc},
	{"resgen",			0, CSAntTask_ResGen},
	{"reslink",			1, CSAntTask_ResLink},
	{"property",		1, Task_Property},
	{"echo",			0, Task_Echo},
	{"fail",			0, Task_Fail},
	{"delete",			0, Task_Delete},
	{"mkdir",			0, Task_Mkdir},
	{"copy",			0, Task_Copy},
	{"csant",			1, Task_CSAnt},
	{"nant",			1, Task_CSAnt},
};
int const CSAntNumTasks = (sizeof(CSAntTasks) / sizeof(CSAntTaskInfo));

#ifdef	__cplusplus
};
#endif
