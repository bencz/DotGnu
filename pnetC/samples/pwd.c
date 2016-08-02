
/* Dump the contents of the fake password file */

#include <stdio.h>
#include <pwd.h>

int main(int argc, char *argv[])
{
  struct passwd *pw;
  setpwent();
  while((pw = getpwent()) != 0)
    {
      printf("%s:%s:%d:%d:%s:%s:%s\n",
      	     pw->pw_name, pw->pw_passwd,
	     (int)(pw->pw_uid), (int)(pw->pw_gid),
	     pw->pw_gecos, pw->pw_dir, pw->pw_shell);
    }
  endpwent();
  return 0;
}
