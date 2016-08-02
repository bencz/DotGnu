/*
 * link_generics.c - Copy generic parameters to the final image.
 *
 * Copyright (C) 2008  Southern Storm Software, Pty Ltd.
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

#include "linker.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#if IL_VERSION_MAJOR > 1
/*
 * Copy the generic parameters for one program item to an other one.
 */
int _ILLinkerConvertGenerics(ILLinker *linker, ILProgramItem *oldItem,
						     ILProgramItem *newItem)
{
	ILUInt32 genericNum;
	ILGenericPar *genPar;

	genericNum = 0;
	while((genPar = ILGenericParGetFromOwner(oldItem, genericNum)) != 0)
	{
		ILGenericPar *newGenPar;
		ILGenericConstraint *genConstr;
		ILProgramItem *constraint;
		ILTypeSpec *spec;

		newGenPar = ILGenericParCreate(linker->image, 0, newItem, genericNum);
		if(!newGenPar)
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		if(!ILGenericParSetName(newGenPar, ILGenericPar_Name(genPar)))
		{
			_ILLinkerOutOfMemory(linker);
			return 0;
		}
		/*
		 * Copy the primary, secondary and ctor constraints.
		 */
		ILGenericParSetFlags(newGenPar, 0, ILGenericParGetFlags(genPar));

		/*
		 * Now copy the type constraints associated with the generic parameter.
		 */
		genConstr = 0;
		while((genConstr = ILGenericParNextConstraint(genPar, genConstr)) != 0)
		{
			constraint = ILConstraint_Type(genConstr);
			spec = ILProgramItemToTypeSpec(constraint);
			if(spec)
			{
				constraint = ILToProgramItem
					(_ILLinkerConvertTypeSpec(linker, ILTypeSpec_Type(spec)));
			}
			else
			{
				constraint = ILToProgramItem
					(_ILLinkerConvertClassRef(linker, (ILClass *)constraint));
			}
			if(!constraint)
			{
				return 0;
			}
			if (!ILGenericParAddConstraint(newGenPar, 0, constraint))
			{
				return 0;
			}
		}

		/*
		 * Now copy the attributes associated with the generic parameter.
		 */
		if(!_ILLinkerConvertAttrs(linker, ILToProgramItem(genPar),
								  ILToProgramItem(newGenPar)))
		{
			return 0;
		}
		++genericNum;
	}
	return 1;
}
#endif /* IL_VERSION_MAJOR > 1 */

#ifdef	__cplusplus
};
#endif
