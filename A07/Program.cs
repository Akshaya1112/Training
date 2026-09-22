// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Implements parsing of a string representation of a double value.
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main () {
      string[] testCases = [" ", "abc", "++24", "!27", "+", "-", "12e", "12e+", "12e-",
         "10.54e23.4e3", "-1.546234e-4", "0", "0.0", "12345", "0.00000325", "10.54e2",
         "1.5e2.5", "1.e3", "1.e+3", "12.54e3.", "12.", "12.e1", ".325", " +625 ",
         "6.25e0", "6.0e0", "6.25e-1", "+6.25e1", "*6.25", "10.625", "15a1", "1.567*2",
         "+-12", "12.-5", ".e1", "-0.325", "  12.456    ", "12 3", "12a"];
      WriteLine ($"{"Input",-20} {"Custom Parse",-20} {"Built-In Parse",-20}Result");
      foreach (var testCase in testCases) {
         double custom = Parse (testCase);
         var original = double.TryParse (testCase, out var builtIn) ? builtIn : NAN;
         var passed = !double.IsNaN (custom) ? Math.Abs (custom - original) < EPSILON
                                             : double.IsNaN (original);
         Write ($"{testCase,-20} {custom,-20} {original,-20}");
         ForegroundColor = passed ? ConsoleColor.Green : ConsoleColor.Red;
         WriteLine (passed ? "PASS" : "FAIL");
         ResetColor ();
      }
   }

   static double Parse (string text) {
      text = text.Trim ();
      int index = 0, digitCount = 0, numberSign = 1, len = text.Length;
      double number = 0;
      if (string.IsNullOrEmpty (text) || text[^1] is not ('.' or >= '0' and <= '9'))
         return NAN;
      // Parse the optional sign
      if (text[index] is '+' or '-') numberSign = text[index++] == '-' ? -1 : 1;
      // Parse the integer part
      while (index < len && char.IsDigit (text[index])) {
         number = text[index++] - '0' + number * 10;
         digitCount++;
      }
      // Parse the decimal part
      int decDigits = 0, dec = 0;
      if (index < len && text[index] == '.') {
         index++;
         while (index < len && char.IsDigit (text[index])) {
            dec = dec * 10 + (text[index++] - '0');
            digitCount++;
            decDigits++;
         }
         if (digitCount == 0) return NAN;
         number += dec / Math.Pow (10, decDigits);
      }
      // Parse the exponent part
      if (index < len && text[index] is 'E' or 'e') {
         index++;
         int expo = 0, expoSign = 1, expoDigits = 0;
         if (text[index] is '+' or '-') expoSign = text[index++] == '-' ? -1 : 1;
         while (index < len && char.IsDigit (text[index])) {
            expo = expo * 10 + (text[index++] - '0');
            expoDigits++;
         }
         if (expoDigits == 0) return NAN;
         double power = Math.Pow (10, expo);
         number = expoSign == 1 ? number * power : number / power;
      }
      // Reject any remaining invalid characters
      if (index < len) return NAN;
      return number * numberSign;
   }
   const double NAN = double.NaN, EPSILON = 1e-6;
}