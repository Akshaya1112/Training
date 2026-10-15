// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Runs the expression evaluator in test or interactive mode.
// ------------------------------------------------------------------------------------------------
using static System.Console;

#region Class Program -----------------------------------------------------------------------------
/// <summary>Runs the expression evaluator</summary>
class Program {
   #region Implementation -------------------------------------------
   // Asks which mode to run: test cases, interactive evaluation, or exit
   static void Main () {
      while (true) {
         Write ("Enter 'test' to run test cases, " +
                "'interactive' to evaluate expressions, or 'exit' to quit: ");
         string mode = (ReadLine () ?? "exit").Trim ().ToLower ();
         switch (mode) {
            case "exit":
               return;
            case "test":
               Tests.Run ();
               break;
            case "interactive":
               if (RunInteractive ()) return;
               break;
            default:
               PrintLine ("Invalid choice.", ConsoleColor.Red);
               break;
         }
         WriteLine ();
      }
   }

   // Evaluates the expressions typed by the user. Returns true if the user asked to
   // exit the program, or false if the user asked to go back to the mode selection
   static bool RunInteractive () {
      Evaluator eval = new ();
      WriteLine ("Enter an expression, 'back' to choose a mode again, or 'exit' to quit.");
      while (true) {
         ForegroundColor = ConsoleColor.Cyan;
         Write ("> ");
         ResetColor ();
         string input = (ReadLine () ?? "exit").Trim ();
         if (input == "exit") return true;
         if (input == "back") return false;
         try {
            double result = eval.Evaluate (input);
            PrintLine (result.ToString (), ConsoleColor.Green);
         } catch (EvalException ex) {
            PrintLine (ex.Message, ConsoleColor.Red);
         }
      }
   }

   // Prints a line of text in the given color
   static void PrintLine (string text, ConsoleColor color) {
      ForegroundColor = color;
      WriteLine (text);
      ResetColor ();
   }
   #endregion
}
#endregion