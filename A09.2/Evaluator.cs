// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Evaluator.cs
// EEvaluates mathematical expressions using operand and operator stacks.
// ------------------------------------------------------------------------------------------------
namespace Eval;

#region Class EvalException -----------------------------------------------------------------------
/// <summary>Raised when an expression cannot be tokenized or evaluated</summary>
class EvalException : Exception {
   /// <summary>Creates an exception with the given message</summary>
   /// <param name="message">The description of the problem</param>
   public EvalException (string message) : base (message) { }
}
#endregion

#region Class Evaluator ---------------------------------------------------------------------------
/// <summary>Evaluates arithmetic expressions, with variables and functions</summary>
class Evaluator {
   #region Properties -----------------------------------------------
   /// <summary>Priority offset added to operators; 10 for each open bracket</summary>
   public int BasePriority { get; private set; }
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Evaluates the expression; "name = expression" stores the result</summary>
   /// <param name="text">The expression text</param>
   /// <returns>The value of the expression</returns>
   /// <exception cref="EvalException">The expression is invalid</exception>
   public double Evaluate (string text) {
      mOperands.Clear ();
      mOperators.Clear ();
      List<Token> tokens = new ();
      var tokenizer = new Tokenizer (this, text);
      for (; ; ) {
         var token = tokenizer.Next ();
         if (token is TEnd) break;
         if (token is TError err) throw new EvalException (err.Message);
         tokens.Add (token);
      }

      // Check if this is a variable assignment
      TVariable? tVariable = null;
      if (tokens.Count > 2 && tokens[0] is TVariable tvar &&
          tokens[1] is TOpArithmetic { Op: '=' }) {
         tVariable = tvar;
         tokens.RemoveRange (0, 2);
      }
      foreach (var t in tokens) Process (t);
      while (mOperators.Count > 0) ApplyOperator ();
      double f = mOperands.Pop ();
      if (tVariable != null) mVars[tVariable.Name] = f;
      return f;
   }

   /// <summary>Returns the value of a variable</summary>
   /// <param name="name">The variable name</param>
   /// <returns>The value last assigned to the variable</returns>
   /// <exception cref="EvalException">The variable has not been assigned</exception>
   public double GetVariable (string name) {
      if (mVars.TryGetValue (name, out double f)) return f;
      throw new EvalException ($"Unknown variable: {name}");
   }

   /// <summary>Checks if the operator can be pushed without applying the one on top</summary>
   /// <param name="op">The operator about to be pushed</param>
   /// <returns>True if it has higher priority, or if it is a right-associative ^</returns>
   public bool OkToPush (TOperator op) {
      if (mOperators.Count == 0) return true;
      TOperator prev = mOperators.Peek ();
      return prev.FinalPriority < op.FinalPriority ||
             (prev.FinalPriority == op.FinalPriority && op is TOpArithmetic { Op: '^' });
   }
   #endregion

   #region Implementation -------------------------------------------
   // Handles one token: pushes numbers and operators, and tracks bracket depth
   void Process (Token token) {
      switch (token) {
         case TNumber num:
            mOperands.Push (num.Value);
            break;
         case TOperator op:
            op.FinalPriority = op.Priority + BasePriority;
            // Unary operators and functions are prefix operators with no left operand,
            // so nothing on the stack may be applied before pushing them
            if (op is not (TOpUnary or TOpFunction))
               while (!OkToPush (op)) ApplyOperator ();
            mOperators.Push (op);
            break;
         case TPunctuation p:
            BasePriority += p.Punct == '(' ? 10 : -10;
            break;
         default:
            throw new EvalException ($"Unknown token: {token}");
      }
   }

   // Pops the top operator and its operands, and pushes the result
   void ApplyOperator () {
      var op = mOperators.Pop ();
      var f1 = mOperands.Pop ();
      if (op is TOpUnary unary) mOperands.Push (unary.Evaluate (f1));
      else if (op is TOpFunction func) mOperands.Push (func.Evaluate (f1));
      else if (op is TOpArithmetic arith) {
         var f2 = mOperands.Pop ();
         mOperands.Push (arith.Evaluate (f2, f1));
      }
   }
   #endregion

   #region Private data ---------------------------------------------
   readonly Dictionary<string, double> mVars = [];    // Variable values by name
   readonly Stack<double> mOperands = new ();             // Operands waiting for an operator
   readonly Stack<TOperator> mOperators = new ();         // Operators waiting to be applied
   #endregion
}
#endregion