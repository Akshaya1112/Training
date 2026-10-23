// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Token.cs
// Defines the token types used by the expression evaluator.
// ------------------------------------------------------------------------------------------------

#region Class Token -------------------------------------------------------------------------------
/// <summary>Base class for all expression tokens</summary>
abstract class Token {
}
#endregion

#region Class TNumber -----------------------------------------------------------------------------
/// <summary>Base class for tokens that represent numeric values</summary>
abstract class TNumber : Token {
   #region Properties -----------------------------------------------
   /// <summary>Gets the numeric value represented by the token</summary>
   public abstract double Value { get; }
   #endregion
}
#endregion

#region Class TLiteral ----------------------------------------------------------------------------
/// <summary>Represents a numeric literal</summary>
class TLiteral (double f) : TNumber {
   #region Properties -----------------------------------------------
   /// <summary>Gets the literal value</summary>
   public override double Value => mValue;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the literal</summary>
   public override string ToString () => $"literal:{Value}";
   #endregion

   #region Private data ---------------------------------------------
   // Stores the literal value
   readonly double mValue = f;
   #endregion
}
#endregion

#region Class TVariable ---------------------------------------------------------------------------
/// <summary>Represents a variable</summary>
/// <param name="eval">Evaluator used to resolve the variable</param>
/// <param name="name">Name of the variable</param>
class TVariable (Evaluator eval, string name) : TNumber {
   #region Properties -----------------------------------------------
   /// <summary>Gets the variable name</summary>
   public string Name { get; } = name;

   /// <summary>Gets the current value of the variable</summary>
   public override double Value => mEval.GetVariable (Name);
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the variable</summary>
   public override string ToString () => $"var:{Name}";
   #endregion

   #region Private data ---------------------------------------------
   // Stores the evaluator used to resolve the variable
   readonly Evaluator mEval = eval;
   #endregion
}
#endregion

#region Class TOperator ---------------------------------------------------------------------------
/// <summary>Base class for all operators</summary>
abstract class TOperator : Token {
   #region Properties -----------------------------------------------
   /// <summary>Gets the priority of the operator</summary>
   public abstract int Priority { get; }
   #endregion
}
#endregion

#region Class TOpPrefix ---------------------------------------------------------------------------
/// <summary>Base class for prefix operators that take a single operand</summary>
abstract class TOpPrefix : TOperator {
   #region Methods --------------------------------------------------
   /// <summary>Evaluates the operator on a single operand</summary>
   /// <param name="f">Operand to process</param>
   /// <returns>The result of the operation</returns>
   public abstract double Evaluate (double f);
   #endregion
}
#endregion

#region Class TOpUnary ----------------------------------------------------------------------------
/// <summary>Represents a unary arithmetic operator</summary>
class TOpUnary (char ch) : TOpPrefix {
   #region Properties -----------------------------------------------
   /// <summary>Gets the priority of the unary operator</summary>
   public override int Priority => 3;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Evaluates the unary arithmetic operation</summary>
   /// <param name="f">Operand to process</param>
   /// <returns>The result of the operation</returns>
   public override double Evaluate (double f) => ch == '-' ? -f : f;

   /// <summary>Returns the string representation of the unary operator</summary>
   public override string ToString () => $"unary:{ch}:{Priority}";
   #endregion
}
#endregion

#region Class TOpFunction -------------------------------------------------------------------------
/// <summary>Represents a mathematical function</summary>
class TOpFunction (string name) : TOpPrefix {
   #region Properties -----------------------------------------------
   /// <summary>Gets the function name</summary>
   public string Func { get; } = name;

   /// <summary>Gets the function priority</summary>
   public override int Priority => 4;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Checks whether the name is a supported function</summary>
   /// <param name="name">Name to check</param>
   /// <returns>True if the name is a supported function</returns>
   public static bool IsFunction (string name) => sNames.Contains (name);

   /// <summary>Returns the string representation of the function</summary>
   public override string ToString () => $"func:{Func}:{Priority}";

   /// <summary>Evaluates the mathematical function</summary>
   /// <param name="f">Function argument</param>
   /// <returns>The function result</returns>
   public override double Evaluate (double f) {
      double result = Calculate (f);
      if (!double.IsFinite (result)) throw new EvalException ($"Invalid argument for {Func}.");
      return result;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Calculates the function (trigonometric functions work in degrees)
   double Calculate (double f) => Func switch {
      "sin" => Math.Sin (double.DegreesToRadians (f)),
      "cos" => Math.Cos (double.DegreesToRadians (f)),
      "tan" => Math.Tan (double.DegreesToRadians (f)),
      "asin" => double.RadiansToDegrees (Math.Asin (f)),
      "acos" => double.RadiansToDegrees (Math.Acos (f)),
      "atan" => double.RadiansToDegrees (Math.Atan (f)),
      "sqrt" => Math.Sqrt (f),
      "log" => Math.Log (f),
      "exp" => Math.Exp (f),
      _ => throw new EvalException ($"Unknown function: {Func}"),
   };
   #endregion

   #region Private data ---------------------------------------------
   // Stores the supported function names
   static readonly string[] sNames = ["sin", "cos", "tan", "asin", "acos",
                                      "atan", "sqrt", "log", "exp"];
   #endregion
}
#endregion

#region Class TOpArithmetic -----------------------------------------------------------------------
/// <summary>Represents a binary arithmetic operator</summary>
class TOpArithmetic (char ch) : TOperator {
   #region Properties -----------------------------------------------
   /// <summary>Gets the arithmetic operator</summary>
   public char Op { get; } = ch;

   /// <summary>Gets the operator priority</summary>
   public override int Priority => Op switch {
      '=' => 0,
      '+' or '-' => 1,
      '*' or '/' => 2,
      _ => 5
   };
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Checks whether the character is an arithmetic operator</summary>
   /// <param name="ch">Character to check</param>
   /// <returns>True if the character is an arithmetic operator</returns>
   public static bool IsOperator (char ch) => "+-*/^=".Contains (ch);

   /// <summary>Returns the string representation of the operator</summary>
   public override string ToString () => $"op:{Op}:{Priority}";

   /// <summary>Evaluates the binary arithmetic operation</summary>
   /// <param name="a">First operand</param>
   /// <param name="b">Second operand</param>
   /// <returns>The result of the operation</returns>
   public double Evaluate (double a, double b) => Op switch {
      '+' => a + b,
      '-' => a - b,
      '*' => a * b,
      '/' => b == 0 ? throw new EvalException ("Divide by zero.") : a / b,
      '^' => Math.Pow (a, b),
      '=' => b,
      _ => throw new EvalException ($"Unknown operator: {Op}")
   };
   #endregion
}
#endregion


#region Class TPunctuation ------------------------------------------------------------------------
/// <summary>Represents punctuation such as parentheses</summary>
class TPunctuation (char ch) : Token {
   #region Properties -----------------------------------------------
   /// <summary>Gets the punctuation character</summary>
   public char Punct { get; } = ch;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the punctuation</summary>
   public override string ToString () => $"punct:{Punct}";
   #endregion
}
#endregion

#region Class TEnd --------------------------------------------------------------------------------
/// <summary>Represents the end of an expression</summary>
class TEnd : Token {
   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the token</summary>
   public override string ToString () => "end";
   #endregion
}
#endregion

#region Class TError ------------------------------------------------------------------------------
/// <summary>Represents a tokenization error</summary>
class TError (string message) : Token {
   #region Properties -----------------------------------------------
   /// <summary>Gets the error message</summary>
   public string Message { get; } = message;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the error</summary>
   public override string ToString () => $"error:{Message}";
   #endregion
}
#endregion