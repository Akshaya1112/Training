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
      Token token = ReadToken ();
      mPrevious = token;
      return token;
   }
   #endregion

   #region Implementation -------------------------------------------
   // Reads the next token from the expression
   Token ReadToken () {
      while (mN < mText.Length) {
         char ch = mText[mN++];
         if (char.IsWhiteSpace (ch)) continue;
         return ch switch {
            '+' or '-' => IsUnary () ? new TOpUnary (ch) : new TOpArithmetic (ch),
            '(' or ')' => new TPunctuation (ch),
            _ when NUMBERCHARS.Contains (ch) => GetNumber (),
            _ when LETTERS.Contains (ch) => GetIdentifier (),
            _ when TOpArithmetic.IsOperator (ch) => new TOpArithmetic (ch),
            _ => new TError ($"Unknown symbol: {ch}")
         };
      }
      return new TEnd ();
   }

   // Determines whether the current plus or minus is unary
   bool IsUnary () => mPrevious is null or TOperator or TPunctuation { Punct: '(' };

   // Consumes the characters that are in the allowed set, and returns the text from 'start'
   string Scan (int start, string allowed) {
      while (mN < mText.Length && allowed.Contains (mText[mN])) mN++;
      return mText[start..mN];
   }

   // Reads an identifier (letters followed by letters or digits) from the expression
   Token GetIdentifier () {
      string sub = Scan (mN - 1, NAMECHARS);
      if (TOpFunction.IsFunction (sub)) return new TOpFunction (sub);
      return new TVariable (mEval, sub);
   }

   // Reads a numeric literal from the expression
   Token GetNumber () {
      string sub = Scan (mN - 1, NUMBERCHARS);
      if (double.TryParse (sub, out double f)) return new TLiteral (f);
      return new TError ($"Invalid number: {sub}");
   }
   #endregion

   #region Private data ---------------------------------------------
   const string DIGITS = "0123456789";                    // Digits
   const string LETTERS = "abcdefghijklmnopqrstuvwxyz";   // Letters
   const string NUMBERCHARS = DIGITS + ".";               // Characters allowed within a number
   const string NAMECHARS = LETTERS + DIGITS;             // Characters allowed within a name
   readonly Evaluator mEval = eval;                       // Stores the evaluator for variable tokens
   readonly string mText = text.ToLower ();               // Stores the expression being tokenized
   int mN;                                                // Stores the current position in the expression
   Token? mPrevious;                                      // Stores the previously generated token
   #endregion
}
#endregion