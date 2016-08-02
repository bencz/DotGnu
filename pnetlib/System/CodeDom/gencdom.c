/*
 * gencdom.c - Generate System.CodeDom classes from rules.txt.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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
#include <string.h>

/*
 * Forward declarations.
 */
static void processInput(FILE *stream, FILE *outstream);
static int parseRule(char *line);
static void generateCode(FILE *stream);
static void generateHeader(FILE *stream);
static void generateFooter(FILE *stream);

int main(int argc, char *argv[])
{
	generateHeader(stdout);
	processInput(stdin, stdout);
	generateFooter(stdout);
	return 0;
}

/*
 * Read a single line from an input stream and handle continuations.
 */
static int readLine(char *buffer, FILE *stream)
{
	int left = BUFSIZ;
	int len;
	while(fgets(buffer, left, stream))
	{
		len = strlen(buffer);
		while(len > 0 && (buffer[len - 1] == '\n' || buffer[len - 1] == '\r'))
		{
			--len;
		}
		buffer[len] = '\0';
		if(len > 0 && buffer[len - 1] == ':')
		{
			buffer += len;
			left -= len;
		}
		else if(len > 0 && buffer[len - 1] == '\\')
		{
			--len;
			buffer[len] = '\0';
			buffer += len;
			left -= len;
		}
		else
		{
			return 1;
		}
	}
	return 0;
}

/*
 * Process input from a stream.
 */
static void processInput(FILE *stream, FILE *outstream)
{
	char buffer[BUFSIZ];
	while(readLine(buffer, stream))
	{
		if(parseRule(buffer))
		{
			generateCode(outstream);
		}
	}
}

/*
 * Determine if a string ends in a specific tail.
 */
static int endsIn(const char *str1, const char *str2)
{
	int len1 = strlen(str1);
	int len2 = strlen(str2);
	if(len1 > len2 && !strncmp(str1 + len1 - len2, str2, len2))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Parsed contents of a rule.
 */
static char *name;
static char *parent;
static char *contentType[16];
static char *contentName[16];
static int contentIsParams[16];
static int contentIsOptional[16];
static int specialConstructors;
static int numContents;

/*
 * Parse a rule line from an input stream.  Rules look like this:
 *
 * CodeBinaryOperatorExpression:
 *		CodeExpression<Left> CodeBinaryOperatorType<Operator> \
 *		CodeExpression<Right>
 */
static int parseRule(char *line)
{
	int implicit;
	char *contents;

	/* Clear the rule information */
	name = "";
	parent = "";
	numContents = 0;
	specialConstructors = 1;

	/* Skip leading white space */
	while(*line != '\0' && (*line == ' ' || *line == '\t' ||
							*line == '\r' || *line == '\n'))
	{
		++line;
	}
	if(*line == '\0' || *line == '#')
	{
		/* Comment line */
		return 0;
	}

	/* Extract the rule name */
	name = line;
	while(*line != '\0' && *line != '-' && *line != ':')
	{
		++line;
	}

	/* Extract the parent class, if it isn't implicit */
	if(*line == '-')
	{
		*line++ = '\0';
		parent = line;
		while(*line != '\0' && *line != ':')
		{
			++line;
		}
		if(*line == ':')
		{
			*line++ = '\0';
		}
		implicit = 0;
	}
	else if(*line == ':')
	{
		*line++ = '\0';
		implicit = 1;
	}
	else
	{
		implicit = 1;
	}
	if(implicit)
	{
		if(endsIn(name, "Expression"))
		{
			parent = "CodeExpression";
		}
		else if(endsIn(name, "Statement"))
		{
			parent = "CodeStatement";
		}
		else if(endsIn(name, "Collection"))
		{
			parent = "CollectionBase";
		}
		else
		{
			parent = "CodeObject";
		}
	}

	/* Extract the contents of the rule */
	while(*line != '\0')
	{
		if(*line == ' ' || *line == '\t' || *line == '\r' || *line == '\n')
		{
			*line++ = '\0';
		}
		else
		{
			/* Extract the contents entry */
			contents = line;
			while(*line != '\0' && *line != ' ' && *line != '\t' &&
			      *line != '\r' && *line != '\n')
			{
				++line;
			}
			if(*line != '\0')
			{
				*line++ = '\0';
			}

			/* Parse the entry into "type<name>" */
			if(*contents == '$')
			{
				specialConstructors = 0;
				++contents;
			}
			if(*contents == '?')
			{
				contentIsOptional[numContents] = 1;
				++contents;
			}
			else
			{
				contentIsOptional[numContents] = 0;
			}
			if(*contents == '*')
			{
				contentIsParams[numContents] = 1;
				++contents;
			}
			else if(*contents == '@')
			{
				contentIsParams[numContents] = 2;
				++contents;
			}
			else
			{
				contentIsParams[numContents] = 0;
			}
			contentType[numContents] = contents;
			while(*contents != '\0' && *contents != '<')
			{
				++contents;
			}
			if(*contents == '<')
			{
				*contents++ = '\0';
				contentName[numContents] = contents;
				while(*contents != '\0' && *contents != '>')
				{
					++contents;
				}
				*contents = '\0';
			}
			else
			{
				contentName[numContents] = "";
			}
			++numContents;
		}
	}

	/* Done */
	return 1;
}

/*
 * Generate code for a collection class.
 */
static void generateCollectionCode(FILE *stream)
{
	char *member = contentType[0];

	/* Output the constructors */
	fprintf(stream, "\t// Constructors.\n");
	fprintf(stream, "\tpublic %s()\n", name);
	fprintf(stream, "\t{\n\t}\n");
	fprintf(stream, "\tpublic %s(%s[] value)\n", name, member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tAddRange(value);\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic %s(%s value)\n", name, name);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tAddRange(value);\n");
	fprintf(stream, "\t}\n\n");

	/* Output the properties */
	fprintf(stream, "\t// Properties.\n");
	fprintf(stream, "\tpublic %s this[int index]\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tget\n");
	fprintf(stream, "\t\t{\n");
	fprintf(stream, "\t\t\treturn (%s)(List[index]);\n", member);
	fprintf(stream, "\t\t}\n");
	fprintf(stream, "\t\tset\n");
	fprintf(stream, "\t\t{\n");
	fprintf(stream, "\t\t\tList[index] = value;\n");
	fprintf(stream, "\t\t}\n");
	fprintf(stream, "\t}\n\n");

	/* Output the methods */
	fprintf(stream, "\t// Methods.\n");
	fprintf(stream, "\tpublic int Add(%s value)\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\treturn List.Add(value);\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic void AddRange(%s[] value)\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tforeach(%s e in value)\n", member);
	fprintf(stream, "\t\t{\n");
	fprintf(stream, "\t\t\tList.Add(e);\n");
	fprintf(stream, "\t\t}\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic void AddRange(%s value)\n", name);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tforeach(%s e in value)\n", member);
	fprintf(stream, "\t\t{\n");
	fprintf(stream, "\t\t\tList.Add(e);\n");
	fprintf(stream, "\t\t}\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic bool Contains(%s value)\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\treturn List.Contains(value);\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic void CopyTo(%s[] array, int index)\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tList.CopyTo(array, index);\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic int IndexOf(%s value)\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\treturn List.IndexOf(value);\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic void Insert(int index, %s value)\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tList.Insert(index, value);\n");
	fprintf(stream, "\t}\n");
	fprintf(stream, "\tpublic void Remove(%s value)\n", member);
	fprintf(stream, "\t{\n");
	fprintf(stream, "\t\tint index = List.IndexOf(value);\n");
	fprintf(stream, "\t\tif(index < 0)\n");
	fprintf(stream, "\t\t{\n");
	fprintf(stream, "\t\t\tthrow new ArgumentException(S._(\"Arg_NotCollMember\"), \"value\");\n");
	fprintf(stream, "\t\t}\n");
	fprintf(stream, "\t\tList.RemoveAt(index);\n");
	fprintf(stream, "\t}\n\n");

	/* Output the class footer */
	fprintf(stream, "}; // class %s\n\n", name);
}

/*
 * Generate code for a specific constructor, if enabled.
 */
static void generateConstructor(FILE *stream, unsigned mask)
{
	int posn;
	int needComma;

	/* Determine if this constructor is enabled */
	for(posn = 0; posn < numContents; ++posn)
	{
		if(!(contentIsOptional[posn]) && (mask & (1 << posn)) == 0)
		{
			return;
		}
	}

	/* Generate the constructor */
	fprintf(stream, "\tpublic %s(", name);
	needComma = 0;
	for(posn = 0; posn < numContents; ++posn)
	{
		if((mask & (1 << posn)) == 0)
		{
			continue;
		}
		if(needComma)
		{
			fputs(", ", stream);
		}
		if(contentIsParams[posn] == 1)
		{
			fprintf(stream, "params %s[] _%s", contentType[posn],
				    contentName[posn]);
		}
		else if(contentIsParams[posn] == 2)
		{
			fprintf(stream, "%s[] _%s", contentType[posn],
				    contentName[posn]);
		}
		else
		{
			fprintf(stream, "%s _%s", contentType[posn],
				    contentName[posn]);
		}
		needComma = 1;
	}
	fprintf(stream, ")\n\t{\n");
	for(posn = 0; posn < numContents; ++posn)
	{
		if((mask & (1 << posn)) == 0)
		{
			continue;
		}
		if(contentIsParams[posn])
		{
			fprintf(stream, "\t\tthis.%s.AddRange(_%s);\n",
					contentName[posn], contentName[posn]);
		}
		else
		{
			fprintf(stream, "\t\tthis._%s = _%s;\n",
					contentName[posn], contentName[posn]);
		}
	}
	fprintf(stream, "\t}\n");
}

/*
 * Generate code for a rule class.
 */
static void generateCode(FILE *stream)
{
	int posn;
	unsigned mask, maxMask;

	/* Output the class header */
	fprintf(stream, "[Serializable]\n");
	fprintf(stream, "#if CONFIG_COM_INTEROP\n");
	fprintf(stream, "[ClassInterface(ClassInterfaceType.AutoDispatch)]\n");
	fprintf(stream, "[ComVisible(true)]\n");
	fprintf(stream, "#endif\n");
	fprintf(stream, "public class %s : %s\n", name, parent);
	fprintf(stream, "{\n");
	fprintf(stream, "\n");

	/* We need to use a different algorithm if this is a collection */
	if(!strcmp(parent, "CollectionBase"))
	{
		generateCollectionCode(stream);
		return;
	}

	/* Output the instance fields */
	if(numContents > 0)
	{
		fprintf(stream, "\t// Internal state.\n");
		for(posn = 0; posn < numContents; ++posn)
		{
			if(contentIsParams[posn])
			{
				fprintf(stream, "\tprivate %sCollection _%s;\n",
						contentType[posn], contentName[posn]);
			}
			else
			{
				fprintf(stream, "\tprivate %s _%s;\n",
						contentType[posn], contentName[posn]);
			}
		}
		fprintf(stream, "\n");
	}

	/* Output the constructors */
	fprintf(stream, "\t// Constructors.\n");
	fprintf(stream, "\tpublic %s()\n", name);
	fprintf(stream, "\t{\n\t}\n");
	if(numContents > 0 && specialConstructors)
	{
		maxMask = (unsigned)(1 << numContents);
		for(mask = 0; mask < maxMask; ++mask)
		{
			generateConstructor(stream, mask);
		}
	}
	fprintf(stream, "\n");

	/* Output the property accessors */
	if(numContents > 0)
	{
		fprintf(stream, "\t// Properties.\n");
		for(posn = 0; posn < numContents; ++posn)
		{
			if(!specialConstructors &&
			   endsIn(contentType[posn], "Collection"))
			{
				fprintf(stream, "\tpublic %s %s\n",
						contentType[posn], contentName[posn]);
				fprintf(stream, "\t{\n\t\tget\n");
				fprintf(stream, "\t\t{\n");
				fprintf(stream, "\t\t\tif(_%s == null)\n", contentName[posn]);
				fprintf(stream, "\t\t\t{\n");
				fprintf(stream, "\t\t\t\t_%s = new %s();\n",
						contentName[posn], contentType[posn]);
				fprintf(stream, "\t\t\t}\n");
				fprintf(stream, "\t\t\treturn _%s;\n", contentName[posn]);
				fprintf(stream, "\t\t}\n");
				fprintf(stream, "\t}\n");
			}
			else if(contentIsParams[posn])
			{
				fprintf(stream, "\tpublic %sCollection %s\n",
						contentType[posn], contentName[posn]);
				fprintf(stream, "\t{\n\t\tget\n");
				fprintf(stream, "\t\t{\n");
				fprintf(stream, "\t\t\tif(_%s == null)\n", contentName[posn]);
				fprintf(stream, "\t\t\t{\n");
				fprintf(stream, "\t\t\t\t_%s = new %sCollection();\n",
						contentName[posn], contentType[posn]);
				fprintf(stream, "\t\t\t}\n");
				fprintf(stream, "\t\t\treturn _%s;\n", contentName[posn]);
				fprintf(stream, "\t\t}\n");
				fprintf(stream, "\t}\n");
			}
			else
			{
				fprintf(stream, "\tpublic %s %s\n",
						contentType[posn], contentName[posn]);
				fprintf(stream, "\t{\n\t\tget\n");
				fprintf(stream, "\t\t{\n");
				fprintf(stream, "\t\t\treturn _%s;\n", contentName[posn]);
				fprintf(stream, "\t\t}\n");
				fprintf(stream, "\t\tset\n");
				fprintf(stream, "\t\t{\n");
				fprintf(stream, "\t\t\t_%s = value;\n", contentName[posn]);
				fprintf(stream, "\t\t}\n");
				fprintf(stream, "\t}\n");
			}
		}
		fprintf(stream, "\n");
	}

	/* Output the class footer */
	fprintf(stream, "}; // class %s\n\n", name);
}

/*
 * Standard copyright message to add to the front of the output.
 */
static char const COPYRIGHT_MSG[] =
	"/*\n"
	" * This file is generated from rules.txt using gencdom - do not edit.\n"
	" *\n"
	" * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.\n"
	" *\n"
	" * This program is free software; you can redistribute it and/or modify\n"
	" * it under the terms of the GNU General Public License as published by\n"
	" * the Free Software Foundation; either version 2 of the License, or\n"
	" * (at your option) any later version.\n"
	" *\n"
	" * This program is distributed in the hope that it will be useful,\n"
	" * but WITHOUT ANY WARRANTY; without even the implied warranty of\n"
	" * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n"
	" * GNU General Public License for more details.\n"
	" *\n"
	" * You should have received a copy of the GNU General Public License\n"
	" * along with this program; if not, write to the Free Software\n"
	" * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA\n"
	" */\n\n";

/*
 * Generate the standard file header.
 */
static void generateHeader(FILE *stream)
{
	fputs(COPYRIGHT_MSG, stream);
	fprintf(stream, "namespace System.CodeDom\n{\n");
	fprintf(stream, "\n");
	fprintf(stream, "#if CONFIG_CODEDOM\n");
	fprintf(stream, "\n");
	fprintf(stream, "using System.Runtime.InteropServices;\n");
	fprintf(stream, "using System.Collections;\n");
	fprintf(stream, "using System.Collections.Specialized;\n");
	fprintf(stream, "\n");
}

/*
 * Generate the standard file footer.
 */
static void generateFooter(FILE *stream)
{
	fprintf(stream, "#endif // CONFIG_CODEDOM\n");
	fprintf(stream, "\n");
	fprintf(stream, "}; // namespace System.CodeDom\n");
}
