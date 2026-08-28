// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Guesses the user's number by determining its binary digits from right to left.
// ------------------------------------------------------------------------------------------------
using static System.Console;

class Program {
   static void Main () {
      int number = 0, divisor = 2;
      WriteLine ("Think of a number between 1 and 100.\nAnswer each question with Y or N.");
      while (divisor <= 128) {
         int remainder = number + divisor / 2;
         Write ($"\nIs the remainder {remainder} when divided by {divisor}? (Y/N): ");
         char answer;
         while (true) {
            answer = char.ToUpper (ReadKey (true).KeyChar);
            if (answer is 'Y' or 'N') break;
         }
         Write (answer);
         if (answer == 'Y') number = remainder;
         divisor *= 2;
      }
      WriteLine ($"\nYour number is {number}.");
   }
}