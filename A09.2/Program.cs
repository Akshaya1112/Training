// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Runs the expression evaluator in test or interactive mode.
// ------------------------------------------------------------------------------------------------
using static System.Console;
namespace Eval;

#region Class Program -----------------------------------------------------------------------------
/// <summary>Runs the expression evaluator in test mode or interactive mode</summary>
class Program {
   #region Implementation -------------------------------------------
   // Shows the mode menu until the user chooses to exit
   static void Main () {
      for (; ; ) {
         Clear ();
         switch (GetMode ()) {
            case 't': RunTests (); break;
            case 'i': RunInteractive (); break;
            default: return;
         }
      }
   }

   // Displays the menu and waits for a single key press (no Enter needed)
   // Returns 't' (test), 'i' (interactive) or 'e' (exit)
   static char GetMode () {
      WriteLine ("Select mode:");
      WriteLine ("  T - Test");
      WriteLine ("  I - Interactive");
      WriteLine ("  E - Exit");
      for (; ; ) {
         char ch = char.ToLower (ReadKey (true).KeyChar);
         if (ch is 't' or 'i' or 'e') return ch;
      }
   }

   // Reads expressions from the user and prints the results until 'exit' is typed
   static void RunInteractive () {
      Clear ();
      WriteLine ("Interactive mode. Type 'exit' to go back to the menu.");
      var eval = new Evaluator ();
      for (; ; ) {
         Write ("> ");
         string text = ReadLine () ?? "";
         if (text == "exit") break;
         try {
            double result = eval.Evaluate (text);
            ForegroundColor = ConsoleColor.Green;
            WriteLine (result);
         } catch (Exception e) {
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine (e.Message);
         }
         ResetColor ();
      }
   }
   #endregion

   #region Test runner ----------------------------------------------
   // Runs all value tests and error tests, then shows the totals
   static void RunTests () {
      Clear ();
      sPassed = sFailed = 0;
      RunValueTests ();
      RunErrorTests ();
      WriteLine ();
      WriteLine ($"Total: {sPassed + sFailed}   Passed: {sPassed}   Failed: {sFailed}");
      WriteLine ();
      Write ("Press any key to return to the menu...");
      ReadKey (true);
   }

   // Evaluates each expression and compares the result with the expected value
   static void RunValueTests () {
      WriteLine ("--- Value tests ---");
      var eval = new Evaluator ();
      foreach (var (expr, expected) in sValueTests) {
         try {
            double actual = eval.Evaluate (expr);
            bool passed = Close (actual, expected);
            PrintStatus ($"{expr}  =>  {actual}", passed,
                         passed ? "" : $"(expected {expected})");
         } catch (Exception e) {
            PrintStatus (expr, false, $"(expected {expected}, threw: {e.Message})");
         }
      }
   }

   // Evaluates each bad expression and checks that the expected error is raised
   static void RunErrorTests () {
      WriteLine ();
      WriteLine ("--- Error tests ---");
      var eval = new Evaluator ();
      foreach (var (expr, expectedMsg) in sErrorTests) {
         string name = $"\"{expr}\"";
         try {
            double actual = eval.Evaluate (expr);
            PrintStatus (name, false, $"(expected an error, got {actual})");
         } catch (Exception e) {
            bool passed = e.Message.Contains (expectedMsg, StringComparison.OrdinalIgnoreCase);
            PrintStatus ($"{name}  =>  {e.Message}", passed,
                         passed ? "" : $"(expected error containing \"{expectedMsg}\")");
         }
      }
   }

   // Compares two doubles with a small tolerance (infinities compare by equality)
   static bool Close (double a, double b) => a.Equals (b) || Math.Abs (a - b) < 1e-9;

   // Displays the test status: only the word PASS is green, only the word FAIL is red
   static void PrintStatus (string name, bool passed, string detail = "") {
      Write ("[");
      ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
      Write (passed ? "PASS" : "FAIL");
      ResetColor ();
      WriteLine (detail.Length == 0 ? $"] {name}" : $"] {name} {detail}");
      if (passed) sPassed++; else sFailed++;
   }
   #endregion

   #region Private data ---------------------------------------------
   static int sPassed, sFailed;    // Number of tests passed and failed in the current run

   // Expression -> expected result
   // Variable tests share one Evaluator and run top to bottom, so an assignment
   // must appear before the lines that use it
   static readonly Dictionary<string, double> sValueTests = new () {
      // Basic arithmetic
      ["2+3"] = 5,
      ["10-3-2"] = 5,               // left associative
      ["2*3+4*5"] = 26,
      ["2+3*4"] = 14,               // precedence
      ["10/4"] = 2.5,
      ["0.1+0.2"] = 0.3,
      [".5+.5"] = 1,
      ["  2  +   3 "] = 5,          // whitespace

      // Power
      ["2^3"] = 8,
      ["2^3^2"] = 512,              // right associative
      ["2+3*4^2"] = 50,
      ["2^0.5"] = Math.Sqrt (2),

      // Parentheses
      ["(2+3)*4"] = 20,
      ["2*(3+4)"] = 14,
      ["10/(2+3)"] = 2,
      ["((2+3))*2"] = 10,

      // Unary operators
      ["-5+3"] = -2,
      ["-2^2"] = -4,
      ["2*-3"] = -6,
      ["3--2"] = 5,
      ["+4"] = 4,
      ["-(2+3)"] = -5,

      // Functions (trigonometry works in degrees)
      ["sin(30)"] = 0.5,
      ["cos(60)"] = 0.5,
      ["tan(45)"] = 1,
      ["asin(1)"] = 90,
      ["acos(1)"] = 0,
      ["atan(1)"] = 45,
      ["sqrt(16)"] = 4,
      ["sqrt(9)+sqrt(16)"] = 7,
      ["2*sin(30)"] = 1,
      ["2^sin(30)"] = Math.Sqrt (2),    // function after a higher priority operator
      ["2^sin 2^-2"] = Math.Pow (2, Math.Sin (0.25 * Math.PI / 180)),
      ["exp(0)"] = 1,
      ["log(1)"] = 0,

      // Division by zero gives infinity
      ["1/0"] = double.PositiveInfinity,

      // Variables (order matters)
      ["x=5"] = 5,
      ["x*2"] = 10,
      ["y=x+1"] = 6,
      ["x=x*2"] = 10,
      ["x+y"] = 16,
      ["x1=10"] = 10,               // names may contain digits
      ["x1+1"] = 11,
      ["total2=x1*2+x"] = 30,
   };

   // Expression -> text the error message must contain ("" means any error is fine)
   static readonly Dictionary<string, string> sErrorTests = new () {
      ["2 $ 3"] = "Unknown symbol",
      ["1.2.3"] = "Invalid number",
      ["unknownvar+1"] = "Unknown variable",
      ["foo(2)"] = "Unknown variable",
      ["2+"] = "",
      [""] = "",
   };
   #endregion
}
#endregion