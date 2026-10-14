// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Runs the expression evaluator.
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class Program -----------------------------------------------------------------------------
/// <summary>Runs the expression evaluator</summary>
class Program {
   #region Implementation -------------------------------------------
   // Runs the expression evaluator
   static void Main () {
      Evaluator eval = new ();
      while (true) {
         ForegroundColor = ConsoleColor.Cyan;
         Write ("> ");
         ResetColor ();
         string input = ReadLine () ?? "";
         if (input == "exit") break;
         try {
            double result = eval.Evaluate (input);
            ForegroundColor = ConsoleColor.Green;
            WriteLine (result);
            ResetColor ();
         } catch (EvalException ex) {
            ForegroundColor = ConsoleColor.Red;
            WriteLine (ex.Message);
            ResetColor ();
         }
      }
   }
   #endregion
}
#endregion