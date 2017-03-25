namespace FormulaGenericLib
{
	public struct DoubleMathProvider : MathProvider<double>
	{
		public double Divide(double a, double b)
		{
			return a / b;
		}

		public double Multiply(double a, double b)
		{
			return a * b;
		}

		public double Add(double a, double b)
		{
			return a + b;
		}

		public double Negate(double a)
		{
			return -a;
		}


		public double Subtract(double a, double b)
		{
			return a - b;
		}


		public bool TryParse(string str, out double v)
		{
			return double.TryParse(str, out v);
		}


		public double Modulus(double a, double b)
		{
			return a % b;
		}
	}
}