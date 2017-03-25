using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormulaGenericLib;
namespace FormulaGenericTest
{
	[TestClass]
	public class UnitTestCompile
	{
		public RPNFormula<IntMathProvider, int> formula;

		[TestInitialize]
		public void Init()
		{
			formula = new RPNFormula<IntMathProvider, int>();
		}


		[TestMethod]
		public void TestCompile()
		{
			Assert.AreEqual(formula.Compile("(a+ab)*c-(a+b)/e").Reason, RPNFormula<IntMathProvider, int>.CompileErrorReason.NoError);
		}
	}
}
