// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Tokenizer.cs
// Converts an expression string into tokens.
// ------------------------------------------------------------------------------------------------

#region Class Tokenizer ---------------------------------------------------------------------------
/// <summary>Converts an expression string into tokens</summary>
class Tokenizer (Evaluator eval, string text) {
   #region Methods --------------------------------------------------
   /// <summary>Gets the next token from the expression</summary>
   /// <returns>The next token</returns>
   public Token Next () {
      while (mN < mText.Length) {
         char ch = char.ToLower (mText[mN++]);
         switch (ch) {
            case ' ' or '\t':
               continue;
            case (>= '0' and <= '9') or '.':
               return SetPrevious (GetNumber ());
            case '(' or ')':
               return SetPrevious (new TPunctuation (ch));
            case '+':
               return SetPrevious (IsUnary () ? new TOpUnary (mEval, '+')
                                              : new TOpArithmetic (mEval, ch));
            case '-':
               return SetPrevious (IsUnary () ? new TOpUnary (mEval, '-')
                                              : new TOpArithmetic (mEval, ch));
            case '*' or '/' or '^' or '=':
               return SetPrevious (new TOpArithmetic (mEval, ch));
            case >= 'a' and <= 'z':
               return SetPrevious (GetIdentifier ());
            default:
               return SetPrevious (new TError ($"Unknown symbol: {ch}"));
         }
      }
      return SetPrevious (new TEnd ());
   }
   #endregion

   #region Implementation -------------------------------------------
   // Stores the token as the previous token and returns it
   Token SetPrevious (Token token) {
      mPrevious = token;
      return token;
   }

   // Determines whether the current minus is unary
   bool IsUnary () => mPrevious is null or TOperator or TPunctuation { Punct: '(' };

   // Reads an identifier from the expression
   Token GetIdentifier () {
      int start = mN - 1;
      while (mN < mText.Length) {
         char ch = char.ToLower (mText[mN++]);
         if (ch is >= 'a' and <= 'z') continue;
         mN--;
         break;
      }
      string sub = mText[start..mN];
      if (mFuncs.Contains (sub)) return new TOpFunction (mEval, sub);
      return new TVariable (mEval, sub);
   }

   // Reads a numeric literal from the expression
   Token GetNumber () {
      int start = mN - 1;
      while (mN < mText.Length) {
         char ch = mText[mN++];
         if (!char.IsDigit (ch) && ch != '.') {
            mN--;
            break;
         }
      }
      string sub = mText[start..mN];
      if (double.TryParse (sub, out double f)) return new TLiteral (f);
      return new TError ($"Invalid number: {sub}");
   }
   #endregion

   #region Private data ---------------------------------------------
   readonly Evaluator mEval = eval;      // Stores the evaluator used to create tokens
   readonly string mText = text;         // Stores the expression being tokenized
   int mN;                               // Stores the current position in the expression
   Token? mPrevious;                     // Stores the previously generated token
   // Stores the supported mathematical functions
   readonly string[] mFuncs = ["sin", "cos", "tan", "sqrt", "log", "exp", "asin", "acos", "atan"];
   #endregion
}
#endregion