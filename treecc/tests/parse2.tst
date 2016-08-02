// test bad node definitions

%node needs_typedef

%node needs_parent unknown_parent

%node bad_fields %typedef =

%node bad_fields2 %typedef =
{
	%abstract int x;
	int y;
}

%node bad_fields3 %typedef =
{
	int x = {0};
	%nocreate int y = {0};
	%nocreate int z = NULL;
	%nocreate int w =;
}
