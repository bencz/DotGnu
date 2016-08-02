/*
 * CPen.c - Pen implementation.
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

#include "CPen.h"
#include "CBrush.h"
#include "CMatrix.h"

#ifdef __cplusplus
extern "C" {
#endif

#define CDashStyle_IsValid(ds) \
	(((ds) >= CDashStyle_Solid) && ((ds) <= CDashStyle_Custom))
#define CDashInfo_MAXCOUNT 6
typedef struct _tagCDashInfo CDashInfo;
struct _tagCDashInfo
{
	const CUInt32 count;
	const CFloat  pattern[CDashInfo_MAXCOUNT];
};
static const CDashInfo CPen_DashInfo[] =
{
	{ 0, { 0.0 }                          }, /* CDashStyle_Solid       */
	{ 2, { 1.0, 1.0 }                     }, /* CDashStyle_Dot         */
	{ 2, { 3.0, 1.0 }                     }, /* CDashStyle_Dash        */
	{ 4, { 3.0, 1.0, 1.0, 1.0 }           }, /* CDashStyle_DashDot     */
	{ 6, { 3.0, 1.0, 1.0, 1.0, 1.0, 1.0 } }, /* CDashStyle_DashDashDot */
	{ 1, { 1.0 }                          }  /* CDashStyle_Custom      */
};


static CStatus
CPen_Initialize(CPen   *_this,
                CBrush *brush,
                CFloat  width)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((brush != 0));

	/* initialize the members */
	_this->dashOffset     = 0.0f;
	_this->miterLimit     = 10.0f;
	_this->width          = width;
	_this->compoundArray  = 0;
	_this->compoundCount  = 0;
	_this->dashPattern    = 0;
	_this->dashCount      = 0;
	_this->transform      = CAffineTransformF_Identity;
	_this->dashCap        = CDashCap_Flat;
	_this->dashStyle      = CDashStyle_Solid;
	_this->customEndCap   = 0;
	_this->customStartCap = 0;
	_this->endCap         = CLineCap_Flat;
	_this->startCap       = CLineCap_Flat;
	_this->lineJoin       = CLineJoin_Miter;
	_this->alignment      = CPenAlignment_Center;

	/* initialize the brush and return */
	return CBrush_Clone(brush, &(_this->brush));
}

static void
CPen_Finalize(CPen *_this)
{
	CASSERT((_this != 0));

	/* finalize, as needed */
	if(_this->brush != 0)
	{
		/* dispose of the brush */
		CBrush_Destroy(&(_this->brush));

		/* dispose of the compound array, as needed */
		if(_this->compoundArray != 0)
		{
			CFree(_this->compoundArray);
			_this->compoundArray = 0;
			_this->compoundCount = 0;
		}

		/* dispose of the dash pattern, as needed */
		if(_this->dashPattern != 0)
		{
			CFree(_this->dashPattern);
			_this->dashPattern = 0;
			_this->dashCount = 0;
		}

		/* dispose of the custom end cap, as needed */
		if(_this->customEndCap != 0)
		{
			CFree(_this->customEndCap);
			_this->customEndCap = 0;
		}

		/* dispose of the custom start cap, as needed */
		if(_this->customStartCap != 0)
		{
			CFree(_this->customStartCap);
			_this->customStartCap = 0;
		}
	}
}

/* Create a pen. */
CStatus
CPen_Create(CPen   **_this,
            CBrush  *brush,
            CFloat   width)
{
	/* declarations */
	CStatus status;

	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* allocate the pen */
	if(!(*_this = (CPen *)CMalloc(sizeof(CPen))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the pen */
	status = CPen_Initialize(*_this, brush, width);

	/* handle initialization failures */
	if(status != CStatus_OK) { *_this = 0; }

	/* return status */
	return status;
}

/* Destroy a pen. */
CStatus
CPen_Destroy(CPen **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require(((*_this) != 0), CStatus_ArgumentNull);

	/* finalize the pen */
	CPen_Finalize(*_this);

	/* null the this pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Get the alignment of this pen. */
CStatus
CPen_GetAlignment(CPen          *_this,
                  CPenAlignment *alignment)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have an alignment pointer */
	CStatus_Require((alignment != 0), CStatus_ArgumentNull);

	/* get the alignment */
	*alignment = _this->alignment;

	/* return successfully */
	return CStatus_OK;
}

/* Set the alignment of this pen. */
CStatus
CPen_SetAlignment(CPen          *_this,
                  CPenAlignment  alignment)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the alignment */
	_this->alignment = alignment;

	/* return successfully */
	return CStatus_OK;
}

/* Get the brush of this pen. */
CStatus
CPen_GetBrush(CPen    *_this,
              CBrush **brush)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* get the brush */
	return CBrush_Clone(_this->brush, brush);
}

/* Set the brush of this pen. */
CStatus
CPen_SetBrush(CPen   *_this,
              CBrush *brush)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a brush pointer */
	CStatus_Require((brush != 0), CStatus_ArgumentNull);

	/* set the brush */
	{
		/* declarations */
		CBrush *b;

		/* clone the given brush */
		CStatus_Check
			(CBrush_Clone
				(brush, &b));

		/* dispose of the current brush */
		CBrush_Destroy(&(_this->brush));

		/* set the brush */
		_this->brush = b;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Set the line and dash caps of this pen. */
CStatus
CPen_SetCaps(CPen     *_this,
             CLineCap  startCap,
             CLineCap  endCap,
             CDashCap  dashCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the line and dash caps */
	_this->startCap = startCap;
	_this->endCap   = endCap;
	_this->dashCap  = dashCap;

	/* return successfully */
	return CStatus_OK;
}

/* Get the color of this pen. */
CStatus
CPen_GetColor(CPen   *_this,
              CColor *color)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a color pointer */
	CStatus_Require((color != 0), CStatus_ArgumentNull);

	/* get the color */
	{
		/* declarations */
		CBrushType bt;

		/* get the brush type */
		CStatus_Check
			(CBrush_GetBrushType
				(_this->brush, &bt));

		/* get the color, if available */
		if(bt == CBrushType_SolidFill)
		{
			return CSolidBrush_GetColor(((CSolidBrush *)_this->brush), color);
		}
		else
		{
			*color = CColor_Empty;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the compound array of this pen. */
CStatus
CPen_GetCompoundArray(CPen     *_this,
                      CFloat  **compoundArray,
                      CUInt32  *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a compound array pointer pointer */
	CStatus_Require((compoundArray != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the compound array */
	{
		/* get the compound array, if available */
		if(_this->compoundCount == 0)
		{
			/* get the defaults */
			*compoundArray = 0;
			*count         = 0;
		}
		else
		{
			/* declarations */
			CUInt32 size;

			/* get the count */
			*count = _this->compoundCount;

			/* get the size */
			size = (*count * sizeof(CFloat));

			/* allocate the compound array */
			*compoundArray = (CFloat *)CMalloc(size);

			/* ensure we have a compound array */
			CStatus_Require
				((*compoundArray != 0), CStatus_OutOfMemory);

			/* get the compound array */
			CMemCopy(*compoundArray, _this->compoundArray, size);
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Set the compound array of this pen. */
CStatus
CPen_SetCompoundArray(CPen         *_this,
                      const CFloat *compoundArray,
                      CUInt32       count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the compound array */
	{
		/* set the compound array, if available */
		if(compoundArray == 0 || count == 0)
		{
			/* dispose of the current compound array, as needed */
			if(_this->compoundCount != 0)
			{
				/* dispose of the current compound array */
				CFree(_this->compoundArray);
			}

			/* null the compound array */
			_this->compoundArray = 0;

			/* set the compound array count */
			_this->compoundCount = 0;
		}
		else
		{
			/* get the size */
			const CUInt32 size = (count * sizeof(CFloat));

			/* validate the compound array */
			{
				/* declarations */
				const CFloat *curr;
				const CFloat *last;
				CFloat        prev;

				/* ensure there are at least two entries */
				CStatus_Require((count >= 2), CStatus_Argument);

				/* get the current pointer */
				curr = compoundArray;

				/* get the last pointer */
				last = (curr + (count - 1));

				/* get the first entry */
				prev = *curr++;

				/* ensure the first entry is zero */
				CStatus_Require((prev == 0.0f), CStatus_Argument);

				/* ensure the last entry is one */
				CStatus_Require((*last == 1.0f), CStatus_Argument);

				/* ensure the entries increase in value */
				while(curr != last)
				{
					/* ensure the current entry is greater than the previous */
					CStatus_Require((*curr > prev), CStatus_Argument);

					/* move to the next position */
					prev = *curr++;
				}

				/* ensure the penultimate entry is less than one */
				CStatus_Require((prev < 1.0f), CStatus_Argument);
			}

			/* allocate the compound array, as needed */
			if(_this->compoundCount != count)
			{
				/* declarations */
				CFloat *tmp;

				/* allocate the compound array */
				tmp = (CFloat *)CMalloc(size);

				/* ensure we have a compound array */
				CStatus_Require((tmp != 0), CStatus_OutOfMemory);

				/* dispose of the current compound array, as needed */
				if(_this->compoundCount != 0)
				{
					/* dispose of the current compound array */
					CFree(_this->compoundArray);
				}

				/* set the compound array */
				_this->compoundArray = tmp;

				/* set the compound array count */
				_this->compoundCount = count;
			}

			/* copy the compound array data */
			CMemCopy(_this->compoundArray, compoundArray, size);
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the custom end cap of this pen. */
CStatus
CPen_GetCustomEndCap(CPen            *_this,
                     CCustomLineCap **customEndCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a custom end cap pointer pointer */
	CStatus_Require((customEndCap != 0), CStatus_ArgumentNull);

	/* get the custom end cap */
#if 0
	if(_this->customEndCap == 0)
	{
		*customEndCap = 0;
	}
	else
	{
		return CCustomLineCap_Clone(_this->customEndCap, customEndCap);
	}
#else
	*customEndCap = 0;
#endif

	/* return successfully */
	return CStatus_OK;
}

/* Set the custom end cap of this pen. */
CStatus
CPen_SetCustomEndCap(CPen           *_this,
                     CCustomLineCap *customEndCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the custom end cap */
#if 0
	{
		/* dispose of the current custom end cap, as needed */
		if(_this->customEndCap != 0)
		{
			/* dispose of the current custom end cap */
			CCustomLineCap_Destroy(&(_this->customEndCap));
		}

		/* set the custom end cap */
		if(customEndCap == 0)
		{
			_this->customEndCap = 0;
		}
		else
		{
			return
				CCustomLineCap_Clone
					(customEndCap, &(_this->customEndCap));
		}
	}
#endif

	/* return successfully */
	return CStatus_OK;
}

/* Get the custom start cap of this pen. */
CStatus
CPen_GetCustomStartCap(CPen            *_this,
                       CCustomLineCap **customStartCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a custom start cap pointer pointer */
	CStatus_Require((customStartCap != 0), CStatus_ArgumentNull);

	/* get the custom start cap */
#if 0
	if(_this->customStartCap == 0)
	{
		*customStartCap = 0;
	}
	else
	{
		return CCustomLineCap_Clone(_this->customStartCap, customStartCap);
	}
#else
	*customStartCap = 0;
#endif

	/* return successfully */
	return CStatus_OK;
}

/* Set the custom start cap of this pen. */
CStatus
CPen_SetCustomStartCap(CPen           *_this,
                       CCustomLineCap *customStartCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the custom start cap */
#if 0
	{
		/* dispose of the current custom start cap, as needed */
		if(_this->customStartCap != 0)
		{
			/* dispose of the current custom start cap */
			CCustomLineCap_Destroy(&(_this->customStartCap));
		}

		/* set the custom start cap */
		if(customStartCap == 0)
		{
			_this->customStartCap = 0;
		}
		else
		{
			return
				CCustomLineCap_Clone
					(customStartCap, &(_this->customStartCap)));
		}
	}
#endif

	/* return successfully */
	return CStatus_OK;
}

/* Get the dash cap of this pen. */
CStatus
CPen_GetDashCap(CPen     *_this,
                CDashCap *dashCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a dash cap pointer */
	CStatus_Require((dashCap != 0), CStatus_ArgumentNull);

	/* get the dash cap */
	*dashCap = _this->dashCap;

	/* return successfully */
	return CStatus_OK;
}

/* Set the dash cap of this pen. */
CStatus
CPen_SetDashCap(CPen     *_this,
                CDashCap  dashCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the dash cap */
	_this->dashCap = dashCap;

	/* return successfully */
	return CStatus_OK;
}

/* Get the dash offset of this pen. */
CStatus
CPen_GetDashOffset(CPen   *_this,
                   CFloat *dashOffset)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a dash offset pointer */
	CStatus_Require((dashOffset != 0), CStatus_ArgumentNull);

	/* get the dash offset */
	*dashOffset = _this->dashOffset;

	/* return successfully */
	return CStatus_OK;
}

/* Set the dash offset of this pen. */
CStatus
CPen_SetDashOffset(CPen   *_this,
                   CFloat  dashOffset)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the dash offset */
	_this->dashOffset = dashOffset;

	/* return successfully */
	return CStatus_OK;
}

/* Get the dash pattern of this pen. */
CStatus
CPen_GetDashPattern(CPen     *_this,
                    CFloat  **dashPattern,
                    CUInt32  *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a dash pattern pointer pointer */
	CStatus_Require((dashPattern != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the dash pattern */
	{
		/* get the dash pattern, if available */
		if(_this->dashCount == 0)
		{
			/* get the defaults */
			*dashPattern = 0;
			*count       = 0;
		}
		else
		{
			/* declarations */
			CUInt32 size;

			/* get the count */
			*count = _this->dashCount;

			/* get the size */
			size = (*count * sizeof(CFloat));

			/* allocate the dash pattern */
			*dashPattern = (CFloat *)CMalloc(size);

			/* ensure we have a dash pattern */
			CStatus_Require((*dashPattern != 0), CStatus_OutOfMemory);

			/* get the dash pattern */
			CMemCopy(*dashPattern, _this->dashPattern, size);
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Set the dash pattern of this pen. */
CStatus
CPen_SetDashPattern(CPen         *_this,
                    const CFloat *dashPattern,
                    CUInt32       count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the dash pattern */
	{
		/* set the dash pattern, if available */
		if(dashPattern == 0 || count == 0)
		{
			/* dispose of the current dash pattern, as needed */
			if(_this->dashCount != 0)
			{
				/* dispose of the current dash pattern */
				CFree(_this->dashPattern);
			}

			/* null the dash pattern */
			_this->dashPattern = 0;

			/* set the dash pattern count */
			_this->dashCount = 0;

			/* set the dash style */
			_this->dashStyle = CDashStyle_Solid;
		}
		else
		{
			/* declarations */
			CFloat *tmp;

			/* allocate the dash pattern */
			tmp = (CFloat *)CMalloc(count * sizeof(CFloat));

			/* ensure we have a dash pattern */
			CStatus_Require((tmp != 0), CStatus_OutOfMemory);

			/* dispose of the current dash pattern, as needed */
			if(_this->dashCount != 0)
			{
				/* dispose of the current dash pattern */
				CFree(_this->dashPattern);
			}

			/* copy the dash pattern data */
			CMemCopy(tmp, dashPattern, (count * sizeof(CFloat)));

			/* set the dash pattern */
			_this->dashPattern = tmp;

			/* set the dash pattern count */
			_this->dashCount = count;

			/* set the dash style */
			_this->dashStyle = CDashStyle_Custom;
		}
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the dash style of this pen. */
CStatus
CPen_GetDashStyle(CPen       *_this,
                  CDashStyle *dashStyle)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a dash style pointer */
	CStatus_Require((dashStyle != 0), CStatus_ArgumentNull);

	/* get the dash style */
	*dashStyle = _this->dashStyle;

	/* return successfully */
	return CStatus_OK;
}

/* Set the dash style of this pen. */
CStatus
CPen_SetDashStyle(CPen       *_this,
                  CDashStyle  dashStyle)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure the dash style is in range */
	CStatus_Require(CDashStyle_IsValid(dashStyle), CStatus_Argument);

	/* set the dash style */
	{
		/* get the dash information for the given style */
		const CDashInfo *info = &(CPen_DashInfo[dashStyle]);

		/* set the dash pattern */
		CStatus_Check
			(CPen_SetDashPattern
				(_this, info->pattern, info->count));

		/* set the dash style */
		_this->dashStyle = dashStyle;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the end cap of this pen. */
CStatus
CPen_GetEndCap(CPen     *_this,
               CLineCap *endCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a end cap pointer */
	CStatus_Require((endCap != 0), CStatus_ArgumentNull);

	/* get the end cap */
	*endCap = _this->endCap;

	/* return successfully */
	return CStatus_OK;
}

/* Set the end cap of this pen. */
CStatus
CPen_SetEndCap(CPen     *_this,
               CLineCap  endCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the end cap */
	_this->endCap = endCap;

	/* return successfully */
	return CStatus_OK;
}

/* Get the line join of this pen. */
CStatus
CPen_GetLineJoin(CPen      *_this,
                 CLineJoin *lineJoin)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a line join pointer */
	CStatus_Require((lineJoin != 0), CStatus_ArgumentNull);

	/* get the line join */
	*lineJoin = _this->lineJoin;

	/* return successfully */
	return CStatus_OK;
}

/* Set the line join of this pen. */
CStatus
CPen_SetLineJoin(CPen      *_this,
                 CLineJoin  lineJoin)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the line join */
	_this->lineJoin = lineJoin;

	/* return successfully */
	return CStatus_OK;
}

/* Get the miter limit of this pen. */
CStatus
CPen_GetMiterLimit(CPen   *_this,
                   CFloat *miterLimit)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a miter limit pointer */
	CStatus_Require((miterLimit != 0), CStatus_ArgumentNull);

	/* get the miter limit */
	*miterLimit = _this->miterLimit;

	/* return successfully */
	return CStatus_OK;
}

/* Set the miter limit of this pen. */
CStatus
CPen_SetMiterLimit(CPen   *_this,
                   CFloat  miterLimit)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the miter limit */
	_this->miterLimit = miterLimit;

	/* return successfully */
	return CStatus_OK;
}

/* Get the type of this pen. */
CStatus
CPen_GetPenType(CPen     *_this,
                CPenType *type)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a type pointer */
	CStatus_Require((type != 0), CStatus_ArgumentNull);

	/* get the type */
	{
		/* declarations */
		CBrushType bt;

		/* get the type */
		CStatus_Check
			(CBrush_GetBrushType
				(_this->brush, &bt));

		/* set the type pointer to the type */
		*type = (CPenType)bt;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the start cap of this pen. */
CStatus
CPen_GetStartCap(CPen     *_this,
                 CLineCap *startCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a start cap pointer */
	CStatus_Require((startCap != 0), CStatus_ArgumentNull);

	/* get the start cap */
	*startCap = _this->startCap;

	/* return successfully */
	return CStatus_OK;
}

/* Set the start cap of this pen. */
CStatus
CPen_SetStartCap(CPen     *_this,
                 CLineCap  startCap)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the start cap */
	_this->startCap = startCap;

	/* return successfully */
	return CStatus_OK;
}

/* Get the width of this pen. */
CStatus
CPen_GetWidth(CPen   *_this,
              CFloat *width)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a width pointer */
	CStatus_Require((width != 0), CStatus_ArgumentNull);

	/* get the width */
	*width = _this->width;

	/* return successfully */
	return CStatus_OK;
}

/* Set the width of this pen. */
CStatus
CPen_SetWidth(CPen   *_this,
              CFloat  width)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the width */
	_this->width = width;

	/* return successfully */
	return CStatus_OK;
}

/* Clone this pen. */
CStatus
CPen_Clone(CPen  *_this,
           CPen **clone)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a clone pointer pointer */
	CStatus_Require((clone != 0), CStatus_ArgumentNull);

	/* create the clone */
	{
		/* get the counts */
		const CUInt32 cc = _this->compoundCount;
		const CUInt32 dc = _this->dashCount;

		/* create the clone pen */
		CStatus_Check
			(CPen_Create
				(clone, _this->brush, _this->width));

		/* deep copy the compound array, as needed */
		if(cc != 0)
		{
			/* get the size */
			const CUInt32 size = (cc * sizeof(CFloat));

			/* allocate the compound array */
			(*clone)->compoundArray = (CFloat *)CMalloc(size);

			/* ensure we have a compound array */
			if((*clone)->compoundArray == 0)
			{
				CPen_Destroy(clone);
				return CStatus_OutOfMemory;
			}

			/* deep copy the compound array */
			CMemCopy((*clone)->compoundArray, _this->compoundArray, size);

			/* set the compound array count */
			(*clone)->compoundCount = cc;
		}

		/* deep copy the dash pattern, as needed */
		if(dc != 0)
		{
			/* get the size */
			const CUInt32 size = (dc * sizeof(CFloat));

			/* allocate the dash pattern */
			(*clone)->dashPattern = (CFloat *)CMalloc(size);

			/* ensure we have a dash pattern */
			if((*clone)->dashPattern == 0)
			{
				CPen_Destroy(clone);
				return CStatus_OutOfMemory;
			}

			/* deep copy the dash pattern */
			CMemCopy((*clone)->dashPattern, _this->dashPattern, size);

			/* set the dash pattern count */
			(*clone)->dashCount = dc;
		}

		/* deep copy the custom end cap, as needed */
#if 0
		if(_this->customEndCap != 0)
		{
			/* declarations */
			Status status;

			/* deep copy the custom end cap */
			status = CCustomLineCap_Clone
				(_this->customEndCap, &((*clone)->customEndCap));

			/* ensure we have a custom end cap */
			if(status != CStatus_OK)
			{
				CPen_Destroy(clone);
				return status;
			}
		}
#endif

		/* deep copy the custom start cap, as needed */
#if 0
		if(_this->customStartCap != 0)
		{
			/* declarations */
			Status status;

			/* deep copy the custom start cap */
			status = CCustomLineCap_Clone
				(_this->customStartCap, &((*clone)->customStartCap));

			/* ensure we have a custom start cap */
			if(status != CStatus_OK)
			{
				CPen_Destroy(clone);
				return status;
			}
		}
#endif

		/* set the remaining clone members */
		(*clone)->dashOffset     = _this->dashOffset;
		(*clone)->miterLimit     = _this->miterLimit;
		(*clone)->transform      = _this->transform;
		(*clone)->dashCap        = _this->dashCap;
		(*clone)->dashStyle      = _this->dashStyle;
		(*clone)->endCap         = _this->endCap;
		(*clone)->startCap       = _this->startCap;
		(*clone)->lineJoin       = _this->lineJoin;
		(*clone)->alignment      = _this->alignment;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the transformation matrix of this pen. */
CStatus
CPen_GetTransform(CPen    *_this,
                  CMatrix *matrix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the transformation */
	return CMatrix_SetTransform(matrix, &(_this->transform));
}

/* Multiply the transformation matrix of this pen by another matrix. */
CStatus
CPen_MultiplyTransform(CPen         *_this,
                       CMatrix      *matrix,
                       CMatrixOrder  order)
{
	/* declarations */
	CAffineTransformF t;

	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* get the matrix transformation */
	CStatus_Check
		(CMatrix_GetTransform
			(matrix, &t));

	/* multiply the transformation */
	CAffineTransformF_Multiply(&(_this->transform), &t, order);

	/* return successfully */
	return CStatus_OK;
}

/* Reset the transformation matrix of this pen. */
CStatus
CPen_ResetTransform(CPen *_this)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* reset the transformation */
	_this->transform = CAffineTransformF_Identity;

	/* return successfully */
	return CStatus_OK;
}

/* Rotate the transformation matrix of this pen. */
CStatus
CPen_RotateTransform(CPen         *_this,
                     CFloat        angle,
                     CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* rotate the transformation */
	CAffineTransformF_Rotate(&(_this->transform), angle, order);

	/* return successfully */
	return CStatus_OK;
}

/* Scale the transformation matrix of this pen. */
CStatus
CPen_ScaleTransform(CPen         *_this,
                    CFloat        sx,
                    CFloat        sy,
                    CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* scale the transformation */
	CAffineTransformF_Scale(&(_this->transform), sx, sy, order);

	/* return successfully */
	return CStatus_OK;
}

/* Set the transformation matrix of this pen. */
CStatus
CPen_SetTransform(CPen    *_this,
                  CMatrix *matrix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the transformation */
	CStatus_Check
		(CMatrix_GetTransform
			(matrix, &(_this->transform)));

	/* return successfully */
	return CStatus_OK;
}

/* Translate the transformation matrix of this pen. */
CStatus
CPen_TranslateTransform(CPen         *_this,
                        CFloat        dx,
                        CFloat        dy,
                        CMatrixOrder  order)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* translate the transformation */
	CAffineTransformF_Translate(&(_this->transform), dx, dy, order);

	/* return successfully */
	return CStatus_OK;
}

/* Get a pattern for this pen. */
CINTERNAL CStatus
CPen_GetPattern(CPen     *_this,
                CPattern *pattern)
{
	/* assertions */
	CASSERT((_this   != 0));
	CASSERT((pattern != 0));

	/* get the pattern */
	CStatus_Check
		(CBrush_GetPattern
			(_this->brush, pattern));

	/* return successfully */
	return CStatus_OK;
}

#ifdef __cplusplus
};
#endif
