namespace FormulaGenericLib
{
	public struct FloatMathProvider : MathProvider<float>
	{
		public float Divide(float a, float b)
		{
			return a / b;
		}

		public float Multiply(float a, float b)
		{
			return a * b;
		}

		public float Add(float a, float b)
		{
			return a + b;
		}

		public float Negate(float a)
		{
			return -a;
		}

		public float Subtract(float a, float b)
		{
			return a - b;
		}


		public bool TryParse(string str, out float v)
		{
			return float.TryParse(str, out v);
		}


		public float Modulus(float a, float b)
		{
			return a % b;
		}
	}
}