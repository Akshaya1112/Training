// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Computer guesses the user's number using Binary Search.
// ------------------------------------------------------------------------------------------------
using static System.Console;
using static System.ConsoleKey;

class Program {
   static void Main () {
      Write (@"Think of a number between 1 and 100.
         H = Number is Higher than the guess
         L = Number is Lower than the guess
         C = Correct");
      int low = MIN, high = MAX, attempts = 0, guess;
      while (low <= high) {
         guess = low + (high - low) / 2;
         attempts++;
         Write ($"\nAttempt {attempts}: My guess is {guess}. (H/L/C): ");
         ConsoleKey hint = ReadGuess ();
         switch (hint) {
            case C:
               WriteLine ($"\nI guessed your number in {attempts} attempts!");
               return;
            case H: low = guess + 1; break;
            default: high = guess - 1; break;
         }
      }
      WriteLine ("\nHints are inconsistent; number could not be determined.");
   }

   // Helper function --------------------------------------------
   static ConsoleKey ReadGuess () {
      ConsoleKey hint;
      while (!((hint = ReadKey (true).Key) is C or H or L));
      Write (hint);
      return hint;
   }

   const int MIN = 1, MAX = 100;
}