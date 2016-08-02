using System;

public class SwitchBlade
{
	public int SwitchMan(int last)
	{
		goto default;
		switch(last)
		{
			case 42:
				try
				{
				}
				finally
				{
				goto case 84;
				}
				return 1;
			case 84:
				return 2;
		}
		goto case 12; 
		return 0;
	}
}
