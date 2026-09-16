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
      string[] testCases = ["10.54e23.4e3", "-1.546234e-4", "0", "0.0", "12345", "0.00000325",
         "10.54e2", "1.5e2.5", "1.e3", "1.e+3", "12.54e3.", "12.", "12.e1", ".325", " +625 ",
         "6.25e0", "6.0e0", "6.25e-1", "+6.25e1", "*6.25", "10.625", "15a1", "1.567*2", "+-12",
         "12.-5", ".e1", "-0.325", "  12.456    "];
      foreach (string testCase in testCases) {
         double parsed = Parse (testCase.Trim ());
         double builtIn = double.TryParse (testCase, out double val) ? val : double.NaN;
         WriteLine ($"Input         : {testCase}");
         WriteLine ($"Custom Parse  : {parsed}");
         WriteLine ($"Built-In Parse: {builtIn}");
      }
   }

   static double Parse (string text) {
      if (string.IsNullOrEmpty (text)) return double.NaN;
      int index = 0, digitCount = 0, numberSign = 1;
      double number = 0;
      if (text[index] is '+' or '-') {
         numberSign = text[index] == '-' ? -1 : 1;
         index++;
      }
      while (index < text.Length && char.IsDigit (text[index])) {
         number = (text[index] - '0') + number * 10;
         digitCount++;
         index++;
      }
      if (index < text.Length && digitCount == 0) return double.NaN;
      int decDigits = 0, dec = 0;
      if (index < text.Length && text[index] == '.') {
         index++;
         while (index < text.Length && char.IsDigit (text[index])) {
            dec = dec * 10 + (text[index] - '0');
            decDigits++;
            index++;
         }
         if (decDigits == 0) return double.NaN;
         if (decDigits > 0) number += dec / Math.Pow (10, decDigits);
      }
      if (index < text.Length && (text[index] == 'E' || text[index] == 'e')) {
         index++;
         int expo = 0, expoSign = 1, expoDigits = 0;
         if (index < text.Length && (text[index] == '+' || text[index] == '-')) {
            if (text[index] == '-') expoSign = -1;
            index++;
         }
         while (index < text.Length && char.IsDigit (text[index])) {
            expo = expo * 10 + (text[index] - '0');
            expoDigits++;
            index++;
         }
         if (expoDigits == 0) return double.NaN;
         double power = Math.Pow (10, expo);
         if (expoSign == 1) number *= power;
         else number /= power;
      }
      if (index < text.Length) return double.NaN;
      return number *= numberSign;
   }
}