// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Tokenizer.cs
// Converts an expression string into tokens.
// ------------------------------------------------------------------------------------------------
namespace Eval;

#region Class Tokenizer ---------------------------------------------------------------------------
/// <summary>Converts an expression string into tokens</summary>
class Tokenizer {
   #region Constructors ---------------------------------------------
   /// <summary>Creates a tokenizer for the given text</summary>
   /// <param name="eval">The evaluator that owns the tokens</param>
   /// <param name="text">The expression text</param>
   public Tokenizer (Evaluator eval, string text) {
      mText = text; mN = 0; mEval = eval;
   }
   #endregion

   #region Methods --------------------------------------------------
   /// <summary>Returns the next token from the text</summary>
   /// <returns>The next token; a TEnd at the end, or a TError for invalid input</returns>
   public Token Next () {
      while (mN < mText.Length) {
         char ch = char.ToLower (mText[mN++]);
         switch (ch) {
            case ' ' or '\t': continue;
            case (>= '0' and <= '9') or '.': return Remember (GetNumber ());
            case '(' or ')': return Remember (new TPunctuation (ch));
            case '+' or '-' or '*' or '/' or '^' or '=': return Remember (GetOperator (ch));
            case >= 'a' and <= 'z': return Remember (GetIdentifier ());
            default: return Remember (new TError ($"Unknown symbol: {ch}"));
         }
      }
      return Remember (new TEnd ());
   }
   #endregion

   #region Implementation -------------------------------------------
   // Stores the token as the previous one and returns it
   Token Remember (Token token) {
      mPrevious = token;
      return token;
   }

   // Creates an operator token; a + or - at the start, after another operator
   // or after an opening bracket is a unary operator
   Token GetOperator (char ch) {
      if ((ch == '+' || ch == '-') &&
          (mPrevious is null or TOperator or TPunctuation { Punct: '(' }))
         return new TOpUnary (mEval, ch);
      return new TOpArithmetic (mEval, ch);
   }

   // Reads a letter followed by letters or digits (like x1) and returns a function
   // token or a variable token
   Token GetIdentifier () {
      int start = mN - 1;
      while (mN < mText.Length) {
         char ch = char.ToLower (mText[mN++]);
         if (ch is (>= 'a' and <= 'z') or (>= '0' and <= '9')) continue;
         mN--; break;
      }
      string sub = mText[start..mN];
      if (mFuncs.Contains (sub)) return new TOpFunction (mEval, sub);
      else return new TVariable (mEval, sub);
   }

   // Reads a run of digits and dots and returns a literal token, or an error token
   Token GetNumber () {
      int start = mN - 1;
      while (mN < mText.Length) {
         char ch = mText[mN++];
         if (ch is (>= '0' and <= '9') or '.') continue;
         mN--; break;
      }
      // Now, mN points to the first character of mText that is not part of the number
      string sub = mText[start..mN];
      if (double.TryParse (sub, out double f)) return new TLiteral (f);
      return new TError ($"Invalid number: {sub}");
   }
   #endregion

   #region Private data ---------------------------------------------
   readonly Evaluator mEval;    // The evaluator that owns this
   readonly string mText;       // The input text we're parsing through
   int mN;                      // Position within the text
   Token? mPrevious;            // The previously returned token

   // Names of the supported functions
   readonly string[] mFuncs = { "sin", "cos", "tan", "sqrt", "log", "exp", "asin", "acos", "atan" };
   #endregion
}
#endregion