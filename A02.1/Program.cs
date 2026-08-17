// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Computer guesses the user's number using Binary Search.
// ------------------------------------------------------------------------------------------------
using static System.Console;
class Program {
   static void Main () {
      const int MIN_NUMBER = 1, MAX_NUMBER = 100;
      Write ("Think of a number between 1 and 100.\nH = my number is higher than the guess" +
             "\nL = my number is lower than the guess\nC = correct");
      int low = MIN_NUMBER, high = MAX_NUMBER, attempts = 0, guess;
      while (low <= high) {
         guess = low + (high - low) / 2;
         attempts++;
         Write ($"\nAttempt {attempts}: My guess is {guess,3} (H/L/C) : ");
         ConsoleKey hint = ReadGuess ();
         if (hint == ConsoleKey.C) {
            WriteLine ($"\nI guessed your number in {attempts} attempts!");
            return;
         }
         if (hint == ConsoleKey.H) low = guess + 1;
         else high = guess - 1;
      }
      WriteLine ("\nHints are inconsistent; number could not be determined.");

      // Helper function -------------------------------------------

      static ConsoleKey ReadGuess () {
         ConsoleKey hint;
         while (!((hint = ReadKey (true).Key) is ConsoleKey.C or ConsoleKey.H or ConsoleKey.L)) ;
         Write (hint);
         return hint;
      }
   }
}