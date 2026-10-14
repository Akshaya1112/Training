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
class TVariable : TNumber {
   #region Constructor ----------------------------------------------
   /// <summary>Initializes a variable token</summary>
   /// <param name="eval">Evaluator used to resolve the variable</param>
   /// <param name="name">Name of the variable</param>
   public TVariable (Evaluator eval, string name) => (Name, mEval) = (name, eval);
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>Gets the variable name</summary>
   public string Name { get; private set; }

   /// <summary>Gets the current value of the variable</summary>
   public override double Value => mEval.GetVariable (Name);
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the variable</summary>
   public override string ToString () => $"var:{Name}";
   #endregion

   #region Private data ---------------------------------------------
   // Stores the evaluator used to resolve the variable
   readonly Evaluator mEval;
   #endregion
}
#endregion

#region Class TOperator ---------------------------------------------------------------------------
/// <summary>Base class for all operators</summary>
abstract class TOperator (Evaluator eval) : Token {
   #region Properties -----------------------------------------------
   /// <summary>Gets the priority of the operator</summary>
   public abstract int Priority { get; }
   #endregion

   #region Private data ---------------------------------------------
   // Stores the evaluator associated with the operator
   readonly protected Evaluator mEval = eval;
   #endregion
}
#endregion

#region Class TOpArithmetic -----------------------------------------------------------------------
/// <summary>Represents a binary arithmetic operator</summary>
class TOpArithmetic (Evaluator eval, char ch) : TOperator (eval) {
   #region Properties -----------------------------------------------
   /// <summary>Gets the arithmetic operator</summary>
   public char Op { get; private set; } = ch;

   /// <summary>Gets the operator priority</summary>
   public override int Priority => sPriority[Op];
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the operator</summary>
   public override string ToString () => $"op:{Op}:{Priority}";

   /// <summary>Evaluates the binary arithmetic operation</summary>
   /// <param name="a">First operand</param>
   /// <param name="b">Second operand</param>
   /// <returns>The result of the operation</returns>
   public double Evaluate (double a, double b) {
      return Op switch {
         '+' => a + b,
         '-' => a - b,
         '*' => a * b,
         '/' => a / b,
         '^' => Math.Pow (a, b),
         '=' => b,
         _ => throw new EvalException ($"Unknown operator: {Op}"),
      };
   }
   #endregion

   #region Private data ---------------------------------------------
   // Stores the priority of each arithmetic operator
   static readonly Dictionary<char, int> sPriority = new (){
      ['+'] = 1,['-'] = 1,['*'] = 2,['/'] = 2, ['^'] = 4,['='] = 0,};
   #endregion
}
#endregion

#region Class TOpUnary ----------------------------------------------------------------------------
/// <summary>Represents a unary arithmetic operator</summary>
class TOpUnary (Evaluator eval, char ch) : TOperator (eval) {
   #region Properties -----------------------------------------------
   /// <summary>Gets the priority of the unary operator</summary>
   public override int Priority => 3;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Evaluates the unary arithmetic operation</summary>
   /// <param name="f">Operand to process</param>
   /// <returns>The result of the operation</returns>
   public double Evaluate (double f) => ch == '-' ? -f : f;

   /// <summary>Returns the string representation of the unary operator</summary>
   public override string ToString () => $"unary:{ch}:{Priority}";
   #endregion
}
#endregion

#region Class TOpFunction -------------------------------------------------------------------------
/// <summary>Represents a mathematical function</summary>
class TOpFunction (Evaluator eval, string name) : TOperator (eval) {
   #region Properties -----------------------------------------------
   /// <summary>Gets the function name</summary>
   public string Func { get; private set; } = name;

   /// <summary>Gets the function priority</summary>
   public override int Priority => 5;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the function</summary>
   public override string ToString () => $"func:{Func}:{Priority}";

   /// <summary>Evaluates the mathematical function</summary>
   /// <param name="f">Function argument</param>
   /// <returns>The function result</returns>
   public double Evaluate (double f) {
      return Func switch {
         "sin" => Math.Sin (D2R (f)),
         "cos" => Math.Cos (D2R (f)),
         "tan" => Math.Tan (D2R (f)),
         "sqrt" => Math.Sqrt (f),
         "log" => Math.Log (f),
         "exp" => Math.Exp (f),
         "asin" => R2D (Math.Asin (f)),
         "acos" => R2D (Math.Acos (f)),
         "atan" => R2D (Math.Atan (f)),
         _ => throw new EvalException ($"Unknown function: {Func}"),
      };
      double D2R (double f) => f * Math.PI / 180;
      double R2D (double f) => f * 180 / Math.PI;
   }
   #endregion
}
#endregion

#region Class TPunctuation ------------------------------------------------------------------------
/// <summary>Represents punctuation such as parentheses</summary>
class TPunctuation (char ch) : Token {
   #region Properties -----------------------------------------------
   /// <summary>Gets the punctuation character</summary>
   public char Punct { get; private set; } = ch;
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
   public string Message { get; private set; } = message;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the error</summary>
   public override string ToString () => $"error:{Message}";
   #endregion
}
#endregion