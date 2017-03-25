namespace FormulaGenericLib
{
	public struct IntMathProvider : MathProvider<int>
	{
		public int Divide(int a, int b)
		{
			return a / b;
		}

		public int Multiply(int a, int b)
		{
			return a * b;
		}

		public int Add(int a, int b)
		{
			return a + b;
		}

		public int Negate(int a)
		{
			return -a;
		}


		public int Subtract(int a, int b)
		{
			return a - b;
		}


		public bool TryParse(string str, out int v)
		{
			return int.TryParse(str, out v);
		}


		public int Modulus(int a, int b)
		{
			return a % b;
		}
	}
}