namespace FormulaGenericLib
{
	public interface MathProvider<T>
	{
		T Divide(T a, T b);
		T Multiply(T a, T b);
		T Add(T a, T b);
		T Negate(T a);
		T Subtract(T a, T b);
		T Modulus(T a, T b);
		bool TryParse(string str, out T v);
	}
}