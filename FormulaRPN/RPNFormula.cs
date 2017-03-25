using System;
using System.Collections.Generic;
public class RPNFormula
{
	/// <summary>
	/// Types of nodes in a formula
	/// </summary>
	private enum NodeType
	{
		/// <summary>
		/// An unset node
		/// </summary>
		Default,

		/// <summary>
		/// A constant (e.g. 123.45)
		/// </summary>
		Constant,

		/// <summary>
		/// A variable (e.g. health)
		/// </summary>
		Variable,

		/// <summary>
		/// Addition operator (i.e. +)
		/// </summary>
		Add,

		/// <summary>
		/// Subtraction operator (i.e. -)
		/// </summary>
		Subtract,

		/// <summary>
		/// Multiplication operator (i.e. *)
		/// </summary>
		Multiply,

		/// <summary>
		/// Division operator (i.e. /)
		/// </summary>
		Divide,

		/// <summary>
		/// Modulus operator (i.e. %)
		/// </summary>
		Modulus
	}

	/// <summary>
	/// A node in the formula
	/// </summary>
	private struct Node
	{
		/// <summary>
		/// Type of the node
		/// </summary>
		public NodeType Type;

		/// <summary>
		/// Name of the variable (if Type == Variable)
		/// </summary>
		public string Name;

		/// <summary>
		/// Value of the variable or constant (if Type == Variable or Constant)
		/// </summary>
		public double Value;

		/// <summary>
		/// If the variable has its value set
		/// </summary>
		public bool HasValue;
	}


	/// <summary>
	/// Reasons why Compile() might fail
	/// </summary>
	public enum CompileErrorReason
	{
		/// <summary>
		/// No error
		/// </summary>
		NoError,

		/// <summary>
		/// The source string was null
		/// </summary>
		SourceNull,

		/// <summary>
		/// The source string was empty
		/// </summary>
		SourceEmpty,

		/// <summary>
		/// A constant or variable was expected
		/// </summary>
		ExpectedConstantOrVariable,

		/// <summary>
		/// The formula must end with a constant or a variable (i.e. not an operator)
		/// </summary>
		MustEndWithConstantOrVariable,

		/// <summary>
		/// A variable can't directly follow a constant. It must be separated by an operator.
		/// </summary>
		VariableCanNotFollowConstant,

		/// <summary>
		/// A constant can't directly follow a variable. It must be separated by an operator.
		/// </summary>
		ConstantCanNotFollowVariable,

		/// <summary>
		/// 
		/// </summary>
		ConstantCanNotFollowBrackets,

		/// <summary>
		/// 
		/// </summary>
		VariableCanNotFollowBrackets,

		/// <summary>
		/// An illegal character was found
		/// </summary>
		IllegalCharacter,

		/// <summary>
		/// An invalid constant (i.e one that couldn't be parsed) was found
		/// </summary>
		InvalidConstant
	}

	/// <summary>
	/// An error compiling a formula's source
	/// </summary>
	public struct CompileError
	{
		/// <summary>
		/// Reason for the compilation failure. Set to NoError on success.
		/// </summary>
		public CompileErrorReason Reason;

		/// <summary>
		/// Index of the relevant character or -1 if no character is relevant
		/// </summary>
		public int CharIndex;
	}

	/// <summary>
	/// Exception that is thrown when evaluating a formula with a variable whose value wasn't
	/// set by <see cref="SetVariable"/>.
	/// </summary>
	public class VariableNotSetException : Exception
	{
		/// <summary>
		/// Name of the variable that isn't set
		/// </summary>
		/// <value>The name of the variable that isn't set</value>
		public string Name { get; private set; }

		/// <summary>
		/// Create the exception
		/// </summary>
		/// <param name="name">Name of the variable that isn't set</param>
		public VariableNotSetException(string name)
		{
			Name = name;
		}
	}


	/// <summary>
	/// Exception that is thrown when evaluating a formula and there is no formula to evaluate
	/// </summary>
	public class NoFormulaException : Exception
	{
	}

	/// <summary>
	/// Nodes of the formula. May be empty.
	/// </summary>
	private List<Node> nodes;

	public RPNFormula(int capacity = 16)
	{
		if (capacity < 2)
		{
			capacity = 2;
		}
		nodes = new List<Node>(capacity);
	}

	/// <summary>
	/// compile() need a stack
	/// </summary>
	private static Stack<char> opstack = new Stack<char>(16);


	/// <summary>
	/// Compile a formula string. After successfully compiling, make sure to call
	/// <see cref="SetVariable"/> for each variable in the formula and then call
	/// <see cref="Evaluate"/>.
	/// </summary>
	/// <param name="source">Source to compile</param>
	/// <returns>
	/// The error resulting from this compilation. Will have the NoError type if successful.
	/// </returns>
	public CompileError Compile(string source)
	{
		// Reset all the nodes
		Reset();

		opstack.Clear();

		//init opstack bottom
		opstack.Push('#');

		int sourceLen = source.Length;

		int startCharIndex = 0;

		NodeType mode = NodeType.Default;

		for (int charIndex = 0; charIndex < source.Length; charIndex++)
		{
			char curChar = source[charIndex];

			switch (mode)
			{
				case NodeType.Default:

					switch (curChar)
					{
						case '(':
							opstack.Push(curChar);
							break;
						case ')':

							while (opstack.Peek() != '(')
							{
								nodes.Add(new Node { Type = GetOp(opstack.Pop()) });
							}

							opstack.Pop();//remove char (
							break;
						case '+':
						case '-':
							var lows1v = opstack.Peek();

							while (lows1v != '#')
							{
								if (IsLeftBrackets(lows1v))
								{
									break;
								}
								else
								{
									nodes.Add(new Node { Type = GetOp(opstack.Pop()) });

									lows1v = opstack.Peek();
								}
							}

							opstack.Push(curChar);
							break;
						case '*':
						case '/':
						case '%':
							var highs1v = opstack.Peek();

							while (highs1v != '#' && highs1v != '+' && highs1v != '-')
							{
								if (IsLeftBrackets(highs1v))
								{
									break;
								}
								else
								{
									nodes.Add(new Node { Type = GetOp(opstack.Pop()) });
								}
							}

							opstack.Push(curChar);
							break;
						default:

							if (IsConstant(curChar))
							{
								mode = NodeType.Constant;
								startCharIndex = charIndex;
								charIndex--;
							}
							else if (IsVariable(curChar))
							{
								mode = NodeType.Variable;
								startCharIndex = charIndex;
								charIndex--;
							}
							else
							{
								return new CompileError
								{
									Reason = CompileErrorReason.IllegalCharacter,
									CharIndex = charIndex
								};
							}
							break;
					}

					break;
				case NodeType.Constant:

					if (IsConstant(curChar))
					{
						continue;
					}
					else if (IsOperator(curChar) || IsRightBrackets(curChar))
					{
						double value;

						if (!double.TryParse(source.Substring(startCharIndex, charIndex - startCharIndex), out value))
						{
							return new CompileError
							{
								Reason = CompileErrorReason.InvalidConstant,
								CharIndex = startCharIndex
							};
						}
						nodes.Add(new Node
						{
							Type = NodeType.Constant,
							Value = value
						});

						charIndex--;

						mode = NodeType.Default;
					}
					else if (IsVariable(curChar))
					{
						return new CompileError
						{
							Reason = CompileErrorReason.VariableCanNotFollowConstant,
							CharIndex = charIndex
						};
					}
					else if (IsLeftBrackets(curChar))
					{
						return new CompileError
						{
							Reason = CompileErrorReason.VariableCanNotFollowBrackets,
							CharIndex = charIndex
						};
					}
					else
					{
						return new CompileError
						{
							Reason = CompileErrorReason.IllegalCharacter,
							CharIndex = charIndex
						};
					}
					break;
				case NodeType.Variable:

					if (IsVariable(curChar))
					{
						continue;
					}
					else if (IsOperator(curChar) || IsRightBrackets(curChar))
					{
						nodes.Add(new Node
						{
							Type = NodeType.Variable,
							Name = source.Substring(startCharIndex, charIndex - startCharIndex)
						});

						charIndex--;

						mode = NodeType.Default;
					}
					else if (IsConstant(curChar))
					{
						return new CompileError
						{
							Reason = CompileErrorReason.ConstantCanNotFollowVariable,
							CharIndex = charIndex
						};
					}
					else if (IsLeftBrackets(curChar))
					{
						return new CompileError
						{
							Reason = CompileErrorReason.ConstantCanNotFollowBrackets,
							CharIndex = charIndex
						};
					}
					else
					{
						return new CompileError
						{
							Reason = CompileErrorReason.IllegalCharacter,
							CharIndex = charIndex
						};
					}
					break;
				default:
					break;
			}
		}

		// Set end node to constant
		if (mode == NodeType.Constant)
		{
			double value;
			if (!double.TryParse(
				source.Substring(startCharIndex, sourceLen - startCharIndex), out value))
			{
				return new CompileError
				{
					Reason = CompileErrorReason.InvalidConstant,
					CharIndex = startCharIndex
				};
			}
			nodes.Add(new Node
			{
				Type = NodeType.Constant,
				Value = value
			});
		}
		// Set end node to variable
		else if (mode == NodeType.Variable)
		{
			nodes.Add(new Node
			{
				Type = NodeType.Variable,
				Name = source.Substring(startCharIndex, sourceLen - startCharIndex)
			});
		}
		// Source must end with a constant or variable. Trailing operators are not allowed.
		else
		{
			return new CompileError
			{
				Reason = CompileErrorReason.MustEndWithConstantOrVariable,
				CharIndex = sourceLen - 1
			};
		}

		while (opstack.Count > 0 && opstack.Peek() != '#')
		{
			nodes.Add(new Node { Type = GetOp(opstack.Pop()) });
		}

		return new CompileError
		{
			Reason = CompileErrorReason.NoError,
			CharIndex = -1
		};
	}

	/// <summary>
	/// Set a variable's value
	/// 
	/// Guaranteed not to allocate any managed memory (a.k.a. "garbage").
	/// </summary>
	/// <param name="name">Name of the variable</param>
	/// <param name="value">Value of the variable</param>
	/// <returns>The number of variables in the formula that were set</returns>
	public int SetVariable(string name, double value)
	{
		int numFound = 0;
		for (int i = 0, count = nodes.Count; i < count; ++i)
		{
			Node curNode = nodes[i];
			if (curNode.Type == NodeType.Variable && curNode.Name == name)
			{
				curNode.Value = value;
				curNode.HasValue = true;
				nodes[i] = curNode;
				numFound++;
			}
		}
		return numFound;
	}

	/// <summary>
	/// Clear the values of all variables. They must be re-set with <see cref="SetVariable"/> before
	/// calling <see cref="Evaluate"/> again.
	/// 
	/// Guaranteed not to allocate any managed memory (a.k.a. "garbage").
	/// </summary>
	public void ClearVariables()
	{
		for (int i = 0, count = nodes.Count; i < count; ++i)
		{
			Node curNode = nodes[i];
			if (curNode.Type == NodeType.Variable)
			{
				curNode.HasValue = false;
				nodes[i] = curNode;
			}
		}
	}

	/// <summary>
	/// Reset to the default state. The compiled formula and any set variables are lost.
	/// 
	/// Guaranteed not to allocate any managed memory (a.k.a. "garbage").
	/// </summary>
	public void Reset()
	{
		nodes.Clear();
	}

	/// <summary>
	/// Eval Stack
	/// </summary>
	private static Stack<Node> evaluateStack = new Stack<Node>(8);

	/// <summary>
	/// Evaluate the compiled formula with the set variable values. Make sure to call
	/// <see cref="Compile"/> first then <see cref="SetVariable"/> for each variable in the formula
	/// before you call this.
	/// 
	/// Guaranteed not to allocate any managed memory (a.k.a. "garbage") if successful.
	/// </summary>
	/// <returns>The result of the formula with the set variable values</returns>
	public double Evaluate()
	{
		evaluateStack.Clear();

		// Requires at least one node
		int numNodes = nodes.Count;
		if (numNodes == 0)
		{
			throw new NoFormulaException();
		}

		// Variables must have been set via SetVariable
		Node firstNode = nodes[0];

		if (firstNode.Type == NodeType.Variable && !firstNode.HasValue)
		{
			throw new VariableNotSetException(firstNode.Name);
		}

		for (int i = 0; i < numNodes; i++)
		{
			if (nodes[i].Type == NodeType.Variable || nodes[i].Type == NodeType.Constant)
			{
				evaluateStack.Push(nodes[i]);
			}
			else
			{
				if (evaluateStack.Count < 2)
				{
					throw new NoFormulaException();
				}
				else
				{
					var op1 = evaluateStack.Pop();

					var op2 = evaluateStack.Pop();

					if (!op1.HasValue || !op2.HasValue)
					{
						throw new VariableNotSetException(op2.Name);
					}

					var result = op1;

					switch (nodes[i].Type)
					{
						case NodeType.Add:
							result.Value = op2.Value + op1.Value;
							break;
						case NodeType.Subtract:
							result.Value = op2.Value - op1.Value;
							break;
						case NodeType.Multiply:
							result.Value = op2.Value * op1.Value;
							break;
						case NodeType.Divide:
							result.Value = op2.Value / op1.Value;
							break;
						case NodeType.Modulus:
							result.Value = op2.Value % op1.Value;
							break;
					}

					evaluateStack.Push(result);
				}
			}
		}

		if (evaluateStack.Count == 1)
			return evaluateStack.Pop().Value;
		else
			throw new NoFormulaException();
	}

	/// <summary>
	/// Get the names of all the variables
	/// </summary>
	/// <param name="variables"></param>
	public void GetVariableNames(HashSet<string> variables)
	{
		for (int i = 0, count = nodes.Count; i < count; ++i)
		{
			Node curNode = nodes[i];
			if (curNode.Type == NodeType.Variable)
			{
				variables.Add(curNode.Name);
			}
		}
	}

	/// <summary>
	/// Get Operator by char (eg: '-' Subtract)
	/// </summary>
	/// <param name="s1char"></param>
	/// <returns></returns>
	private NodeType GetOp(char s1char)
	{
		switch (s1char)
		{
			case '+':
				return NodeType.Add;
			case '-':
				return NodeType.Subtract;
			case '*':
				return NodeType.Multiply;
			case '/':
				return NodeType.Divide;
			case '%':
				return NodeType.Modulus;
			default:
				return NodeType.Default;
		}
	}

	/// <summary>
	/// Is it the char is operator
	/// </summary>
	/// <param name="curChar"></param>
	/// <returns></returns>
	private bool IsOperator(char curChar)
	{
		if (curChar == '+'
					|| curChar == '-'
					|| curChar == '*'
					|| curChar == '/'
					|| curChar == '%') return true;
		return false;
	}

	/// <summary>
	/// Constant agreement
	/// </summary>
	/// <param name="curChar"></param>
	/// <returns></returns>
	private bool IsConstant(char curChar)
	{
		if ((curChar >= '0' && curChar <= '9') || curChar == '.')
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Variable agreement
	/// </summary>
	/// <param name="curChar"></param>
	/// <returns></returns>
	private bool IsVariable(char curChar)
	{
		if ((curChar >= 'a' && curChar <= 'z') || (curChar >= 'A' && curChar <= 'Z') || curChar == '_')
			return true;
		return false;
	}

	/// <summary>
	/// Left Brackets agreement
	/// </summary>
	/// <param name="curChar"></param>
	/// <returns></returns>
	private bool IsLeftBrackets(char curChar)
	{
		return curChar == '(';
	}

	/// <summary>
	/// Right Brackets agreement
	/// </summary>
	/// <param name="curChar"></param>
	/// <returns></returns>
	private bool IsRightBrackets(char curChar)
	{
		return curChar == ')';
	}
}