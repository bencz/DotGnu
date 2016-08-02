/*
 * Financial.cs - Implementation of the
 *			"Microsoft.VisualBasic.Financial" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic
{

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class Financial
{
	// This class cannot be instantiated.
	private Financial() {}

	// Utility functions (based on kspread).
	static double getPay(double rate, double nper, double pv,
						 double fv, double type)
			{
				double pvif, fvifa;
				pvif = Math.Pow(1 + rate, nper);
				fvifa = (Math.Pow(1 + rate, nper) - 1.0) / rate;
				return ((-pv * pvif - fv) / ((1.0 + rate * type) * fvifa));
			}
	static double getPrinc(double start, double pay,
						   double rate, double period)
			{
				return (start * Math.Pow(1.0 + rate, period) +
					pay * ((Math.Pow(1 + rate, period) - 1) / rate));
			}

	// Calculate depreciation.
	public static double DDB(double Cost, double Salvage,
							 double Life, double Period,
							 [Optional] [DefaultValue(2.0)] double Factor)
			{
				double temp, temp2, temp3, temp4, temp5;
				if(Cost < 0.0 || Salvage < 0.0 || Life <= 0.0 ||
				   Period < 0.0 || Factor < 0.0)
				{
					throw new ArgumentException(S._("VB_InvalidDDB"));
				}
				if(Cost == 0.0)
				{
					return 0.0;
				}
				if(Life < 2.0)
				{
					return Cost - Salvage;
				}
				if(Life == 2.0)
				{
					if(Period > 1.0)
					{
						return 0.0;
					}
					else
					{
						return Cost - Salvage;
					}
				}
				if(Period <= 1.0)
				{
					temp = Cost * Factor / Life;
					temp2 = Cost - Salvage;
					if(temp < temp2)
					{
						return temp;
					}
					else
					{
						return temp2;
					}
				}
				temp2 = (Life - Factor) / Life;
				temp3 = Period - 1.0;
				temp = (Cost * Factor / Life) * Math.Pow(temp2, temp3);
				temp4 = Cost * (1.0 - Math.Pow(temp2, Period));
				temp5 = temp4 - Cost + Salvage;
				if(temp5 > 0.0)
				{
					temp -= temp5;
				}
				if(temp >= 0.0)
				{
					return temp;
				}
				else
				{
					return 0.0;
				}
			}

	// Calculate future value.
	public static double FV(double Rate, double NPer, double Pmt,
							 [Optional] [DefaultValue(0.0)] double PV,
							 [Optional] [DefaultValue(DueDate.EndOfPeriod)]
							 		DueDate Due)
			{
				double rate1, endRate, temp;
				if(Rate == 0.0)
				{
					return -PV - NPer * Pmt;
				}
				rate1 = Rate + 1.0;
				if(Due == DueDate.EndOfPeriod)
				{
					endRate = 1.0;
				}
				else
				{
					endRate = rate1;
				}
				temp = Math.Pow(rate1, NPer);
				return -PV * temp - (Pmt / Rate * endRate) * (temp - 1.0);
			}

	// Calculate interest payment (algorithm from kspread).
	public static double IPmt(double Rate, double Per, double NPer, double PV,
							  [Optional] [DefaultValue(0.0)] double FV,
							  [Optional] [DefaultValue(DueDate.EndOfPeriod)]
							 		DueDate Due)
			{
				double type;
				if(Due == DueDate.EndOfPeriod)
				{
					type = 0.0;
				}
				else
				{
					type = 1.0;
				}
				double payment = getPay(Rate, NPer, PV, FV, type);
				double ipmt = -(getPrinc(PV, payment, Rate, Per - 1.0));
				return ipmt * Rate;
			}

	// Calculate internal rate of return.
	[TODO]
	public static double IRR(ref double[] ValueArray,
							 [Optional] [DefaultValue(0.1)] double Guess)
			{
				// TODO
				return 0.0;
			}

	// Calculate modified internal rate of return.
	[TODO]
	public static double MIRR(ref double[] ValueArray,
							  double FinanceRate, double ReinvestRate)
			{
				// TODO
				return 0.0;
			}

	// Calculate number of periods (algorithm from kspread).
	public static double NPer(double Rate, double Pmt, double PV,
							  [Optional] [DefaultValue(0.0)] double FV,
							  [Optional] [DefaultValue(DueDate.EndOfPeriod)]
							 		DueDate Due)
			{
				if(Rate <= 0.0)
				{
					throw new ArgumentException(S._("VB_InvalidNPer"));
				}
				double type;
				if(Due == DueDate.EndOfPeriod)
				{
					type = 0.0;
				}
				else
				{
					type = 1.0;
				}
				double d = Pmt * (1.0 + Rate * type) - FV * Rate;
				double d2 = PV * Rate + Pmt * (1.0 + Rate * type);
				double res = d / d2;
				if(res <= 0.0)
				{
					throw new ArgumentException(S._("VB_InvalidNPer"));
				}
				return Math.Log(res) / Math.Log(1.0 + Rate);
			}

	// Calculate net present value.
	[TODO]
	public static double NPV(double Rate, ref double[] ValueArray)
			{
				// TODO
				return 0.0;
			}

	// Calculate payment.
	public static double Pmt(double Rate, double NPer, double PV,
							 [Optional] [DefaultValue(0.0)] double FV,
							 [Optional] [DefaultValue(DueDate.EndOfPeriod)]
							 		DueDate Due)
			{
				double type;
				if(Due == DueDate.EndOfPeriod)
				{
					type = 0.0;
				}
				else
				{
					type = 1.0;
				}
				return getPay(Rate, NPer, PV, FV, type);
			}

	// Calculate principal payment (algorithm from kspread).
	public static double PPmt(double Rate, double Per, double NPer, double PV,
							  [Optional] [DefaultValue(0.0)] double FV,
							  [Optional] [DefaultValue(DueDate.EndOfPeriod)]
							 		DueDate Due)
			{
				double type;
				if(Due == DueDate.EndOfPeriod)
				{
					type = 0.0;
				}
				else
				{
					type = 1.0;
				}
				double pay = getPay(Rate, NPer, PV, FV, type);
				double ipmt = (-getPrinc(PV, pay, Rate, Per - 1.0)) * Rate;
				return pay - ipmt;
			}

	// Calculate present value.
	public static double PV(double Rate, double NPer, double Pmt,
							[Optional] [DefaultValue(0.0)] double FV,
							[Optional] [DefaultValue(DueDate.EndOfPeriod)]
							 		DueDate Due)
			{
				double rate1, endRate, fullRate;
				if(Rate == 0.0)
				{
					return -FV - Pmt * NPer;
				}
				rate1 = Rate + 1.0;
				if(Due == DueDate.EndOfPeriod)
				{
					endRate = 1.0;
				}
				else
				{
					endRate = rate1;
				}
				fullRate = Math.Pow(rate1, NPer);
				return -(FV + Pmt * endRate * ((fullRate - 1.0) / Rate))
								/ fullRate;
			}

	// Calculate rate.
	public static double Rate(double NPer, double Pmt, double PV,
							  [Optional] [DefaultValue(0.0)] double FV,
							  [Optional] [DefaultValue(DueDate.EndOfPeriod)]
							 		DueDate Due,
							  [Optional] [DefaultValue(0.1)] double Guess)
			{
				double rate, rate2, temp, temp2, swap;
				if(NPer <= 0.0)
				{
					throw new ArgumentException(S._("VB_InvalidRate"));
				}
				rate = Guess;
				temp = RateInternal(NPer, Pmt, PV, FV, Due, rate);
				if(temp > 0.0)
				{
					rate2 = rate / 2;
				}
				else
				{
					rate2 = rate * 2;
				}
				temp2 = RateInternal(NPer, Pmt, PV, FV, Due, rate2);
				for(int iter = 0; iter < 40; ++iter)
				{
					if(temp == temp2)
					{
						if(rate2 > rate)
						{
							rate -= 1.0e-5;
						}
						else
						{
							rate += 1.0e-5;
						}
						temp = RateInternal(NPer, Pmt, PV, FV, Due, rate);
						if(temp == temp2)
						{
							throw new ArgumentException
								(S._("VB_CouldntCalculateRate"));
						}
					}
					rate = rate2 - ((rate2 - rate) * temp2) / (temp2 - temp);
					temp = RateInternal(NPer, Pmt, PV, FV, Due, rate);
					if(Math.Abs(temp) < 1.0e-8)
					{
						return rate;
					}
					swap = rate;
					rate = rate2;
					rate2 = swap;
					swap = temp;
					temp = temp2;
					temp2 = swap;
				}
				throw new ArgumentException(S._("VB_CouldntCalculateRate"));
			}
	private static double RateInternal(double NPer, double Pmt, double PV,
							  		   double FV, DueDate Due, double Rate)
			{
				double tempRate, rate1, endRate;
				if(Rate == 0.0)
				{
					return PV + Pmt * NPer + FV;
				}
				tempRate = Math.Pow(Rate + 1.0, NPer);
				rate1 = Rate + 1.0;
				if(Due == DueDate.EndOfPeriod)
				{
					endRate = 1.0;
				}
				else
				{
					endRate = rate1;
				}
				return PV * tempRate +
					   Pmt * endRate * (tempRate - 1.0) / Rate + FV;
			}

	// Calculate straight-line depreciation (algorithm from kspread).
	public static double SLN(double Cost, double Salvage, double Life)
			{
				if(Life <= 0.0)
				{
					throw new ArgumentException(S._("VB_InvalidSLN"));
				}
				return (Cost - Salvage) / Life;
			}

	// Calculate sum of years for depreciation (algorithm from kspread).
	public static double SYD(double Cost, double Salvage,
							 double Life, double Period)
			{
				if(Life <= 0.0)
				{
					throw new ArgumentException(S._("VB_InvalidSYD"));
				}
				return ((Cost - Salvage) / (Life - Period + 1.0) * 2.0) /
							(Life * (Life + 1.0));
			}

}; // class Financial

}; // namespace Microsoft.VisualBasic
