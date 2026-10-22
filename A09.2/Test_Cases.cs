// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Tests_Cases.cs
// Test cases for the expression evaluator. Run by entering 'test' when the program starts.
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class Tests -------------------------------------------------------------------------------
/// <summary>Runs test cases against the expression evaluator and prints the result</summary>
static class Tests {
   #region Methods --------------------------------------------------
   /// <summary>Runs all the test cases</summary>
   public static void Run () {
      Evaluator eval = new ();
      int total = 0, passed = 0;
      WriteLine ($"\n  EXPRESSION EVALUATOR\n  {sLine}");
      WriteLine ($"  {"Input",-INPUTWIDTH}{"Expected",-VALUEWIDTH}" +
                 $"{"Output",-VALUEWIDTH}Status\n  {sLine}");
      foreach (string line in sCases) {
         // A line starting with '#' is a group heading
         if (line.StartsWith ('#')) {
            WriteLine ($"\n  {line.Substring (1).Trim ()}");
            continue;
         }
         total++;
         if (RunCase (eval, line)) passed++;
      }
      string result = passed == total ? "ALL TESTS PASSED" : "SOME TESTS FAILED";
      WriteLine ($"\n  {sLine}");
      WriteLine ($"  Total: {total}   Passed: {passed}   Failed: {total - passed}  =>  {result}");
      WriteLine ($"  {sLine}");
   }
   #endregion

   #region Implementation -------------------------------------------
   // Runs a test case written as "expression => expected" and prints its row.
   // The expected part is either a number or an error message. Returns true if the test passed
   static bool RunCase (Evaluator eval, string line) {
      string[] parts = line.Split (" => ");
      string expr = parts[0], expected = parts[1];
      bool isNumber = double.TryParse (expected, out double value);
      bool passed;
      string actual;
      try {
         double result = eval.Evaluate (expr);
         passed = isNumber && IsEqual (result, value);
         actual = Format (result);
      } catch (EvalException ex) {
         passed = ex.Message == expected;
         actual = $"Error: {ex.Message}";
      }
      string shown = isNumber ? Format (value) : $"Error: {expected}";
      string input = expr.Length == 0 ? "(empty)" : expr.Trim ();
      Write ($"  {input,-INPUTWIDTH}{shown,-VALUEWIDTH}{actual,-VALUEWIDTH}");
      ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
      WriteLine (passed ? "PASS" : "FAIL");
      ResetColor ();
      return passed;
   }

   // Checks whether two numbers are equal within a small tolerance
   static bool IsEqual (double a, double b) => Math.Abs (a - b) < 0.000001;

   // Formats a number for the report (rounded, so floating-point noise is hidden)
   static string Format (double f) => Math.Round (f, 6).ToString ();
   #endregion

   #region Private data ---------------------------------------------
   // Column widths and the total width of the report lines
   const int INPUTWIDTH = 22, VALUEWIDTH = 40, LINEWIDTH = 108;
   // Stores the line printed between the sections of the report
   static readonly string sLine = new ('-', LINEWIDTH);
   // Each case is written as "expression => expected", where expected is a number or an error message.
   // A line starting with '#' is a group heading
   static readonly string[] sCases = [
      "# Basic arithmetic and precedence",
      "2 + 3 * 4 => 14",
      "2 * 3 + 4 => 10",
      "10 - 4 - 3 => 3",
      "7 / 2 => 3.5",
      "2 ^ 3 ^ 2 => 512",
      "(1 + 2) * 3 => 9",
      ".5 + .5 => 1",
      "   2   +   3    => 5",
      "# Unary plus and minus",
      "-3 => -3",
      "--3 => 3",
      "-2 ^ 2 => 4",
      "-10 ^ 2 => 100",
      "2 ^ -2 => 0.25",
      "2 * -3 ^ 2 => 18",
      "5 - -3 => 8",
      "5 - +3 => 2",
      "-(2 + 1) => -3",
      "---4+5--2+3 => 6",
      "# Functions",
      "sin(30) => 0.5",
      "sin 45 => 0.7071067812",
      "sin -45 => -0.7071067812",
      "-sin 45 => -0.7071067812",
      "SIN(30) => 0.5",
      "sin sin 0 => 0",
      "cos 0 => 1",
      "tan(45) => 1",
      "asin 1 => 90",
      "acos 0 => 90",
      "atan -1 => -45",
      "sqrt 25 => 5",
      "log 10 => 2.302585093",
      "exp 1-2 => 0.7182818285",
      "# Variables (order matters)",
      "x = 5 => 5",
      "y = x + 1 => 6",
      "x * y => 30",
      "x = x * 2 => 10",
      "X + y => 16",
      "x1 = 3 => 3",
      "# Error handling",
      "5 / 0 => Divide by zero.",
      "sqrt -100 => Invalid argument for sqrt.",
      "log(0) => Invalid argument for log.",
      "(1 + 2 => Missing closing parenthesis.",
      "1 + 2) => Unexpected closing parenthesis.",
      " => Invalid expression.",
      "2 3 => Invalid expression.",
      "5 + => Missing operand.",
      "* 3 => Missing operand.",
      "sin => Missing operand.",
      "2 $ 3 => Unknown symbol: $",
      "1.2.3 => Invalid number: 1.2.3",
      "foo => Unknown variable: foo",
      "2 = 3 => Unexpected '='.",
      "x = y = 5 => Unexpected '='."
   ];
   #endregion
}
#endregion