using System;

class foo {

	private static int _x;

	public foo(int x) {
		_x = x;
	}

	public static int bar() {
		return _x;
	}
}

class bar {

	static int foo;

	static void Main() {
	
		foo x;
		int y;

		//System.Net.Dns z;

		foo = 1;	// class member
		x = new foo(foo);

		y = foo.bar();
	}
};

