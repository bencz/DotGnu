#include <errno.h>
#include <stddef.h>
#include <dirent.h>

int main(int argc, char *argv[])
{
	DIR *dirp;
	struct dirent *dp;

	errno = 0;
	if(!(dirp = opendir(".")))
	{
		printf("opendir(\".\") failed ... errno: %d\n", errno);
		return errno;
	}

	errno = 0;
	while((dp = readdir(dirp)))
	{
		printf("entry: %s", dp->d_name);
		if (errno)
		{
			printf(" ... errno: %d", errno);
			errno = 0;
		}
		printf("\n");
	}

	if (errno)
	{
		printf("errno: %d\n", errno);
	}

	closedir(dirp);

	return errno;
}
