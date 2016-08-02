/*
 * verify_ann.c - Verify instructions related to annotations.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#if defined(IL_VERIFY_GLOBALS)

/* No globals required */

#elif defined(IL_VERIFY_LOCALS)

/* No locals required */

#else /* IL_VERIFY_CODE */

case IL_OP_ANN_DATA_S:
{
	/* Skip the short data annotation */
	insnSize = (((ILUInt32)(pc[1])) & 0xFF) + 2;
}
break;

case IL_OP_PREFIX + IL_PREFIX_OP_ANN_DATA:
{
	/* Skip the long data annotation */
	insnSize = (IL_READ_UINT32(pc + 2) + 6);
}
break;

case IL_OP_ANN_PHI:
{
	/* Skip the "phi" annotation */
	insnSize = ((ILUInt32)IL_READ_UINT16(pc + 1)) * 2 + 3;
}
break;

#endif /* IL_VERIFY_CODE */
