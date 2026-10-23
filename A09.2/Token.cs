// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Token.cs
// Defines the token types used by the expression evaluator.
// ------------------------------------------------------------------------------------------------
namespace Eval;

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
/// <summary>A number written directly in the expression, like 3.14</summary>
class TLiteral : TNumber {
   #region Constructors ---------------------------------------------
   /// <summary>Creates a literal token with the given value</summary>
   /// <param name="f">The numeric value</param>
   public TLiteral (double f) => mValue = f;
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>Gets the literal value</summary>
   public override double Value => mValue;
   #endregion

   #region Methods --------------------------------------------------
   public override string ToString () => $"literal:{Value}";
   #endregion

   #region Private data ---------------------------------------------
   readonly double mValue;    // Stores the literal value
   #endregion
}
#endregion

#region Class TVariable ---------------------------------------------------------------------------
/// <summary>A variable name; its value is looked up in the evaluator when needed</summary>
class TVariable : TNumber {
   #region Constructors ---------------------------------------------
   /// <summary>Creates a variable token</summary>
   /// <param name="eval">The evaluator that stores the variable values</param>
   /// <param name="name">The variable name</param>
   public TVariable (Evaluator eval, string name) => (Name, mEval) = (name, eval);
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>The name of the variable</summary>
   public string Name { get; private set; }

   /// <summary>The current value of the variable</summary>
   /// <exception cref="EvalException">The variable has not been assigned</exception>
   public override double Value => mEval.GetVariable (Name);
   #endregion

   #region Methods --------------------------------------------------
   public override string ToString () => $"var:{Name}";
   #endregion

   #region Private data ---------------------------------------------
   readonly Evaluator mEval;    // Stores the evaluator used to resolve the variable
   #endregion
}
#endregion

#region Class TOperator ---------------------------------------------------------------------------
/// <summary>Base class for all operators (arithmetic, unary and functions)</summary>
abstract class TOperator : Token {
   #region Constructors ----------------------------------------------
   /// <summary>Creates an operator token</summary>
   /// <param name="eval">The evaluator that owns this operator</param>
   protected TOperator (Evaluator eval) => mEval = eval;
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>The basic priority of this operator (higher binds tighter)</summary>
   public abstract int Priority { get; }

   /// <summary>The priority after adding the offset caused by enclosing brackets</summary>
   public int FinalPriority { get; set; }
   #endregion

   #region Private data ---------------------------------------------
   protected readonly Evaluator mEval;    // The evaluator that owns this operator
   #endregion
}
#endregion

#region Class TOpArithmetic -----------------------------------------------------------------------
/// <summary>A binary operator: + - * / ^ or the assignment operator =</summary>
class TOpArithmetic : TOperator {
   #region Constructors ---------------------------------------------
   /// <summary>Creates a binary operator token</summary>
   /// <param name="eval">The evaluator that owns this operator</param>
   /// <param name="ch">The operator character</param>
   public TOpArithmetic (Evaluator eval, char ch) : base (eval) => Op = ch;
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>The operator character</summary>
   public char Op { get; private set; }

   /// <summary>The basic priority of this operator</summary>
   public override int Priority => sPriority[Op];
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Applies the operator to the two operands</summary>
   /// <param name="a">The left operand</param>
   /// <param name="b">The right operand</param>
   /// <returns>The result of the operation</returns>
   /// <exception cref="EvalException">The operator is not supported</exception>
   public double Evaluate (double a, double b) {
      return Op switch {
         '+' => a + b,
         '-' => a - b,
         '*' => a * b,
         '/' => a / b,
         '^' => Math.Pow (a, b),
         _ => throw new EvalException ($"Unknown operator: {Op}"),
      };
   }

   public override string ToString () => $"op:{Op}:{Priority}";
   #endregion

   #region Private data ---------------------------------------------
   // Priority of each operator character
   static Dictionary<char, int> sPriority = new () {
      ['+'] = 1, ['-'] = 1, ['*'] = 2, ['/'] = 2, ['^'] = 4, ['='] = 4,
   };
   #endregion
}
#endregion

#region Class TOpUnary ----------------------------------------------------------------------------
/// <summary>A unary plus or minus, like the minus in -5</summary>
class TOpUnary : TOperator {
   #region Constructors ---------------------------------------------
   /// <summary>Creates a unary operator token</summary>
   /// <param name="eval">The evaluator that owns this operator</param>
   /// <param name="ch">The operator character, '+' or '-'</param>
   public TOpUnary (Evaluator eval, char ch) : base (eval) => Op = ch;
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>The operator character</summary>
   public char Op { get; private set; }

   /// <summary>The basic priority of this operator</summary>
   public override int Priority => 2;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Applies the operator to the operand</summary>
   /// <param name="f">The operand</param>
   /// <returns>The negated value for '-', otherwise the value unchanged</returns>
   public double Evaluate (double f) => Op == '-' ? -f : f;

   public override string ToString () => $"unary:{Op}:{Priority}";
   #endregion
}
#endregion

#region Class TOpFunction -------------------------------------------------------------------------
/// <summary>A named function like sin or sqrt (trigonometry works in degrees)</summary>
class TOpFunction : TOperator {
   #region Constructors ---------------------------------------------
   /// <summary>Creates a function token</summary>
   /// <param name="eval">The evaluator that owns this function</param>
   /// <param name="name">The function name</param>
   public TOpFunction (Evaluator eval, string name) : base (eval) => Func = name;
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>The name of the function</summary>
   public string Func { get; private set; }

   /// <summary>The basic priority of this operator</summary>
   public override int Priority => 3;
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Applies the function to the argument</summary>
   /// <param name="f">The argument (in degrees for trigonometric functions)</param>
   /// <returns>The result (in degrees for inverse trigonometric functions)</returns>
   /// <exception cref="EvalException">The function is not supported</exception>
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
         _ => throw new EvalException ($"Unknown function: {Func}")
      };
      double D2R (double f) => f * Math.PI / 180;   // Converts degrees to radians
      double R2D (double f) => f * 180 / Math.PI;   // Converts radians to degrees
   }

   public override string ToString () => $"func:{Func}:{Priority}";
   #endregion
}
#endregion

#region Class TPunctuation ------------------------------------------------------------------------
/// <summary>A bracket character: ( or )</summary>
class TPunctuation : Token {
   #region Constructors ---------------------------------------------
   /// <summary>Creates a punctuation token</summary>
   /// <param name="ch">The bracket character</param>
   public TPunctuation (char ch) => Punct = ch;
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>The bracket character</summary>
   public char Punct { get; private set; }
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the punctuation</summary>
   public override string ToString () => $"punct:{Punct}";
   #endregion
}
#endregion

#region Class TEnd --------------------------------------------------------------------------------
/// <summary>Marks the end of the input text</summary>
class TEnd : Token {
   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the token</summary>
   public override string ToString () => "end";
   #endregion
}
#endregion

#region Class TError ------------------------------------------------------------------------------
/// <summary>Reports a problem found while tokenizing, like an unknown symbol</summary>
class TError : Token {
   #region Constructors ---------------------------------------------
   /// <summary>Creates an error token</summary>
   /// <param name="message">The description of the problem</param>
   public TError (string message) => Message = message;
   #endregion

   #region Properties -----------------------------------------------
   /// <summary>The description of the problem</summary>
   public string Message { get; private set; }
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the string representation of the error</summary>
   public override string ToString () => $"error:{Message}";
   #endregion
}
#endregion