// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch- July 2026.
// Copyright (c) Metamation India.
// -------------------------------------------------------------------------------------------------
// Program.cs
// Program to generate a random number and let the user guess it with hints.
namespace A02;

class Program {
   static void Main () {
      Random rd = new ();
      int number = rd.Next (1, 101);
      int guess, attempts = 0, low = 1, high = 100;
      while (attempts < 7) {
         Console.Write ("Enter your guess (1-100): ");
         while (!int.TryParse (Console.ReadLine (), out guess) || guess < 1 || guess > 100)
            Console.Write ("Enter a number between 1 and 100: ");
         attempts++;
         if (guess > number) {
            high = guess - 1;
            Console.WriteLine ($"Your guess is high. Try guessing between {low} to {high}");
         } else if (guess < number) {
            low = guess + 1;
            Console.WriteLine ($"Your guess is low. Try guessing between {low} to {high}");
         } else {
            Console.WriteLine ($"You guessed correctly. Attempts: {attempts}");
            return;
         }
      }
      Console.WriteLine ($"\nYou have used all 7 attempts.");
      Console.WriteLine ($"The correct number was : {number}");
   }
}
