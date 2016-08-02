/*
 * CRegionInterpreter.c - Region interpreter implementation.
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

#include "CRegionInterpreter.h"

#ifdef __cplusplus
extern "C" {
#endif

static const CRegionInterpreter CRegionInterpreter_Zero;

CINTERNAL void
CRegionInterpreter_Initialize(CRegionInterpreter            *_this,
                              const CRegionInterpreterClass *_class)
{
	/* assertions */
	CASSERT((_this != 0));

	/* initialize the stack */
	CRegionStack_Initialize(&(_this->stack));

	/* initialize the class */
	_this->_class = _class;
}

CINTERNAL void
CRegionInterpreter_Finalize(CRegionInterpreter *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* finalize the stack */
	CRegionStack_Finalize(&(_this->stack));

	/* finalize the remaining members */
	_this->_class = 0;
}

#define CRegionInterpreter_Op(_this, op, left, right, data) \
	((_this)->_class->Op((_this), (op), (left), (right), (data)))
#define CRegionInterpreter_Data(_this, node, data) \
	((_this)->_class->Data((_this), (node), (data)))
#define CRegionInterpreter_FreeData(_this, data) \
	((_this)->_class->FreeData((data)))

CINTERNAL CStatus
CRegionInterpreter_Interpret(CRegionInterpreter  *_this,
                             CRegionNode         *head,
                             void               **data)
{
	/* declarations */
	CRegionStack *stack;
	void         *tmp;
	CStatus       status;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((head  != 0));
	CASSERT((data  != 0));

	/* get the stack pointer */
	stack = &(_this->stack);

	/* reset the stack */
	CRegionStack_Reset(&stack);

	/* handle the trivial case */
	if(CRegionNode_IsData(head))
	{
		return CRegionInterpreter_Data(_this, head, data);
	}
	else
	{
		/* set the data to the default */
		*data = 0;
		tmp   = 0;

		/* push the head */
		CStatus_Check
			(CRegionStack_Push
				(&stack, (CRegionOp *)head));
	}

	/* interpret the region */
	while(_this->stack.count > 0)
	{
		/* get the top of the stack */
		CRegionStackNode *top = &CRegionStack_Top(*stack);

		/* handle the current node */
		if(top->visited == 0)
		{
			/* update the visited flag */
			++(top->visited);

			/* handle the left operand node */
			if(top->op->left == 0)
			{
				tmp = 0;
			}
			else if(CRegionNode_IsData(top->op->left))
			{
				/* process the data */
				status =
					CRegionInterpreter_Data
						(_this, top->op->left, &tmp);

				/* handle data failures */
				if(status != CStatus_OK) { goto GOTO_FreeData; }
			}
			else
			{
				/* push the operation */
				status =
					CRegionStack_Push
						(&stack, ((CRegionOp *)top->op->left));

				/* handle push failures */
				if(status != CStatus_OK) { goto GOTO_FreeData; }
			}
		}
		else if(top->visited == 1)
		{
			/* update the visited flag */
			++(top->visited);

			/* set the left operand data */
			top->left = tmp;

			/* handle the right operand node */
			if(top->op->right == 0)
			{
				tmp = 0;
			}
			else if(CRegionNode_IsData(top->op->right))
			{
				/* process the data */
				status =
					CRegionInterpreter_Data
						(_this, top->op->right, &tmp);

				/* handle data failures */
				if(status != CStatus_OK) { goto GOTO_FreeData; }
			}
			else
			{
				/* push the operation */
				status =
					CRegionStack_Push
						(&stack, ((CRegionOp *)top->op->right));

				/* handle push failures */
				if(status != CStatus_OK) { goto GOTO_FreeData; }
			}
		}
		else
		{
			/* set the right operand data */
			top->right = tmp;

			/* perform the operation */
			status =
				CRegionInterpreter_Op
					(_this, top->op, top->left, top->right, &tmp);

			/* pop the operation */
			CRegionStack_Pop(&stack);

			/* handle operation failures */
			if(status != CStatus_OK) { goto GOTO_FreeData; }
		}
	}

	/* set the data */
	*data = tmp;

	/* return successfully */
	return CStatus_OK;

GOTO_FreeData:

	/* free the data */
	if(_this->_class->FreeData != 0)
	{
		/* free the data */
		while(_this->stack.count > 0)
		{
			/* get the top of the stack */
			CRegionStackNode *top = &CRegionStack_Top(*stack);

			/* free the left data, as needed */
			if(top->left != 0)
			{
				CRegionInterpreter_FreeData(_this, top->left);
				top->left = 0;
			}

			/* free the right data, as needed */
			if(top->right != 0)
			{
				CRegionInterpreter_FreeData(_this, top->right);
				top->right = 0;
			}

			/* pop the operation */
			CRegionStack_Pop(&stack);
		}
	}

	/* return status */
	return status;
}


#ifdef __cplusplus
};
#endif
