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
   // Shows the main menu and runs the selected mode: test cases, interactive evaluation, or exit
   static void Main () {
      while (true) {
         PrintTitle ("MAIN MENU");
         WriteLine ("  [t] Test cases    [i] Interactive mode    [e] Exit");
         Write ("  Select an option: ");
         char key = char.ToLower (ReadKey ().KeyChar);
         WriteLine ();
         switch (key) {
            case 'e':
               return;
            case 't':
               Clear ();
               Tests.Run ();
               break;
            case 'i':
               Clear ();
               if (RunInteractive ()) return;
               break;
            default:
               PrintLine ("Invalid choice.", ConsoleColor.Red);
               break;
         }
      }
   }

   // Evaluates the expressions typed by the user. Returns true if the user asked to
   // exit the program, or false if the user asked to go back to the main menu
   static bool RunInteractive () {
      Evaluator eval = new ();
      PrintTitle ("INTERACTIVE MODE");
      WriteLine ("  Type an expression, 'b' to go back to the menu, or 'e' to exit.");
      while (true) {
         ForegroundColor = ConsoleColor.Cyan;
         Write ("  > ");
         ResetColor ();
         string input = (ReadLine () ?? "e").Trim ();
         string command = input.ToLower ();
         if (command == "e") return true;
         if (command == "b") return false;
         try {
            double result = eval.Evaluate (input);
            PrintLine (result.ToString (), ConsoleColor.Green);
         } catch (EvalException ex) {
            PrintLine (ex.Message, ConsoleColor.Red);
         }
      }
   }

   // Prints a title followed by a line
   static void PrintTitle (string title) {
      WriteLine ($"\n  {title}\n  {new string ('-', LINEWIDTH)}");
   }

   // Prints an indented line of text in the given color
   static void PrintLine (string text, ConsoleColor color) {
      ForegroundColor = color;
      WriteLine ($"  {text}");
      ResetColor ();
   }
   #endregion

   #region Private data ---------------------------------------------
   // Width of the line printed below the titles
   const int LINEWIDTH = 60;
   #endregion
}
#endregion