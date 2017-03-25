
using FormulaLib;
using System;
namespace FormulaRPN
{
	class Program
	{
		static void Main(string[] args)
		{
			var source1 = "(a+ab)*c-(a+b)/e";

			var formulaR = new RPNFormula();

			Console.WriteLine(formulaR.Compile(source1).Reason);

			formulaR.SetVariable("a", 1);

			formulaR.SetVariable("b", 1);

			formulaR.SetVariable("ab", 2);

			formulaR.SetVariable("c", 2);

			formulaR.SetVariable("e", 1);

			Console.WriteLine(string.Format("({0}+{2})*{3}-({0}+{1})/{4} = {5}", 1, 1, 2, 2, 1, formulaR.Evaluate()));

			Console.ReadLine();
		}
	}
}
