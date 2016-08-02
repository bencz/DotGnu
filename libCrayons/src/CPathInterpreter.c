/*
 * CPathInterpreter.c - Path interpreter implementation.
 *
 * Copyright (C) 2005  Free Software Foundation, Inc.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#include "CPathInterpreter.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL CStatus
CPathInterpreter_Interpret(CPathInterpreter *_this,
                           const CPointF    *points,
                           const CByte      *types,
                           CUInt32           count)
{
	/* declarations */
	const CPointF *currP;
	const CPointF *end;
	const CByte   *currT;

	/* assertions */
	CASSERT((_this                != 0));
	CASSERT((_this->_class        != 0));
	CASSERT((_this->_class->Move  != 0));
	CASSERT((_this->_class->Line  != 0));
	CASSERT((_this->_class->Curve != 0));
	CASSERT((_this->_class->Close != 0));
	CASSERT((points               != 0));
	CASSERT((types                != 0));

	/* get the type input pointer */
	currT = types;

	/* get the point input pointer */
	currP = points;

	/* get the end of input pointer */
	end  = (currP + count);

	/* interpret the path */
	while(currP != end)
	{
		/* declarations */
		CByte type;

		/* get the current type */
		type = (*currT & CPathType_TypeMask);

		/* process point based on type */
		if(type == CPathType_Line)
		{
			/* line to point */
			CStatus_Check
				(CPathInterpreter_Line
					(_this, CPoint_X(*currP), CPoint_Y(*currP), *currT));
		}
		else if(type == CPathType_Bezier)
		{
			/* declarations */
			CFloat x1, y1, x2, y2, x3, y3;

			/* assertions */
			CASSERT(((currP + 2) != end));

			/* get the first point coordinates */
			x1 = CPoint_X(*currP);
			y1 = CPoint_Y(*currP);

			/* advance to the second point */
			++currP; ++currT;

			/* assertions */
			CASSERT(((*currT & CPathType_TypeMask) == CPathType_Bezier));

			/* get the second point coordinates */
			x2 = CPoint_X(*currP);
			y2 = CPoint_Y(*currP);

			/* advance to the third point */
			++currP; ++currT;

			/* assertions */
			CASSERT(((*currT & CPathType_TypeMask) == CPathType_Bezier));

			/* get the third point coordinates */
			x3 = CPoint_X(*currP);
			y3 = CPoint_Y(*currP);

			/* curve to point */
			CStatus_Check
				(CPathInterpreter_Curve
					(_this, x1, y1, x2, y2, x3, y3, *currT));
		}
		else
		{
			/* assertions */
			CASSERT((type == CPathType_Start));

			/* move to point */
			CStatus_Check
				(CPathInterpreter_Move
					(_this, CPoint_X(*currP), CPoint_Y(*currP), *currT));
		}

		/* close the subpath, as needed */
		if((*currT & CPathType_CloseSubpath) != 0)
		{
			CStatus_Check
				(CPathInterpreter_Close
					(_this));
        }

		/* move to the next input position */
		++currP; ++currT;
	}

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
