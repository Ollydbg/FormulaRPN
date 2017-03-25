using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormulaLib;

namespace FormulaTest
{
	[TestClass]
	public class UnitTestCompile
	{
		public RPNFormula formulaR;

		[TestInitialize]
		public void Init()
		{
			formulaR = new RPNFormula();
		}

		[TestMethod]
		public void TestCompile()
		{
			var source1 = "(a+ab)*c-(a+b)/e";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.NoError);
		}

		[TestMethod]
		public void TestErrorConstantCanNotFollowVariable()
		{
			var source1 = "(a+ab)*c-(a+b)/1e";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.ConstantCanNotFollowVariable);
		}

		[TestMethod]
		public void TestErrorVariableCanNotFollowConstant()
		{
			var source1 = "(a+ab)*c-(a+b)/e1";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.VariableCanNotFollowConstant);
		}


		[TestMethod]
		public void TestErrorVariableCanNotFollowBrackets()
		{
			var source1 = "(a(+ab)*c-(a+b)/e";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.VariableCanNotFollowBrackets);
		}


		[TestMethod]
		public void TestErrorConstantCanNotFollowBrackets()
		{
			var source1 = "(1(+ab)*c-(a+b)/e";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.ConstantCanNotFollowBrackets);
		}

		[TestMethod]
		public void TestErrorIllegalCharacter()
		{
			var source1 = "(1 (+ab)*c-(a+b)/e";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.IllegalCharacter);
		}

		[TestMethod]
		public void TestErrorMustEndWithConstantOrVariable()
		{
			var source1 = "(a+ab)*c-(a+b)/e+";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.MustEndWithConstantOrVariable);
		}


		[TestMethod]
		public void TestErrorSourceEmpty()
		{
			var source1 = "";

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.SourceEmpty);
		}

		[TestMethod]
		public void TestErrorSourceNull()
		{
			string source1 = null;

			Assert.AreEqual(formulaR.Compile(source1).Reason, RPNFormula.CompileErrorReason.SourceNull);
		}
	}
}
