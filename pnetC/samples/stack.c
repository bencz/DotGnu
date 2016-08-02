#include <stdio.h>
#include <malloc.h>

typedef struct _node
{
    int a;
    struct _node *prev;
}node;

node* push_int(node* old_node,int cada)
{
    node *retval;
    retval = (node*) malloc(sizeof(node));
    retval->a=cada;
    retval->prev=old_node;
    return retval;
}


int main()
{
    node * stack=NULL;
    node *tmp=NULL;
    int i;
    for(i=0;i<100;i++)
    {
        stack=push_int(stack,i);
    }
    for(tmp=stack;tmp!=NULL;tmp=tmp->prev)
    {
        printf("%d\n",tmp->a);
    }
    return 0;
}
