/*
 * CStringFormat.c - String format implementation.
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

#include "CStringFormat.h"

#ifdef __cplusplus
extern "C" {
#endif

static void
CStringFormat_Initialize(CStringFormat     *_this,
                         CStringFormatFlag  flags,
                         CLanguageID        language)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the members */
	_this->alignment           = CStringAlignment_Near;
	_this->lineAlignment       = CStringAlignment_Near;
	_this->formatFlags         = flags;
	_this->hotkeyPrefix        = CHotkeyPrefix_None;
	_this->trimming            = CStringTrimming_None;
	_this->method              = CDigitSubstitute_None;
	_this->language            = language;
	_this->firstTabOffset      = 0;
	_this->tabStops            = 0;
	_this->tabStopCount        = 0;
	_this->characterRanges     = 0;
	_this->characterRangeCount = 0;
}

static void
CStringFormat_Finalize(CStringFormat *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the members */
	{
		/* finalize the tab stop list, as needed */
		if(_this->tabStops != 0)
		{
			CFree(_this->tabStops);
			_this->tabStopCount = 0;
			_this->tabStops     = 0;
		}

		/* finalize the character range list, as needed */
		if(_this->characterRanges != 0)
		{
			CFree(_this->characterRanges);
			_this->characterRangeCount = 0;
			_this->characterRanges     = 0;
		}
	}
}

/* Create a string format. */
CStatus
CStringFormat_Create(CStringFormat     **_this,
                     CStringFormatFlag   flags,
                     CLanguageID         language)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* allocate the string format */
	if(!(*_this = (CStringFormat *)CMalloc(sizeof(CStringFormat))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the string format */
	CStringFormat_Initialize(*_this, flags, language);

	/* return successfully */
	return CStatus_OK;
}

/* Destroy a string format. */
CStatus
CStringFormat_Destroy(CStringFormat **_this)
{
	/* ensure we have a this pointer pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a this pointer */
	CStatus_Require(((*_this) != 0), CStatus_ArgumentNull);

	/* finalize the string format */
	CStringFormat_Finalize(*_this);

	/* free the string format */
	CFree(*_this);

	/* null the this pointer */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/* Get the generic default string format. */
CStatus
CStringFormat_CreateGenericDefault(CStringFormat **_this)
{
	/* create the format */
	CStatus_Check
		(CStringFormat_Create
			(_this, 0, 0));

	/* initialize the members */
	(*_this)->trimming = CStringTrimming_Character;
	(*_this)->method   = CDigitSubstitute_User;

	/* return successfully */
	return CStatus_OK;
}

/* Get the generic typographic string format. */
CStatus
CStringFormat_CreateGenericTypographic(CStringFormat **_this)
{
	/* get the flags */
	const CStringFormatFlag flags =
		(CStringFormatFlag_NoFitBlackBox |
		 CStringFormatFlag_LineLimit     |
		 CStringFormatFlag_NoClip);

	/* create the format */
	CStatus_Check
		(CStringFormat_Create
			(_this, flags, 0));

	/* initialize the members */
	(*_this)->method = CDigitSubstitute_User;

	/* return successfully */
	return CStatus_OK;
}

/* Clone this string format. */
CStatus
CStringFormat_Clone(CStringFormat  *_this,
                    CStringFormat **clone)
{
	/* declarations */
	CStatus status;

	/* create the format */
	CStatus_Check
		(CStringFormat_Create
			(clone, _this->formatFlags, _this->language));

	/* set the clone tab stops */
	status =
		CStringFormat_SetTabStops
			(*clone, _this->firstTabOffset, _this->tabStops,
			 _this->tabStopCount);

	/* handle tab stop cloning failures */
	if(status != CStatus_OK)
	{
		CStringFormat_Destroy(clone);
		return status;
	}

	/* set the clone character ranges */
	status =
		CStringFormat_SetCharacterRanges
			(*clone, _this->characterRanges, _this->characterRangeCount);

	/* handle character range cloning failures */
	if(status != CStatus_OK)
	{
		CStringFormat_Destroy(clone);
		return status;
	}

	/* clone remaining members */
	(*clone)->alignment     = _this->alignment;
	(*clone)->lineAlignment = _this->lineAlignment;
	(*clone)->hotkeyPrefix  = _this->hotkeyPrefix;
	(*clone)->trimming      = _this->trimming;
	(*clone)->method        = _this->method;

	/* return successfully */
	return CStatus_OK;
}

/* Get the alignment of this string format. */
CStatus
CStringFormat_GetAlignment(CStringFormat    *_this,
                           CStringAlignment *alignment)
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

/* Set the alignment of this string format. */
CStatus
CStringFormat_SetAlignment(CStringFormat    *_this,
                           CStringAlignment  alignment)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the alignment */
	_this->alignment = alignment;

	/* return successfully */
	return CStatus_OK;
}

/* Get the character ranges of this string format. */
CStatus
CStringFormat_GetCharacterRanges(CStringFormat    *_this,
                                 CCharacterRange **characterRanges,
                                 CUInt32          *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a character range list pointer */
	CStatus_Require((characterRanges != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the character ranges */
	{
		/* get the count and size */
		const CUInt32 cnt  = _this->characterRangeCount;
		const CUInt32 size = (cnt * sizeof(CCharacterRange));

		/* allocate the character range list */
		if(!(*characterRanges = (CCharacterRange *)CMalloc(size)))
		{
			*count = 0;
			return CStatus_OutOfMemory;
		}

		/* get the character ranges */
		CMemCopy(*characterRanges, _this->characterRanges, size);

		/* get the count */
		*count = cnt;
	}

	/* return successfully */
	return CStatus_OK;
}

/* Set the character ranges of this string format. */
CStatus
CStringFormat_SetCharacterRanges(CStringFormat   *_this,
                                 CCharacterRange *characterRanges,
                                 CUInt32          count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a character range list */
	CStatus_Require((characterRanges != 0), CStatus_ArgumentNull);

	/* set the character ranges */
	{
		/* get the size */
		const CUInt32 size = (count * sizeof(CCharacterRange));

		/* clear the character range list, as needed */
		if(count == 0)
		{
			/* free the existing character range list, as needed */
			if(_this->characterRanges != 0)
			{
				CFree(_this->characterRanges);
			}

			/* reset the count */
			_this->characterRangeCount = 0;

			/* return successfully */
			return CStatus_OK;
		}

		/* reallocate the character range list, as needed */
		if(count != _this->characterRangeCount)
		{
			/* declarations */
			CCharacterRange *tmp;

			/* allocate the character range list */
			if(!(tmp = (CCharacterRange *)CMalloc(size)))
			{
				return CStatus_OutOfMemory;
			}

			/* free the existing character range list, as needed */
			if(_this->characterRanges != 0)
			{
				CFree(_this->characterRanges);
			}

			/* set the character range list */
			_this->characterRanges = tmp;

			/* set the count */
			_this->characterRangeCount = count;
		}

		/* copy the character ranges */
		CMemCopy(_this->characterRanges, characterRanges, size);
	}

	/* return successfully */
	return CStatus_OK;
}

/* Get the digit substitution settings of this string format. */
CStatus
CStringFormat_GetDigitSubstitution(CStringFormat    *_this,
                                   CDigitSubstitute *method,
                                   CLanguageID      *language)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a method pointer */
	CStatus_Require((method != 0), CStatus_ArgumentNull);

	/* ensure we have a language pointer */
	CStatus_Require((language != 0), CStatus_ArgumentNull);

	/* get the method */
	*method = _this->method;

	/* get the language */
	*language = _this->language;

	/* return successfully */
	return CStatus_OK;
}

/* Set the digit substitution settings of this string format. */
CStatus
CStringFormat_SetDigitSubstitution(CStringFormat    *_this,
                                   CDigitSubstitute  method,
                                   CLanguageID       language)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the method */
	_this->method = method;

	/* set the language */
	_this->language = language;

	/* return successfully */
	return CStatus_OK;
}

/* Get the format flags of this string format. */
CStatus
CStringFormat_GetFormatFlags(CStringFormat     *_this,
                             CStringFormatFlag *formatFlags)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a format flags pointer */
	CStatus_Require((formatFlags != 0), CStatus_ArgumentNull);

	/* get the format flags */
	*formatFlags = _this->formatFlags;

	/* return successfully */
	return CStatus_OK;
}

/* Set the format flags of this string format. */
CStatus
CStringFormat_SetFormatFlags(CStringFormat     *_this,
                             CStringFormatFlag  formatFlags)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the format flags */
	_this->formatFlags = formatFlags;

	/* return successfully */
	return CStatus_OK;
}

/* Get the hotkey prefix mode of this string format. */
CStatus
CStringFormat_GetHotkeyPrefix(CStringFormat *_this,
                              CHotkeyPrefix *hotkeyPrefix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a hotkey prefix mode pointer */
	CStatus_Require((hotkeyPrefix != 0), CStatus_ArgumentNull);

	/* get the hotkey prefix mode */
	*hotkeyPrefix = _this->hotkeyPrefix;

	/* return successfully */
	return CStatus_OK;
}

/* Set the hotkey prefix mode of this string format. */
CStatus
CStringFormat_SetHotkeyPrefix(CStringFormat *_this,
                              CHotkeyPrefix  hotkeyPrefix)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the hotkey prefix mode */
	_this->hotkeyPrefix = hotkeyPrefix;

	/* return successfully */
	return CStatus_OK;
}

/* Get the line alignment of this string format. */
CStatus
CStringFormat_GetLineAlignment(CStringFormat    *_this,
                               CStringAlignment *lineAlignment)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a line alignment pointer */
	CStatus_Require((lineAlignment != 0), CStatus_ArgumentNull);

	/* get the line alignment */
	*lineAlignment = _this->lineAlignment;

	/* return successfully */
	return CStatus_OK;
}

/* Set the line alignment of this string format. */
CStatus
CStringFormat_SetLineAlignment(CStringFormat    *_this,
                               CStringAlignment  lineAlignment)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the line alignment */
	_this->lineAlignment = lineAlignment;

	/* return successfully */
	return CStatus_OK;
}

/* Get the tab stop settings of this string format. */
CStatus
CStringFormat_GetTabStops(CStringFormat  *_this,
                          CFloat         *firstTabOffset,
                          CFloat        **tabStops,
                          CUInt32        *count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a first tab offset pointer */
	CStatus_Require((firstTabOffset != 0), CStatus_ArgumentNull);

	/* ensure we have a tab stop list pointer */
	CStatus_Require((tabStops != 0), CStatus_ArgumentNull);

	/* ensure we have a count pointer */
	CStatus_Require((count != 0), CStatus_ArgumentNull);

	/* get the tab stops */
	{
		/* get the count and size */
		const CUInt32 cnt  = _this->tabStopCount;
		const CUInt32 size = (cnt * sizeof(CFloat));

		/* allocate the tab stop list */
		if(!(*tabStops = (CFloat *)CMalloc(size)))
		{
			*count = 0;
			return CStatus_OutOfMemory;
		}

		/* get the tab stops */
		CMemCopy(*tabStops, _this->tabStops, size);

		/* get the count */
		*count = cnt;
	}

	/* get the first tab offset */
	*firstTabOffset = _this->firstTabOffset;

	/* return successfully */
	return CStatus_OK;
}

/* Set the tab stop settings of this string format. */
CStatus
CStringFormat_SetTabStops(CStringFormat *_this,
                          CFloat         firstTabOffset,
                          CFloat        *tabStops,
                          CUInt32        count)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a tab stop list */
	CStatus_Require((tabStops != 0), CStatus_ArgumentNull);

	/* set the tab stops */
	{
		/* get the size */
		const CUInt32 size = (count * sizeof(CFloat));

		/* clear the tab stop list, as needed */
		if(count == 0)
		{
			/* free the existing tab stop list, as needed */
			if(_this->tabStops != 0)
			{
				CFree(_this->tabStops);
			}

			/* reset the count */
			_this->tabStopCount = 0;

			/* return successfully */
			return CStatus_OK;
		}

		/* reallocate the tab stop list, as needed */
		if(count != _this->tabStopCount)
		{
			/* declarations */
			CFloat *tmp;

			/* allocate the tab stop list */
			if(!(tmp = (CFloat *)CMalloc(size)))
			{
				return CStatus_OutOfMemory;
			}

			/* free the existing tab stop list, as needed */
			if(_this->tabStops != 0)
			{
				CFree(_this->tabStops);
			}

			/* set the tab stop list */
			_this->tabStops = tmp;

			/* set the count */
			_this->tabStopCount = count;
		}

		/* copy the tab stops */
		CMemCopy(_this->tabStops, tabStops, size);
	}

	/* set the first tab offset */
	_this->firstTabOffset = firstTabOffset;

	/* return successfully */
	return CStatus_OK;
}

/* Get the trimming mode of this string format. */
CStatus
CStringFormat_GetTrimming(CStringFormat   *_this,
                          CStringTrimming *trimming)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* ensure we have a trimming mode pointer */
	CStatus_Require((trimming != 0), CStatus_ArgumentNull);

	/* get the trimming mode */
	*trimming = _this->trimming;

	/* return successfully */
	return CStatus_OK;
}

/* Set the trimming mode of this string format. */
CStatus
CStringFormat_SetTrimming(CStringFormat   *_this,
                          CStringTrimming  trimming)
{
	/* ensure we have a this pointer */
	CStatus_Require((_this != 0), CStatus_ArgumentNull);

	/* set the trimming mode */
	_this->trimming = trimming;

	/* return successfully */
	return CStatus_OK;
}

#ifdef __cplusplus
};
#endif
