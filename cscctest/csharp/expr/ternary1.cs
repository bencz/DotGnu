using System;

enum Dino
{
	None=0,
	Trex=1
};

class FooBar
{
	static void Foo(bool x)
	{
		String s=(x ? Dino.Trex : 0).ToString();
		s=(x ? 0 : Dino.Trex).ToString();
	}	
}
