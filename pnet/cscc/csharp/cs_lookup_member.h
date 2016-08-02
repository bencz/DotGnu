/*
 * cs_lookup_member.h - Header file for member lookup routines.
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

#ifndef _CSCC_CS_MEMBER_LOOKUP_H
#define _CSCC_CS_MEMBER_LOOKUP_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Extra member kinds.
 */
#define	CS_MEMBERKIND_TYPE			20
#define	CS_MEMBERKIND_TYPE_NODE		21
#define	CS_MEMBERKIND_NAMESPACE		22

/*
 * A list of members that results from a lookup on a type.
 */
typedef struct _tagCSMemberLookupInfo CSMemberLookupInfo;

/*
 * A member that results from a member lookup.
 */
typedef struct _tagCSMemberInfo CSMemberInfo;

/*
 * Initialize the member lookup pool.
 */
void CSMemberInfoInit();

/*
 * Destroy the member lookup pool.
 */
void CSMemberInfoDestroy();

/*
 * Free a member info item.
 */
void CSMemberInfoFree(CSMemberInfo *item);

/*
 * Create a method group that contains a single method.
 */
void *CSCreateMethodGroup(ILMethod *method);

/*
 * Get the n'th member from a method or indexer group.
 * Returns NULL at the end of the group.
 */
ILProgramItem *CSGetGroupMember(void *group, unsigned long n);

/*
 * Remove the n'th member from a method group.
 * Returns the new group.
 */
void *CSRemoveGroupMember(void *group, unsigned long n);

/*
 * Set the candidate form for the n'th member of a method group.
 */
void CSSetGroupMemberForm(void *group, unsigned long n, int form);

/*
 * Get the candidate form for the n'th member of a method group.
 */
int CSGetGroupMemberForm(void *group, unsigned long n);

#ifdef	__cplusplus
};
#endif

#endif /* _CSCC_CS_MEMBER_LOOKUP_H */
