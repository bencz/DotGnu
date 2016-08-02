/*
 * cg_rbtree.h - Red-black tree implementation.
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

#ifndef	_CODEGEN_CG_RBTREE_H
#define	_CODEGEN_CG_RBTREE_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Structure of a red-black tree node.
 */
typedef struct _tagILRBTreeNode ILRBTreeNode;
struct _tagILRBTreeNode
{
	ILRBTreeNode   *_left, *_right, *_iterParent;
	int				_red : 1;
	int				_duplicate : 1;
	int				_continue : 1;
	int				kind : 29;

};

/*
 * Comparison function for searching red-black trees.
 */
typedef int (*ILRBCompareFunc)(void *key, ILRBTreeNode *node);

/*
 * Function for free'ing nodes within red-black trees.
 */
typedef void (*ILRBFreeFunc)(ILRBTreeNode *node);

/*
 * Data structure that holds a reference to a red-black tree.
 */
typedef struct _tagILRBTree
{
	ILRBTreeNode		head;
	ILRBTreeNode		nil;
	ILRBCompareFunc		compareFunc;
	ILRBFreeFunc		freeFunc;

} ILRBTree;

#define IL_RB_TREE_ITER_LD		0
#define IL_RB_TREE_ITER_LU		1
#define IL_RB_TREE_ITER_RD		2
#define IL_RB_TREE_ITER_RU		3
#define IL_RB_TREE_ITER_RET		4

/*
 * Initialize a red-black tree to empty.
 */
void ILRBTreeInit(ILRBTree *tree, ILRBCompareFunc compareFunc,
				  ILRBFreeFunc freeFunc);

/*
 * Search for a specific key within a red-black tree.
 */
ILRBTreeNode *ILRBTreeSearch(ILRBTree *tree, void *key);

/*
 * Get the next node with a specific key.
 */
ILRBTreeNode *ILRBTreeNext(ILRBTreeNode *node);

/*
 * Insert a new node into a red-black tree.
 */
void ILRBTreeInsert(ILRBTree *tree, ILRBTreeNode *node, void *key);

/*
 * Free all nodes within a red-black tree.
 */
void ILRBTreeFree(ILRBTree *tree);

/*
 * Get the root of a red-black tree.
 */
ILRBTreeNode *ILRBTreeGetRoot(ILRBTree *tree);

/*
 * Get the left or right sub-node of a red-black tree node.
 * Do not use "node->_left" and "node->_right" because they
 * may be used for other purposes in trees with duplicates,
 * or they may be a value other than NULL for tree edges.
 */
ILRBTreeNode *ILRBTreeGetLeft(ILRBTree *tree, ILRBTreeNode *node);
ILRBTreeNode *ILRBTreeGetRight(ILRBTree *tree, ILRBTreeNode *node);

/*
 * Return next node from a red-black tree.
 * Start iteration by calling with node=NULL.
 * Nodes are returned in sorted order.
 */
ILRBTreeNode *ILRBTreeIterNext(ILRBTree *tree, ILRBTreeNode *node, int *iter);

#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_CG_RBTREE_H */
