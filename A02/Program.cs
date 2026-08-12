// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// ------------------------------------------------------------------------------------------------
// Program.cs
// Program to generate a random number and let the user guess it with hints.
// ------------------------------------------------------------------------------------------------
class Program {
   static void Main () {
      Random rd = new ();
      int number = rd.Next (1, 101), guess, attempts = 0, low = 1, high = 100;
      const int LIMIT = 7;
      while (attempts < LIMIT && low < high) {
         do Console.Write ($"Enter a number between {low} and {high}: ");
         while (!int.TryParse (Console.ReadLine (), out guess) || guess < low || guess > high);
         attempts++;
         if (guess == number) {
            Console.WriteLine ($"You guessed correctly. Attempts: {attempts}");
            break;
         }
         if (attempts == LIMIT) {
            Console.WriteLine ($"\nYou have used all {LIMIT} attempts." +
                                $"\nThe correct number was: {number}");
            break;
         }
         bool isHigh = guess > number;
         if (isHigh) high = guess - 1;
         else low = guess + 1;
         if (low == high) {
            Console.WriteLine ($"Only {low} remains. The next guess would be {low}. " +
                   $"Attempts: {attempts + 1}");
            break;
         }
         Console.WriteLine ($"Your guess is {(isHigh ? "high" : "low")}. " +
                            $"Try guessing between {low} and {high}");
      }
   }
}
