/*
 * cs_lookup_member.c - Member lookup routines.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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


#ifdef	__cplusplus
extern	"C" {
#endif

static ILMemPool _MemberInfoPool;

struct _tagCSMemberInfo
{
	ILProgramItem *member;
	ILClass       *owner;
	short		   kind;
	short		   form;
	CSMemberInfo  *next;
};

struct _tagCSMemberLookupInfo
{
	int			   num;
	CSMemberInfo  *members;
	CSMemberInfo  *lastMember;
};

/*
 * Iterator control structure for CSMemberLookupInfo.
 */
typedef struct
{
	CSMemberLookupInfo *info;
	CSMemberInfo       *current;
	CSMemberInfo       *last;

} CSMemberLookupIter;

/*
 * Initialize a member results set.
 */
static void InitMembers(CSMemberLookupInfo *results)
{
	results->num = 0;
	results->members = 0;
	results->lastMember = 0;
}

/*
 * Add a member to a results set.
 */
static void AddMember(CSMemberLookupInfo *results,
					  ILProgramItem *member, ILClass *owner,
					  int kind)
{
	CSMemberInfo *info;

	/* Check to make sure that the member isn't already in the list.
	   This can happen when the same member is located along different
	   paths in an interface inheritance hierarchy */
	info = results->members;
	while(info != 0)
	{
		if(info->member == member)
		{
			return;
		}
		info = info->next;
	}

	/* Add the new member to the list */
	info = (CSMemberInfo *)ILMemPoolAllocItem(&_MemberInfoPool);
	if(!info)
	{
		CCOutOfMemory();
	}
	info->member = member;
	info->owner = owner;
	info->kind = kind;
	info->form = 0;
	info->next = 0;
	if(results->lastMember)
	{
		results->lastMember->next = info;
	}
	else
	{
		results->members = info;
	}
	results->lastMember = info;
	++(results->num);
}

/*
 * Free the contents of a member lookup results list.
 */
static void FreeMembers(CSMemberLookupInfo *results)
{
	CSMemberInfo *info, *next;
	info = results->members;
	while(info != 0)
	{
		next = info->next;
		ILMemPoolFree(&_MemberInfoPool, info);
		info = next;
	}
	results->num = 0;
	results->members = 0;
	results->lastMember = 0;
}

/*
 * Initialize a member iterator.
 */
static void MemberIterInit(CSMemberLookupIter *iter,
						   CSMemberLookupInfo *results)
{
	iter->info = results;
	iter->current = 0;
	iter->last = 0;
}

/*
 * Get the next item from a member iterator.
 */
static CSMemberInfo *MemberIterNext(CSMemberLookupIter *iter)
{
	if(iter->current)
	{
		iter->last = iter->current;
		iter->current = iter->current->next;
	}
	else
	{
		iter->current = iter->info->members;
		iter->last = 0;
	}
	return iter->current;
}

/*
 * Remove the current item from a member iterator.
 */
static void MemberIterRemove(CSMemberLookupIter *iter)
{
	if(iter->current == iter->info->lastMember)
	{
		iter->info->lastMember = iter->last;
	}
	if(iter->last)
	{
		iter->last->next = iter->current->next;
		ILMemPoolFree(&_MemberInfoPool, iter->current);
		iter->current = iter->last;
	}
	else
	{
		iter->info->members = iter->current->next;
		ILMemPoolFree(&_MemberInfoPool, iter->current);
		iter->current = 0;
		iter->last = 0;
	}
	--(iter->info->num);
}

/*
 * Initialize the member lookup pool.
 */
void CSMemberInfoInit()
{
	ILMemPoolInit(&_MemberInfoPool, sizeof(CSMemberInfo), 10);
}

/*
 * Destroy the member lookup pool.
 */
void CSMemberInfoDestroy()
{
	ILMemPoolDestroy(&_MemberInfoPool);
}

/*
 * Free a member info item.
 */
void CSMemberInfoFree(CSMemberInfo *item)
{
	if(item)
	{
		ILMemPoolFree(&_MemberInfoPool, item);
	}
}

void *CSCreateMethodGroup(ILMethod *method)
{
	CSMemberLookupInfo results;

	/* Clear the results buffer */
	InitMembers(&results);

	/* Add the method as a group member */
	AddMember(&results, ILToProgramItem(method),
			  ILMethod_Owner(method), IL_META_MEMBERKIND_METHOD);

	/* Return the group to the caller */
	return results.members;
}

ILProgramItem *CSGetGroupMember(void *group, unsigned long n)
{
	CSMemberInfo *member = (CSMemberInfo *)group;
	while(member != 0)
	{
		if(n <= 0)
		{
			return (ILProgramItem *)(member->member);
		}
		--n;
		member = member->next;
	}
	return 0;
}

void *CSRemoveGroupMember(void *group, unsigned long n)
{
	CSMemberInfo *member = (CSMemberInfo *)group;
	CSMemberInfo *last = 0;
	while(member != 0)
	{
		if(n <= 0)
		{
			if(last)
			{
				last->next = member->next;
				ILMemPoolFree(&_MemberInfoPool, member);
				return group;
			}
			else
			{
				last = member->next;
				ILMemPoolFree(&_MemberInfoPool, member);
				return (void *)last;
			}
		}
		--n;
		last = member;
		member = member->next;
	}
	return group;
}

void CSSetGroupMemberForm(void *group, unsigned long n, int form)
{
	CSMemberInfo *member = (CSMemberInfo *)group;
	while(member != 0)
	{
		if(n <= 0)
		{
			member->form = (short)form;
			return;
		}
		--n;
		member = member->next;
	}
}

int CSGetGroupMemberForm(void *group, unsigned long n)
{
	CSMemberInfo *member = (CSMemberInfo *)group;
	while(member != 0)
	{
		if(n <= 0)
		{
			return member->form;
		}
		--n;
		member = member->next;
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
