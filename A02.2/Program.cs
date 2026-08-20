// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Guesses the user's number by determining its binary digits from right to left.
// ------------------------------------------------------------------------------------------------
using static System.Console;
class Program {
   static void Main (string[] args) {
      const int MIN_NUMBER = 1, MAX_NUMBER = 100;
      int number = 0, divisor = 2;
      WriteLine ("Think of a number between 1 and 100.\nAnswer each question with Y or N.");
      while (divisor <= 128) {
         int bit = ReadBit ();
         number += bit * (divisor / 2);
         divisor *= 2;
      }
      if (number is < MIN_NUMBER or > MAX_NUMBER) {
         WriteLine ("\nHints are inconsistent; no number between 1 and 100 matches.");
         return;
      }
      WriteLine ($"\nYour number is {number}.");

      // Helper function --------------------------------------------
      int ReadBit () {
         int half = divisor / 2;
         Write ($"\nIs the remainder {(number + half) % divisor,-3} " +
                $"when divided by {divisor,-3}? (Y/N): ");
         char answer;
         while ((answer = char.ToUpper (ReadKey (true).KeyChar)) is not ('Y' or 'N')) ;
         Write (answer);
         return answer == 'Y' ? 1 : 0;
      }
   }
}