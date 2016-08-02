using System;

public struct invoke3
{
	int a;

	public invoke3(int a)
	{
		this.a = a;
	}

	public override String ToString()
	{
		return base.ToString();
	}

	public String Test1()
	{
		return a.ToString();
	}

	public String Test2(int[] b)
	{
		return b[0].ToString();
	}

	public String Test3(int b)
	{
		return b.ToString();
	}

	public String Test4()
	{
		int b = 0;
	
		return b.ToString();
	}

	public String Test5()
	{
		return 0.ToString();
	}
}
