/*
 * CBlend.c - Gradient blending implementation.
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

#include "CBlend.h"
#include "CMath.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL CStatus
CBlend_Initialize(CBlend  *_this,
                  CUInt32  count)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((count != 0));

	/* allocate the factors */
	if(!(_this->factors = (CFloat *)CMalloc(count * sizeof(CFloat))))
	{
		return CStatus_OutOfMemory;
	}

	/* allocate the positions */
	if(!(_this->positions = (CFloat *)CMalloc(count * sizeof(CFloat))))
	{
		CFree(_this->factors);
		return CStatus_OutOfMemory;
	}

	/* set the count */
	_this->count = count;

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL CStatus
CBlend_Copy(CBlend *_this,
            CBlend *copy)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((copy  != 0));

	/* copy the blend */
	{
		/* get the count */
		const CUInt32 count = _this->count;

		/* get the size */
		const CUInt32 size = (count * sizeof(CFloat));

		/* handle trivial case */
		if(count == 0)
		{
			*copy = CBlend_Zero;
		}

		/* allocate the factors */
		if(!(copy->factors = (CFloat *)CMalloc(size)))
		{
			return CStatus_OutOfMemory;
		}

		/* copy the factors */
		CMemCopy(copy->factors, _this->factors, size);

		/* allocate the positions */
		if(!(copy->positions = (CFloat *)CMalloc(size)))
		{
			CFree(copy->factors);
			return CStatus_OutOfMemory;
		}

		/* copy the positions */
		CMemCopy(copy->positions, _this->positions, size);

		/* copy the count */
		copy->count = count;
	}

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL void
CBlend_SetTriangularShape(CBlend *_this,
                          CFloat  focus,
                          CFloat  scale)
{
	/* declarations */
	CFloat *currF;
	CFloat *currP;

	/* assertions */
	CASSERT((_this            != 0));
	CASSERT((_this->factors   != 0));
	CASSERT((_this->positions != 0));

	/* get the output pointers */
	currF = _this->factors;
	currP = _this->positions;

	/* set the blend factors and positions */
	if(focus == 0.0f)
	{
		/* assertions */
		CASSERT((_this->count == CBlend_TriangularHalfCount));

		/* set the starting factor and position */
		*currF++ = scale;
		*currP++ = 0.0f;

		/* set the ending factor and position */
		*currF = 0.0f;
		*currP = 1.0f;
	}
	else if(focus == 1.0f)
	{
		/* assertions */
		CASSERT((_this->count == CBlend_TriangularHalfCount));

		/* set the starting factor and position */
		*currF++ = 0.0f;
		*currP++ = 0.0f;

		/* set the ending factor and position */
		*currF = scale;
		*currP = 1.0f;
	}
	else
	{
		/* assertions */
		CASSERT((_this->count == CBlend_TriangularFullCount));

		/* set the starting factor and position */
		*currF++ = 0.0f;
		*currP++ = 0.0f;

		/* set the intermediate factor and position */
		*currF++ = scale;
		*currP++ = focus;

		/* set the ending factor and position */
		*currF = 0.0f;
		*currP = 1.0f;
	}
}

CINTERNAL void
CBlend_SetSigmaBellShape(CBlend *_this,
                         CFloat  focus,
                         CFloat  scale)
{
	/*\
	|*| NOTE: The cumulative distribution function of the standard normal
	|*|       distribution (i.e. the bell curve) is used here.
	|*|
	|*|   cfd(x) = (1 / 2) * (1 + erf(z(x)))
	|*|     z(x) = (x - mu) / (sigma * sqrt(2))
	|*|
	|*|    sigma = standard deviation
	|*|       mu = mean
	|*|      cfd = cumulative distribution function
	|*|      erf = error function
	|*|     sqrt = square root function
	|*|
	|*|
	|*| see: http://en.wikipedia.org/wiki/Normal_distribution
	|*| see: http://mathworld.wolfram.com/NormalDistribution.html
	|*|
	\*/

	/* declarations */
	CFloat *currF;
	CFloat *currP;
	CFloat *last;

	/* assertions */
	CASSERT((_this            != 0));
	CASSERT((_this->factors   != 0));
	CASSERT((_this->positions != 0));

	/* get the output pointers */
	currF = _this->factors;
	currP = _this->positions;

	/* get the end of output pointer */
	last = (currF + (_this->count - 1));

	/* define the error function input calculation */
	#define _CALC_Z(pos) (((pos) * zmul) + zoff)

	/* define the factor calculation */
	#define _CALC_F(pos) ((CMath_Erf(_CALC_Z(pos)) * fmul) + foff)

	/* set the blend factors and positions */
	if(focus == 0.0f)
	{
		/* declarations */
		CFloat pos;

		/* calculate distance between samples */
		const CDouble delta = (1.0f / 255.0f);

		/* calculate error function input multiplier (sigma = 1/2) */
		const CDouble zmul = (1.0f / (0.5f * CMath_Sqrt(2.0f)));

		/* calculate error function input offset (mu = 1/2) */
		const CDouble zoff = -(0.5f * zmul);

		/* calculate the curve extrema */
		const CDouble cmax  = (0.5f * (1.0f - CMath_Erf(_CALC_Z(0.0f))));
		const CDouble cmin  = (0.5f * (1.0f - CMath_Erf(_CALC_Z(1.0f))));
		const CDouble cdiff = (cmax - cmin);

		/* precalculate factor calculation values */
		const CDouble foff =  ((scale * (0.5f - cmin)) / cdiff);
		const CDouble fmul = -((scale * (0.5f       )) / cdiff);

		/* assertions */
		CASSERT((_this->count == CBlend_SigmaBellHalfCount));

		/* set the starting factor and position */
		*currF++ = scale;
		*currP++ = 0.0f;

		/* set the position to the first sample */
		pos = delta;

		/* set the intermediate factors and positions */
		while(currF != last)
		{
			/* set the current factor and position */
			*currF++ = _CALC_F(pos);
			*currP++ = pos;

			/* update the position */
			pos += delta;
		}

		/* set the ending factor and position */
		*currF = 0.0f;
		*currP = 1.0f;
	}
	else if(focus == 1.0f)
	{
		/* declarations */
		CFloat pos;

		/* calculate distance between samples */
		const CDouble delta = (1.0f / 255.0f);

		/* calculate error function input multiplier (sigma = 1/2) */
		const CDouble zmul = (1.0f / (0.5f * CMath_Sqrt(2.0f)));

		/* calculate error function input offset (mu = 1/2) */
		const CDouble zoff = -(0.5f * zmul);

		/* calculate the curve extrema */
		const CDouble cmax  = (0.5f * (1.0f + CMath_Erf(_CALC_Z(1.0f))));
		const CDouble cmin  = (0.5f * (1.0f + CMath_Erf(_CALC_Z(0.0f))));
		const CDouble cdiff = (cmax - cmin);

		/* precalculate factor calculation values */
		const CDouble foff = ((scale * (0.5f - cmin)) / cdiff);
		const CDouble fmul = ((scale * (0.5f       )) / cdiff);

		/* assertions */
		CASSERT((_this->count == CBlend_SigmaBellHalfCount));

		/* set the starting factor and position */
		*currF++ = 0.0f;
		*currP++ = 0.0f;

		/* set the position to the first sample */
		pos = delta;

		/* set the intermediate factors and positions */
		while(currF != last)
		{
			/* set the current factor and position */
			*currF++ = _CALC_F(pos);
			*currP++ = pos;

			/* update the position */
			pos += delta;
		}

		/* set the ending factor and position */
		*currF = scale;
		*currP = 1.0f;
	}
	else
	{
		/* declarations */
		CDouble pos, delta, zmul, zoff, cmax, cmin, cdiff, foff, fmul;

		/* get the middle of output pointer */
		CFloat *const middle = (currF + 256);

		/* assertions */
		CASSERT((_this->count == CBlend_SigmaBellFullCount));

		/* calculate distance between samples */
		delta = (focus / 255.0f);

		/* calculate error function input multiplier (sigma = focus/4) */
		zmul = (1.0f / (focus * 0.25f * CMath_Sqrt(2.0f)));

		/* calculate error function input offset (mu = focus/2) */
		zoff = -(focus * 0.5f * zmul);

		/* calculate the curve extrema */
		cmax  = (0.5f * (1.0f + CMath_Erf(_CALC_Z(focus))));
		cmin  = (0.5f * (1.0f + CMath_Erf(_CALC_Z(0.0f))));
		cdiff = (cmax - cmin);

		/* precalculate factor calculation values */
		foff = ((scale * (0.5f - cmin)) / cdiff);
		fmul = ((scale * (0.5f       )) / cdiff);

		/* set the starting factor and position */
		*currF++ = 0.0f;
		*currP++ = 0.0f;

		/* set the position to the first pre-peak sample */
		pos = delta;

		/* set the intermediate pre-peak factors and positions */
		while(currF != middle)
		{
			/* set the current factor and position */
			*currF++ = _CALC_F(pos);
			*currP++ = pos;

			/* update the position */
			pos += delta;
		}

		/* set the peak factor and position */
		*currF++ = scale;
		*currP++ = focus;

		/* calculate distance between samples */
		delta = ((1.0f - focus) / 255.0f);

		/* calculate error function input multiplier (sigma = (1-focus)/4) */
		zmul = (1.0f / ((1.0f - focus) * 0.25f * CMath_Sqrt(2.0f)));

		/* calculate error function input offset (mu = (1+focus)/2) */
		zoff = -((1.0f + focus) * 0.5f * zmul);

		/* calculate the curve extrema */
		cmax  = (0.5f * (1.0f - CMath_Erf(_CALC_Z(focus))));
		cmin  = (0.5f * (1.0f - CMath_Erf(_CALC_Z(1.0f))));
		cdiff = (cmax - cmin);

		/* precalculate factor calculation values */
		foff =  ((scale * (0.5f - cmin)) / cdiff);
		fmul = -((scale * (0.5f       )) / cdiff);

		/* set the position to the first post-peak sample */
		pos = focus + delta;

		/* set the intermediate post-peak factors and positions */
		while(currF != last)
		{
			/* set the current factor and position */
			*currF++ = _CALC_F(pos);
			*currP++ = pos;

			/* update the position */
			pos += delta;
		}

		/* set the ending factor and position */
		*currF = 0.0f;
		*currP = 1.0f;
	}

	/* undefine macros */
	#undef _CALC_Z
	#undef _CALC_F
}

CINTERNAL void
CBlend_Finalize(CBlend *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the blend, as needed */
	if(_this->count != 0)
	{
		CFree(_this->factors);
		CFree(_this->positions);
		_this->count = 0;
	}
}

CINTERNAL CStatus
CColorBlend_Copy(CColorBlend *_this,
                 CColorBlend *copy)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((copy  != 0));

	/* copy the color blend */
	{
		/* get the count */
		const CUInt32 count = _this->count;

		/* get the color size */
		const CUInt32 sizeC = (count * sizeof(CColor));

		/* get the position size */
		const CUInt32 sizeP = (count * sizeof(CFloat));

		/* handle trivial case */
		if(count == 0)
		{
			*copy = CColorBlend_Zero;
		}

		/* allocate the colors */
		if(!(copy->colors = (CColor *)CMalloc(sizeC)))
		{
			return CStatus_OutOfMemory;
		}

		/* copy the colors */
		CMemCopy(copy->colors, _this->colors, sizeC);

		/* allocate the positions */
		if(!(copy->positions = (CFloat *)CMalloc(sizeP)))
		{
			CFree(copy->colors);
			return CStatus_OutOfMemory;
		}

		/* copy the positions */
		CMemCopy(copy->positions, _this->positions, sizeP);

		/* copy the count */
		copy->count = count;
	}

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL void
CColorBlend_Finalize(CColorBlend *_this)
{
	/* assertions */
	CASSERT((_this        != 0));

	/* finalize the blend, as needed */
	if(_this->count != 0)
	{
		CFree(_this->colors);
		CFree(_this->positions);
		_this->count = 0;
	}
}


#ifdef __cplusplus
};
#endif
