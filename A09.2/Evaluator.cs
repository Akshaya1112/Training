// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Evaluator.cs
// Evaluates mathematical expressions using operand and operator stacks.
// ------------------------------------------------------------------------------------------------

#region Class EvalException -----------------------------------------------------------------------
/// <summary>Represents an error while evaluating an expression</summary>
class EvalException (string message) : Exception (message) {
}
#endregion

#region Class Evaluator ---------------------------------------------------------------------------
/// <summary>Evaluates mathematical expressions</summary>
class Evaluator {
   #region Methods --------------------------------------------------
   /// <summary>Evaluates the supplied expression</summary>
   /// <param name="text">Expression to evaluate</param>
   /// <returns>The evaluated result</returns>
   public double Evaluate (string text) {
      mOperands.Clear ();
      mOperators.Clear ();
      List<Token> tokens = [];
      var tokenizer = new Tokenizer (this, text);
      while (true) {
         var token = tokenizer.Next ();
         if (token is TEnd) break;
         if (token is TError err) throw new EvalException (err.Message);
         tokens.Add (token);
      }
      // A leading "name =" makes this an assignment
      TVariable? tVariable = null;
      if (tokens.Count >= 2 && tokens[0] is TVariable tvar && IsAssignment (tokens[1])) {
         tVariable = tvar;
         tokens.RemoveRange (0, 2);
      }
      if (tokens.Any (IsAssignment)) throw new EvalException ("Unexpected '='.");
      foreach (var token in tokens) Process (token);
      while (mOperators.Count > 0) {
         if (mOperators.Peek () is TPunctuation)
            throw new EvalException ("Missing closing parenthesis.");
         ApplyOperator ();
      }
      if (mOperands.Count != 1) throw new EvalException ("Invalid expression.");
      double result = mOperands.Pop ();
      if (tVariable != null) mVars[tVariable.Name] = result;
      return result;
   }

   /// <summary>Gets the value of a variable</summary>
   /// <param name="name">Name of the variable</param>
   /// <returns>The value of the variable</returns>
   public double GetVariable (string name) {
      if (mVars.TryGetValue (name, out double f)) return f;
      throw new EvalException ($"Unknown variable: {name}");
   }
   #endregion

   #region Implementation -------------------------------------------
   // Checks whether the token is the assignment operator
   static bool IsAssignment (Token token) => token is TOpArithmetic arith && arith.Op == '=';

   // Processes a token using the operand and operator stacks
   void Process (Token token) {
      switch (token) {
         case TNumber num:
            mOperands.Push (num.Value);
            break;
         case TOperator op:
            PushOperator (op);
            break;
         case TPunctuation { Punct: '(' }:
            mOperators.Push (token);
            break;
         case TPunctuation { Punct: ')' }:
            CloseParenthesis ();
            break;
         default:
            throw new EvalException ($"Unknown token: {token}");
      }
   }

   // Pushes an operator after applying the operators that must be applied before it.
   // Prefix operators have no left operand, so nothing on the stack is applied for them
   void PushOperator (TOperator op) {
      if (op is not TOpPrefix) while (ShouldApplyTop (op)) ApplyOperator ();
      mOperators.Push (op);
   }

   // Checks whether the operator on top of the stack must be applied before the incoming operator.
   bool ShouldApplyTop (TOperator op) =>
      mOperators.Count > 0 && mOperators.Peek () is TOperator top
      && (top.Priority > op.Priority || (top.Priority == op.Priority
      && op is not TOpArithmetic { Op: '^' }));

   // Processes a closing parenthesis and applies its contained operators
   void CloseParenthesis () {
      while (mOperators.Count > 0 && mOperators.Peek () is TOperator) ApplyOperator ();
      if (mOperators.Count == 0) throw new EvalException ("Unexpected closing parenthesis.");
      mOperators.Pop ();
      // Applies the function preceding the opening parenthesis, if present
      if (mOperators.Count > 0 && mOperators.Peek () is TOpFunction) ApplyOperator ();
   }

   // Applies the operator at the top of the operator stack
   void ApplyOperator () {
      switch (mOperators.Pop ()) {
         case TOpPrefix prefix:
            mOperands.Push (prefix.Evaluate (PopOperand ()));
            break;
         case TOpArithmetic arith:
            double b = PopOperand ();
            double a = PopOperand ();
            mOperands.Push (arith.Evaluate (a, b));
            break;
         default:
            throw new EvalException ("Invalid operator.");
      }
   }

   // Pops an operand from the operand stack
   double PopOperand () {
      if (mOperands.Count == 0) throw new EvalException ("Missing operand.");
      return mOperands.Pop ();
   }
   #endregion

   #region Private data ---------------------------------------------
   readonly Dictionary<string, double> mVars = [];   // Stores assigned variables and their values
   readonly Stack<double> mOperands = new ();        // Stores numeric operands
   readonly Stack<Token> mOperators = new ();        // Stores operators and parentheses
   #endregion
}
#endregion